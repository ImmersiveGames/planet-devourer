using Immersive.Framework.PlayerParticipation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private Text nextPlayerLabel;
    [SerializeField] private Text devicePromptLabel;
    [SerializeField] private Text statusLabel;

    private bool joiningOpen;
    private bool awaitingDevice;
    private int successfulJoins;

    private Gamepad simulatedDevice1;
    private Gamepad simulatedDevice2;

    private bool joinActionEnabledByThis;
    private bool simulateDevice1ActionEnabledByThis;
    private bool simulateDevice2ActionEnabledByThis;

    private void OnEnable()
    {
        Subscribe(joinAction, OnJoinPerformed, ref joinActionEnabledByThis);
        Subscribe(simulateDevice1Action, OnSimulateDevice1Performed, ref simulateDevice1ActionEnabledByThis);
        Subscribe(simulateDevice2Action, OnSimulateDevice2Performed, ref simulateDevice2ActionEnabledByThis);
        RefreshView();
    }

    private void OnDisable()
    {
        Unsubscribe(joinAction, OnJoinPerformed, joinActionEnabledByThis);
        Unsubscribe(simulateDevice1Action, OnSimulateDevice1Performed, simulateDevice1ActionEnabledByThis);
        Unsubscribe(simulateDevice2Action, OnSimulateDevice2Performed, simulateDevice2ActionEnabledByThis);

        joinActionEnabledByThis = false;
        simulateDevice1ActionEnabledByThis = false;
        simulateDevice2ActionEnabledByThis = false;
    }

    private void OnDestroy()
    {
        RemoveSimulatedDevice(ref simulatedDevice1);
        RemoveSimulatedDevice(ref simulatedDevice2);
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

        joiningOpen = true;
        awaitingDevice = false;

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

        joiningOpen = false;
        awaitingDevice = false;

        SetStatus(result.IgnoredNoChange
            ? "Joining was already closed."
            : "Joining is closed.");

        RefreshView();
    }

    public void RequestJoin()
    {
        if (!joiningOpen)
        {
            SetStatus("Open Joining before requesting another Player.");
            return;
        }

        if (successfulJoins >= SupportedTutorialPlayers)
        {
            SetStatus("Both tutorial Players are already joined.");
            return;
        }

        awaitingDevice = true;
        SetStatus("Waiting for an input device.");
        RefreshView();
    }

    public void CancelJoinRequest()
    {
        if (!awaitingDevice)
        {
            return;
        }

        awaitingDevice = false;
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

        if (successfulJoins != 0)
        {
            SetStatus("Simulate Device 1 is reserved for the first tutorial Join. Use key 2 for the second simulated device.");
            return;
        }

        simulatedDevice1 ??= InputSystem.AddDevice<Gamepad>();
        TryJoinWithDevice(simulatedDevice1, "Simulated Gamepad 1");
    }

    private void OnSimulateDevice2Performed(InputAction.CallbackContext context)
    {
        _ = context;

        if (!CanConsumeDeviceInput())
        {
            return;
        }

        if (successfulJoins != 1)
        {
            SetStatus("Simulate Device 2 is reserved for the second tutorial Join.");
            return;
        }

        simulatedDevice2 ??= InputSystem.AddDevice<Gamepad>();
        TryJoinWithDevice(simulatedDevice2, "Simulated Gamepad 2");
    }

    private bool CanConsumeDeviceInput()
    {
        return joiningOpen &&
               awaitingDevice &&
               successfulJoins < SupportedTutorialPlayers;
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

        awaitingDevice = false;
        successfulJoins++;

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
        bool completed = successfulJoins >= SupportedTutorialPlayers;

        SetActive(joiningClosedRoot, !joiningOpen && !completed);
        SetActive(joiningOpenRoot, joiningOpen && !completed);
        SetActive(joinReadyRoot, joiningOpen && !awaitingDevice && !completed);
        SetActive(awaitingDeviceRoot, joiningOpen && awaitingDevice && !completed);
        SetActive(completedRoot, completed);

        if (nextPlayerLabel != null && !completed)
        {
            nextPlayerLabel.text = successfulJoins == 0
                ? "PLAYER 1"
                : "PLAYER 2";
        }

        if (devicePromptLabel != null && awaitingDevice)
        {
            devicePromptLabel.text = successfulJoins == 0
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
