using UnityEngine;
namespace _Project.Scripts
{
    [DisallowMultipleComponent]
    public sealed class FirstGameRuntimeObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool destroyPreviousBeforeSpawn = true;

        private GameObject _spawned;

        private void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        [ContextMenu("Spawn Runtime Object")]
        public void Spawn()
        {
            if (prefab == null)
            {
                Debug.LogWarning("[FirstGame] Runtime prefab is missing.", this);
                return;
            }

            if (destroyPreviousBeforeSpawn && _spawned != null)
            {
                Destroy(_spawned);
            }

            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

            _spawned = Instantiate(prefab, position, rotation);
            _spawned.name = prefab.name + " (Runtime)";
        }

        [ContextMenu("Destroy Runtime Object")]
        public void DestroySpawned()
        {
            if (_spawned == null)
            {
                return;
            }

            Destroy(_spawned);
            _spawned = null;
        }
    }
}