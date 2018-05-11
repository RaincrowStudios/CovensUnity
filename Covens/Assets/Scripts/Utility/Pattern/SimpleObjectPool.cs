using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This object pool is designed to be as simple as possible and works by activating and deactivating the gameobject
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class SimpleObjectPool
{

    public GameObject m_Template;
    public int m_StartAmount = 0;
    public List<GameObject> m_ObjectPool = new System.Collections.Generic.List<GameObject>();

    public List<GameObject> GameObjectList
    {
        get { return m_ObjectPool; }
    }

    public void Setup()
    {
        m_Template.SetActive(false);
        InstantiateAmount(m_StartAmount);
    }

    public GameObject Spawn()
    {
        GameObject pInstance = GetAvailable();
        if (pInstance == null)
        {
            GameObject pNewInstance = Instantiate(true);
            m_ObjectPool.Add(pNewInstance);
            return pNewInstance;
        }
        pInstance.SetActive(true);
        return pInstance;
    }
    public T Spawn<T>()
    {
        GameObject pInstance = Spawn();
        T pScript = pInstance.GetComponent<T>();
        if (pScript != null)
            return pScript;

        Debug.LogError("couldn't get the specified component. retuning null");
        return default(T);
    }

    public void DespawnAll()
    {
        for (int i = 0; i < m_ObjectPool.Count; i++)
        {
            m_ObjectPool[i].SetActive(false);
        }
    }

    private void InstantiateAmount(int iAmount = 1)
    {
        if (iAmount <= 0)
            return;
        for (int i = 0; i < iAmount; i++)
        {
            GameObject pInstance = Instantiate(false);
        }
    }

    private GameObject Instantiate(bool bVisibility)
    {
        GameObject pNewInstance = GameObject.Instantiate(m_Template, m_Template.transform.parent, true);
        pNewInstance.SetActive(bVisibility);
        return pNewInstance;
    }

    private GameObject GetAvailable()
    {
        for(int i = 0; i < m_ObjectPool.Count; i++)
        {
            // need to be optimized
            if (!m_ObjectPool[i].activeSelf)
            {
                return m_ObjectPool[i];
            }
        }
        return null;
    }

}
