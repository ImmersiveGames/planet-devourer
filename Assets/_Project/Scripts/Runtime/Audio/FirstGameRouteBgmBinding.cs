using Immersive.Audio.Authoring;
using Immersive.Framework.Authoring;
using Immersive.Framework.RouteLifecycle;
using UnityEngine;

namespace Project._Project.Scripts.Runtime.Audio
{
    /// <summary>
    /// FIRSTGAME scene-authored Route lifecycle adapter for BGM.
    /// This consumes framework RouteContent callbacks without adding an audio dependency to the framework.
    ///
    /// Startup Activity BGM is resolved through an explicit scene-authored reference, not global scene search.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameRouteBgmBinding : RouteContentBehaviour
    {
        [SerializeField] private AudioBgmCueAsset routeBgm;
        [SerializeField] private FirstGameBgmDirector director;
        [SerializeField] private FirstGameActivityBgmBinding startupActivityBgmBinding;

        protected override void OnRouteContentEntered(RouteContentLifecycleContext context)
        {
            if (director == null)
            {
                Debug.LogError("[FIRSTGAME_BGM] Route BGM binding requires a FirstGameBgmDirector.", this);
                return;
            }

            ActivityAsset startupActivity = context.Route != null && context.Route.HasStartupActivity
                ? context.Route.StartupActivity
                : null;

            bool hasStartupActivity = startupActivity != null;
            director.SetRouteBgm(routeBgm, hasStartupActivity);

            if (!hasStartupActivity)
            {
                return;
            }

            if (startupActivityBgmBinding != null
                && startupActivityBgmBinding.TryApplyStartupActivityBgm(director, startupActivity, context.RouteName))
            {
                return;
            }

            Debug.LogWarning(
                $"[FIRSTGAME_BGM] Route has Startup Activity but no valid explicit Startup Activity BGM binding was assigned. route='{context.RouteName}' startupActivity='{FormatActivity(startupActivity)}'. Route BGM fallback will be applied.",
                this);
            director.Refresh();
        }

        protected override void OnRouteContentExited(RouteContentLifecycleContext context)
        {
            if (director == null)
            {
                Debug.LogError("[FIRSTGAME_BGM] Route BGM binding requires a FirstGameBgmDirector.", this);
                return;
            }

            director.ClearRouteBgm(routeBgm);
        }

        private static string FormatActivity(ActivityAsset activity)
        {
            return activity != null ? activity.ActivityName : "<none>";
        }
    }
}
