using System;
using System.Collections.Generic;
using System.Linq;
using Immersive.Framework.ActivityFlow;
using UnityEngine;
namespace _Project.Demo01.Scripts.Activity_Readiness
{
    [DisallowMultipleComponent]
    public sealed class ReadinessPreparationArea : MonoBehaviour
    {
        private const string LogPrefix =
            "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Header("Readiness")]
        [SerializeField]
        private ActivityReadinessParticipant participant;

        [Header("Area")]
        [SerializeField]
        private BoxCollider preparationVolume;

        [SerializeField, Min(0f)]
        private float positionTolerance = 0.05f;

        [Header("Subjects")]
        [SerializeField]
        private Transform[] subjects = Array.Empty<Transform>();

        private bool _isObserving;
        private bool _completionSent;
        private int _lastInsideCount = -1;
        private float _progress;

        public event Action<float> ProgressChanged;

        public float Progress => _progress;

        public int SubjectCount =>
            subjects?.Length ?? 0;

        public void BeginObservation()
        {
            StopObservation();

            if (!TryValidateConfiguration(out string error))
            {
                Debug.LogError(
                    $"{LogPrefix} area-observation='rejected' reason='{error}'.",
                    this);
                return;
            }

            _isObserving = true;
            _completionSent = false;
            _lastInsideCount = -1;

            EvaluateArea();

            Debug.Log(
                $"{LogPrefix} area-observation='started' subjects='{subjects.Length}'.",
                this);
        }

        public void ReleaseObservation()
        {
            StopObservation();
            _progress = 0f;
            ProgressChanged?.Invoke(_progress);

            Debug.Log(
                $"{LogPrefix} area-observation='released'.",
                this);
        }

        private void Update()
        {
            if (_isObserving)
            {
                EvaluateArea();
            }
        }

        private void EvaluateArea()
        {
            int insideCount = subjects.Count(subject => IsInsideVolume(subject.position));

            _progress = subjects.Length > 0
                ? (float)insideCount / subjects.Length
                : 0f;

            if (insideCount != _lastInsideCount)
            {
                _lastInsideCount = insideCount;
                ProgressChanged?.Invoke(_progress);

                Debug.Log(
                    $"{LogPrefix} area-progress='{insideCount}/{subjects.Length}' normalized='{_progress:0.00}'.",
                    this);
            }

            bool allSubjectsInside = insideCount == subjects.Length;
            if (allSubjectsInside && !_completionSent)
            {
                CompleteReadiness();
            }
        }

        private void CompleteReadiness()
        {
            _completionSent = true;
            _isObserving = false;

            if (participant.State != ActivityReadinessParticipantState.Preparing)
            {
                Debug.LogWarning(
                    $"{LogPrefix} completion='rejected-locally' participantState='{participant.State}' occurrence='{participant.Occurrence}'.",
                    this);
                return;
            }

            participant.CompletePreparation();

            Debug.Log(
                $"{LogPrefix} completion='submitted' condition='all-subjects-inside' subjects='{subjects.Length}' occurrence='{participant.Occurrence}'.",
                this);
        }

        private bool IsInsideVolume(Vector3 worldPosition)
        {
            Transform volumeTransform = preparationVolume.transform;
            Vector3 localPosition = volumeTransform.InverseTransformPoint(worldPosition);
            Vector3 relativePosition = localPosition - preparationVolume.center;
            Vector3 halfSize = preparationVolume.size * 0.5f;
            halfSize += Vector3.one * positionTolerance;

            return Mathf.Abs(relativePosition.x) <= halfSize.x
                && Mathf.Abs(relativePosition.y) <= halfSize.y
                && Mathf.Abs(relativePosition.z) <= halfSize.z;
        }

        private void StopObservation()
        {
            _isObserving = false;
            _completionSent = false;
            _lastInsideCount = -1;
        }

        private bool TryValidateConfiguration(out string error)
        {
            if (participant == null)
            {
                error = "Activity Readiness Participant is missing.";
                return false;
            }

            if (preparationVolume == null)
            {
                error = "Preparation Volume is missing.";
                return false;
            }

            if (subjects == null || subjects.Length == 0)
            {
                error = "No preparation subjects are configured.";
                return false;
            }

            var uniqueSubjects = new HashSet<Transform>();

            for (int index = 0; index < subjects.Length; index++)
            {
                Transform subject = subjects[index];

                if (subject == null)
                {
                    error = $"Subject {index + 1} is missing.";
                    return false;
                }

                if (!uniqueSubjects.Add(subject))
                {
                    error = $"Subject '{subject.name}' is configured more than once.";
                    return false;
                }
            }

            Vector3 size = preparationVolume.size;
            if (size.x <= 0f || size.y <= 0f || size.z <= 0f)
            {
                error = "Preparation Volume must have a positive size.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
