using Immersive.Foundation.Events;
using Immersive.Framework.Camera;
using Immersive.Framework.GameFlow;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.Gameplay.Diagnostics
{
    /// <summary>
    /// FIRSTGAME-only controls for the manual C9M consumer validation.
    /// Uses public framework boundaries and never selects or applies a camera winner.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameC9MCameraValidationControls : MonoBehaviour
    {
        private const string LogPrefix = "[C9M_FIRSTGAME_CAMERA]";
        private const int ControlsWindowId = 9031;

        [Header("Canonical Flow")]
        [SerializeField] private ActivityRequestTrigger activityRequestTrigger;

        [Header("Camera Evidence")]
        [SerializeField] private CameraOutputSessionBinding outputSession;
        [SerializeField] private RouteCameraRequestBinding routeBinding;
        [SerializeField] private LocalPlayerCameraRequestBinding playerBinding;
        [SerializeField] private ActivityCameraRequestBinding activityBinding;

        [Header("Manual Keys")]
        [SerializeField] private Key clearActivityKey = Key.F6;
        [SerializeField] private Key requestActivityKey = Key.F7;
        [SerializeField] private Key releasePlayerKey = Key.F8;
        [SerializeField] private Key restorePlayerKey = Key.F9;
        [SerializeField] private Key captureEvidenceKey = Key.F10;

        [Header("Visible Controls")]
        [SerializeField] private bool showControls = true;
        [SerializeField] private Rect controlsRect = new Rect(12f, 12f, 310f, 260f);

        private IEventBinding activityRequestEvents;

        private void OnEnable()
        {
            if (activityRequestTrigger != null)
            {
                activityRequestEvents =
                    activityRequestTrigger.SubscribeRequestEvents(OnActivityRequestEvent);
            }
        }

        private void Start()
        {
            CaptureEvidence("gameplay-entry");
            Debug.Log(
                $"{LogPrefix} controls-ready='True' " +
                $"clearActivity='{clearActivityKey}' requestActivity='{requestActivityKey}' " +
                $"releasePlayer='{releasePlayerKey}' restorePlayer='{restorePlayerKey}' " +
                $"captureEvidence='{captureEvidenceKey}'.",
                this);
        }

        private void OnDisable()
        {
            activityRequestEvents?.Dispose();
            activityRequestEvents = null;
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard[clearActivityKey].wasPressedThisFrame)
            {
                ClearActivity();
            }

            if (keyboard[requestActivityKey].wasPressedThisFrame)
            {
                RequestActivity();
            }

            if (keyboard[releasePlayerKey].wasPressedThisFrame)
            {
                SetPlayerEligibility(false);
            }

            if (keyboard[restorePlayerKey].wasPressedThisFrame)
            {
                SetPlayerEligibility(true);
            }

            if (keyboard[captureEvidenceKey].wasPressedThisFrame)
            {
                CaptureEvidence("manual-capture");
            }
        }

        private void OnGUI()
        {
            if (!showControls)
            {
                return;
            }

            controlsRect = GUI.Window(
                ControlsWindowId,
                controlsRect,
                DrawControls,
                "C9M Camera Validation");
        }

        private void DrawControls(int windowId)
        {
            GUILayout.Label("Activity > Player > Route");

            if (GUILayout.Button("F6 - Clear Activity"))
            {
                ClearActivity();
            }

            if (GUILayout.Button("F7 - Request Activity"))
            {
                RequestActivity();
            }

            if (GUILayout.Button("F8 - Release Player Camera"))
            {
                ReleaseLocalPlayerCamera();
            }

            if (GUILayout.Button("F9 - Restore Player Camera"))
            {
                RestoreLocalPlayerCamera();
            }

            if (GUILayout.Button("F10 - Capture Evidence"))
            {
                CaptureCameraEvidence();
            }

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        public void ClearActivity()
        {
            if (!TryValidateActivityTrigger("clear-activity"))
            {
                return;
            }

            activityRequestTrigger.ClearActivity();
        }

        public void RequestActivity()
        {
            if (!TryValidateActivityTrigger("request-activity"))
            {
                return;
            }

            activityRequestTrigger.RequestActivity();
        }

        public void ReleaseLocalPlayerCamera()
        {
            SetPlayerEligibility(false);
        }

        public void RestoreLocalPlayerCamera()
        {
            SetPlayerEligibility(true);
        }

        public void CaptureCameraEvidence()
        {
            CaptureEvidence("manual-api-capture");
        }

        private void SetPlayerEligibility(bool eligible)
        {
            if (playerBinding == null)
            {
                LogBlocked(
                    eligible ? "restore-player" : "release-player",
                    "LocalPlayerCameraRequestBinding reference is missing.");
                return;
            }

            bool succeeded = playerBinding.SetLocalPlayerEligible(eligible);
            string step = eligible ? "player-eligible" : "player-released";

            Debug.Log(
                $"{LogPrefix} action='{step}' succeeded='{succeeded}' " +
                $"bindingStatus='{playerBinding.LastStatus}' " +
                $"diagnostic='{playerBinding.LastDiagnostic}'.",
                this);

            CaptureEvidence(step);
        }

        private void OnActivityRequestEvent(ActivityRequestTriggerEvent requestEvent)
        {
            if (requestEvent == null || !requestEvent.IsCompleted)
            {
                return;
            }

            string step = requestEvent.ClearsActivity
                ? "activity-cleared"
                : "activity-requested";

            Debug.Log(
                $"{LogPrefix} action='{step}' outcome='{requestEvent.Outcome}' " +
                $"succeeded='{requestEvent.Succeeded}' message='{requestEvent.Message}'.",
                this);

            CaptureEvidence(step);
        }

        private bool TryValidateActivityTrigger(string action)
        {
            if (activityRequestTrigger == null)
            {
                LogBlocked(action, "ActivityRequestTrigger reference is missing.");
                return false;
            }

            if (activityRequestTrigger.IsRequestInFlight)
            {
                LogBlocked(action, "An Activity request is already in flight.");
                return false;
            }

            return true;
        }

        private void CaptureEvidence(string step)
        {
            if (outputSession == null)
            {
                LogBlocked(step, "CameraOutputSessionBinding reference is missing.");
                return;
            }

            if (!outputSession.TryGetSession(out CameraOutputSession session, out string diagnostic))
            {
                LogBlocked(step, diagnostic);
                return;
            }

            CameraOutputContextSnapshot snapshot = session.Context.CaptureSnapshot();
            string winnerRequest = snapshot.HasWinner
                ? snapshot.Winner.RequestId.ToString()
                : "none";
            string winnerOwner = snapshot.HasWinner
                ? snapshot.Winner.Owner.ToString()
                : "none";
            string winnerRig = snapshot.HasWinner && snapshot.Winner.Rig.Composer != null
                ? snapshot.Winner.Rig.Composer.name
                : "none";
            int winnerPrecedence = snapshot.HasWinner
                ? snapshot.Winner.Policy.Precedence
                : 0;

            Debug.Log(
                $"{LogPrefix} evidence step='{step}' output='{snapshot.OutputId}' " +
                $"requests='{snapshot.AdmittedRequestCount}' hasWinner='{snapshot.HasWinner}' " +
                $"winnerRequest='{winnerRequest}' winnerOwner='{winnerOwner}' " +
                $"winnerPrecedence='{winnerPrecedence}' winnerRig='{winnerRig}' " +
                $"routeStatus='{StatusOf(routeBinding)}' playerStatus='{StatusOf(playerBinding)}' " +
                $"activityStatus='{StatusOf(activityBinding)}' blockingIssues='0'.",
                this);
        }

        private void LogBlocked(string action, string reason)
        {
            Debug.LogError(
                $"{LogPrefix} action='{action}' status='Blocked' reason='{reason}' blockingIssues='1'.",
                this);
        }

        private static string StatusOf(RouteCameraRequestBinding binding)
        {
            return binding != null ? binding.LastStatus : "Missing";
        }

        private static string StatusOf(LocalPlayerCameraRequestBinding binding)
        {
            return binding != null ? binding.LastStatus : "Missing";
        }

        private static string StatusOf(ActivityCameraRequestBinding binding)
        {
            return binding != null ? binding.LastStatus : "Missing";
        }
    }
}
