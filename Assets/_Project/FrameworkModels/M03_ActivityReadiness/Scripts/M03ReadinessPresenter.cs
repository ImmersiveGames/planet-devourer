using UnityEngine;

namespace FirstGame.FrameworkModels.ActivityReadiness
{
    [DisallowMultipleComponent]
    public sealed class M03ReadinessPresenter : MonoBehaviour
    {
        private const string LogPrefix = "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Header("Labels")]
        [SerializeField] private TextMesh statusLabel;
        [SerializeField] private TextMesh detailLabel;

        [Header("Visuals")]
        [SerializeField] private GameObject waitingVisual;
        [SerializeField] private GameObject readyVisual;
        [SerializeField] private GameObject preparedContent;

        private void Awake()
        {
            ResetPresentation();
        }

        private void OnDisable()
        {
            ResetPresentation();
        }

        public void ShowPreparing()
        {
            Present("Waiting", "Preparation in progress", true, false, false);
            Debug.Log($"{LogPrefix} presentation='Preparing' object='{gameObject.name}'.", this);
        }

        public void ShowReady()
        {
            Present("Ready", "Prepared content is available", false, true, true);
            Debug.Log($"{LogPrefix} presentation='Ready' object='{gameObject.name}'.", this);
        }

        public void ShowNotReady()
        {
            Present("Not Ready", "Content is unavailable", false, false, false);
            Debug.Log($"{LogPrefix} presentation='NotReady' object='{gameObject.name}'.", this);
        }

        public void ResetPresentation()
        {
            Present("Waiting", "Waiting for Activity preparation", true, false, false);
        }

        private void Present(
            string status,
            string detail,
            bool showWaiting,
            bool showReady,
            bool showPreparedContent)
        {
            if (statusLabel != null)
            {
                statusLabel.text = status;
            }

            if (detailLabel != null)
            {
                detailLabel.text = detail;
            }

            if (waitingVisual != null)
            {
                waitingVisual.SetActive(showWaiting);
            }

            if (readyVisual != null)
            {
                readyVisual.SetActive(showReady);
            }

            if (preparedContent != null)
            {
                preparedContent.SetActive(showPreparedContent);
            }
        }
    }
}
