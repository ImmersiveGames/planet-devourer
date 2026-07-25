using UnityEngine;
using UnityEngine.InputSystem;

namespace PlanetDevourer.Player
{
    /// <summary>
    /// Minimal consumer-side movement example for a Scene-Provided Logical Player Actor.
    ///
    /// The CharacterController belongs to the Actor.
    /// The PlayerInput belongs to the outer Local Player Host and is assigned explicitly
    /// by the composed Player prefab.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneProvidedPlayerMovement : MonoBehaviour
    {
        [Header("Explicit Bindings")]
        [SerializeField]
        [Tooltip("PlayerInput owned by the outer Local Player Host. This reference is assigned in the composed Player prefab.")]
        private PlayerInput playerInput;

        [SerializeField]
        [Tooltip("CharacterController owned by this Logical Player Actor.")]
        private CharacterController characterController;

        [Header("Movement")]
        [SerializeField]
        [Tooltip("Action name resolved from the PlayerInput instance-specific actions. A map-qualified name such as 'Player/Move' is also valid.")]
        private string moveActionName = "Move";

        [SerializeField, Min(0f)]
        private float moveSpeed = 5f;

        [SerializeField]
        [Tooltip("Downward acceleration applied through CharacterController.Move.")]
        private float gravity = -20f;

        [SerializeField]
        [Tooltip("Small downward velocity retained while grounded.")]
        private float groundedVerticalVelocity = -2f;

        private InputAction moveAction;
        private float verticalVelocity;
        private bool initialized;

        public PlayerInput PlayerInput => playerInput;

        public CharacterController CharacterController => characterController;

        private void OnEnable()
        {
            if (TryInitialize(out string issue))
            {
                return;
            }

            Debug.LogError(
                $"[FIRSTGAME][SceneProvidedPlayerMovement] Movement initialization failed on '{name}'. {issue}",
                this);
            enabled = false;
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            Vector2 input =
                moveAction.enabled
                    ? moveAction.ReadValue<Vector2>()
                    : Vector2.zero;

            Vector3 planarVelocity = new Vector3(input.x, 0f, input.y);
            if (planarVelocity.sqrMagnitude > 1f)
            {
                planarVelocity.Normalize();
            }

            planarVelocity *= moveSpeed;

            if (characterController.isGrounded &&
                verticalVelocity < 0f)
            {
                verticalVelocity = groundedVerticalVelocity;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 velocity = planarVelocity;
            velocity.y = verticalVelocity;

            characterController.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Explicit runtime binding hook for future composition adapters.
        /// It does not search the hierarchy or use a global service.
        /// </summary>
        public bool TryBindPlayerInput(
            PlayerInput input,
            out string issue)
        {
            playerInput = input;
            bool succeeded = TryInitialize(out issue);
            if (succeeded && !enabled)
            {
                enabled = true;
            }

            return succeeded;
        }

        private bool TryInitialize(out string issue)
        {
            initialized = false;
            moveAction = null;

            if (playerInput == null)
            {
                issue =
                    "Player Input is missing. Assign the PlayerInput owned by the outer Local Player Host.";
                return false;
            }

            if (characterController == null)
            {
                issue =
                    "Character Controller is missing. Assign the CharacterController owned by this Actor.";
                return false;
            }

            if (playerInput.actions == null)
            {
                issue =
                    $"PlayerInput '{playerInput.name}' has no Input Actions asset.";
                return false;
            }

            string normalizedActionName =
                string.IsNullOrWhiteSpace(moveActionName)
                    ? string.Empty
                    : moveActionName.Trim();

            if (string.IsNullOrEmpty(normalizedActionName))
            {
                issue = "Move Action Name is missing.";
                return false;
            }

            moveAction =
                playerInput.actions.FindAction(
                    normalizedActionName,
                    false);

            if (moveAction == null)
            {
                issue =
                    $"Move action '{normalizedActionName}' was not found in PlayerInput '{playerInput.name}'.";
                return false;
            }

            initialized = true;
            issue = string.Empty;
            return true;
        }

        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);

            if (gravity > 0f)
            {
                gravity = -gravity;
            }

            if (groundedVerticalVelocity > 0f)
            {
                groundedVerticalVelocity = -groundedVerticalVelocity;
            }
        }
    }
}
