using System;
using UnityEngine;
namespace _Project.Demo02.Scripts.ManagerProvisionedPlayer.Commands
{
    public enum ManagerProvisionedPlayerCommand
    {
        None = 0,
        OpenJoining = 10,
        CloseJoining = 20,
        RequestJoin = 30,
        SelectDefaultActor = 40,
        RequestJoinAndSelectDefaultActor = 50
    }

    public readonly struct ManagerProvisionedPlayerCommandRequest
    {
        public ManagerProvisionedPlayerCommandRequest(
            ManagerProvisionedPlayerCommand command,
            string source,
            string reason)
        {
            Command = command;
            Source = source;
            Reason = reason;
        }

        public ManagerProvisionedPlayerCommand Command { get; }

        public string Source { get; }

        public string Reason { get; }
    }

    public readonly struct ManagerProvisionedPlayerCommandResult
    {
        private ManagerProvisionedPlayerCommandResult(
            ManagerProvisionedPlayerCommand command,
            bool succeeded,
            string diagnostic)
        {
            Command = command;
            Succeeded = succeeded;
            Diagnostic = diagnostic ?? string.Empty;
        }

        public ManagerProvisionedPlayerCommand Command { get; }

        public bool Succeeded { get; }

        public string Diagnostic { get; }

        public static ManagerProvisionedPlayerCommandResult Success(
            ManagerProvisionedPlayerCommand command,
            string diagnostic)
        {
            return new ManagerProvisionedPlayerCommandResult(
                command,
                true,
                diagnostic);
        }

        public static ManagerProvisionedPlayerCommandResult Failure(
            ManagerProvisionedPlayerCommand command,
            string diagnostic)
        {
            return new ManagerProvisionedPlayerCommandResult(
                command,
                false,
                diagnostic);
        }

        public override string ToString()
        {
            return
                $"command='{Command}' " +
                $"succeeded='{Succeeded}' " +
                $"diagnostic='{Diagnostic}'";
        }
    }

    [CreateAssetMenu(
        fileName = "Manager Provisioned Player Command Channel",
        menuName =
            "FIRSTGAME/Demo 02/Manager-Provisioned Player Command Channel")]
    public sealed class ManagerProvisionedPlayerCommandChannel :
        ScriptableObject
    {
        [NonSerialized]
        private UnityEngine.Object receiverOwner;

        [NonSerialized]
        private Func<
            ManagerProvisionedPlayerCommandRequest,
            ManagerProvisionedPlayerCommandResult> receiver;

        public bool HasReceiver
        {
            get
            {
                ClearDestroyedReceiver();

                return receiverOwner != null &&
                       receiver != null;
            }
        }

        public string ReceiverName =>
            HasReceiver
                ? receiverOwner.name
                : "<none>";

        public bool TryBindReceiver(
            UnityEngine.Object owner,
            Func<
                ManagerProvisionedPlayerCommandRequest,
                ManagerProvisionedPlayerCommandResult> handler,
            out string issue)
        {
            ClearDestroyedReceiver();

            if (owner == null)
            {
                issue =
                    "Manager-Provisioned Player command receiver " +
                    "requires an explicit owner.";
                return false;
            }

            if (handler == null)
            {
                issue =
                    "Manager-Provisioned Player command receiver " +
                    "requires an execution handler.";
                return false;
            }

            if (HasReceiver &&
                !ReferenceEquals(receiverOwner, owner))
            {
                issue =
                    "Manager-Provisioned Player command channel is already " +
                    $"bound to receiver '{receiverOwner.name}'.";
                return false;
            }

            receiverOwner = owner;
            receiver = handler;
            issue = string.Empty;
            return true;
        }

        public bool TryReleaseReceiver(
            UnityEngine.Object owner,
            out string issue)
        {
            ClearDestroyedReceiver();

            if (!HasReceiver)
            {
                issue = string.Empty;
                return true;
            }

            if (!ReferenceEquals(receiverOwner, owner))
            {
                issue =
                    "Manager-Provisioned Player command channel cannot be " +
                    $"released by '{owner?.name ?? "<null>"}' because it is " +
                    $"owned by '{receiverOwner.name}'.";
                return false;
            }

            receiverOwner = null;
            receiver = null;
            issue = string.Empty;
            return true;
        }

        public ManagerProvisionedPlayerCommandResult Execute(
            ManagerProvisionedPlayerCommand command,
            string source,
            string reason)
        {
            if (command == ManagerProvisionedPlayerCommand.None)
            {
                return ManagerProvisionedPlayerCommandResult.Failure(
                    command,
                    "A valid Manager-Provisioned Player command is required.");
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return ManagerProvisionedPlayerCommandResult.Failure(
                    command,
                    "Manager-Provisioned Player command source is required.");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                return ManagerProvisionedPlayerCommandResult.Failure(
                    command,
                    "Manager-Provisioned Player command reason is required.");
            }

            ClearDestroyedReceiver();

            if (!HasReceiver)
            {
                return ManagerProvisionedPlayerCommandResult.Failure(
                    command,
                    "Manager-Provisioned Player command channel has no " +
                    "active receiver.");
            }

            var request =
                new ManagerProvisionedPlayerCommandRequest(
                    command,
                    source.Trim(),
                    reason.Trim());

            try
            {
                ManagerProvisionedPlayerCommandResult result =
                    receiver.Invoke(request);

                if (result.Command != command)
                {
                    return ManagerProvisionedPlayerCommandResult.Failure(
                        command,
                        "Manager-Provisioned Player receiver returned a " +
                        $"result for command '{result.Command}' instead of " +
                        $"'{command}'.");
                }

                if (string.IsNullOrWhiteSpace(result.Diagnostic))
                {
                    return ManagerProvisionedPlayerCommandResult.Failure(
                        command,
                        "Manager-Provisioned Player receiver returned no " +
                        "diagnostic.");
                }

                return result;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, receiverOwner);

                return ManagerProvisionedPlayerCommandResult.Failure(
                    command,
                    $"Receiver '{ReceiverName}' threw " +
                    $"'{exception.GetType().Name}': {exception.Message}");
            }
        }

        private void ClearDestroyedReceiver()
        {
            if (receiverOwner != null)
            {
                return;
            }

            receiverOwner = null;
            receiver = null;
        }
    }
}