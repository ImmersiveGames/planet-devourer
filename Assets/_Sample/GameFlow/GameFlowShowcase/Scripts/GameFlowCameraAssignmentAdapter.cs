using Immersive.Framework.Authoring;
using Immersive.Framework.Camera;
using Immersive.Framework.CameraAuthoring;
using Immersive.Framework.RouteLifecycle;
using UnityEngine;

namespace ImmersiveGames.PlanetDevourer.Samples.GameFlow
{
    /// <summary>
    /// Maps this sample's Activity transitions to explicit Session Camera Assignment commands.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameFlowCameraAssignmentAdapter : MonoBehaviour,
        IRouteActivityTransitionObserver,
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

        public void OnActivityTransitionCommitted(RouteActivityTransitionContext context)
        {
            SessionCameraAssignmentAsset previousAssignment = ResolveAssignment(context.PreviousActivity);
            SessionCameraAssignmentAsset targetAssignment = ResolveAssignment(context.CurrentActivity);

            if (previousAssignment == null && targetAssignment == null)
            {
                return;
            }

            if (previousAssignment == null)
            {
                ExecuteActivate(context, targetAssignment);
                return;
            }

            if (targetAssignment == null)
            {
                ExecuteClear(context, previousAssignment);
                return;
            }

            if (ReferenceEquals(previousAssignment, targetAssignment))
            {
                return;
            }

            ExecuteReplace(context, previousAssignment, targetAssignment);
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
            RouteActivityTransitionContext context,
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
            RouteActivityTransitionContext context,
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
            RouteActivityTransitionContext context,
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
            RouteActivityTransitionContext context,
            string command,
            SessionCameraAssignmentAsset previousAssignment,
            SessionCameraAssignmentAsset targetAssignment,
            string issue)
        {
            Debug.LogError(
                $"Game Flow Session Camera command failed. Activity='{FormatActivity(context.CurrentActivity)}' " +
                $"PreviousActivity='{FormatActivity(context.PreviousActivity)}' " +
                $"NextActivity='{FormatActivity(context.CurrentActivity)}' command='{command}' " +
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
