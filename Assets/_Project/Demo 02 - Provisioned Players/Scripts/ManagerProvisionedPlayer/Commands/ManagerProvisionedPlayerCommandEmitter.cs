using UnityEngine;

namespace FirstGame.FrameworkModels.Demo02.ManagerProvisionedPlayer
{
    [DisallowMultipleComponent]
    public sealed class ManagerProvisionedPlayerCommandEmitter :
        MonoBehaviour
    {
        private const string DefaultSource =
            "FIRSTGAME.Demo02.ManagerProvisionedPlayerMenu";

        [Header("Cross-Scene Command Boundary")]
        [SerializeField]
        private ManagerProvisionedPlayerCommandChannel commandChannel;

        [Header("Command Identity")]
        [SerializeField]
        private string source = DefaultSource;

        [Header("Runtime Evidence")]
        [SerializeField]
        private ManagerProvisionedPlayerCommand lastCommand =
            ManagerProvisionedPlayerCommand.None;

        [SerializeField]
        private bool lastCommandSucceeded;

        [SerializeField]
        private int emittedCommandCount;

        [SerializeField]
        [TextArea(3, 10)]
        private string lastDiagnostic =
            "No Manager-Provisioned Player command has been emitted.";

        public ManagerProvisionedPlayerCommand LastCommand =>
            lastCommand;

        public bool LastCommandSucceeded =>
            lastCommandSucceeded;

        public int EmittedCommandCount =>
            emittedCommandCount;

        public string LastDiagnostic =>
            lastDiagnostic;

        public bool HasActiveReceiver =>
            commandChannel != null &&
            commandChannel.HasReceiver;

        public string ActiveReceiverName =>
            commandChannel != null
                ? commandChannel.ReceiverName
                : "<missing-channel>";

        public void OpenJoining()
        {
            Execute(
                ManagerProvisionedPlayerCommand.OpenJoining,
                "open-joining-from-route-menu");
        }

        public void CloseJoining()
        {
            Execute(
                ManagerProvisionedPlayerCommand.CloseJoining,
                "close-joining-from-route-menu");
        }

        public void RequestJoin()
        {
            Execute(
                ManagerProvisionedPlayerCommand.RequestJoin,
                "request-authorized-local-player-join");
        }

        public void SelectDefaultActor()
        {
            Execute(
                ManagerProvisionedPlayerCommand.SelectDefaultActor,
                "select-default-actor-for-joined-player");
        }

        public void RequestJoinAndSelectDefaultActor()
        {
            Execute(
                ManagerProvisionedPlayerCommand
                    .RequestJoinAndSelectDefaultActor,
                "join-and-select-default-actor");
        }

        private void Execute(
            ManagerProvisionedPlayerCommand command,
            string reason)
        {
            emittedCommandCount++;
            lastCommand = command;

            if (!TryValidateConfiguration(out string issue))
            {
                CompleteLocalFailure(
                    command,
                    issue);
                return;
            }

            ManagerProvisionedPlayerCommandResult result =
                commandChannel.Execute(
                    command,
                    source,
                    reason);

            lastCommandSucceeded = result.Succeeded;
            lastDiagnostic = string.IsNullOrWhiteSpace(
                    result.Diagnostic)
                ? "Command returned no diagnostic."
                : result.Diagnostic.Trim();

            if (result.Succeeded)
            {
                Debug.Log(
                    $"[FIRSTGAME_M07_COMMAND_EMITTER] " +
                    $"status='Succeeded' command='{command}' " +
                    $"receiver='{commandChannel.ReceiverName}' " +
                    $"diagnostic='{lastDiagnostic}'",
                    this);

                return;
            }

            Debug.LogError(
                $"[FIRSTGAME_M07_COMMAND_EMITTER] " +
                $"status='Failed' command='{command}' " +
                $"receiver='{commandChannel.ReceiverName}' " +
                $"diagnostic='{lastDiagnostic}'",
                this);
        }

        private bool TryValidateConfiguration(
            out string issue)
        {
            if (commandChannel == null)
            {
                issue =
                    "Manager-Provisioned Player command emitter requires " +
                    "an explicit command channel.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                issue =
                    "Manager-Provisioned Player command emitter requires " +
                    "an explicit command source.";
                return false;
            }

            if (!commandChannel.HasReceiver)
            {
                issue =
                    "Manager-Provisioned Player command channel has no " +
                    "active persistent receiver.";
                return false;
            }

            issue = string.Empty;
            return true;
        }

        private void CompleteLocalFailure(
            ManagerProvisionedPlayerCommand command,
            string diagnostic)
        {
            lastCommandSucceeded = false;
            lastDiagnostic =
                string.IsNullOrWhiteSpace(diagnostic)
                    ? $"{command} failed without a diagnostic."
                    : diagnostic.Trim();

            Debug.LogError(
                $"[FIRSTGAME_M07_COMMAND_EMITTER] " +
                $"status='FailedBeforeDispatch' command='{command}' " +
                $"diagnostic='{lastDiagnostic}'",
                this);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            source = DefaultSource;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                source = DefaultSource;
            }
            else
            {
                source = source.Trim();
            }
        }
#endif
    }
}