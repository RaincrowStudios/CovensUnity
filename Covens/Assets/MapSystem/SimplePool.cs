using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool<T> where T : Component
{
    private string m_PrefabPath;
    private T m_Prefab;
    private List<T> m_AvailablePool;
    private HashSet<T> m_UnavailablePool;
    private Transform m_Container;

    public SimplePool(T prefab, int startAmount)
    {
        m_Container = new GameObject($"[SimplePool]{prefab.name}").transform;
        m_Container.gameObject.SetActive(false);
        GameObject.DontDestroyOnLoad(m_Container.gameObject);

        m_Prefab = prefab;
        m_Prefab.gameObject.SetActive(false);

        if (prefab.gameObject.scene.rootCount != 0)
        {
            m_Prefab.transform.SetParent(m_Container);
            m_AvailablePool = new List<T>() { m_Prefab };

            for (int i = 0; i < startAmount - 1; i++)
                Instantiate();
        }
        else
        {
            m_AvailablePool = new List<T>();

            for (int i = 0; i < startAmount; i++)
                Instantiate();
        }

        m_UnavailablePool = new HashSet<T>();
    }

    public SimplePool(string prefabPath)
    {
        m_Container = new GameObject($"[SimplePool]{prefabPath}").transform;
        m_PrefabPath = prefabPath;
        m_AvailablePool = new List<T>();
        m_UnavailablePool = new HashSet<T>();
    }

    public T Spawn()
    {
        return Spawn(null);
    }

    public T Spawn(Transform parent)
    {
        T instance = null;
        if (m_AvailablePool.Count <= 0)
            Instantiate();

        instance = m_AvailablePool[0];

        m_AvailablePool.RemoveAt(0);
        m_UnavailablePool.Add(instance);
        instance.transform.SetParent(parent);
        instance.gameObject.SetActive(true);
        return instance;
    }

    private void Instantiate()
    {
        if (m_Prefab == null)
            m_Prefab = Resources.Load<T>(m_PrefabPath);

        m_AvailablePool.Add(Object.Instantiate(m_Prefab));
        m_AvailablePool[m_AvailablePool.Count - 1].transform.SetParent(m_Container);
    }

    public void Despawn(T instance)
    {
        if (!m_UnavailablePool.Contains(instance))
            return;

        instance.gameObject.SetActive(false);
        instance.transform.SetParent(m_Container);
        m_UnavailablePool.Remove(instance);
        m_AvailablePool.Add(instance);
    }

    public void DespawnAll()
    {
        foreach (T _instance in m_UnavailablePool)
        {
            _instance.gameObject.SetActive(false);
            _instance.transform.SetParent(m_Container);
            m_AvailablePool.Add(_instance);
        }
        m_UnavailablePool.Clear();
    }
}
