using Immersive.Audio.Authoring;
using Immersive.Framework.ActivityFlow;
using Immersive.Framework.Authoring;
using UnityEngine;

namespace Project._Project.Scripts.Runtime.Audio
{
    /// <summary>
    /// FIRSTGAME scene-authored Activity lifecycle adapter for BGM.
    /// This consumes framework ActivityContent callbacks without adding an audio dependency to the framework.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameActivityBgmBinding : ActivityContentBehaviour
    {
        [SerializeField] private AudioBgmCueAsset activityBgm;
        [SerializeField] private FirstGameActivityBgmPolicy policy = FirstGameActivityBgmPolicy.UseOwnOrRoute;
        [SerializeField] private FirstGameBgmDirector director;

        internal bool TryApplyStartupActivityBgm(
            FirstGameBgmDirector expectedDirector,
            ActivityAsset expectedActivity,
            string routeName)
        {
            if (director == null)
            {
                Debug.LogError("[FIRSTGAME_BGM] Activity BGM binding requires a FirstGameBgmDirector.", this);
                return false;
            }

            if (expectedDirector != null && !ReferenceEquals(director, expectedDirector))
            {
                Debug.LogWarning(
                    $"[FIRSTGAME_BGM] Startup Activity BGM binding ignored because it targets a different director. route='{routeName}' activityBgm='{FormatCue(activityBgm)}'.",
                    this);
                return false;
            }

            if (!MatchesActivity(expectedActivity))
            {
                Debug.LogWarning(
                    $"[FIRSTGAME_BGM] Startup Activity BGM binding ignored because it does not match the Route Startup Activity. route='{routeName}' expectedActivity='{FormatActivity(expectedActivity)}' bindingActivity='{FormatActivity(ResolveAssignedActivity())}' activityBgm='{FormatCue(activityBgm)}'.",
                    this);
                return false;
            }

            Debug.Log(
                $"[FIRSTGAME_BGM] Startup Activity BGM pre-applied from explicit Route binding. route='{routeName}' activity='{FormatActivity(expectedActivity)}' activityBgm='{FormatCue(activityBgm)}' policy='{policy}'.",
                this);
            director.SetActivityBgm(activityBgm, policy);
            return true;
        }

        protected override void OnActivityContentEntered(ActivityContentLifecycleContext context)
        {
            if (director == null)
            {
                Debug.LogError("[FIRSTGAME_BGM] Activity BGM binding requires a FirstGameBgmDirector.", this);
                return;
            }

            director.SetActivityBgm(activityBgm, policy);
        }

        protected override void OnActivityContentExited(ActivityContentLifecycleContext context)
        {
            if (director == null)
            {
                Debug.LogError("[FIRSTGAME_BGM] Activity BGM binding requires a FirstGameBgmDirector.", this);
                return;
            }

            bool deferRefreshForActivityTransition = context.NextActivity != null
                && !ReferenceEquals(context.NextActivity, context.Activity);

            director.ClearActivityBgm(activityBgm, deferRefreshForActivityTransition);
        }

        private bool MatchesActivity(ActivityAsset expectedActivity)
        {
            if (expectedActivity == null)
            {
                return false;
            }

            ActivityAsset assignedActivity = ResolveAssignedActivity();
            return ReferenceEquals(assignedActivity, expectedActivity);
        }

        private ActivityAsset ResolveAssignedActivity()
        {
            if (TryGetComponent(out ActivityLocalVisibilityAdapter adapter) && adapter != null)
            {
                return adapter.Activity;
            }

            return null;
        }

        private static string FormatCue(AudioBgmCueAsset cue)
        {
            return cue != null ? cue.name : "<silence>";
        }

        private static string FormatActivity(ActivityAsset activity)
        {
            return activity != null ? activity.ActivityName : "<none>";
        }
    }
}
