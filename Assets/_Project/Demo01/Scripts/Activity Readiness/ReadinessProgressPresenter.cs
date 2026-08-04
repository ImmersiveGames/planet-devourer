using Immersive.Framework.ActivityFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace _Project.Demo01.Scripts.Activity_Readiness
{
    [DisallowMultipleComponent]
    public sealed class ReadinessProgressPresenter : MonoBehaviour
    {
        private const string LogPrefix =
            "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Header("Progress Source")]
        [SerializeField]
        private ReadinessPreparationArea progressSource;

        [Header("Framework Readiness")]
        [SerializeField]
        private ActivityReadinessEvents readinessEvents;

        [Header("Progress")]
        [SerializeField]
        private Image progressFill;

        [SerializeField]
        private TMP_Text progressLabel;

        [SerializeField]
        private TMP_Text statusLabel;

        [Header("Diagnostics")]
        [SerializeField]
        private TMP_Text diagnosticsLabel;

        [Header("Ready Content")]
        [SerializeField]
        private GameObject preparedContent;

        [Header("Presentation")]
        [SerializeField]
        private Color preparingColor = new(1f, 0.65f, 0f, 1f);

        [SerializeField]
        private Color localCompleteColor = new(1f, 0.85f, 0.2f, 1f);

        [SerializeField]
        private Color readyColor = new(0.2f, 0.8f, 0.25f, 1f);

        [SerializeField]
        private Color notReadyColor = new(0.8f, 0.2f, 0.2f, 1f);

        private bool _frameworkReady;

        private void Awake()
        {
            ValidateReferences();
            ResetPresentation();
        }

        private void OnEnable()
        {
            if (progressSource == null)
            {
                return;
            }

            progressSource.ProgressChanged += HandleProgressChanged;
            HandleProgressChanged(progressSource.Progress);
        }

        private void OnDisable()
        {
            if (progressSource != null)
            {
                progressSource.ProgressChanged -= HandleProgressChanged;
            }
        }

        public void BeginPreparing()
        {
            _frameworkReady = false;
            SetProgressVisual(0f);

            if (statusLabel != null)
            {
                statusLabel.text = "PREPARING";
            }

            if (progressFill != null)
            {
                progressFill.color = preparingColor;
            }

            SetPreparedContentActive(false);
            RefreshReadinessDiagnostics();
        }

        public void ShowReady()
        {
            _frameworkReady = true;
            SetProgressVisual(1f);

            if (statusLabel != null)
            {
                statusLabel.text = "READY";
            }

            if (progressFill != null)
            {
                progressFill.color = readyColor;
            }

            SetPreparedContentActive(true);
            RefreshReadinessDiagnostics();
        }

        public void ShowNotReady()
        {
            _frameworkReady = false;

            if (statusLabel != null)
            {
                statusLabel.text = "NOT READY";
            }

            if (progressFill != null)
            {
                progressFill.color = notReadyColor;
            }

            SetPreparedContentActive(false);
            RefreshReadinessDiagnostics();
        }

        public void ResetPresentation()
        {
            _frameworkReady = false;
            SetProgressVisual(0f);

            if (statusLabel != null)
            {
                statusLabel.text = "WAITING";
            }

            if (progressFill != null)
            {
                progressFill.color = preparingColor;
            }

            SetPreparedContentActive(false);
            RefreshReadinessDiagnostics();
        }

        private void HandleProgressChanged(float normalizedProgress)
        {
            float progress = Mathf.Clamp01(normalizedProgress);
            SetProgressVisual(progress);

            if (_frameworkReady)
            {
                return;
            }

            bool localPreparationComplete = progress >= 0.999f;

            if (statusLabel != null)
            {
                statusLabel.text = localPreparationComplete
                    ? "LOCAL PREPARATION COMPLETE"
                    : "PREPARING";
            }

            if (progressFill != null)
            {
                progressFill.color = localPreparationComplete
                    ? localCompleteColor
                    : preparingColor;
            }
        }

        private void RefreshReadinessDiagnostics()
        {
            if (diagnosticsLabel == null)
            {
                return;
            }

            if (readinessEvents == null ||
                readinessEvents.LastRevision <= 0)
            {
                diagnosticsLabel.text = "No readiness snapshot";
                return;
            }

            ActivityReadinessSnapshot snapshot =
                readinessEvents.LastSnapshot;

            diagnosticsLabel.text =
                $"Reason: {snapshot.Reason}\n" +
                $"Participants: {snapshot.ParticipantCount}\n" +
                $"Required: {snapshot.RequiredCount}\n" +
                $"Optional: {snapshot.OptionalCount}\n" +
                $"Pending: {snapshot.PendingCount}\n" +
                $"Completed: {snapshot.CompletedCount}\n" +
                $"Failed: {snapshot.FailedCount}\n" +
                $"Revision: {snapshot.Revision}";
        }

        private void SetProgressVisual(float progress)
        {
            if (progressFill != null)
            {
                progressFill.fillAmount = progress;
            }

            if (progressLabel != null)
            {
                int percentage =
                    Mathf.RoundToInt(progress * 100f);

                progressLabel.text = $"{percentage}%";
            }
        }

        private void SetPreparedContentActive(bool active)
        {
            if (preparedContent == null ||
                preparedContent.activeSelf == active)
            {
                return;
            }

            preparedContent.SetActive(active);
        }

        private void ValidateReferences()
        {
            if (progressSource == null)
            {
                Debug.LogError(
                    $"{LogPrefix} progress-presentation='invalid' " +
                    "reason='Progress Source is missing.'.",
                    this);
            }

            if (readinessEvents == null)
            {
                Debug.LogError(
                    $"{LogPrefix} readiness-diagnostics='invalid' " +
                    "reason='Activity Readiness Events is missing.'.",
                    this);
            }

            if (diagnosticsLabel == null)
            {
                Debug.LogError(
                    $"{LogPrefix} readiness-diagnostics='invalid' " +
                    "reason='Diagnostics Label is missing.'.",
                    this);
            }
        }
    }
}