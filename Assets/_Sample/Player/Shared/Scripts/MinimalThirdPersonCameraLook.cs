using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGameplayInputConsumerBinding))]
    public sealed class MinimalThirdPersonLook : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference lookAction;

        [SerializeField]
        private Transform trackingPivot;

        [SerializeField, Min(0f)]
        private float lookSensitivity = 0.1f;

        [SerializeField]
        private float minimumPitch = -60f;

        [SerializeField]
        private float maximumPitch = 70f;

        private PlayerGameplayInputConsumerBinding _gameplayInput;
        private float _pitch;

        private void Awake()
        {
            _gameplayInput =
                GetComponent<PlayerGameplayInputConsumerBinding>();

            if (trackingPivot != null)
            {
                _pitch = Mathf.Clamp(
                    NormalizeSignedAngle(
                        trackingPivot.localEulerAngles.x),
                    minimumPitch,
                    maximumPitch);
            }
        }

        private void Update()
        {
            if (_gameplayInput == null ||
                !_gameplayInput.GameplayReady ||
                trackingPivot == null ||
                lookAction == null)
            {
                return;
            }

            if (!_gameplayInput.TryReadValue(
                    lookAction,
                    out Vector2 look))
            {
                return;
            }

            if (look.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            // Horizontal look rotates the physical Player.
            transform.Rotate(
                0f,
                look.x * lookSensitivity,
                0f,
                Space.Self);

            // Vertical look belongs only to the Camera pivot.
            _pitch = Mathf.Clamp(
                _pitch - look.y * lookSensitivity,
                minimumPitch,
                maximumPitch);

            trackingPivot.localRotation =
                Quaternion.Euler(
                    _pitch,
                    0f,
                    0f);
        }

        private void OnValidate()
        {
            if (maximumPitch < minimumPitch)
            {
                maximumPitch = minimumPitch;
            }
        }

        private static float NormalizeSignedAngle(
            float angle)
        {
            return angle > 180f
                ? angle - 360f
                : angle;
        }
    }
}