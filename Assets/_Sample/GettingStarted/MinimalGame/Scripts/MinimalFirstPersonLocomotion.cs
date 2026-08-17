using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public sealed class MinimalFirstPersonLocomotion : MonoBehaviour
{
    private const string LogPrefix = "[Sample00][MinimalFirstPersonLocomotion]";

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;

    [Header("Look")]
    [SerializeField] private Transform cameraMount;
    [SerializeField, Min(0f)] private float lookSensitivity = 0.1f;
    [SerializeField] private float minimumPitch = -80f;
    [SerializeField] private float maximumPitch = 80f;

    private CharacterController characterController;
    private PlayerGameplayInputConsumerBinding gameplayInputConsumer;
    private IPlayerGameplayInputReader gameplayInputReader;
    private float pitch;

    private string lastRuntimeState;
    private string lastMoveIssue;
    private string lastLookIssue;
    private bool reportedReady;
    private bool reportedMoveInput;
    private bool reportedLookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        gameplayInputConsumer = GetComponent<PlayerGameplayInputConsumerBinding>();
        gameplayInputReader = gameplayInputConsumer;

        if (cameraMount != null)
        {
            pitch = NormalizeSignedAngle(cameraMount.localEulerAngles.x);
            pitch = Mathf.Clamp(pitch, minimumPitch, maximumPitch);
        }

        ReportAuthoringState();
    }

    private void Update()
    {
        if (characterController == null)
        {
            ReportRuntimeState("BLOCKED CharacterController is missing.");
            return;
        }

        if (gameplayInputConsumer == null || gameplayInputReader == null)
        {
            ReportRuntimeState(
                "BLOCKED PlayerGameplayInputConsumerBinding is missing on the same GameObject.");
            return;
        }

        if (!gameplayInputReader.GameplayReady)
        {
            reportedReady = false;

            ReportRuntimeState(
                $"INPUT_NOT_READY hasBinding={gameplayInputConsumer.HasCurrentGameplayBinding} " +
                $"gameplayReady={gameplayInputConsumer.GameplayReady} " +
                $"bindingRevision={gameplayInputConsumer.BindingRevision} " +
                $"diagnostic='{gameplayInputConsumer.Diagnostic}'");

            return;
        }

        if (!reportedReady)
        {
            reportedReady = true;
            lastRuntimeState = string.Empty;

            Debug.Log(
                $"{LogPrefix} READY hasBinding={gameplayInputConsumer.HasCurrentGameplayBinding} " +
                $"gameplayReady={gameplayInputConsumer.GameplayReady} " +
                $"bindingRevision={gameplayInputConsumer.BindingRevision}.");
        }

        ApplyMove();
        ApplyLook();
    }

    private void ApplyMove()
    {
        if (moveAction == null)
        {
            ReportMoveIssue("MOVE_BLOCKED Move InputActionReference is not assigned.");
            return;
        }

        if (!gameplayInputReader.TryReadValue(moveAction, out Vector2 move))
        {
            ReportMoveIssue(
                $"MOVE_BLOCKED runtime read failed. diagnostic='{gameplayInputConsumer.Diagnostic}'");
            return;
        }

        lastMoveIssue = string.Empty;

        Vector2 planarInput = Vector2.ClampMagnitude(move, 1f);
        if (planarInput.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        if (!reportedMoveInput)
        {
            reportedMoveInput = true;
            Debug.Log(
                $"{LogPrefix} MOVE_INPUT received value={planarInput}.");
        }

        Quaternion yawRotation =
            Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Vector3 planarDirection =
            yawRotation * new Vector3(planarInput.x, 0f, planarInput.y);

        Vector3 before = transform.position;

        CollisionFlags collisionFlags = characterController.Move(
            planarDirection * (moveSpeed * Time.deltaTime));

        Vector3 displacement = transform.position - before;

        if (displacement.sqrMagnitude <= 0.0000001f)
        {
            ReportMoveIssue(
                $"MOVE_NO_DISPLACEMENT input={planarInput} " +
                $"speed={moveSpeed} collisionFlags={collisionFlags} " +
                $"position={transform.position}.");
        }
    }

    private void ApplyLook()
    {
        if (lookAction == null)
        {
            ReportLookIssue("LOOK_BLOCKED Look InputActionReference is not assigned.");
            return;
        }

        if (cameraMount == null)
        {
            ReportLookIssue("LOOK_BLOCKED CameraMount is not assigned.");
            return;
        }

        if (!gameplayInputReader.TryReadValue(lookAction, out Vector2 look))
        {
            ReportLookIssue(
                $"LOOK_BLOCKED runtime read failed. diagnostic='{gameplayInputConsumer.Diagnostic}'");
            return;
        }

        lastLookIssue = string.Empty;

        if (look.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        if (!reportedLookInput)
        {
            reportedLookInput = true;
            Debug.Log(
                $"{LogPrefix} LOOK_INPUT received value={look}.");
        }

        float yawDelta = look.x * lookSensitivity;
        float pitchDelta = look.y * lookSensitivity;

        transform.Rotate(0f, yawDelta, 0f, Space.Self);

        pitch = Mathf.Clamp(
            pitch - pitchDelta,
            minimumPitch,
            maximumPitch);

        cameraMount.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void ReportAuthoringState()
    {
        if (gameplayInputConsumer == null)
        {
            Debug.LogError(
                $"{LogPrefix} AUTHORING_BLOCKED " +
                "PlayerGameplayInputConsumerBinding must be on the same GameObject.");
        }

        if (moveAction == null)
        {
            Debug.LogError(
                $"{LogPrefix} AUTHORING_BLOCKED Move InputActionReference is not assigned.");
        }

        if (lookAction == null)
        {
            Debug.LogError(
                $"{LogPrefix} AUTHORING_BLOCKED Look InputActionReference is not assigned.");
        }

        if (cameraMount == null)
        {
            Debug.LogError(
                $"{LogPrefix} AUTHORING_BLOCKED CameraMount is not assigned.");
        }
    }

    private void ReportRuntimeState(string state)
    {
        if (string.Equals(lastRuntimeState, state, System.StringComparison.Ordinal))
        {
            return;
        }

        lastRuntimeState = state;
        Debug.LogWarning($"{LogPrefix} {state}");
    }

    private void ReportMoveIssue(string issue)
    {
        if (string.Equals(lastMoveIssue, issue, System.StringComparison.Ordinal))
        {
            return;
        }

        lastMoveIssue = issue;
        Debug.LogWarning($"{LogPrefix} {issue}");
    }

    private void ReportLookIssue(string issue)
    {
        if (string.Equals(lastLookIssue, issue, System.StringComparison.Ordinal))
        {
            return;
        }

        lastLookIssue = issue;
        Debug.LogWarning($"{LogPrefix} {issue}");
    }

    private void OnValidate()
    {
        if (maximumPitch < minimumPitch)
        {
            maximumPitch = minimumPitch;
        }
    }

    private static float NormalizeSignedAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
