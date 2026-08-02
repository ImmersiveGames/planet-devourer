using Immersive.Framework.Actors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetDevourer.Demo02.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerActorDeclaration))]
    [RequireComponent(typeof(CharacterController))]
    public sealed class SceneProvidedPlayerMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private string moveActionName = "Move";

        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        [SerializeField, Min(0f)]
        private float rotationSpeed = 720f;

        [SerializeField]
        private float gravity = -20f;

        private PlayerActorDeclaration playerActor;
        private CharacterController characterController;

        private PlayerInput boundPlayerInput;
        private InputAction moveAction;

        private float verticalVelocity;
        private bool missingActionLogged;

        private void Awake()
        {
            playerActor = GetComponent<PlayerActorDeclaration>();
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!TryResolveEnabledMoveAction())
            {
                return;
            }

            Vector2 input = moveAction.ReadValue<Vector2>();

            Vector3 horizontalDirection =
                new Vector3(input.x, 0f, input.y);

            if (horizontalDirection.sqrMagnitude > 1f)
            {
                horizontalDirection.Normalize();
            }

            if (characterController.isGrounded &&
                verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity =
                horizontalDirection * moveSpeed;

            velocity.y = verticalVelocity;

            characterController.Move(
                velocity * Time.deltaTime);

            RotateTowards(horizontalDirection);
        }

        private bool TryResolveEnabledMoveAction()
        {
            PlayerInput currentPlayerInput =
                playerActor.PlayerInput;

            if (currentPlayerInput == null ||
                currentPlayerInput.actions == null)
            {
                boundPlayerInput = null;
                moveAction = null;
                missingActionLogged = false;
                return false;
            }

            if (currentPlayerInput != boundPlayerInput)
            {
                boundPlayerInput = currentPlayerInput;

                moveAction =
                    boundPlayerInput.actions.FindAction(
                        moveActionName,
                        throwIfNotFound: false);

                missingActionLogged = false;
            }

            if (moveAction == null)
            {
                if (!missingActionLogged)
                {
                    Debug.LogError(
                        $"[{nameof(SceneProvidedPlayerMovement)}] " +
                        $"PlayerInput does not contain an action named " +
                        $"'{moveActionName}'.",
                        this);

                    missingActionLogged = true;
                }

                return false;
            }

            // PlayerInput and future framework Input Gates own whether
            // the action is enabled. This component does not override it.
            return moveAction.enabled;
        }

        private void RotateTowards(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction,
                    Vector3.up);

            transform.rotation =
                Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime);
        }
    }
}