using UnityEngine;

namespace Project._Project.Scripts.Runtime.GameCamera
{
    /// <summary>
    /// Scene-authored optional Cinemachine targets for a FIRSTGAME camera rig.
    /// Route and Activity bindings may share the same anchor host.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameCameraAnchorHost : MonoBehaviour
    {
        [SerializeField] private Transform trackingTarget;
        [SerializeField] private Transform lookAtTarget;

        public Transform TrackingTarget => trackingTarget;

        public Transform LookAtTarget => lookAtTarget;

        public bool HasAnyTarget => trackingTarget != null || lookAtTarget != null;
    }
}
