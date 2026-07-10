using Immersive.Framework.Reset;
using Immersive.Framework.Reset.Unity;
using UnityEngine;

namespace FirstGame.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class FirstGameMinimalLoopObjective : MonoBehaviour, IUnityResettable, IUnityResettableMetadata
    {
        [Header("Identity")]
        [SerializeField] private string objectiveId = "firstgame.minimal-loop.goal";

        [Header("References")]
        [SerializeField] private Transform expectedPlayer;
        [SerializeField] private Transform objectiveVisual;
        [SerializeField] private Renderer objectiveRenderer;

        [Header("Presentation")]
        [SerializeField] private Vector3 completedLocalOffset = new Vector3(0f, 1.5f, 0f);
        [SerializeField] private Color idleColor = new Color(0.15f, 0.55f, 1f, 1f);
        [SerializeField] private Color completedColor = new Color(0.2f, 1f, 0.35f, 1f);

        private Vector3 _baselineLocalPosition;
        private bool _baselineCaptured;
        private bool _completed;
        private MaterialPropertyBlock _propertyBlock;

        public string ObjectiveId => objectiveId;
        public bool IsCompleted => _completed;

        public string ResetParticipantId => objectiveId + ".state";
        public ResetParticipantRequiredness ResetRequiredness => ResetParticipantRequiredness.Required;
        public int ResetOrder => 20;
        public string ResetDisplayName => "FIRSTGAME Minimal Loop Objective";
        public string ResetSource => nameof(FirstGameMinimalLoopObjective);
        public string ResetReason => "firstgame.minimal-loop.reset";

        public event System.Action<FirstGameMinimalLoopObjective> Completed;
        public event System.Action<FirstGameMinimalLoopObjective> Restored;

        private void Awake()
        {
            CaptureBaseline();
            ApplyIdlePresentation();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_completed || expectedPlayer == null || other == null)
            {
                return;
            }

            Transform candidate = other.transform;
            if (candidate != expectedPlayer && !candidate.IsChildOf(expectedPlayer))
            {
                return;
            }

            Complete();
        }

        public void Complete()
        {
            if (_completed)
            {
                return;
            }

            CaptureBaseline();
            _completed = true;
            ApplyCompletedPresentation();

            Debug.Log(
                $"[G1B_FIRSTGAME_LOOP] status='ObjectiveCompleted' objectiveId='{objectiveId}'.");

            Completed?.Invoke(this);
        }

        public ResetParticipantResult Reset(ResetContext context)
        {
            CaptureBaseline();
            _completed = false;
            ApplyIdlePresentation();

            Debug.Log(
                $"[G1B_FIRSTGAME_LOOP] status='ObjectiveRestored' objectiveId='{objectiveId}' reason='{context.Reason}'.");

            Restored?.Invoke(this);

            return ResetParticipantResult.CreateSucceeded(
                context.Participant,
                nameof(FirstGameMinimalLoopObjective),
                context.Reason,
                $"FIRSTGAME objective '{objectiveId}' returned to its initial state.");
        }

        private void CaptureBaseline()
        {
            if (_baselineCaptured || objectiveVisual == null)
            {
                return;
            }

            _baselineLocalPosition = objectiveVisual.localPosition;
            _baselineCaptured = true;
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void ApplyIdlePresentation()
        {
            if (objectiveVisual != null && _baselineCaptured)
            {
                objectiveVisual.localPosition = _baselineLocalPosition;
                objectiveVisual.localRotation = Quaternion.identity;
            }

            ApplyColor(idleColor);
        }

        private void ApplyCompletedPresentation()
        {
            if (objectiveVisual != null && _baselineCaptured)
            {
                objectiveVisual.localPosition = _baselineLocalPosition + completedLocalOffset;
                objectiveVisual.localRotation = Quaternion.Euler(0f, 45f, 0f);
            }

            ApplyColor(completedColor);
        }

        private void ApplyColor(Color color)
        {
            if (objectiveRenderer == null)
            {
                return;
            }

            _propertyBlock ??= new MaterialPropertyBlock();
            objectiveRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor("_BaseColor", color);
            _propertyBlock.SetColor("_Color", color);
            objectiveRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
