using System.Collections;
using Immersive.Framework.ActivityFlow;
using UnityEngine;

namespace FirstGame.FrameworkModels.ActivityReadiness
{
    [DisallowMultipleComponent]
    public sealed class M03PreparationSequence : MonoBehaviour
    {
        private const string LogPrefix = "[FIRSTGAME_M03_ACTIVITY_READINESS]";

        [Header("Required References")]
        [SerializeField] private ActivityReadinessParticipant readinessParticipant;
        [SerializeField] private Transform preparationVisual;

        [Header("Preparation Visual")]
        [SerializeField, Min(0.1f)] private float preparationDuration = 1.5f;
        [SerializeField] private Vector3 preparedLocalPosition = new Vector3(0f, 1f, 0f);

        private Coroutine preparationRoutine;
        private Vector3 initialLocalPosition;
        private bool hasInitialLocalPosition;

        private void Awake()
        {
            CaptureInitialLocalPosition();
            RestoreVisual();
        }

        private void OnDisable()
        {
            StopPreparationSequence();
        }

        public void BeginPreparation()
        {
            StopPreparationSequence();
            CaptureInitialLocalPosition();
            RestoreVisual();

            if (readinessParticipant == null || preparationVisual == null)
            {
                Debug.LogError(
                    $"{LogPrefix} Preparation cannot start because the participant or visual reference is missing.",
                    this);
                return;
            }

            Debug.Log($"{LogPrefix} preparation='started' object='{gameObject.name}'.", this);
            preparationRoutine = StartCoroutine(ExecutePreparationSequence());
        }

        public void ReleasePreparation()
        {
            StopPreparationSequence();
            RestoreVisual();
            Debug.Log($"{LogPrefix} preparation='released' object='{gameObject.name}'.", this);
        }

        private IEnumerator ExecutePreparationSequence()
        {
            Vector3 startPosition = preparationVisual.localPosition;
            float elapsed = 0f;
            while (elapsed < preparationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / preparationDuration);
                preparationVisual.localPosition = Vector3.Lerp(
                    startPosition,
                    preparedLocalPosition,
                    progress);
                yield return null;
            }

            preparationVisual.localPosition = preparedLocalPosition;
            preparationRoutine = null;
            Debug.Log($"{LogPrefix} preparation='completed' object='{gameObject.name}'.", this);
            readinessParticipant.CompletePreparation();
        }

        private void StopPreparationSequence()
        {
            if (preparationRoutine == null)
            {
                return;
            }

            StopCoroutine(preparationRoutine);
            preparationRoutine = null;
        }

        private void CaptureInitialLocalPosition()
        {
            if (preparationVisual == null || hasInitialLocalPosition)
            {
                return;
            }

            initialLocalPosition = preparationVisual.localPosition;
            hasInitialLocalPosition = true;
        }

        private void RestoreVisual()
        {
            if (preparationVisual != null && hasInitialLocalPosition)
            {
                preparationVisual.localPosition = initialLocalPosition;
            }
        }
    }
}
