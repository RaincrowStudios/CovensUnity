using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : class
{
    private string m_PrefabPath;
    private T m_Prefab;
    private List<T> m_AvailablePool;
    private HashSet<T> m_UnavailablePool;

    private System.Action<T> m_OnSpawn;
    private System.Action<T> m_OnDespawn;

    public SimplePool(T prefab, int startAmount, System.Action<T> onSpawn = null, System.Action<T> onDespawn = null)
    {
        m_Prefab = prefab;
        m_AvailablePool = new List<T>();
        m_OnSpawn = onSpawn;
        m_OnDespawn = onDespawn;
        m_UnavailablePool = new HashSet<T>();
        for (int i = 0; i < startAmount; i++)
        {
            Instantiate();
        }
    }

    public SimplePool(string prefabPath, System.Action<T> onSpawn = null, System.Action<T> onDespawn = null)
    {
        m_PrefabPath = prefabPath;
        m_AvailablePool = new List<T>();
        m_UnavailablePool = new HashSet<T>();
        m_OnSpawn = onSpawn;
        m_OnDespawn = onDespawn;
    }

    public T Spawn()
    {
        T instance = null;
        if (m_AvailablePool.Count <= 0)
            Instantiate();

        instance = m_AvailablePool[0];

        m_AvailablePool.RemoveAt(0);
        m_UnavailablePool.Add(instance);
        m_OnSpawn?.Invoke(instance);
        return instance;
    }

    private void Instantiate()
    {
        if (m_Prefab == null)
            m_Prefab = Resources.Load(m_PrefabPath) as T;

        m_AvailablePool.Add(Object.Instantiate(m_Prefab as Object) as T);
    }

    public void Despawn(T instance)
    {
        if (!m_UnavailablePool.Contains(instance))
            return;

        m_UnavailablePool.Remove(instance);
        m_AvailablePool.Add(instance);
        m_OnDespawn?.Invoke(instance);
    }
}
