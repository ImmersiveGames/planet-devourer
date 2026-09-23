using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class GameFlowVisitorPreparation : MonoBehaviour
{
    [Header("Participants")]
    [SerializeField]
    private List<Transform> participants = new();

    [SerializeField]
    private Transform commonOrigin;

    [Header("Preparation")]
    [SerializeField, Min(0f)]
    private float preparationDuration = 3f;

    [SerializeField]
    private UnityEvent preparationCompleted = new();

    private readonly List<Vector3> targetPositions = new();
    private Coroutine preparationRoutine;

    private void Awake()
    {
        CaptureTargetPositions();
        MoveParticipantsToCommonOrigin();
    }

    private void OnDisable()
    {
        StopPreparation();
    }

    public void BeginPreparation()
    {
        if (!CanPrepare())
        {
            return;
        }

        StopPreparation();
        SetParticipantsActive(true);
        MoveParticipantsToCommonOrigin();

        if (preparationDuration <= 0f)
        {
            MoveParticipantsToTargetPositions();
            preparationCompleted.Invoke();
            return;
        }

        preparationRoutine = StartCoroutine(PreparationRoutine());
    }

    public void ReleasePreparation()
    {
        StopPreparation();

        if (commonOrigin != null)
        {
            MoveParticipantsToCommonOrigin();
        }

        SetParticipantsActive(false);
    }

    private IEnumerator PreparationRoutine()
    {
        Vector3 originPosition = commonOrigin.position;
        float elapsed = 0f;

        while (elapsed < preparationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / preparationDuration);

            for (int i = 0; i < participants.Count; i++)
            {
                Transform participant = participants[i];
                if (participant == null)
                {
                    continue;
                }

                participant.position = Vector3.Lerp(
                    originPosition,
                    targetPositions[i],
                    normalizedTime);
            }

            yield return null;
        }

        MoveParticipantsToTargetPositions();
        preparationRoutine = null;
        preparationCompleted.Invoke();
    }

    private void CaptureTargetPositions()
    {
        targetPositions.Clear();

        for (int i = 0; i < participants.Count; i++)
        {
            Transform participant = participants[i];
            targetPositions.Add(
                participant != null ? participant.position : Vector3.zero);
        }
    }

    private bool CanPrepare()
    {
        if (commonOrigin == null)
        {
            Debug.LogError(
                "[GameFlow Sample][Readiness] Common Origin is not assigned.",
                this);
            return false;
        }

        if (targetPositions.Count != participants.Count)
        {
            Debug.LogError(
                "[GameFlow Sample][Readiness] Participant targets were not initialized correctly.",
                this);
            return false;
        }

        for (int i = 0; i < participants.Count; i++)
        {
            if (participants[i] != null)
            {
                return true;
            }
        }

        Debug.LogError(
            "[GameFlow Sample][Readiness] At least one participant must be assigned.",
            this);
        return false;
    }

    private void MoveParticipantsToCommonOrigin()
    {
        if (commonOrigin == null)
        {
            return;
        }

        Vector3 originPosition = commonOrigin.position;

        for (int i = 0; i < participants.Count; i++)
        {
            Transform participant = participants[i];
            if (participant != null)
            {
                participant.position = originPosition;
            }
        }
    }

    private void MoveParticipantsToTargetPositions()
    {
        int count = Mathf.Min(participants.Count, targetPositions.Count);

        for (int i = 0; i < count; i++)
        {
            Transform participant = participants[i];
            if (participant != null)
            {
                participant.position = targetPositions[i];
            }
        }
    }

    private void SetParticipantsActive(bool active)
    {
        for (int i = 0; i < participants.Count; i++)
        {
            Transform participant = participants[i];
            if (participant != null)
            {
                participant.gameObject.SetActive(active);
            }
        }
    }

    private void StopPreparation()
    {
        if (preparationRoutine == null)
        {
            return;
        }

        StopCoroutine(preparationRoutine);
        preparationRoutine = null;
    }
}
