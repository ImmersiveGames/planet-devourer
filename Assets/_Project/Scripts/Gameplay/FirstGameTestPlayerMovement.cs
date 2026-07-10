using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterController))]
    [AddComponentMenu("FIRSTGAME/Gameplay/Test Player Movement")]
    public sealed class FirstGameTestPlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private CharacterController characterController;

        [Header("Input")]
        [SerializeField] private string actionMapName = "Player";
        [SerializeField] private string moveActionName = "Move";

        [Header("Movement")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 5f;

        [Min(0f)]
        [SerializeField] private float gravity = 20f;

        [SerializeField] private bool faceMovementDirection = true;

        [Min(0f)]
        [SerializeField] private float rotationSpeed = 720f;

        private InputAction moveAction;
        private float verticalVelocity;

        private void Reset()
        {
            playerInput = GetComponent<PlayerInput>();
            characterController = GetComponent<CharacterController>();
        }

        private void OnValidate()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
        }

        private void OnEnable()
        {
            ResolveReferences();
            ResolveMoveAction();
        }

        private void Update()
        {
            if (moveAction == null || !moveAction.enabled)
            {
                ApplyGravityOnly();
                return;
            }

            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 planarDirection = new Vector3(input.x, 0f, input.y);

            if (planarDirection.sqrMagnitude > 1f)
            {
                planarDirection.Normalize();
            }

            UpdateVerticalVelocity();

            Vector3 velocity =
                planarDirection * moveSpeed
                + Vector3.up * verticalVelocity;

            characterController.Move(
                velocity * Time.deltaTime);

            if (faceMovementDirection
                && planarDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(
                        planarDirection,
                        Vector3.up);

                transform.rotation =
                    Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime);
            }
        }

        private void ResolveReferences()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (playerInput == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} requires PlayerInput.");
            }

            if (characterController == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} requires CharacterController.");
            }

            if (playerInput.actions == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} requires PlayerInput.actions.");
            }
        }

        private void ResolveMoveAction()
        {
            InputActionMap actionMap =
                playerInput.actions.FindActionMap(
                    actionMapName,
                    throwIfNotFound: false);

            if (actionMap == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} could not find action map " +
                    $"'{actionMapName}' in PlayerInput.actions.");
            }

            moveAction =
                actionMap.FindAction(
                    moveActionName,
                    throwIfNotFound: false);

            if (moveAction == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} could not find action " +
                    $"'{actionMapName}/{moveActionName}'.");
            }

            if (moveAction.type != InputActionType.Value
                && moveAction.type != InputActionType.PassThrough)
            {
                throw new InvalidOperationException(
                    $"{nameof(FirstGameTestPlayerMovement)} requires " +
                    $"'{actionMapName}/{moveActionName}' to be Value or PassThrough.");
            }
        }

        private void ApplyGravityOnly()
        {
            UpdateVerticalVelocity();

            characterController.Move(
                Vector3.up
                * verticalVelocity
                * Time.deltaTime);
        }

        private void UpdateVerticalVelocity()
        {
            if (characterController.isGrounded
                && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
                return;
            }

            verticalVelocity -= gravity * Time.deltaTime;
        }
    }
}
