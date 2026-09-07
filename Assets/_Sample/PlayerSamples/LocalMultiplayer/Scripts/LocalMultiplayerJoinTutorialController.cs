using Immersive.Framework.PlayerParticipation;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
namespace _Sample.PlayerSamples.LocalMultiplayer.Scripts
{
    [DisallowMultipleComponent]
    public sealed class LocalMultiplayerJoinTutorialController : MonoBehaviour
    {
        private const int SupportedTutorialPlayers = 2;

        [Header("Framework Commands")]
        [SerializeField] private PlayerSessionOpenJoiningCommandTrigger openJoiningCommand;
        [SerializeField] private PlayerSessionCloseJoiningCommandTrigger closeJoiningCommand;
        [SerializeField] private PlayerSessionJoinCommandTrigger joinCommand;

        [Header("Joining Input")]
        [SerializeField] private InputActionReference joinAction;
        [SerializeField] private InputActionReference simulateDevice1Action;
        [SerializeField] private InputActionReference simulateDevice2Action;

        [Header("Tutorial Views")]
        [SerializeField] private GameObject joiningClosedRoot;
        [SerializeField] private GameObject joiningOpenRoot;
        [SerializeField] private GameObject joinReadyRoot;
        [SerializeField] private GameObject awaitingDeviceRoot;
        [SerializeField] private GameObject completedRoot;

        [Header("Tutorial Labels")]
        [SerializeField] private TMP_Text nextPlayerLabel;
        [SerializeField] private TMP_Text devicePromptLabel;
        [SerializeField] private TMP_Text statusLabel;

        private bool _joiningOpen;
        private bool _awaitingDevice;
        private int _successfulJoins;

        private Gamepad _simulatedDevice1;
        private Gamepad _simulatedDevice2;

        private bool _joinActionEnabledByThis;
        private bool _simulateDevice1ActionEnabledByThis;
        private bool _simulateDevice2ActionEnabledByThis;

        private void OnEnable()
        {
            Subscribe(joinAction, OnJoinPerformed, ref _joinActionEnabledByThis);
            Subscribe(simulateDevice1Action, OnSimulateDevice1Performed, ref _simulateDevice1ActionEnabledByThis);
            Subscribe(simulateDevice2Action, OnSimulateDevice2Performed, ref _simulateDevice2ActionEnabledByThis);
            RefreshView();
        }

        private void OnDisable()
        {
            Unsubscribe(joinAction, OnJoinPerformed, _joinActionEnabledByThis);
            Unsubscribe(simulateDevice1Action, OnSimulateDevice1Performed, _simulateDevice1ActionEnabledByThis);
            Unsubscribe(simulateDevice2Action, OnSimulateDevice2Performed, _simulateDevice2ActionEnabledByThis);

            _joinActionEnabledByThis = false;
            _simulateDevice1ActionEnabledByThis = false;
            _simulateDevice2ActionEnabledByThis = false;
        }

        private void OnDestroy()
        {
            RemoveSimulatedDevice(ref _simulatedDevice1);
            RemoveSimulatedDevice(ref _simulatedDevice2);
        }

        public void OpenJoining()
        {
            if (openJoiningCommand == null)
            {
                SetStatus("Open Joining command is not assigned.");
                return;
            }

            openJoiningCommand.Invoke();

            PlayerParticipationOperationResult result = openJoiningCommand.LastOpenJoiningResult;
            if (result == null || !result.Completed)
            {
                SetStatus(result != null
                    ? $"Open Joining failed: {result.Status}. {result.Message}"
                    : "Open Joining returned no result.");
                return;
            }

            _joiningOpen = true;
            _awaitingDevice = false;

            SetStatus(result.IgnoredNoChange
                ? "Joining was already open."
                : "Joining is open.");

            RefreshView();
        }

        public void CloseJoining()
        {
            if (closeJoiningCommand == null)
            {
                SetStatus("Close Joining command is not assigned.");
                return;
            }

            closeJoiningCommand.Invoke();

            PlayerParticipationOperationResult result = closeJoiningCommand.LastCloseJoiningResult;
            if (result == null || !result.Completed)
            {
                SetStatus(result != null
                    ? $"Close Joining failed: {result.Status}. {result.Message}"
                    : "Close Joining returned no result.");
                return;
            }

            _joiningOpen = false;
            _awaitingDevice = false;

            SetStatus(result.IgnoredNoChange
                ? "Joining was already closed."
                : "Joining is closed.");

            RefreshView();
        }

        public void RequestJoin()
        {
            if (!_joiningOpen)
            {
                SetStatus("Open Joining before requesting another Player.");
                return;
            }

            if (_successfulJoins >= SupportedTutorialPlayers)
            {
                SetStatus("Both tutorial Players are already joined.");
                return;
            }

            _awaitingDevice = true;
            SetStatus("Waiting for an input device.");
            RefreshView();
        }

        public void CancelJoinRequest()
        {
            if (!_awaitingDevice)
            {
                return;
            }

            _awaitingDevice = false;
            SetStatus("Join request cancelled.");
            RefreshView();
        }

