using UnityEngine;

namespace FirstGame.FrameworkModels.M02
{
    /// <summary>
    /// Local visual-only receiver for the M02 lifecycle proof.
    /// Framework callbacks are wired explicitly through UnityEvents in the prefab.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class M02LifecycleVisualPresenter : MonoBehaviour
    {
        [SerializeField] private MeshRenderer visualPlaceholder;
        [SerializeField] private TextMesh label;
        [SerializeField] private string initialLabel = "Initial";

        [Header("Advanced / Debug")]
        [SerializeField] private string lastEvent = "Initial";
        [SerializeField] private int receivedEventCount;

        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            Present(initialLabel, Color.gray);
        }

        public void OnAvailable() => Present("Scene Available", new Color(0.2f, 0.8f, 0.35f));

        public void OnReleasing() => Present("Scene Releasing", new Color(0.95f, 0.5f, 0.15f));

        public void OnEntered() => Present("Entered", new Color(0.2f, 0.65f, 1f));

        public void OnExited() => Present("Exited", new Color(0.95f, 0.25f, 0.3f));

        private void Present(string eventName, Color color)
        {
            lastEvent = eventName;
            receivedEventCount++;

            Debug.Log(
                $"<color=#66CCFF>[M02_LIFECYCLE]</color> " +
                $"event='{eventName}' object='{gameObject.name}' " +
                $"scene='{gameObject.scene.name}' count='{receivedEventCount}'.",
                this);

            if (label != null)
            {
                label.text = eventName;
            }

            if (visualPlaceholder == null)
            {
                return;
            }

            propertyBlock ??= new MaterialPropertyBlock();
            visualPlaceholder.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", color);
            visualPlaceholder.SetPropertyBlock(propertyBlock);
        }
    }
}
