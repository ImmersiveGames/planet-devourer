using Immersive.Audio.Authoring;
using Immersive.Audio.Contracts;
using Immersive.Audio.Unity.Hosts;
using UnityEngine;

namespace Project._Project.Scripts.Runtime.Audio
{
    /// <summary>
    /// FIRSTGAME application adapter between Immersive Framework route/activity lifecycle callbacks
    /// and the standalone Immersive Audio package.
    ///
    /// Priority:
    /// Startup Activity BGM > Activity BGM > retained Activity BGM for current Route > Route BGM > Silence.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameBgmDirector : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioRuntimeHost audioRuntimeHost;

        [Header("Diagnostics")]
        [SerializeField] private bool logTransitions = true;

        private AudioBgmCueAsset currentRouteBgm;
        private AudioBgmCueAsset currentActivityBgm;
        private AudioBgmCueAsset retainedActivityBgmForCurrentRoute;
        private AudioBgmCueAsset currentEffectiveBgm;
        private FirstGameActivityBgmPolicy currentActivityPolicy = FirstGameActivityBgmPolicy.UseOwnOrRoute;
        private bool hasActiveActivityBgmBinding;

        public AudioBgmCueAsset CurrentRouteBgm => currentRouteBgm;

        public AudioBgmCueAsset CurrentActivityBgm => currentActivityBgm;

        public AudioBgmCueAsset RetainedActivityBgmForCurrentRoute => retainedActivityBgmForCurrentRoute;

        public AudioBgmCueAsset CurrentEffectiveBgm => currentEffectiveBgm;

        public void SetRouteBgm(AudioBgmCueAsset cue)
        {
            SetRouteBgm(cue, false);
        }

        public void SetRouteBgm(AudioBgmCueAsset cue, bool deferRefreshForStartupActivity)
        {
            currentRouteBgm = cue;
            currentActivityBgm = null;
            retainedActivityBgmForCurrentRoute = null;
            hasActiveActivityBgmBinding = false;
            currentActivityPolicy = FirstGameActivityBgmPolicy.UseOwnOrRoute;

            Log($"Route BGM set. routeBgm='{FormatCue(cue)}' retainedActivityBgm='<cleared>' deferRefreshForStartupActivity='{deferRefreshForStartupActivity}'.");

            if (!deferRefreshForStartupActivity)
            {
                Refresh();
            }
        }

        public void ClearRouteBgm(AudioBgmCueAsset cue)
        {
            if (!ReferenceEquals(currentRouteBgm, cue))
            {
                Log($"Route BGM clear ignored as stale. requested='{FormatCue(cue)}' currentRouteBgm='{FormatCue(currentRouteBgm)}'.");
                return;
            }

            currentRouteBgm = null;
            currentActivityBgm = null;
            retainedActivityBgmForCurrentRoute = null;
            hasActiveActivityBgmBinding = false;
            currentActivityPolicy = FirstGameActivityBgmPolicy.UseOwnOrRoute;

            Log("Route BGM cleared. Activity retention cleared with Route scope.");
            Refresh();
        }

        public void SetActivityBgm(AudioBgmCueAsset cue, FirstGameActivityBgmPolicy policy)
        {
            hasActiveActivityBgmBinding = true;
            currentActivityPolicy = policy;

            if (policy == FirstGameActivityBgmPolicy.Stop)
            {
                currentActivityBgm = null;
                Log("Activity BGM policy Stop applied.");
                Refresh();
                return;
            }

            currentActivityBgm = cue;

            if (policy == FirstGameActivityBgmPolicy.UseOwnOrKeepPreviousActivity && cue != null)
            {
                retainedActivityBgmForCurrentRoute = cue;
            }

            Log(
                $"Activity BGM set. activityBgm='{FormatCue(cue)}' policy='{policy}' retainedActivityBgm='{FormatCue(retainedActivityBgmForCurrentRoute)}'.");
            Refresh();
        }

        public void ClearActivityBgm(AudioBgmCueAsset cue)
        {
            ClearActivityBgm(cue, false);
        }

        public void ClearActivityBgm(AudioBgmCueAsset cue, bool deferRefreshForActivityTransition)
        {
            if (cue != null && !ReferenceEquals(currentActivityBgm, cue))
            {
                Log($"Activity BGM clear ignored as stale. requested='{FormatCue(cue)}' currentActivityBgm='{FormatCue(currentActivityBgm)}'.");
                return;
            }

            currentActivityBgm = null;
            hasActiveActivityBgmBinding = false;
            currentActivityPolicy = FirstGameActivityBgmPolicy.UseOwnOrRoute;

            Log($"Activity BGM cleared. retainedActivityBgm='{FormatCue(retainedActivityBgmForCurrentRoute)}' deferRefresh='{deferRefreshForActivityTransition}'.");

            if (!deferRefreshForActivityTransition)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            AudioBgmCueAsset next = ResolveEffectiveBgm();

            if (ReferenceEquals(currentEffectiveBgm, next))
            {
                Log($"BGM refresh skipped. effectiveBgm='{FormatCue(next)}'.");
                return;
            }

            currentEffectiveBgm = next;

            if (audioRuntimeHost == null)
            {
                Debug.LogError(
                    $"[FIRSTGAME_BGM] AudioRuntimeHost is missing. effectiveBgm='{FormatCue(next)}'.",
                    this);
                return;
            }

            AudioPlaybackResult result = next != null
                ? audioRuntimeHost.PlayBgm(next)
                : audioRuntimeHost.StopBgm();

            Log($"BGM applied. effectiveBgm='{FormatCue(next)}' status='{result.Status}'.");
        }

        private AudioBgmCueAsset ResolveEffectiveBgm()
        {
            if (!hasActiveActivityBgmBinding)
            {
                return currentRouteBgm;
            }

            return currentActivityPolicy switch
            {
                FirstGameActivityBgmPolicy.Stop => null,
                FirstGameActivityBgmPolicy.UseOwnOrKeepPreviousActivity => currentActivityBgm != null
                    ? currentActivityBgm
                    : retainedActivityBgmForCurrentRoute != null
                        ? retainedActivityBgmForCurrentRoute
                        : currentRouteBgm,
                _ => currentActivityBgm != null ? currentActivityBgm : currentRouteBgm
            };
        }

        private void Log(string message)
        {
            if (!logTransitions)
            {
                return;
            }

            Debug.Log($"[FIRSTGAME_BGM] {message}", this);
        }

        private static string FormatCue(AudioBgmCueAsset cue)
        {
            return cue != null ? cue.name : "<silence>";
        }
    }
}
