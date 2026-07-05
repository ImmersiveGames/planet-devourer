using Immersive.Framework.Reset;
using Immersive.Framework.Reset.Unity;
using UnityEngine;

namespace Project._Project.Scripts.Runtime.Player
{
    /// <summary>
    /// Minimal FIRSTGAME gameplay-state reset participant.
    ///
    /// This component demonstrates the consumer-side IUnityResettable model.
    /// Transform reset remains owned by UnityTransformResetParticipant; this component
    /// only resets local gameplay state owned by the player prototype.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGamePlayerResettableState : MonoBehaviour, IUnityResettable, IUnityResettableMetadata
    {
        private const string ParticipantId = "firstgame.player.resettable-state";

        [Header("FIRSTGAME Runtime State")]
        [SerializeField] private int baselineStateValue;
        [SerializeField] private int runtimeStateValue;

        private int _resetCount;

        public string ResetParticipantId => ParticipantId;

        public ResetParticipantRequiredness ResetRequiredness => ResetParticipantRequiredness.Required;

        public int ResetOrder => 100;

        public string ResetDisplayName => "FirstGame Player Resettable State";

        public string ResetSource => nameof(FirstGamePlayerResettableState);

        public string ResetReason => ParticipantId;

        private void Awake()
        {
            runtimeStateValue = baselineStateValue;
        }

        [ContextMenu("FIRSTGAME/Mutate Runtime State")]
        public void MutateRuntimeState()
        {
            runtimeStateValue++;
        }

        public ResetParticipantResult Reset(ResetContext context)
        {
            _resetCount++;
            runtimeStateValue = baselineStateValue;

            Debug.Log(
                $"[FIRSTGAME_RESETTABLE] id='{ResetParticipantId}' resetCount='{_resetCount}' state='{runtimeStateValue}' reason='{context.Reason}'",
                this);

            return ResetParticipantResult.CreateSucceeded(
                context.Participant,
                nameof(FirstGamePlayerResettableState),
                context.Reason,
                $"FirstGame player resettable state restored. resetCount='{_resetCount}' state='{runtimeStateValue}'.");
        }
    }
}
