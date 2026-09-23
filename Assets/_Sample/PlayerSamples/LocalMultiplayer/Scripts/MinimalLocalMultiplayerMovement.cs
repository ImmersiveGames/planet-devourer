using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Immersive.Framework.Samples.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerGameplayInputReader))]
    [RequireComponent(typeof(CharacterController))]
    public sealed class MinimalLocalMultiplayerMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        private InputActionReference moveAction;

        [Header("Movement")]
        [SerializeField, Min(0f)]
        private float moveSpeed = 4f;

        private IPlayerGameplayInputReader _gameplayInputReader;
        private CharacterController _characterController;

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

            Vector3 worldDirection = new Vector3(
                planarInput.x,
                0f,
                planarInput.y);

            _characterController.Move(
                worldDirection * (moveSpeed * Time.deltaTime));

            transform.rotation = Quaternion.LookRotation(
                worldDirection,
                Vector3.up);
        }

        private void ValidateSetup()
        {
            if (_gameplayInputReader == null)
            {
                Debug.LogError(
                    $"{nameof(MinimalLocalMultiplayerMovement)} requires " +
                    $"{nameof(PlayerGameplayInputReader)} on the same Presentation GameObject.",
                    this);
            }

            if (_characterController == null)
            {
                Debug.LogError(
                    $"{nameof(MinimalLocalMultiplayerMovement)} requires " +
                    $"{nameof(CharacterController)} on the same Presentation GameObject.",
                    this);
            }

            if (moveAction == null)
            {
                Debug.LogError(
                    $"{nameof(MinimalLocalMultiplayerMovement)} requires an authored Move InputActionReference.",
                    this);
            }
        }
    }
}