using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public enum StartupPoolMode { Awake, Start, CallManually };

    [System.Serializable]
    public struct StartupPool
    {
        public int size;
        public GameObject prefab;
    }

    private static List<GameObject> TempList = new List<GameObject>();

    public StartupPool[] _startupPools = new StartupPool[0];
    public StartupPoolMode startupPoolMode = StartupPoolMode.Awake;

    private Dictionary<GameObject, List<GameObject>> _pooledObjects = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, GameObject> _spawnedObjects = new Dictionary<GameObject, GameObject>();
    private bool _startupPoolsCreated;

    protected void Awake()
    {
        if (startupPoolMode == StartupPoolMode.Awake)
        {
            CreateStartupPools();
        }
    }

    protected void Start()
    {
        if (startupPoolMode == StartupPoolMode.Start)
        {
            CreateStartupPools();
        }
    }

    public void CreateStartupPools()
    {
        if (!_startupPoolsCreated)
        {
            var pools = _startupPools;
            if (pools != null && pools.Length > 0)
            {
                for (int i = 0; i < pools.Length; ++i)
                {
                    CreatePool(pools[i].prefab, pools[i].size);
                }
            }
            _startupPoolsCreated = true;
        }
    }

    public void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
    {
        CreatePool(prefab.gameObject, initialPoolSize);
    }

    public void CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null && !_pooledObjects.ContainsKey(prefab))
        {
            var list = new List<GameObject>();
            _pooledObjects.Add(prefab, list);

            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = transform;
                while (list.Count < initialPoolSize)
                {
                    var obj = Instantiate(prefab);
                    obj.transform.SetParent(parent);
                    list.Add(obj);
                }
                prefab.SetActive(active);
            }
        }
    }

    public T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation, bool worldPositionStays = true) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, rotation, worldPositionStays).GetComponent<T>();
    }
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, bool worldPositionStays = true) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, rotation, worldPositionStays).GetComponent<T>();
    }
    public T Spawn<T>(T prefab, Transform parent, Vector3 position, bool worldPositionStays = true) where T : Component
    {
        return Spawn(prefab.gameObject, parent, position, Quaternion.identity, worldPositionStays).GetComponent<T>();
    }
    public T Spawn<T>(T prefab, Vector3 position) where T : Component
    {
        return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
    }
    public T Spawn<T>(T prefab, Transform parent, bool worldPositionStays = true) where T : Component
    {
        return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity, worldPositionStays).GetComponent<T>();
    }
    public T Spawn<T>(T prefab) where T : Component
    {
        return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
    }
    public GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, bool worldPositionStays = true)
    {
        Transform trans;
        GameObject obj;
        if (_pooledObjects.TryGetValue(prefab, out List<GameObject> list))
        {
            obj = null;
            if (list.Count > 0)
            {
                while (obj == null && list.Count > 0)
                {
                    obj = list[0];
                    list.RemoveAt(0);
                }
                if (obj != null)
                {
                    trans = obj.transform;
                    trans.SetParent(parent, worldPositionStays);
                    trans.localPosition = position;
                    trans.localRotation = rotation;
                    obj.SetActive(true);
                    _spawnedObjects.Add(obj, prefab);
                    return obj;
                }
            }
            obj = Instantiate(prefab);
            trans = obj.transform;
            trans.SetParent(parent, worldPositionStays);
            trans.localPosition = position;
            trans.localRotation = rotation;
            _spawnedObjects.Add(obj, prefab);
            return obj;
        }
        else
        {
            obj = Instantiate(prefab);
            trans = obj.GetComponent<Transform>();
            trans.SetParent(parent, worldPositionStays);
            trans.localPosition = position;
            trans.localRotation = rotation;
            return obj;
        }
    }

    public void Recycle<T>(T obj, bool worldPositionStays = true) where T : Component
    {
        Recycle(obj.gameObject, worldPositionStays);
    }
    public void Recycle(GameObject obj, bool worldPositionStays = true)
    {
        if (_spawnedObjects.TryGetValue(obj, out GameObject prefab))
            Recycle(obj, prefab, worldPositionStays);
        else
            Destroy(obj);
    }
    private void Recycle(GameObject obj, GameObject prefab, bool worldPositionStays = true)
    {
        _pooledObjects[prefab].Add(obj);
        _spawnedObjects.Remove(obj);
        obj.transform.SetParent(transform, worldPositionStays);
        obj.SetActive(false);
    }

    public void RecycleAll<T>(T prefab, bool worldPositionStays = true) where T : Component
    {
        RecycleAll(prefab.gameObject, worldPositionStays);
    }
    public void RecycleAll(GameObject prefab, bool worldPositionStays = true)
    {
        foreach (var item in _spawnedObjects)
            if (item.Value == prefab)
                TempList.Add(item.Key);
        for (int i = 0; i < TempList.Count; ++i)
            Recycle(TempList[i], worldPositionStays);
        TempList.Clear();
    }
    public void RecycleAll(bool worldPositionStays = true)
    {
        TempList.AddRange(_spawnedObjects.Keys);
        for (int i = 0; i < TempList.Count; ++i)
            Recycle(TempList[i], worldPositionStays);
        TempList.Clear();
    }

    public bool IsSpawned(GameObject obj)
    {
        return _spawnedObjects.ContainsKey(obj);
    }

    public int CountPooled<T>(T prefab) where T : Component
    {
        return CountPooled(prefab.gameObject);
    }
    public int CountPooled(GameObject prefab)
    {
        if (_pooledObjects.TryGetValue(prefab, out List<GameObject> list))
            return list.Count;
        return 0;
    }

    public int CountSpawned<T>(T prefab) where T : Component
    {
        return CountSpawned(prefab.gameObject);
    }
    public int CountSpawned(GameObject prefab)
    {
        int count = 0;
        foreach (GameObject instancePrefab in _spawnedObjects.Values)
            if (prefab == instancePrefab)
                ++count;
        return count;
    }

    public int CountAllPooled()
    {
        int count = 0;
        foreach (var list in _pooledObjects.Values)
            count += list.Count;
        return count;
    }

    public List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        if (_pooledObjects.TryGetValue(prefab, out List<GameObject> pooled))
            list.AddRange(pooled);
        return list;
    }
    public List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        if (_pooledObjects.TryGetValue(prefab.gameObject, out List<GameObject> pooled))
            for (int i = 0; i < pooled.Count; ++i)
                list.Add(pooled[i].GetComponent<T>());
        return list;
    }

    public List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        foreach (var item in _spawnedObjects)
            if (item.Value == prefab)
                list.Add(item.Key);
        return list;
    }
    public List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        var prefabObj = prefab.gameObject;
        foreach (var item in _spawnedObjects)
            if (item.Value == prefabObj)
                list.Add(item.Key.GetComponent<T>());
        return list;
    }

    public void DestroyPooled(GameObject prefab)
    {
        if (_pooledObjects.TryGetValue(prefab, out List<GameObject> pooled))
        {
            for (int i = 0; i < pooled.Count; ++i)
            {
                Destroy(pooled[i]);
            }
            pooled.Clear();
        }
    }
    public void DestroyPooled<T>(T prefab) where T : Component
    {
        DestroyPooled(prefab.gameObject);
    }

    public void DestroyAll(GameObject prefab)
    {
        RecycleAll(prefab);
        DestroyPooled(prefab);
    }
    public void DestroyAll<T>(T prefab, bool immediate = false) where T : Component
    {
        DestroyAll(prefab.gameObject);
    }
}
