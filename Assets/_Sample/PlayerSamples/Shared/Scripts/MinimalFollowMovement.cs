using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGameplayInputReader))]
    [RequireComponent(typeof(CharacterController))]
    public sealed class MinimalFollowMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private InputActionReference moveAction;

        [Header("Movement Reference")]
        [SerializeField]
        private Transform movementReference;

        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        [SerializeField, Min(0f)]
        private float rotationSpeed = 720f;

        private CharacterController _characterController;
        private IPlayerGameplayInputReader _gameplayInputReader;

        private void Awake()
        {
            _gameplayInputReader = GetComponent<PlayerGameplayInputReader>();
            _characterController = GetComponent<CharacterController>();

            ValidateSetup();
        }

        private void Update()
        {
            if (_characterController == null ||
                !_characterController.enabled ||
                _gameplayInputReader == null ||
                !_gameplayInputReader.GameplayReady ||
                moveAction == null ||
                movementReference == null)
            {
                return;
            }

            if (!_gameplayInputReader.TryReadValue(moveAction, out Vector2 move))
            {
                return;
            }

            Vector2 planarInput = Vector2.ClampMagnitude(move, 1f);
            if (planarInput.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector3 referenceForward = movementReference.forward;
            Vector3 referenceRight = movementReference.right;

            referenceForward.y = 0f;
            referenceRight.y = 0f;

            if (referenceForward.sqrMagnitude <= 0.0001f ||
                referenceRight.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            referenceForward.Normalize();
            referenceRight.Normalize();

            Vector3 worldDirection =
                referenceRight * planarInput.x +
                referenceForward * planarInput.y;

            if (worldDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            worldDirection.Normalize();

            _characterController.Move(
                worldDirection * (moveSpeed * Time.deltaTime));

            RotateTowards(worldDirection);
        }

        private void RotateTowards(Vector3 worldDirection)
        {
            if (rotationSpeed <= 0f)
            {
                transform.rotation =
                    Quaternion.LookRotation(worldDirection, Vector3.up);

                return;
            }

            Quaternion targetRotation =
                Quaternion.LookRotation(worldDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        private void ValidateSetup()
        {
            if (_characterController == null)
            {
                Debug.LogError(
                    "MinimalFollowMovement requires CharacterController on the same Presentation GameObject.",
                    this);
            }

            if (_gameplayInputReader == null)
            {
                Debug.LogError(
                    "MinimalFollowMovement requires PlayerGameplayInputReader on the same Presentation GameObject.",
                    this);
            }

            if (moveAction == null)
            {
                Debug.LogError(
                    "MinimalFollowMovement requires an authored Move InputActionReference.",
                    this);
            }

            if (movementReference == null)
            {
                Debug.LogError(
                    "MinimalFollowMovement requires an authored movement reference Transform.",
                    this);
            }
        }
    }
}
