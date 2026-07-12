using Immersive.Framework.Camera;
using UnityEngine;

namespace FirstGame.Gameplay.Diagnostics
{
    /// <summary>
    /// Visible FIRSTGAME proof surface for explicit camera override authority.
    /// It never selects a winner or mutates Cinemachine directly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameC9MCameraValidationControls :
        MonoBehaviour,
        ICameraOutputSessionConsumer,
        ISessionCameraOverrideConsumer
    {
        private const string LogPrefix =
            "[FIRSTGAME_CAMERA_OVERRIDE]";

        private const int WindowId = 9031;

        [SerializeField]
        private CameraOutputSessionBinding outputSession;

        [SerializeField]
        private LocalPlayerCameraRequestBinding playerBinding;

        [SerializeField]
        private ActivityCameraOverrideBinding activityOverride;

        [SerializeField]
        private RouteCameraOverrideBinding routeOverride;

        [SerializeField]
        private SessionCameraOverrideBinding sessionOverride;

        [SerializeField]
        private bool showControls = true;

        [SerializeField]
        private Rect controlsRect =
            new Rect(12f, 12f, 330f, 300f);

        private void Start()
        {
            CaptureEvidence("gameplay-entry");
        }

        private void OnGUI()
        {
            if (!showControls)
            {
                return;
            }

            controlsRect = GUI.Window(
                WindowId,
                controlsRect,
                DrawControls,
                "Camera Override Authority");
        }

        private void DrawControls(int windowId)
        {
            GUILayout.Label(
                "Player < Activity < Route < Session");

            DrawButton(
                "Activity Override",
                activityOverride,
                true);

            DrawButton(
                "Activity Release",
                activityOverride,
                false);

            DrawButton(
                "Route Override",
                routeOverride,
                true);

            DrawButton(
                "Route Release",
                routeOverride,
                false);

            DrawButton(
                "Session Override",
                sessionOverride,
                true);

            DrawButton(
                "Session Release",
                sessionOverride,
                false);

            if (GUILayout.Button("Capture Evidence"))
            {
                CaptureEvidence("manual-capture");
            }

            GUI.DragWindow(
                new Rect(0f, 0f, 10000f, 24f));
        }

        private void DrawButton(
            string label,
            ScopedCameraOverrideBinding binding,
            bool request)
        {
            if (GUILayout.Button(label))
            {
                Execute(
                    label,
                    binding,
                    request);
            }
        }

        public void RequestActivityOverride()
        {
            Execute(
                "activity-request",
                activityOverride,
                true);
        }

        public void ReleaseActivityOverride()
        {
            Execute(
                "activity-release",
                activityOverride,
                false);
        }

        public void RequestRouteOverride()
        {
            Execute(
                "route-request",
                routeOverride,
                true);
        }

        public void ReleaseRouteOverride()
        {
            Execute(
                "route-release",
                routeOverride,
                false);
        }

        public void RequestSessionOverride()
        {
            Execute(
                "session-request",
                sessionOverride,
                true);
        }

        public void ReleaseSessionOverride()
        {
            Execute(
                "session-release",
                sessionOverride,
                false);
        }

        private void Execute(
            string action,
            ScopedCameraOverrideBinding binding,
            bool request)
        {
            if (binding == null)
            {
                Debug.LogError(
                    $"{LogPrefix} action='{action}' " +
                    "status='Blocked' " +
                    "reason='override binding missing'.",
                    this);
                return;
            }

            CameraOverrideResult result =
                request
                    ? binding.RequestOverride()
                    : binding.ReleaseOverride();

            Debug.Log(
                $"{LogPrefix} action='{action}' " +
                $"operation='{result.Operation}' " +
                $"succeeded='{result.Succeeded}' " +
                $"active='{result.IsActive}' " +
                $"diagnostic='{result.Diagnostic}'.",
                this);

            CaptureEvidence(action);
        }

        private void CaptureEvidence(
            string step)
        {
            if (outputSession == null)
            {
                Debug.LogError(
                    $"{LogPrefix} step='{step}' " +
                    "status='Blocked' " +
                    "reason='camera output session is not attached'.",
                    this);
                return;
            }

            if (!outputSession.TryGetSession(
                    out CameraOutputSession session,
                    out string diagnostic))
            {
                Debug.LogError(
                    $"{LogPrefix} step='{step}' " +
                    "status='Blocked' " +
                    $"reason='{diagnostic}'.",
                    this);
                return;
            }

            CameraOutputContextSnapshot snapshot =
                session.Context.CaptureSnapshot();

            string winner =
                snapshot.HasWinner
                    ? snapshot.Winner.RequestId.ToString()
                    : "none";

            string owner =
                snapshot.HasWinner
                    ? snapshot.Winner.Owner.ToString()
                    : "none";

            int precedence =
                snapshot.HasWinner
                    ? snapshot.Winner.Policy.Precedence
                    : 0;

            Debug.Log(
                $"{LogPrefix} step='{step}' " +
                $"requests='{snapshot.AdmittedRequestCount}' " +
                $"winner='{winner}' " +
                $"owner='{owner}' " +
                $"precedence='{precedence}' " +
                $"player='{StatusOf(playerBinding)}' " +
                $"activity='{StatusOf(activityOverride)}' " +
                $"route='{StatusOf(routeOverride)}' " +
                $"session='{StatusOf(sessionOverride)}'.",
                this);
        }

        private static string StatusOf(
            LocalPlayerCameraRequestBinding binding)
        {
            return binding != null
                ? binding.LastStatus
                : "Missing";
        }

        private static string StatusOf(
            ScopedCameraOverrideBinding binding)
        {
            return binding != null
                ? binding.LastStatus
                : "Missing";
        }

        public void AttachOutputSession(
            CameraOutputSessionBinding binding)
        {
            outputSession = binding;
        }

        public void DetachOutputSession(
            string reason)
        {
            outputSession = null;
        }

        public void AttachSessionCameraOverride(
            SessionCameraOverrideBinding binding)
        {
            sessionOverride = binding;
        }
    }
}
