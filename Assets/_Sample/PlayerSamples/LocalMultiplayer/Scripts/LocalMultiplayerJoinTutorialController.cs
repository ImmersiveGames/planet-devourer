using System.Collections.Generic;
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
    [SerializeField] private PlayerSessionJoinTrigger joinTrigger;
    [SerializeField] private PlayerSessionObserver sessionObserver;

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
    private bool _hasStableSessionState;
    private bool _sessionRefreshPending;
    private readonly HashSet<int> _ownedDeviceIds = new();
    private int _pendingJoinedDeviceId = -1;
    private string _pendingJoinedDeviceLabel;

    private void OnEnable()
    {
        if (joinInputSource != null)
        {
            joinInputSource.DeviceRequested += OnDeviceRequested;
        }

        TryBindSessionAccess();
        ScheduleSessionRefresh();
        RefreshView();
    }

    private void Update()
    {
        // Scoped consumers are bound after scene composition. Retry only until
        // access becomes available; after that all state updates are event-driven.
        if (_sessionAccess == null && TryBindSessionAccess())
        {
            ScheduleSessionRefresh();
        }

        ProcessPendingSessionRefresh();
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

        _awaitingDevice = false;

        TryBindSessionAccess();
        ScheduleSessionRefresh();

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

        _awaitingDevice = false;

        TryBindSessionAccess();
        ScheduleSessionRefresh();

        SetStatus(result.IgnoredNoChange
            ? "Joining was already closed."
            : "Joining is closed.");

        RefreshView();
    }

    public void RequestJoin()
    {
        TryBindSessionAccess();

        if (!_hasStableSessionState || _sessionRefreshPending)
        {
            SetStatus("Player Session state is still updating. Try again after the tutorial refreshes.");
            return;
        }

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
        return _hasStableSessionState &&
            !_sessionRefreshPending &&
            _joiningOpen &&
            _awaitingDevice &&
            !AreBothPlayersJoined();
    }

    private void TryJoinWithDevice(InputDevice device, string deviceLabel)
    {
        if (joinTrigger == null)
        {
            SetStatus("Join trigger is not assigned.");
            return;
        }

        if (device == null || !device.added)
        {
            SetStatus("Join requires a valid added InputDevice.");
            return;
        }

        int nextPlayerNumber = GetNextPlayerNumber();
        if (nextPlayerNumber == 0)
        {
            SetStatus("Both tutorial Players are already joined.");
            return;
        }

        if (_ownedDeviceIds.Contains(device.deviceId))
        {
            SetStatus(
                $"{deviceLabel} is already owned by a current Player. " +
                $"Waiting for an unowned device for Player {nextPlayerNumber}.");
            return;
        }

        LocalPlayerJoinOutcome result = joinTrigger.RequestJoin(
            new LocalPlayerJoinRequest(
                nameof(LocalMultiplayerJoinTutorialController),
                "local-multiplayer-join",
                device));
        if (result == null)
        {
            SetStatus("Join returned no result.");
            return;
        }

        if (result.Code != LocalPlayerJoinOutcomeCode.Joined ||
            result.AdmissionState != LocalPlayerJoinAdmissionState.Admitted)
        {
            SetStatus(DescribeJoinOutcome(result));
            RefreshView();
            return;
        }

        _awaitingDevice = false;

        // The command can publish Slot Joined before its physical Host finishes
        // registration. Consume the coalesced canonical state on a later frame.
        _hasStableSessionState = false;
        TryBindSessionAccess();
        ScheduleSessionRefresh();

        _pendingJoinedDeviceId = device.deviceId;
        _pendingJoinedDeviceLabel = deviceLabel;
        SetStatus($"Join admitted for {deviceLabel}; reading the assigned Slot from Player Session.");

        RefreshView();
    }

    private bool TryBindSessionAccess()
    {
        if (_sessionAccess != null && _sessionAccess.Snapshot.IsAvailable)
        {
            return true;
        }

        ReleaseSessionAccess();

        if (sessionObserver == null ||
            !sessionObserver.TryGetAccess(
                out IPlayerSessionScopedAccess resolvedAccess,
                out _))
        {
            return false;
        }

        _sessionAccess = resolvedAccess;
        _sessionAccess.Changed += OnSessionChanged;
        ScheduleSessionRefresh();
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
        _hasStableSessionState = false;
        _sessionRefreshPending = false;
        _ownedDeviceIds.Clear();
    }

    private void OnSessionChanged(PlayerSessionChange change)
    {
        _ = change;

        // Several notifications can be raised by one Framework mutation. Never
        // inspect its intermediate state synchronously from this callback.
        _hasStableSessionState = false;
        ScheduleSessionRefresh();
    }

    private void ScheduleSessionRefresh()
    {
        if (_sessionAccess != null)
        {
            _sessionRefreshPending = true;
        }
    }

    private void ProcessPendingSessionRefresh()
    {
        if (!_sessionRefreshPending)
        {
            return;
        }

        _sessionRefreshPending = false;
        if (!RefreshSessionState())
        {
            return;
        }

        if (AreBothPlayersJoined())
        {
            _awaitingDevice = false;
        }

        RefreshView();
        if (!TryReportPendingJoin())
        {
            SetStatus(BuildSessionStatus());
        }
    }

    private bool RefreshSessionState()
    {
        _hasStableSessionState = false;

        if (_sessionAccess == null ||
            !_sessionAccess.TryGetObservation(
                out PlayerSessionScopedObservationSnapshot observation) ||
            observation == null ||
            !observation.IsAvailable)
        {
            return false;
        }

        bool playerOneJoined = false;
        bool playerTwoJoined = false;
        var ownedDeviceIds = new HashSet<int>();

        for (int index = 0; index < observation.Slots.Count; index++)
        {
            PlayerSessionScopedSlotObservation slot = observation.Slots[index];
            if (slot.Slot.ConfiguredIndex == PlayerOneConfiguredIndex)
            {
                playerOneJoined = slot.IsJoined;
            }
            else if (slot.Slot.ConfiguredIndex == PlayerTwoConfiguredIndex)
            {
                playerTwoJoined = slot.IsJoined;
            }

            if (!slot.IsJoined)
            {
                continue;
            }

            if (!slot.HasInputOwnershipEvidence ||
                slot.InputOwnership.Devices.Count == 0)
            {
                return false;
            }

            for (int deviceIndex = 0;
                 deviceIndex < slot.InputOwnership.Devices.Count;
                 deviceIndex++)
            {
                ownedDeviceIds.Add(
                    slot.InputOwnership.Devices[deviceIndex].DeviceId);
            }
        }

        _joiningOpen = observation.Participation != null &&
            observation.Participation.JoiningOpen;
        _playerOneJoined = playerOneJoined;
        _playerTwoJoined = playerTwoJoined;

        _ownedDeviceIds.Clear();
        foreach (int deviceId in ownedDeviceIds)
        {
            _ownedDeviceIds.Add(deviceId);
        }

        _hasStableSessionState = true;

        return true;
    }

    private bool TryReportPendingJoin()
    {
        if (_pendingJoinedDeviceId < 0 ||
            sessionObserver == null ||
            !sessionObserver.TryGetObservation(
                out PlayerSessionScopedObservationSnapshot observation) ||
            observation == null ||
            !observation.IsAvailable)
        {
            return false;
        }

        for (int slotIndex = 0; slotIndex < observation.Slots.Count; slotIndex++)
        {
            PlayerSessionScopedSlotObservation slot = observation.Slots[slotIndex];
            if (!slot.IsJoined || !slot.HasInputOwnershipEvidence)
            {
                continue;
            }

            for (int deviceIndex = 0;
                 deviceIndex < slot.InputOwnership.Devices.Count;
                 deviceIndex++)
            {
                if (slot.InputOwnership.Devices[deviceIndex].DeviceId != _pendingJoinedDeviceId)
                {
                    continue;
                }

                string slotId = slot.Slot.PlayerSlotId.IsValid
                    ? slot.Slot.PlayerSlotId.StableText
                    : "<invalid-slot>";
                int playerIndex = slot.InputOwnership.UnityPlayerIndex;
                string deviceLabel = _pendingJoinedDeviceLabel ?? "Input device";

                SetStatus(
                    $"{slotId} joined with {deviceLabel} " +
                    $"(deviceId={_pendingJoinedDeviceId}, playerIndex={playerIndex}).");
                Debug.Log(
                    "[FG_LOCAL_MULTIPLAYER_TUTORIAL] " +
                    $"status='Joined' slot='{slotId}' device='{deviceLabel}' " +
                    $"deviceId='{_pendingJoinedDeviceId}' playerIndex='{playerIndex}'.");

                _pendingJoinedDeviceId = -1;
                _pendingJoinedDeviceLabel = null;
                return true;
            }
        }

        return false;
    }

    private static string DescribeJoinOutcome(LocalPlayerJoinOutcome outcome)
    {
        switch (outcome.Code)
        {
            case LocalPlayerJoinOutcomeCode.FailedRollback:
                return $"Join rollback failed; original outcome: {outcome.OriginalCode?.ToString() ?? "unknown"}.";
            case LocalPlayerJoinOutcomeCode.FailedAdmission:
            case LocalPlayerJoinOutcomeCode.FailedUnknown:
                return $"Join failed: {outcome.Code} (admission={outcome.AdmissionState}).";
            case LocalPlayerJoinOutcomeCode.Joined:
                return $"Join returned an inconsistent success outcome (admission={outcome.AdmissionState}).";
            default:
                return $"Join rejected: {outcome.Code}.";
        }
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
