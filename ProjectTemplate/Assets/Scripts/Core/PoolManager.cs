using System.Collections.Generic;
using UnityEngine;

namespace RoguelikeSurvivor
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        private readonly Dictionary<string, Queue<GameObject>> _pools = new();
        private readonly Dictionary<string, Transform> _poolParents = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PreWarm(GameObject prefab, int count)
        {
            var key = prefab.name;
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<GameObject>();
                var parent = new GameObject($"Pool_{key}");
                parent.transform.SetParent(transform);
                _poolParents[key] = parent.transform;
            }

            for (var i = 0; i < count; i++)
            {
                var obj = Instantiate(prefab, _poolParents[key]);
                obj.name = key;
                obj.SetActive(false);
                _pools[key].Enqueue(obj);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var key = prefab.name;

            if (!_pools.ContainsKey(key) || _pools[key].Count == 0)
            {
                PreWarm(prefab, 1);
            }

            var obj = _pools[key].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawn();
            }

            return obj;
        }

        public void Despawn(GameObject obj)
        {
            var key = obj.name;

            if (obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnDespawn();
            }

            obj.SetActive(false);

            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<GameObject>();
            }

            _pools[key].Enqueue(obj);
        }
    }
}
