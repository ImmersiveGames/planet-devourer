using Immersive.Framework.PlayerParticipation;
using Immersive.Logging.Loggers;
using Immersive.Logging.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _Sample.GettingStarted.MinimalGame.Scripts
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class MinimalFirstPersonLocomotion : MonoBehaviour
    {
        private static readonly ScopedLogger Log =
            UnityLoggingFactory.CreateLogger()
                .For<MinimalFirstPersonLocomotion>("Immersive.Sample00");

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

        private CharacterController _characterController;
        private IPlayerGameplayInputReader _gameplayInputReader;
        private float _pitch;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _gameplayInputReader = GetComponent<PlayerGameplayInputReader>();

            if (cameraMount != null)
            {
                _pitch = Mathf.Clamp(
                    NormalizeSignedAngle(cameraMount.localEulerAngles.x),
                    minimumPitch,
                    maximumPitch);
            }

            ValidateSetup();
        }

        private void Update()
        {
            if (_characterController == null ||
                _gameplayInputReader == null ||
                !_gameplayInputReader.GameplayReady)
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

            var planarInput = Vector2.ClampMagnitude(move, 1f);
            if (planarInput.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var yawRotation =
                Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            var planarDirection =
                yawRotation * new Vector3(planarInput.x, 0f, planarInput.y);

            _characterController.Move(
                planarDirection * (moveSpeed * Time.deltaTime));
        }

        private void ApplyLook()
        {
            if (lookAction == null ||
                cameraMount == null ||
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

            _pitch = Mathf.Clamp(
                _pitch - look.y * lookSensitivity,
                minimumPitch,
                maximumPitch);

            cameraMount.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void ValidateSetup()
        {
            if (_characterController == null)
            {
                Log.Error("CharacterController is missing.");
            }

            if (_gameplayInputReader == null)
            {
                Log.Error("PlayerGameplayInputReader is missing on the same GameObject.");
            }

            if (moveAction == null)
            {
                Log.Error("Move InputActionReference is not assigned.");
            }

            if (lookAction == null)
            {
                Log.Error("Look InputActionReference is not assigned.");
            }

            if (cameraMount == null)
            {
                Log.Error("CameraMount is not assigned.");
            }
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
}
