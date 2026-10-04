using Immersive.Framework.ActivityFlow;
using Immersive.Framework.Authoring;
using Immersive.Framework.Camera;
using Immersive.Framework.CameraAuthoring;
using UnityEngine;

namespace ImmersiveGames.PlanetDevourer.Samples.GameFlow
{
    /// <summary>
    /// Maps this sample's Activity transitions to explicit Session Camera Assignment commands.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameFlowCameraAssignmentAdapter : ActivityContentBehaviour,
        ISessionCameraAssignmentCommandConsumer
    {
        [Header("Activity Assignments")]
        [SerializeField] private ActivityAsset activityA;
        [SerializeField] private SessionCameraAssignmentAsset assignmentA;
        [SerializeField] private ActivityAsset activityB;
        [SerializeField] private SessionCameraAssignmentAsset assignmentB;

        private ISessionCameraAssignmentCommandPort _commands;
        private ISessionCameraAssignmentCommandPort _lastReleasedCommands;

        public bool IsBoundToSessionCameraAssignmentCommands(
            ISessionCameraAssignmentCommandPort commands) =>
            commands != null && ReferenceEquals(_commands, commands);

        public bool TryBindSessionCameraAssignmentCommands(
            ISessionCameraAssignmentCommandPort commands,
            out string issue)
        {
            if (commands == null)
            {
                issue = "Session Camera Assignment command binding requires a non-null command port.";
                return false;
            }

            if (_commands == null)
            {
                _commands = commands;
                _lastReleasedCommands = null;
                issue = string.Empty;
                return true;
            }

            if (ReferenceEquals(_commands, commands))
            {
                issue = string.Empty;
                return true;
            }

            issue = "Session Camera Assignment command binding rejected a different port while already bound.";
            return false;
        }

        public bool TryReleaseSessionCameraAssignmentCommands(
            ISessionCameraAssignmentCommandPort expectedCommands,
            out string issue)
        {
            if (expectedCommands == null)
            {
                issue = "Session Camera Assignment command release requires the exact non-null command port.";
                return false;
            }

            if (_commands == null)
            {
                if (ReferenceEquals(_lastReleasedCommands, expectedCommands))
                {
                    issue = string.Empty;
                    return true;
                }

                issue = "Session Camera Assignment command release rejected a port that is not bound.";
                return false;
            }

            if (!ReferenceEquals(_commands, expectedCommands))
            {
                issue = "Session Camera Assignment command release rejected a different or stale port.";
                return false;
            }

            _commands = null;
            _lastReleasedCommands = expectedCommands;
            issue = string.Empty;
            return true;
        }

        protected override void OnActivityContentEntered(ActivityContentLifecycleContext context)
        {
            SessionCameraAssignmentAsset targetAssignment = ResolveAssignment(context.Activity);
            if (targetAssignment == null)
            {
                return;
            }

            SessionCameraAssignmentAsset previousAssignment = ResolveAssignment(context.PreviousActivity);
            if (ReferenceEquals(previousAssignment, targetAssignment))
            {
                return;
            }

            if (previousAssignment != null)
            {
                ExecuteReplace(context, previousAssignment, targetAssignment);
                return;
            }

            ExecuteActivate(context, targetAssignment);
        }

        protected override void OnActivityContentExited(ActivityContentLifecycleContext context)
        {
            SessionCameraAssignmentAsset previousAssignment = ResolveAssignment(context.Activity);
            SessionCameraAssignmentAsset targetAssignment = ResolveAssignment(context.NextActivity);
            if (previousAssignment == null || targetAssignment != null)
            {
                return;
            }

            ExecuteClear(context, previousAssignment);
        }

        private SessionCameraAssignmentAsset ResolveAssignment(ActivityAsset activity)
        {
            if (activity == null)
            {
                return null;
            }

            if (ReferenceEquals(activity, activityA))
            {
                return assignmentA;
            }

            if (ReferenceEquals(activity, activityB))
            {
                return assignmentB;
            }

            return null;
        }

        private void ExecuteActivate(
            ActivityContentLifecycleContext context,
            SessionCameraAssignmentAsset targetAssignment)
        {
            if (_commands == null)
            {
                ReportFailure(context, "Activate", null, targetAssignment,
                    "Session Camera Assignment command port is not bound.");
                return;
            }

            bool succeeded = _commands.TryActivate(targetAssignment, out string issue);
            if (!succeeded)
            {
                ReportFailure(context, "Activate", null, targetAssignment, issue);
            }
        }

        private void ExecuteReplace(
            ActivityContentLifecycleContext context,
            SessionCameraAssignmentAsset previousAssignment,
            SessionCameraAssignmentAsset targetAssignment)
        {
            if (_commands == null)
            {
                ReportFailure(context, "Replace", previousAssignment, targetAssignment,
                    "Session Camera Assignment command port is not bound.");
                return;
            }

            bool succeeded = _commands.TryReplace(previousAssignment, targetAssignment, out string issue);
            if (!succeeded)
            {
                ReportFailure(context, "Replace", previousAssignment, targetAssignment, issue);
            }
        }

        private void ExecuteClear(
            ActivityContentLifecycleContext context,
            SessionCameraAssignmentAsset previousAssignment)
        {
            if (_commands == null)
            {
                ReportFailure(context, "Clear", previousAssignment, null,
                    "Session Camera Assignment command port is not bound.");
                return;
            }

            bool succeeded = _commands.TryClear(previousAssignment, out string issue);
            if (!succeeded)
            {
                ReportFailure(context, "Clear", previousAssignment, null, issue);
            }
        }

        private void ReportFailure(
            ActivityContentLifecycleContext context,
            string command,
            SessionCameraAssignmentAsset previousAssignment,
            SessionCameraAssignmentAsset targetAssignment,
            string issue)
        {
            Debug.LogError(
                $"Game Flow Session Camera command failed. Activity='{FormatActivity(context.Activity)}' " +
                $"PreviousActivity='{FormatActivity(context.PreviousActivity)}' " +
                $"NextActivity='{FormatActivity(context.NextActivity)}' command='{command}' " +
                $"previousAssignment='{FormatAssignment(previousAssignment)}' " +
                $"targetAssignment='{FormatAssignment(targetAssignment)}' issue='{FormatIssue(issue)}'.",
                this);
        }

        private static string FormatActivity(ActivityAsset activity) =>
            activity != null ? activity.ActivityName : "<none>";

        private static string FormatAssignment(SessionCameraAssignmentAsset assignment) =>
            assignment != null ? assignment.name : "<none>";

        private static string FormatIssue(string issue) =>
            string.IsNullOrWhiteSpace(issue) ? "<empty>" : issue.Replace("'", "\\'");
    }
}
