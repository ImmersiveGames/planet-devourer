using Immersive.Framework.Camera;
using UnityEngine;

namespace FirstGame.Gameplay
{
    /// <summary>
    /// Explicit FIRSTGAME UI bridge for the authored scoped camera overrides.
    /// </summary>
    public sealed class FirstGameCameraOverrideControls : MonoBehaviour
    {
        [SerializeField] private ActivityCameraOverrideBinding activityCamera;
        [SerializeField] private RouteCameraOverrideBinding routeCamera;
        [SerializeField] private SessionCameraOverrideBinding sessionCamera;

        public void RequestActivityCamera() => activityCamera.RequestOverride();
        public void ReleaseActivityCamera() => activityCamera.ReleaseOverride();
        public void RequestRouteCamera() => routeCamera.RequestOverride();
        public void ReleaseRouteCamera() => routeCamera.ReleaseOverride();
        public void RequestSessionCamera() => sessionCamera.RequestOverride();
        public void ReleaseSessionCamera() => sessionCamera.ReleaseOverride();

        public void ReturnToPlayerCamera()
        {
            routeCamera.ReleaseOverride();
            activityCamera.ReleaseOverride();
        }
    }
}
