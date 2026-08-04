using UnityEngine;
namespace _Project.Demo01.Scripts.Routes_and_Activities
{
    [DisallowMultipleComponent]
    public sealed class LifecycleCanvasEventReporter : MonoBehaviour
    {
        [SerializeField]
        private LifecycleCanvasPresenter presenter;

        [SerializeField]
        private LifecycleCanvasScope scope = LifecycleCanvasScope.Scene;

        [SerializeField]
        private string identity = string.Empty;

        public LifecycleCanvasPresenter Presenter => presenter;
        public LifecycleCanvasScope Scope => scope;
        public string Identity => identity;

        public void ReportAvailable()
        {
            Report(LifecycleCanvasEventKind.Available);
        }

        public void ReportReleasing()
        {
            Report(LifecycleCanvasEventKind.Releasing);
        }

        public void ReportEntered()
        {
            Report(LifecycleCanvasEventKind.Entered);
        }

        public void ReportExited()
        {
            Report(LifecycleCanvasEventKind.Exited);
        }

        private void Report(LifecycleCanvasEventKind eventKind)
        {
            if (!ValidateConfiguration(eventKind.ToString(), out LifecycleCanvasPresenter targetPresenter))
            {
                return;
            }

            targetPresenter.RecordEvent(scope, eventKind, identity, this);
        }

        private bool ValidateConfiguration(
            string operation,
            out LifecycleCanvasPresenter targetPresenter)
        {
            targetPresenter = presenter;

            if (targetPresenter == null)
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE] Lifecycle Canvas Event Reporter has no presenter reference. " +
                    $"source='{BuildTransformPath(transform)}' scope='{scope}' operation='{operation}'.",
                    this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(identity))
            {
                Debug.LogError(
                    "[FIRSTGAME_LIFECYCLE] Lifecycle Canvas Event Reporter has no explicit identity. " +
                    $"source='{BuildTransformPath(transform)}' scope='{scope}' operation='{operation}'.",
                    this);
                return false;
            }

            return true;
        }

        private static string BuildTransformPath(Transform current)
        {
            if (current == null)
            {
                return "<missing>";
            }

            string path = current.name;
            while (current.parent != null)
            {
                current = current.parent;
                path = current.name + "/" + path;
            }

            return path;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            identity = identity?.Trim() ?? string.Empty;
        }
#endif
    }
}
