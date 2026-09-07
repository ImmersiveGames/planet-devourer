using Immersive.Framework.PlayerParticipation;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class LocalMultiplayerJoinTutorialController : MonoBehaviour
{
    private const int PlayerOneConfiguredIndex = 0;
    private const int PlayerTwoConfiguredIndex = 1;

    [Header("Framework Commands")]
    [SerializeField] private PlayerSessionOpenJoiningCommandTrigger openJoiningCommand;
    [SerializeField] private PlayerSessionCloseJoiningCommandTrigger closeJoiningCommand;
    [SerializeField] private PlayerSessionJoinCommandTrigger joinCommand;

    [Header("Input Source")]
    [SerializeField] private LocalMultiplayerJoinInputSource joinInputSource;

    [Header("Tutorial Views")]
    [SerializeField] private GameObject joiningClosedRoot;
    [SerializeField] private GameObject joiningOpenRoot;
    [SerializeField] private GameObject joinReadyRoot;
    [SerializeField] private GameObject awaitingDeviceRoot;
    [SerializeField] private GameObject completedRoot;
    [SerializeField] private GameObject leavePlayer1Button;
    [SerializeField] private GameObject leavePlayer2Button;

    [Header("Tutorial Labels")]
    [SerializeField] private TMP_Text nextPlayerLabel;
    [SerializeField] private TMP_Text devicePromptLabel;
    [SerializeField] private TMP_Text statusLabel;

    private IPlayerSessionScopedAccess _sessionAccess;
    private bool _joiningOpen;
    private bool _awaitingDevice;
    private bool _playerOneJoined;
    private bool _playerTwoJoined;

    private void OnEnable()
    {
        if (joinInputSource != null)
        {
            joinInputSource.DeviceRequested += OnDeviceRequested;
        }

        TryBindSessionAccess();
        RefreshView();
    }

    private void Update()
    {
        // Scoped consumers are bound after scene composition. Retry only until
        // access becomes available; after that all state updates are event-driven.
        if (_sessionAccess == null && TryBindSessionAccess())
        {
            RefreshView();
        }
    }

    private void OnDisable()
    {
        if (joinInputSource != null)
        {
            joinInputSource.DeviceRequested -= OnDeviceRequested;
        }

        ReleaseSessionAccess();
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

        TryBindSessionAccess();
        RefreshSessionState();

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

        TryBindSessionAccess();
        RefreshSessionState();

        SetStatus(result.IgnoredNoChange
            ? "Joining was already closed."
            : "Joining is closed.");

        RefreshView();
    }

    public void RequestJoin()
    {
        TryBindSessionAccess();
        RefreshSessionState();

        if (!_joiningOpen)
        {
            SetStatus("Open Joining before requesting another Player.");
            return;
        }

        if (AreBothPlayersJoined())
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

    private void OnDeviceRequested(InputDevice device, string deviceLabel)
    {
        if (!CanConsumeDeviceInput())
        {
            return;
        }

        TryJoinWithDevice(device, deviceLabel);
    }

    private bool CanConsumeDeviceInput()
    {
        RefreshSessionState();

        return _joiningOpen &&
            _awaitingDevice &&
            !AreBothPlayersJoined();
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

        // Re-read canonical Slot occupancy instead of counting Join events.
        TryBindSessionAccess();
        RefreshSessionState();

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

    private bool TryBindSessionAccess()
    {
        if (_sessionAccess != null && _sessionAccess.Snapshot.IsAvailable)
        {
            return true;
        }

        ReleaseSessionAccess();

        if (joinCommand == null ||
            !joinCommand.TryGetAccess(
                out IPlayerSessionScopedAccess resolvedAccess,
                out _))
        {
            return false;
        }

        _sessionAccess = resolvedAccess;
        _sessionAccess.Changed += OnSessionChanged;
        RefreshSessionState();
        return true;
    }

    private void ReleaseSessionAccess()
    {
        if (_sessionAccess == null)
        {
            return;
        }

        _sessionAccess.Changed -= OnSessionChanged;
        _sessionAccess = null;
    }

    private void OnSessionChanged(PlayerSessionChange change)
    {
        if (!RefreshSessionState())
        {
            return;
        }

        if (AreBothPlayersJoined())
        {
            _awaitingDevice = false;
        }

        RefreshView();
        SetStatus(BuildSessionStatus());
    }

    private bool RefreshSessionState()
    {
        if (_sessionAccess == null ||
            !_sessionAccess.TryGetObservation(
                out PlayerSessionScopedObservationSnapshot observation) ||
            observation == null ||
            !observation.IsAvailable)
        {
            return false;
        }

        if (observation.Participation != null)
        {
            _joiningOpen = observation.Participation.JoiningOpen;
        }

        _playerOneJoined = IsConfiguredSlotJoined(
            observation, PlayerOneConfiguredIndex);
        _playerTwoJoined = IsConfiguredSlotJoined(
            observation, PlayerTwoConfiguredIndex);

        return true;
    }

    private static bool IsConfiguredSlotJoined(
        PlayerSessionScopedObservationSnapshot observation,
        int configuredIndex)
    {
        if (observation == null || !observation.IsAvailable)
        {
            return false;
        }

        for (int index = 0; index < observation.Slots.Count; index++)
        {
            PlayerSessionScopedSlotObservation slot = observation.Slots[index];
            if (slot.Slot.ConfiguredIndex == configuredIndex)
            {
                return slot.IsJoined;
            }
        }

        return false;
    }

    private bool AreBothPlayersJoined()
    {
        return _playerOneJoined && _playerTwoJoined;
    }

    private int GetNextPlayerNumber()
    {
        if (!_playerOneJoined)
        {
            return 1;
        }

        if (!_playerTwoJoined)
        {
            return 2;
        }

        return 0;
    }

    private string BuildSessionStatus()
    {
        if (_playerOneJoined && _playerTwoJoined)
        {
            return "Player 1 and Player 2 are joined.";
        }

        if (_playerOneJoined)
        {
            return "Player 1 is joined. Waiting for Player 2.";
        }

        if (_playerTwoJoined)
        {
            return "Player 2 is joined. Waiting for Player 1.";
        }

        return "Waiting for Player 1.";
    }

    private void RefreshView()
    {
        RefreshSessionState();

        bool completed = AreBothPlayersJoined();

        SetActive(joiningClosedRoot, !_joiningOpen && !completed);
        SetActive(joiningOpenRoot, _joiningOpen && !completed);
        SetActive(joinReadyRoot, _joiningOpen && !_awaitingDevice && !completed);
        SetActive(awaitingDeviceRoot, _joiningOpen && _awaitingDevice && !completed);
        SetActive(completedRoot, completed);

        SetActive(leavePlayer1Button, _playerOneJoined);
        SetActive(leavePlayer2Button, _playerTwoJoined);

        int nextPlayerNumber = GetNextPlayerNumber();

        if (nextPlayerLabel != null && !completed)
        {
            nextPlayerLabel.text = nextPlayerNumber == 2
                ? "PLAYER 2"
                : "PLAYER 1";
        }

        if (devicePromptLabel != null && _awaitingDevice && !completed)
        {
            devicePromptLabel.text = nextPlayerNumber == 2
                ? "INPUT DEVICE REQUIRED\nPress START on another gamepad or press 2 to simulate Device 2."
                : "INPUT DEVICE REQUIRED\nPress START on a gamepad or press 1 to simulate Device 1.";
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
}
