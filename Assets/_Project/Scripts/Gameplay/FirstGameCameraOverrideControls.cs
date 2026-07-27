using System;
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

        private void Awake()
        {
            ValidateReferences();
        }

        public void RequestActivityCamera() =>
            RequireActivityCamera().RequestOverride();

        public void ReleaseActivityCamera() =>
            RequireActivityCamera().ReleaseOverride();

        public void RequestRouteCamera() =>
            RequireRouteCamera().RequestOverride();

        public void ReleaseRouteCamera() =>
            RequireRouteCamera().ReleaseOverride();

        public void ReturnToPlayerCamera()
        {
            RequireRouteCamera().ReleaseOverride();
            RequireActivityCamera().ReleaseOverride();
        }

        private void ValidateReferences()
        {
            RequireActivityCamera();
            RequireRouteCamera();
        }

        private ActivityCameraOverrideBinding RequireActivityCamera()
        {
            if (activityCamera == null)
            {
                throw new InvalidOperationException(
                    "FirstGameCameraOverrideControls requires an explicit Activity Camera binding.");
            }

            return activityCamera;
        }

        private RouteCameraOverrideBinding RequireRouteCamera()
        {
            if (routeCamera == null)
            {
                throw new InvalidOperationException(
                    "FirstGameCameraOverrideControls requires an explicit Route Camera binding.");
            }

            return routeCamera;
        }
    }
}
