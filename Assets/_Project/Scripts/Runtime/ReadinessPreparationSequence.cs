using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Project._Project.Scripts.Runtime
{
    [DisallowMultipleComponent]
    public sealed class ReadinessPreparationSequence : MonoBehaviour
    {
        private const string LogPrefix =
            "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Serializable]
        private sealed class MoveStep
        {
            [SerializeField]
            private Transform subject;

            [SerializeField]
            private Transform destination;

            public Transform Subject => subject;

            public Transform Destination => destination;
        }

        [Header("Sequence")]
        [SerializeField]
        private MoveStep[] steps = Array.Empty<MoveStep>();

        [SerializeField, Min(0f)]
        private float delayBeforeFirstObject = 0.5f;

        [SerializeField, Min(0f)]
        private float delayBetweenObjects = 0.5f;

        [SerializeField, Min(0.01f)]
        private float moveDuration = 1.25f;

        [SerializeField]
        private AnimationCurve movementCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Vector3[] _initialPositions =
            Array.Empty<Vector3>();

        private Quaternion[] _initialRotations =
            Array.Empty<Quaternion>();

        private Coroutine _sequenceRoutine;

        private void Awake()
        {
            CaptureInitialPoses();
        }

        private void OnDisable()
        {
            StopSequence();
        }

        /// <summary>
        /// Called by ActivityReadinessParticipant.PreparationStarted.
        /// Starts only the local visual preparation.
        /// </summary>
        public void BeginPreparation()
        {
            StopSequence();

            if (!TryValidateConfiguration(out string error))
            {
                Debug.LogError(
                    $"{LogPrefix} preparation='rejected' " +
                    $"reason='{error}'.",
                    this);

                return;
            }

            ResetSubjects();

            _sequenceRoutine =
                StartCoroutine(ExecuteSequence());

            Debug.Log(
                $"{LogPrefix} preparation='started' " +
                $"steps='{steps.Length}'.",
                this);
        }

        /// <summary>
        /// Called by ActivityReadinessParticipant.PreparationReleased.
        /// Stops local work and restores the authored initial poses.
        /// </summary>
        public void ReleasePreparation()
        {
            StopSequence();
            ResetSubjects();

            Debug.Log(
                $"{LogPrefix} preparation='released'.",
                this);
        }

        private IEnumerator ExecuteSequence()
        {
            if (delayBeforeFirstObject > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    delayBeforeFirstObject);
            }

            for (int index = 0; index < steps.Length; index++)
            {
                yield return MoveSubject(
                    steps[index]);

                Debug.Log(
                    $"{LogPrefix} object-arrived='{index + 1}' " +
                    $"total='{steps.Length}' " +
                    $"subject='{steps[index].Subject.name}'.",
                    this);

                bool hasAnotherStep =
                    index < steps.Length - 1;

                if (hasAnotherStep &&
                    delayBetweenObjects > 0f)
                {
                    yield return new WaitForSecondsRealtime(
                        delayBetweenObjects);
                }
            }

            _sequenceRoutine = null;

            Debug.Log(
                $"{LogPrefix} local-sequence='completed' " +
                "readiness='not-completed-by-sequence'.",
                this);
        }

        private IEnumerator MoveSubject(
            MoveStep step)
        {
            Transform subject =
                step.Subject;

            Transform destination =
                step.Destination;

            Vector3 startPosition =
                subject.position;

            Quaternion startRotation =
                subject.rotation;

            Vector3 targetPosition =
                destination.position;

            Quaternion targetRotation =
                destination.rotation;

            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float normalizedTime =
                    Mathf.Clamp01(
                        elapsed / moveDuration);

                float evaluatedTime =
                    movementCurve != null
                        ? movementCurve.Evaluate(
                            normalizedTime)
                        : normalizedTime;

                Vector3 position =
                    Vector3.LerpUnclamped(
                        startPosition,
                        targetPosition,
                        evaluatedTime);

                Quaternion rotation =
                    Quaternion.SlerpUnclamped(
                        startRotation,
                        targetRotation,
                        evaluatedTime);

                subject.SetPositionAndRotation(
                    position,
                    rotation);

                yield return null;
            }

            subject.SetPositionAndRotation(
                targetPosition,
                targetRotation);
        }

        private void CaptureInitialPoses()
        {
            int stepCount =
                steps?.Length ?? 0;

            _initialPositions =
                new Vector3[stepCount];

            _initialRotations =
                new Quaternion[stepCount];

            for (int index = 0;
                 index < stepCount;
                 index++)
            {
                Transform subject =
                    steps[index]?.Subject;

                if (subject == null)
                {
                    continue;
                }

                _initialPositions[index] =
                    subject.position;

                _initialRotations[index] =
                    subject.rotation;
            }
        }

        private void ResetSubjects()
        {
            int count =
                Mathf.Min(
                    steps?.Length ?? 0,
                    _initialPositions.Length);

            for (int index = 0;
                 index < count;
                 index++)
            {
                Transform subject =
                    steps[index]?.Subject;

                if (subject == null)
                {
                    continue;
                }

                subject.SetPositionAndRotation(
                    _initialPositions[index],
                    _initialRotations[index]);
            }
        }

        private void StopSequence()
        {
            if (_sequenceRoutine == null)
            {
                return;
            }

            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        private bool TryValidateConfiguration(
            out string error)
        {
            if (steps == null ||
                steps.Length == 0)
            {
                error =
                    "No movement steps are configured.";

                return false;
            }

            var configuredSubjects =
                new HashSet<Transform>();

            for (int index = 0;
                 index < steps.Length;
                 index++)
            {
                MoveStep step =
                    steps[index];

                if (step == null)
                {
                    error =
                        $"Step {index + 1} is missing.";

                    return false;
                }

                if (step.Subject == null)
                {
                    error =
                        $"Step {index + 1} has no Subject.";

                    return false;
                }

                if (step.Destination == null)
                {
                    error =
                        $"Step {index + 1} has no Destination.";

                    return false;
                }

                if (step.Subject ==
                    step.Destination)
                {
                    error =
                        $"Step {index + 1} uses the same " +
                        "Transform as Subject and Destination.";

                    return false;
                }

                if (!configuredSubjects.Add(
                        step.Subject))
                {
                    error =
                        $"Subject '{step.Subject.name}' " +
                        "is configured more than once.";

                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}