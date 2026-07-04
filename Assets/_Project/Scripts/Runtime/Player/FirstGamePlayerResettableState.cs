using Immersive.Framework.Reset;
using Immersive.Framework.Reset.Unity;
using UnityEngine;
namespace Project._Project.Scripts.Runtime.Player
{
    public sealed class FirstGamePlayerResettableState : MonoBehaviour, IUnityResettable, IUnityResettableMetadata
    {
        private int _resetCount;
        private Vector3 _baselinePosition;

        public string ResetParticipantId => "firstgame.player.reset-probe";

        public ResetParticipantRequiredness ResetRequiredness => ResetParticipantRequiredness.Required;

        public int ResetOrder => 100;

        public string ResetDisplayName => "FirstGame Player Reset Probe";

        public string ResetSource => nameof(FirstGamePlayerResettableState);

        public string ResetReason => "firstgame.player.reset-probe";

        private void Awake()
        {
            _baselinePosition = transform.position;
        }

        public ResetParticipantResult Reset(ResetContext context)
        {
            _resetCount++;
            transform.position = _baselinePosition;

            Debug.Log($"[FIRSTGAME_RESETTABLE] id='{ResetParticipantId}' resetCount='{_resetCount}' reason='{context.Reason}'");

            return ResetParticipantResult.CreateSucceeded(
                context.Participant,
                nameof(FirstGamePlayerResettableState),
                context.Reason,
                $"FirstGame player reset probe executed. resetCount='{_resetCount}'.");
        }
    }
}