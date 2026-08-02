using Immersive.Framework.PlayerParticipation;
using UnityEngine;

namespace FirstGame.FrameworkModels.Demo02.ManagerProvisionedPlayer
{
    [DisallowMultipleComponent]
    public sealed class ManagerProvisionedPlayerCommandReceiver :
        MonoBehaviour
    {
        [Header("Cross-Scene Command Boundary")]
        [SerializeField]
        private ManagerProvisionedPlayerCommandChannel commandChannel;

        [Header("Framework Product Endpoints")]
        [SerializeField]
        private LocalPlayerProvisioningAuthoring provisioningAuthoring;

        [SerializeField]
        private LocalPlayerActorSelectionRequestAuthoring actorSelectionRequests;

        [Header("Runtime Evidence")]
        [SerializeField]
        private ManagerProvisionedPlayerCommand lastCommand =
            ManagerProvisionedPlayerCommand.None;

        [SerializeField]
        private int executedCommandCount;

        [SerializeField]
        [TextArea(3, 10)]
        private string lastDiagnostic =
            "No Manager-Provisioned Player command has executed.";

        public ManagerProvisionedPlayerCommand LastCommand =>
            lastCommand;

        public int ExecutedCommandCount =>
            executedCommandCount;

        public string LastDiagnostic =>
            lastDiagnostic;

        private void OnEnable()
        {
            if (!TryValidateConfiguration(out string issue))
            {
                RecordBindingFailure(issue);
                return;
            }

            if (!commandChannel.TryBindReceiver(
                    this,
                    ExecuteCommand,
                    out issue))
            {
                RecordBindingFailure(issue);
                return;
            }

            lastDiagnostic =
                $"Command receiver bound. channel='{commandChannel.name}' " +
                $"receiver='{name}'.";

            Debug.Log(
                $"[FIRSTGAME_M07_COMMAND_RECEIVER] status='Bound' " +
                $"channel='{commandChannel.name}' receiver='{name}'",
                this);
        }

        private void OnDisable()
        {
            if (commandChannel == null)
            {
                return;
            }

            if (!commandChannel.TryReleaseReceiver(
                    this,
                    out string issue))
            {
                Debug.LogError(
                    $"[FIRSTGAME_M07_COMMAND_RECEIVER] " +
                    $"status='ReleaseFailed' issue='{issue}'",
                    this);

                return;
            }

            Debug.Log(
                $"[FIRSTGAME_M07_COMMAND_RECEIVER] status='Released' " +
                $"channel='{commandChannel.name}' receiver='{name}'",
                this);
        }

        private ManagerProvisionedPlayerCommandResult ExecuteCommand(
            ManagerProvisionedPlayerCommandRequest request)
        {
            lastCommand = request.Command;
            executedCommandCount++;

            if (!TryValidateConfiguration(out string issue))
            {
                return CompleteFailure(
                    request.Command,
                    issue);
            }

            if (!provisioningAuthoring.RuntimeReady)
            {
                return CompleteFailure(
                    request.Command,
                    "Local Player provisioning is not ready. " +
                    provisioningAuthoring.RuntimeDiagnostic);
            }

            switch (request.Command)
            {
                case ManagerProvisionedPlayerCommand.OpenJoining:
                    return OpenJoining(request);

                case ManagerProvisionedPlayerCommand.CloseJoining:
                    return CloseJoining(request);

                case ManagerProvisionedPlayerCommand.RequestJoin:
                    return RequestJoin(request);

                case ManagerProvisionedPlayerCommand.SelectDefaultActor:
                    return SelectDefaultActor(request);

                case ManagerProvisionedPlayerCommand
                    .RequestJoinAndSelectDefaultActor:
                    return RequestJoinAndSelectDefaultActor(request);

                default:
                    return CompleteFailure(
                        request.Command,
                        $"Unsupported Manager-Provisioned Player command " +
                        $"'{request.Command}'.");
            }
        }

        private ManagerProvisionedPlayerCommandResult OpenJoining(
            ManagerProvisionedPlayerCommandRequest request)
        {
            PlayerParticipationOperationResult result =
                provisioningAuthoring.OpenJoining(
                    request.Source,
                    request.Reason);

            PlayerParticipationSnapshot snapshot =
                provisioningAuthoring.RuntimeSnapshot;

            if (snapshot != null && snapshot.JoiningOpen)
            {
                return CompleteSuccess(
                    request.Command,
                    result?.ToDiagnosticString() ??
                    "Local Player joining is open.");
            }

            return CompleteFailure(
                request.Command,
                "OpenJoining did not leave the Session open for joining. " +
                result?.ToDiagnosticString());
        }

        private ManagerProvisionedPlayerCommandResult CloseJoining(
            ManagerProvisionedPlayerCommandRequest request)
        {
            PlayerParticipationOperationResult result =
                provisioningAuthoring.CloseJoining(
                    request.Source,
                    request.Reason);

            PlayerParticipationSnapshot snapshot =
                provisioningAuthoring.RuntimeSnapshot;

            if (snapshot != null && !snapshot.JoiningOpen)
            {
                return CompleteSuccess(
                    request.Command,
                    result?.ToDiagnosticString() ??
                    "Local Player joining is closed.");
            }

            return CompleteFailure(
                request.Command,
                "CloseJoining did not close the Session joining window. " +
                result?.ToDiagnosticString());
        }

        private ManagerProvisionedPlayerCommandResult RequestJoin(
            ManagerProvisionedPlayerCommandRequest request)
        {
            LocalPlayerJoinResult result =
                provisioningAuthoring.RequestJoin(
                    request.Source,
                    request.Reason);

            if (result != null && result.Succeeded)
            {
                return CompleteSuccess(
                    request.Command,
                    result.ToDiagnosticString());
            }

            return CompleteFailure(
                request.Command,
                "Authorized Local Player join failed. " +
                result?.ToDiagnosticString());
        }

        private ManagerProvisionedPlayerCommandResult SelectDefaultActor(
            ManagerProvisionedPlayerCommandRequest request)
        {
            if (!TryGetSingleJoinedSlot(
                    out PlayerSlotRuntimeSnapshot slot,
                    out string issue))
            {
                return CompleteFailure(
                    request.Command,
                    issue);
            }

            return SelectDefaultActor(
                request,
                slot);
        }

        private ManagerProvisionedPlayerCommandResult
            RequestJoinAndSelectDefaultActor(
                ManagerProvisionedPlayerCommandRequest request)
        {
            PlayerSlotRuntimeSnapshot slot;

            if (!TryGetSingleJoinedSlot(
                    out slot,
                    out string slotIssue))
            {
                PlayerParticipationSnapshot snapshot =
                    provisioningAuthoring.RuntimeSnapshot;

                if (snapshot != null && snapshot.JoinedCount > 0)
                {
                    return CompleteFailure(
                        request.Command,
                        slotIssue);
                }

                if (snapshot == null || !snapshot.JoiningOpen)
                {
                    PlayerParticipationOperationResult opened =
                        provisioningAuthoring.OpenJoining(
                            request.Source,
                            "open-before-authorized-join");

                    snapshot = provisioningAuthoring.RuntimeSnapshot;

                    if (snapshot == null || !snapshot.JoiningOpen)
                    {
                        return CompleteFailure(
                            request.Command,
                            "Could not open Local Player joining. " +
                            opened?.ToDiagnosticString());
                    }
                }

                LocalPlayerJoinResult joined =
                    provisioningAuthoring.RequestJoin(
                        request.Source,
                        request.Reason);

                if (joined == null || !joined.Succeeded)
                {
                    return CompleteFailure(
                        request.Command,
                        "Authorized Local Player join failed. " +
                        joined?.ToDiagnosticString());
                }

                slot = joined.Slot;
            }

            return SelectDefaultActor(
                request,
                slot);
        }

        private ManagerProvisionedPlayerCommandResult SelectDefaultActor(
            ManagerProvisionedPlayerCommandRequest request,
            PlayerSlotRuntimeSnapshot slot)
        {
            if (!actorSelectionRequests.RuntimeReady)
            {
                return CompleteFailure(
                    request.Command,
                    "Default Actor selection endpoint is not ready. " +
                    actorSelectionRequests
                        .PlayerActorSelectionRuntimeBindingDiagnostic);
            }

            PlayerActorSelectionResult result =
                actorSelectionRequests.RequestDefaultActorSelection(
                    slot.PlayerSlotId,
                    slot.SelectionRevision,
                    request.Source,
                    "select-default-actor");

            if (result != null && result.Succeeded)
            {
                return CompleteSuccess(
                    request.Command,
                    result.ToDiagnosticString());
            }

            return CompleteFailure(
                request.Command,
                "Default Actor selection failed. " +
                result?.ToDiagnosticString());
        }

        private bool TryGetSingleJoinedSlot(
            out PlayerSlotRuntimeSnapshot joinedSlot,
            out string issue)
        {
            joinedSlot = default;

            PlayerParticipationSnapshot snapshot =
                provisioningAuthoring.RuntimeSnapshot;

            if (snapshot == null || snapshot.Slots == null)
            {
                issue =
                    "Player participation snapshot is unavailable.";
                return false;
            }

            int joinedCount = 0;

            for (int index = 0;
                 index < snapshot.Slots.Count;
                 index++)
            {
                PlayerSlotRuntimeSnapshot candidate =
                    snapshot.Slots[index];

                if (!candidate.IsJoined)
                {
                    continue;
                }

                joinedSlot = candidate;
                joinedCount++;
            }

            if (joinedCount == 1)
            {
                issue = string.Empty;
                return true;
            }

            issue = joinedCount == 0
                ? "No joined Player Slot is available."
                : $"Expected exactly one joined Player Slot, " +
                  $"but found '{joinedCount}'.";

            return false;
        }

        private ManagerProvisionedPlayerCommandResult CompleteSuccess(
            ManagerProvisionedPlayerCommand command,
            string diagnostic)
        {
            lastDiagnostic =
                string.IsNullOrWhiteSpace(diagnostic)
                    ? $"{command} succeeded."
                    : diagnostic.Trim();

            Debug.Log(
                $"[FIRSTGAME_M07_COMMAND] status='Succeeded' " +
                $"command='{command}' diagnostic='{lastDiagnostic}'",
                this);

            return ManagerProvisionedPlayerCommandResult.Success(
                command,
                lastDiagnostic);
        }

        private ManagerProvisionedPlayerCommandResult CompleteFailure(
            ManagerProvisionedPlayerCommand command,
            string diagnostic)
        {
            lastDiagnostic =
                string.IsNullOrWhiteSpace(diagnostic)
                    ? $"{command} failed without a diagnostic."
                    : diagnostic.Trim();

            Debug.LogError(
                $"[FIRSTGAME_M07_COMMAND] status='Failed' " +
                $"command='{command}' diagnostic='{lastDiagnostic}'",
                this);

            return ManagerProvisionedPlayerCommandResult.Failure(
                command,
                lastDiagnostic);
        }

        private bool TryValidateConfiguration(
            out string issue)
        {
            if (commandChannel == null)
            {
                issue =
                    "Manager-Provisioned Player command receiver requires " +
                    "an explicit command channel.";
                return false;
            }

            if (provisioningAuthoring == null)
            {
                issue =
                    "Manager-Provisioned Player command receiver requires " +
                    "an explicit LocalPlayerProvisioningAuthoring.";
                return false;
            }

            if (actorSelectionRequests == null)
            {
                issue =
                    "Manager-Provisioned Player command receiver requires " +
                    "an explicit LocalPlayerActorSelectionRequestAuthoring.";
                return false;
            }

            if (!ReferenceEquals(
                    provisioningAuthoring.gameObject,
                    gameObject))
            {
                issue =
                    "Provisioning authoring and command receiver must share " +
                    "the same persistent GameObject.";
                return false;
            }

            if (!ReferenceEquals(
                    actorSelectionRequests.gameObject,
                    gameObject))
            {
                issue =
                    "Actor selection requests and command receiver must " +
                    "share the same persistent GameObject.";
                return false;
            }

            issue = string.Empty;
            return true;
        }

        private void RecordBindingFailure(
            string issue)
        {
            lastDiagnostic =
                string.IsNullOrWhiteSpace(issue)
                    ? "Command receiver binding failed."
                    : issue.Trim();

            Debug.LogError(
                $"[FIRSTGAME_M07_COMMAND_RECEIVER] " +
                $"status='BindingFailed' issue='{lastDiagnostic}'",
                this);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            provisioningAuthoring =
                GetComponent<LocalPlayerProvisioningAuthoring>();

            actorSelectionRequests =
                GetComponent<
                    LocalPlayerActorSelectionRequestAuthoring>();
        }

        private void OnValidate()
        {
            if (provisioningAuthoring == null)
            {
                provisioningAuthoring =
                    GetComponent<LocalPlayerProvisioningAuthoring>();
            }

            if (actorSelectionRequests == null)
            {
                actorSelectionRequests =
                    GetComponent<
                        LocalPlayerActorSelectionRequestAuthoring>();
            }
        }
#endif
    }
}