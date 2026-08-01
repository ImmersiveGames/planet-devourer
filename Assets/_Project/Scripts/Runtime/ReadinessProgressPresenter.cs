using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project._Project.Scripts.Runtime
{
    [DisallowMultipleComponent]
    public sealed class ReadinessProgressPresenter : MonoBehaviour
    {
        private const string LogPrefix =
            "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Header("Progress Source")]
        [SerializeField]
        private ReadinessPreparationArea progressSource;

        [Header("Progress")]
        [SerializeField]
        private Image progressFill;

        [SerializeField]
        private TMP_Text progressLabel;

        [SerializeField]
        private TMP_Text statusLabel;

        [Header("Ready Content")]
        [SerializeField]
        private GameObject preparedContent;

        [Header("Presentation")]
        [SerializeField]
        private Color preparingColor = new Color(1f, 0.65f, 0f, 1f);

        [SerializeField]
        private Color localCompleteColor = new Color(1f, 0.85f, 0.2f, 1f);

        [SerializeField]
        private Color readyColor = new Color(0.2f, 0.8f, 0.25f, 1f);

        [SerializeField]
        private Color notReadyColor = new Color(0.8f, 0.2f, 0.2f, 1f);

        private bool _frameworkReady;

        private void Awake()
        {
            ResetPresentation();

            if (progressSource == null)
            {
                Debug.LogError(
                    $"{LogPrefix} progress-presentation='invalid' reason='Progress Source is missing.'.",
                    this);
            }
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

        private void SetProgressVisual(float progress)
        {
            if (progressFill != null)
            {
                progressFill.fillAmount = progress;
            }

            if (progressLabel != null)
            {
                int percentage = Mathf.RoundToInt(progress * 100f);
                progressLabel.text = $"{percentage}%";
            }
        }

        private void SetPreparedContentActive(bool active)
        {
            if (preparedContent == null || preparedContent.activeSelf == active)
            {
                return;
            }

            preparedContent.SetActive(active);
        }
    }
}
