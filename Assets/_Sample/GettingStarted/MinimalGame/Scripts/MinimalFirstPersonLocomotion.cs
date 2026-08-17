using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.GettingStarted
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class MinimalFirstPersonLocomotion : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform cameraMount;

        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        [Header("Look")]
        [SerializeField, Min(0f)]
        private float lookSensitivity = 0.1f;

        [SerializeField, Range(0f, 89f)]
        private float maxPitch = 85f;

        private CharacterController characterController;
        private InputAction moveAction;
        private InputAction lookAction;
        private float pitch;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            PlayerInput playerInput = GetComponent<PlayerInput>();

            if (cameraMount == null)
            {
                Debug.LogError(
                    $"{nameof(MinimalFirstPersonLocomotion)} requires an explicit Camera Mount.",
                    this);

                enabled = false;
                return;
            }

            moveAction = playerInput.actions.FindAction("Move", true);
            lookAction = playerInput.actions.FindAction("Look", true);
        }

        private void Update()
        {
            Vector2 move = moveAction.ReadValue<Vector2>();

            Vector3 worldMove =
                transform.right * move.x +
                transform.forward * move.y;

            if (worldMove.sqrMagnitude > 1f)
            {
                worldMove.Normalize();
            }

            characterController.SimpleMove(worldMove * moveSpeed);

            Vector2 look = lookAction.ReadValue<Vector2>();

            transform.Rotate(
                0f,
                look.x * lookSensitivity,
                0f,
                Space.Self);

            pitch = Mathf.Clamp(
                pitch - look.y * lookSensitivity,
                -maxPitch,
                maxPitch);

            cameraMount.localRotation =
                Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}
