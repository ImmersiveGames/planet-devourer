using Immersive.Framework.ActivityRestart;
using Immersive.Framework.ObjectReset;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstGame.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class FirstGameMinimalLoopControls : MonoBehaviour
    {
        [SerializeField] private ObjectResetGroupTrigger resetTrigger;
        [SerializeField] private ActivityRestartTrigger activityRestartTrigger;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.rKey.wasPressedThisFrame)
            {
                if (resetTrigger == null)
                {
                    Debug.LogError("[G1B_FIRSTGAME_LOOP] status='ResetUnavailable' reason='Reset trigger reference is missing'.");
                    return;
                }

                resetTrigger.RequestObjectResetGroup();
            }

            if (keyboard.tKey.wasPressedThisFrame)
            {
                if (activityRestartTrigger == null)
                {
                    Debug.LogError("[G1B_FIRSTGAME_LOOP] status='RestartUnavailable' reason='Activity restart trigger reference is missing'.");
                    return;
                }

                activityRestartTrigger.RequestActivityRestart();
            }
        }
    }
}