        private void OnJoinPerformed(InputAction.CallbackContext context)
        {
            if (!CanConsumeDeviceInput())
            {
                return;
            }

            InputDevice device = context.control?.device;
            if (device == null || !device.added)
            {
                SetStatus("Join input did not provide a valid added InputDevice.");
                return;
            }

            TryJoinWithDevice(device, device.displayName);
        }

        private void OnSimulateDevice1Performed(InputAction.CallbackContext context)
        {
            _ = context;

            if (!CanConsumeDeviceInput())
            {
                return;
            }

            if (_successfulJoins != 0)
            {
                SetStatus("Simulate Device 1 is reserved for the first tutorial Join. Use key 2 for the second simulated device.");
                return;
            }

            _simulatedDevice1 ??= InputSystem.AddDevice<Gamepad>();
            TryJoinWithDevice(_simulatedDevice1, "Simulated Gamepad 1");
        }

        private void OnSimulateDevice2Performed(InputAction.CallbackContext context)
        {
            _ = context;

            if (!CanConsumeDeviceInput())
            {
                return;
            }

            if (_successfulJoins != 1)
            {
                SetStatus("Simulate Device 2 is reserved for the second tutorial Join.");
                return;
            }

            _simulatedDevice2 ??= InputSystem.AddDevice<Gamepad>();
            TryJoinWithDevice(_simulatedDevice2, "Simulated Gamepad 2");
        }

        private bool CanConsumeDeviceInput()
        {
            return _joiningOpen &&
                _awaitingDevice &&
                _successfulJoins < SupportedTutorialPlayers;
        }

        private void TryJoinWithDevice(InputDevice device, string deviceLabel)
        {
            if (joinCommand == null)
            {
                SetStatus("Join command is not assigned.");
                return;
            }

            if (device == null || !device.added)
            {
                SetStatus("Join requires a valid added InputDevice.");
                return;
            }

            joinCommand.InvokeFromDevice(device);

            LocalPlayerJoinResult result = joinCommand.LastJoinResult;
            if (result == null)
            {
                SetStatus("Join returned no result.");
                return;
            }

            if (!result.Succeeded)
            {
                SetStatus($"Join rejected: {result.Status}. {result.Message}");
                RefreshView();
                return;
            }

            _awaitingDevice = false;
            _successfulJoins++;

            string slotId = result.Slot.PlayerSlotId.IsValid
                ? result.Slot.PlayerSlotId.StableText
                : "<invalid-slot>";

            SetStatus(
                $"{slotId} joined with {deviceLabel} " +
                $"(deviceId={device.deviceId}, playerIndex={result.UnityPlayerIndex}).");

            Debug.Log(
                "[FG_LOCAL_MULTIPLAYER_TUTORIAL] " +
                $"status='Joined' slot='{slotId}' " +
                $"device='{deviceLabel}' deviceId='{device.deviceId}' " +
                $"playerIndex='{result.UnityPlayerIndex}'.");

            RefreshView();
        }

        private void RefreshView()
        {
            bool completed = _successfulJoins >= SupportedTutorialPlayers;

            SetActive(joiningClosedRoot, !_joiningOpen && !completed);
            SetActive(joiningOpenRoot, _joiningOpen && !completed);
            SetActive(joinReadyRoot, _joiningOpen && !_awaitingDevice && !completed);
            SetActive(awaitingDeviceRoot, _joiningOpen && _awaitingDevice && !completed);
            SetActive(completedRoot, completed);

            if (nextPlayerLabel != null && !completed)
            {
                nextPlayerLabel.text = _successfulJoins == 0
                    ? "PLAYER 1"
                    : "PLAYER 2";
            }

            if (devicePromptLabel != null && _awaitingDevice)
            {
                devicePromptLabel.text = _successfulJoins == 0
                    ? "INPUT DEVICE REQUIRED\nPress START on a gamepad or press 1 to simulate Device 1."
                    : "INPUT DEVICE REQUIRED\nPress START on another gamepad or press 2 to simulate Device 2.";
            }
        }

        private void SetStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.text = message;
            }

            Debug.Log($"[FG_LOCAL_MULTIPLAYER_TUTORIAL] {message}");
        }

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null && target.activeSelf != active)
            {
                target.SetActive(active);
            }
        }

        private static void Subscribe(
            InputActionReference reference,
            System.Action<InputAction.CallbackContext> callback,
            ref bool enabledByThis)
        {
            enabledByThis = false;

            InputAction action = reference != null ? reference.action : null;
            if (action == null)
            {
                return;
            }

            action.performed += callback;

            if (!action.enabled)
            {
                action.Enable();
                enabledByThis = true;
            }
        }

        private static void Unsubscribe(
            InputActionReference reference,
            System.Action<InputAction.CallbackContext> callback,
            bool enabledByThis)
        {
            InputAction action = reference != null ? reference.action : null;
            if (action == null)
            {
                return;
            }

            action.performed -= callback;

            if (enabledByThis && action.enabled)
            {
                action.Disable();
            }
        }

        private static void RemoveSimulatedDevice(ref Gamepad device)
        {
            if (device != null && device.added)
            {
                InputSystem.RemoveDevice(device);
            }

            device = null;
        }
    }
}
