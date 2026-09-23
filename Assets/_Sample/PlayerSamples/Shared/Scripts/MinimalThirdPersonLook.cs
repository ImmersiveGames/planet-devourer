using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGameplayInputReader))]
    public sealed class MinimalThirdPersonLook : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private InputActionReference lookAction;

        [Header("Camera")]
        [SerializeField]
        private Transform trackingPivot;

        [Header("Look")]
        [SerializeField, Min(0f)]
        private float lookSensitivity = 0.1f;

        [SerializeField]
        private float minimumPitch = -60f;

        [SerializeField]
        private float maximumPitch = 70f;

        private IPlayerGameplayInputReader _gameplayInputReader;
        private float _pitch;

        private void Awake()
        {
            _gameplayInputReader = GetComponent<PlayerGameplayInputReader>();

            if (trackingPivot != null)
            {
                _pitch = Mathf.Clamp(
                    NormalizeSignedAngle(trackingPivot.localEulerAngles.x),
                    minimumPitch,
                    maximumPitch);
            }

            ValidateSetup();
        }

        private void Update()
        {
            if (_gameplayInputReader == null ||
                !_gameplayInputReader.GameplayReady ||
                trackingPivot == null ||
                lookAction == null)
            {
                return;
            }

            if (!_gameplayInputReader.TryReadValue(lookAction, out Vector2 look) ||
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

            trackingPivot.localRotation =
                Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void ValidateSetup()
        {
            if (_gameplayInputReader == null)
            {
                Debug.LogError(
                    "MinimalThirdPersonLook requires PlayerGameplayInputReader on the same Presentation GameObject.",
                    this);
            }

            if (lookAction == null)
            {
                Debug.LogError(
                    "MinimalThirdPersonLook requires an authored Look InputActionReference.",
                    this);
            }

            if (trackingPivot == null)
            {
                Debug.LogError(
                    "MinimalThirdPersonLook requires an authored camera tracking pivot.",
                    this);
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
