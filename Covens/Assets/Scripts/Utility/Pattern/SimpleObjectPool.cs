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
    public Transform m_Parent;
    public List<GameObject> m_ObjectPool = new System.Collections.Generic.List<GameObject>();


    public List<GameObject> GameObjectList
    {
        get { return m_ObjectPool; }
    }
    public Transform Parent
    {
        get { return m_Parent != null ? m_Parent : m_Template.transform.parent; }
    }

    public void Setup()
    {
        m_Template.SetActive(false);
        InstantiateAmount(m_StartAmount);
    }


    #region spawn

    /// <summary>
    /// spawns an object. If has no instance to recicle, create a new one
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// spanws an object ang returns its component
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Spawn<T>()
    {
        GameObject pInstance = Spawn();
        T pScript = pInstance.GetComponent<T>();
        if (pScript != null)
            return pScript;

        Debug.LogError("couldn't get the specified component. retuning null");
        return default(T);
    }

    #endregion


    #region despawn

    /// <summary>
    /// despawns all objects
    /// </summary>
    public void DespawnAll()
    {
        for (int i = 0; i < m_ObjectPool.Count; i++)
        {
            Despawn(m_ObjectPool[i]);
        }
    }

    /// <summary>
    /// despanws a single object
    /// </summary>
    /// <param name="pObject"></param>
    public void Despawn(GameObject pObject)
    {
        pObject.SetActive(false);
    }

    #endregion


    #region utilities

    /// <summary>
    /// returns all active game in this pool
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetActiveGameObjectList()
    {
        List<GameObject> vActiveList = new List<GameObject>();
        for (int i = 0; i < GameObjectList.Count; i++)
        {
            if (GameObjectList[i].activeSelf)
            {
                vActiveList.Add(GameObjectList[i]);
            }
        }
        return vActiveList;
    }
    public List<T> GetActiveGameObjectList<T>()
    {
        List<T> vActiveList = new List<T>();
        for (int i = 0; i < GameObjectList.Count; i++)
        {
            if (GameObjectList[i].activeSelf)
            {
                vActiveList.Add(GameObjectList[i].GetComponent<T>());
            }
        }
        return vActiveList;
    }

    #endregion


    #region private methods

    private void InstantiateAmount(int iAmount = 1)
    {
        if (iAmount <= 0)
            return;
        for (int i = 0; i < iAmount; i++)
        {
            GameObject pInstance = Instantiate(false);
            m_ObjectPool.Add(pInstance);
        }
    }

    private GameObject Instantiate(bool bVisibility)
    {
        GameObject pNewInstance = GameObject.Instantiate(m_Template, Parent, false);
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

    #endregion



}
