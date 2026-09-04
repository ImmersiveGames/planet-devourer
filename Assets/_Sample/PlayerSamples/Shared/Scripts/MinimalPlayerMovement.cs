using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGameplayInputReader))]
    [RequireComponent(typeof(CharacterController))]
    public sealed class MinimalPlayerMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private InputActionReference moveAction;

        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

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
                moveAction == null)
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

            Quaternion yawRotation =
                Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            Vector3 worldDirection =
                yawRotation * new Vector3(planarInput.x, 0f, planarInput.y);

            _characterController.Move(
                worldDirection * (moveSpeed * Time.deltaTime));
        }

        private void ValidateSetup()
        {
            if (_characterController == null)
            {
                Debug.LogError(
                    "MinimalPlayerMovement requires CharacterController on the same Presentation GameObject.",
                    this);
            }

            if (_gameplayInputReader == null)
            {
                Debug.LogError(
                    "MinimalPlayerMovement requires PlayerGameplayInputReader on the same Presentation GameObject.",
                    this);
            }

            if (moveAction == null)
            {
                Debug.LogError(
                    "MinimalPlayerMovement requires an authored Move InputActionReference.",
                    this);
            }
        }
    }
}
