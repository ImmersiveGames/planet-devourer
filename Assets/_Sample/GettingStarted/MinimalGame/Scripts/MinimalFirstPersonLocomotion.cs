using Immersive.Framework.Actors;
using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerGameplayInputReader))]
[RequireComponent(typeof(CharacterController))]
public sealed class MinimalFirstPersonLocomotion : MonoBehaviour
{
    [Header("Input")]
    [SerializeField]
    private InputActionReference moveAction;

    [SerializeField]
    private InputActionReference lookAction;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float moveSpeed = 5f;

    [Header("Look")]
    [SerializeField, Min(0f)]
    private float lookSensitivity = 0.1f;

    [SerializeField]
    private float minimumPitch = -80f;

    [SerializeField]
    private float maximumPitch = 80f;

    private CharacterController _characterController;
    private IPlayerGameplayInputReader _gameplayInputReader;
    private Transform _observationTransform;
    private float _pitch;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _gameplayInputReader = GetComponent<PlayerGameplayInputReader>();

        if (TryResolveObservationTransform(out Transform observationTransform))
        {
            _observationTransform = observationTransform;
            _pitch = Mathf.Clamp(
                NormalizeSignedAngle(_observationTransform.localEulerAngles.x),
                minimumPitch,
                maximumPitch);
        }

        ValidateSetup();
    }

    private void Update()
    {
        if (_characterController == null ||
            !_characterController.enabled ||
            _gameplayInputReader == null ||
            !_gameplayInputReader.RuntimeGameplayAvailable)
        {
            return;
        }

        ApplyMove();
        ApplyLook();
    }

    private void ApplyMove()
    {
        if (moveAction == null ||
            !_gameplayInputReader.TryReadValue(moveAction, out Vector2 move))
        {
            return;
        }

        Vector2 planarInput = Vector2.ClampMagnitude(move, 1f);
        if (planarInput.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        Quaternion yawRotation =
            Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        Vector3 planarDirection =
            yawRotation * new Vector3(planarInput.x, 0f, planarInput.y);

        _characterController.Move(
            planarDirection * (moveSpeed * Time.deltaTime));
    }

    private void ApplyLook()
    {
        if (lookAction == null ||
            !_gameplayInputReader.TryReadValue(lookAction, out Vector2 look) ||
            look.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        transform.Rotate(
            0f,
            look.x * lookSensitivity,
            0f,
            Space.Self);

        if (_observationTransform == null)
        {
            return;
        }

        _pitch = Mathf.Clamp(
            _pitch - look.y * lookSensitivity,
            minimumPitch,
            maximumPitch);

        _observationTransform.localRotation =
            Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void ValidateSetup()
    {
        if (_characterController == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion requires CharacterController on the Actor occurrence root.",
                this);
        }

        if (_gameplayInputReader == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion requires PlayerGameplayInputReader on the Actor occurrence root.",
                this);
        }

        if (moveAction == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion requires an authored Move InputActionReference.",
                this);
        }

        if (lookAction == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion requires an authored Look InputActionReference.",
                this);
        }
    }

    private bool TryResolveObservationTransform(out Transform observationTransform)
    {
        observationTransform = null;
        ActorDeclaration actorDeclaration = GetComponent<ActorDeclaration>();
        if (actorDeclaration == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion requires ActorDeclaration on the same Actor occurrence root. Look pitch is disabled; movement and yaw remain available.",
                this);
            return false;
        }

        ActorCameraSubjectAuthoring subjectAuthoring =
            actorDeclaration.GetComponent<ActorCameraSubjectAuthoring>();
        if (subjectAuthoring == null)
        {
            Debug.LogError(
                "MinimalFirstPersonLocomotion could not resolve ActorCameraSubjectAuthoring on its containing Actor occurrence. Look pitch is disabled; movement and yaw remain available.",
                this);
            return false;
        }

        if (!subjectAuthoring.TryResolveObservation(
                actorDeclaration,
                out observationTransform,
                out string issue))
        {
            Debug.LogError(
                $"MinimalFirstPersonLocomotion could not resolve the Actor occurrence ObservationTransform. Look pitch is disabled; movement and yaw remain available. {issue}",
                this);
            observationTransform = null;
            return false;
        }

        return true;
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
