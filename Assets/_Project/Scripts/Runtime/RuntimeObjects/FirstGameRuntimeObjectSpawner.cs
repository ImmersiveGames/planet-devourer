using UnityEngine;
namespace Project._Project.Scripts.Runtime.RuntimeObjects
{
    /// <summary>
    /// Minimal FIRSTGAME runtime-object spawner used by the consumer sample scene.
    ///
    /// This is intentionally consumer-side code. The spawned prefab owns its own
    /// framework reset subject/participants; the spawner only demonstrates the
    /// game-side action that creates a runtime object.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstGameRuntimeObjectSpawner : MonoBehaviour
    {
        private const string LogPrefix = "[FIRSTGAME_RUNTIME_OBJECT]";
        private const string DefaultReason = "firstgame.runtime.box";

        [Header("Runtime Object")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform spawnPoint;

        [Header("Spawn Policy")]
        [SerializeField] private bool spawnOnStart;
        [SerializeField] private bool destroyPreviousBeforeSpawn = true;
        [SerializeField] private string spawnReason = DefaultReason;

        private GameObject _spawned;
        private int _spawnCount;

        private void Start()
        {
            if (spawnOnStart)
            {
                Spawn();
            }
        }

        [ContextMenu("FIRSTGAME/Spawn Runtime Object")]
        public void Spawn()
        {
            if (prefab == null)
            {
                Debug.LogWarning(
                    $"{LogPrefix} spawn skipped. reason='{spawnReason}' issue='MissingPrefab'",
                    this);
                return;
            }

            if (destroyPreviousBeforeSpawn && _spawned != null)
            {
                Destroy(_spawned);
            }

            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

            _spawnCount++;
            _spawned = Instantiate(prefab, position, rotation);
            _spawned.name = $"{prefab.name} (Runtime {_spawnCount})";

            Debug.Log(
                $"{LogPrefix} spawned. reason='{spawnReason}' prefab='{prefab.name}' instance='{_spawned.name}' count='{_spawnCount}'",
                this);
        }

        [ContextMenu("FIRSTGAME/Destroy Runtime Object")]
        public void DestroySpawned()
        {
            if (_spawned == null)
            {
                Debug.Log(
                    $"{LogPrefix} destroy skipped. reason='{spawnReason}' issue='NoRuntimeObject'",
                    this);
                return;
            }

            string instanceName = _spawned.name;
            Destroy(_spawned);
            _spawned = null;

            Debug.Log(
                $"{LogPrefix} destroyed. reason='{spawnReason}' instance='{instanceName}'",
                this);
        }
    }
}
