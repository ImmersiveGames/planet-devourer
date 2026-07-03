using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame
{
    /// <summary>
    /// Minimal consumer-side movement prototype for FIRSTGAME.
    /// Not a framework component.
    /// 
    /// Expected Input Actions:
    /// - Move: Vector2
    /// - Optional: Reset/Pause handled elsewhere by framework triggers.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGamePlayerMover : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private string actionMapName = "Player";
        [SerializeField] private string moveActionName = "Move";

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private bool useLocalSpace;
        [SerializeField] private bool lockY = true;

        [Header("Optional Rigidbody")]
        [SerializeField] private Rigidbody targetRigidbody;

        private InputAction _moveAction;
        private Vector3 _initialPosition;

        private void Reset()
        {
            playerInput = GetComponent<PlayerInput>();
            targetRigidbody = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            _initialPosition = transform.position;

            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }

            if (targetRigidbody == null)
            {
                targetRigidbody = GetComponent<Rigidbody>();
            }

            ResolveMoveAction();
        }

        private void OnEnable()
        {
            ResolveMoveAction();
            _moveAction?.Enable();
        }

        private void OnDisable()
        {
            _moveAction?.Disable();
        }

        private void Update()
        {
            if (targetRigidbody != null)
            {
                return;
            }

            Vector3 delta = BuildMovementDelta(Time.deltaTime);
            transform.position += delta;
        }

        private void FixedUpdate()
        {
            if (targetRigidbody == null)
            {
                return;
            }

            Vector3 delta = BuildMovementDelta(Time.fixedDeltaTime);
            targetRigidbody.MovePosition(targetRigidbody.position + delta);
        }

        [ContextMenu("Reset To Initial Position")]
        public void ResetToInitialPosition()
        {
            if (targetRigidbody != null)
            {
                targetRigidbody.linearVelocity = Vector3.zero;
                targetRigidbody.angularVelocity = Vector3.zero;
                targetRigidbody.position = _initialPosition;
                return;
            }

            transform.position = _initialPosition;
        }

        private Vector3 BuildMovementDelta(float deltaTime)
        {
            Vector2 input = _moveAction != null
                ? _moveAction.ReadValue<Vector2>()
                : Vector2.zero;

            Vector3 direction = new(input.x, 0f, input.y);

            if (!lockY)
            {
                direction = new Vector3(input.x, input.y, 0f);
            }

            if (direction.sqrMagnitude > 1f)
            {
                direction.Normalize();
            }

            if (useLocalSpace)
            {
                direction = transform.TransformDirection(direction);
            }

            return moveSpeed * deltaTime * direction;
        }

        private void ResolveMoveAction()
        {
            _moveAction = null;

            if (playerInput == null || playerInput.actions == null)
            {
                Debug.LogWarning(
                    "[FirstGame][FirstGamePlayerMover] PlayerInput or InputActionAsset is missing.",
                    this);
                return;
            }

            InputActionMap map = playerInput.actions.FindActionMap(actionMapName, throwIfNotFound: false);
            if (map == null)
            {
                Debug.LogWarning(
                    $"[FirstGame][FirstGamePlayerMover] Action map '{actionMapName}' was not found.",
                    this);
                return;
            }

            _moveAction = map.FindAction(moveActionName, throwIfNotFound: false);
            if (_moveAction == null)
            {
                Debug.LogWarning(
                    $"[FirstGame][FirstGamePlayerMover] Move action '{actionMapName}/{moveActionName}' was not found.",
                    this);
            }
        }
    }
}