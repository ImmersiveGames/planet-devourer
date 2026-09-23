using System;
using UnityEngine;
using UnityEngine.InputSystem;

    /// <summary>
    /// Owns pre-player Joining input observation only: it isolates a runtime-only
    /// clone of the authored InputActionAsset (so PlayerInput device pairing on
    /// later Players never restricts this listener) and reports device intent to
    /// consumers. Ownership of the simulated test Gamepads and their keyboard
    /// movement projection lives in LocalMultiplayerKeyboardGamepadSimulator. This
    /// component has no knowledge of tutorial/UI state or Framework Join semantics.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocalMultiplayerJoinInputSource : MonoBehaviour
    {
        [Header("Joining Input")]
        [SerializeField] private InputActionReference joinAction;
        [SerializeField] private InputActionReference simulateDevice1Action;
        [SerializeField] private InputActionReference simulateDevice2Action;

        [Header("Simulated Devices")]
        [SerializeField] private LocalMultiplayerKeyboardGamepadSimulator keyboardGamepadSimulator;

        /// <summary>
        /// Raised whenever a device requests to join. The string is a
        /// presentation/diagnostic label only (e.g. "Simulated Gamepad 1") and
        /// carries no Player Slot identity.
        /// </summary>
        public event Action<InputDevice, string> DeviceRequested;

        // Runtime-only clone of the authored InputActionAsset. The first
        // PlayerInput uses the authored actions instance as-is and restricts its
        // devices, so this listener must observe unpaired devices through its own
        // clone instead of subscribing to InputActionReference.action directly.
        private InputActionAsset _runtimeActions;
        private InputAction _runtimeJoinAction;
        private InputAction _runtimeSimulateDevice1Action;
        private InputAction _runtimeSimulateDevice2Action;

        private void OnEnable()
        {
            InitializeRuntimeJoiningActions();
        }

        private void OnDisable()
        {
            TeardownRuntimeJoiningActions();
        }

        private void InitializeRuntimeJoiningActions()
        {
            InputAction sourceJoinAction = joinAction != null ? joinAction.action : null;
            InputAction sourceSimulateDevice1Action = simulateDevice1Action != null ? simulateDevice1Action.action : null;
            InputAction sourceSimulateDevice2Action = simulateDevice2Action != null ? simulateDevice2Action.action : null;

            InputActionAsset sourceAsset = ResolveSharedAsset(
                sourceJoinAction, sourceSimulateDevice1Action, sourceSimulateDevice2Action);

            if (sourceAsset == null)
            {
                Debug.Log(
                    "[FG_LOCAL_MULTIPLAYER_JOIN_INPUT] " +
                    "Joining actions are not assigned to a common InputActionAsset.");
                return;
            }

            _runtimeActions = Instantiate(sourceAsset);

            _runtimeJoinAction = ResolveRuntimeAction(_runtimeActions, sourceJoinAction);
            _runtimeSimulateDevice1Action = ResolveRuntimeAction(_runtimeActions, sourceSimulateDevice1Action);
            _runtimeSimulateDevice2Action = ResolveRuntimeAction(_runtimeActions, sourceSimulateDevice2Action);

            if (_runtimeJoinAction == null ||
                _runtimeSimulateDevice1Action == null ||
                _runtimeSimulateDevice2Action == null)
            {
                Debug.Log(
                    "[FG_LOCAL_MULTIPLAYER_JOIN_INPUT] " +
                    "Failed to resolve runtime Joining actions from the cloned InputActionAsset.");
                TeardownRuntimeJoiningActions();
                return;
            }

            _runtimeJoinAction.performed += OnJoinPerformed;
            _runtimeSimulateDevice1Action.performed += OnSimulateDevice1Performed;
            _runtimeSimulateDevice2Action.performed += OnSimulateDevice2Performed;

            _runtimeJoinAction.Enable();
            _runtimeSimulateDevice1Action.Enable();
            _runtimeSimulateDevice2Action.Enable();
        }

        private void TeardownRuntimeJoiningActions()
        {
            if (_runtimeJoinAction != null)
            {
                _runtimeJoinAction.performed -= OnJoinPerformed;
                _runtimeJoinAction.Disable();
                _runtimeJoinAction = null;
            }

            if (_runtimeSimulateDevice1Action != null)
            {
                _runtimeSimulateDevice1Action.performed -= OnSimulateDevice1Performed;
                _runtimeSimulateDevice1Action.Disable();
                _runtimeSimulateDevice1Action = null;
            }

            if (_runtimeSimulateDevice2Action != null)
            {
                _runtimeSimulateDevice2Action.performed -= OnSimulateDevice2Performed;
                _runtimeSimulateDevice2Action.Disable();
                _runtimeSimulateDevice2Action = null;
            }

            if (_runtimeActions != null)
            {
                Destroy(_runtimeActions);
                _runtimeActions = null;
            }
        }

        private void OnJoinPerformed(InputAction.CallbackContext context)
        {
            InputDevice device = context.control?.device;
            if (device == null || !device.added)
            {
                return;
            }

            DeviceRequested?.Invoke(device, device.displayName);
        }

        private void OnSimulateDevice1Performed(InputAction.CallbackContext context)
        {
            _ = context;

            if (keyboardGamepadSimulator == null)
            {
                Debug.LogError(
                    "[FG_LOCAL_MULTIPLAYER_JOIN_INPUT] " +
                    "Keyboard Gamepad Simulator is not assigned; cannot simulate Device 1.");
                return;
            }

            Gamepad device = keyboardGamepadSimulator.GetOrCreateDevice1();
            DeviceRequested?.Invoke(device, "Simulated Gamepad 1");
        }

        private void OnSimulateDevice2Performed(InputAction.CallbackContext context)
        {
            _ = context;

            if (keyboardGamepadSimulator == null)
            {
                Debug.LogError(
                    "[FG_LOCAL_MULTIPLAYER_JOIN_INPUT] " +
                    "Keyboard Gamepad Simulator is not assigned; cannot simulate Device 2.");
                return;
            }

            Gamepad device = keyboardGamepadSimulator.GetOrCreateDevice2();
            DeviceRequested?.Invoke(device, "Simulated Gamepad 2");
        }

        private static InputActionAsset ResolveSharedAsset(
            InputAction joinSourceAction,
            InputAction simulateDevice1SourceAction,
            InputAction simulateDevice2SourceAction)
        {
            if (joinSourceAction == null || simulateDevice1SourceAction == null || simulateDevice2SourceAction == null)
            {
                return null;
            }

            InputActionAsset asset = joinSourceAction.actionMap != null ? joinSourceAction.actionMap.asset : null;
            if (asset == null)
            {
                return null;
            }

            InputActionAsset simulateDevice1Asset = simulateDevice1SourceAction.actionMap != null
                ? simulateDevice1SourceAction.actionMap.asset
                : null;
            InputActionAsset simulateDevice2Asset = simulateDevice2SourceAction.actionMap != null
                ? simulateDevice2SourceAction.actionMap.asset
                : null;

            if (simulateDevice1Asset != asset || simulateDevice2Asset != asset)
            {
                return null;
            }

            return asset;
        }

        private static InputAction ResolveRuntimeAction(InputActionAsset runtimeAsset, InputAction sourceAction)
        {
            if (runtimeAsset == null || sourceAction == null)
            {
                return null;
            }

            Guid targetId = sourceAction.id;

            foreach (InputActionMap map in runtimeAsset.actionMaps)
            {
                foreach (InputAction candidate in map.actions)
                {
                    if (candidate.id == targetId)
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }
    }
