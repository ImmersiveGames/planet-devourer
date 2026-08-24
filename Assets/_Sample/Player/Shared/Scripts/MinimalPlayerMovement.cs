using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerGameplayInputConsumerBinding))]
    public sealed class MinimalPlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference moveAction;

        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        private CharacterController _characterController;
        private PlayerGameplayInputConsumerBinding _gameplayInput;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _gameplayInput = GetComponent<PlayerGameplayInputConsumerBinding>();
        }

        private void Update()
        {
            if (_characterController == null ||
                _gameplayInput == null ||
                !_gameplayInput.GameplayReady ||
                moveAction == null)
            {
                return;
            }

            if (!_gameplayInput.TryReadValue(moveAction, out Vector2 move))
            {
                return;
            }

            move = Vector2.ClampMagnitude(move, 1f);

            if (move.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector3 localDirection =
                new Vector3(move.x, 0f, move.y);

            Vector3 worldDirection =
                transform.TransformDirection(localDirection);

            _characterController.Move(
                worldDirection * (moveSpeed * Time.deltaTime));
        }
    }
}