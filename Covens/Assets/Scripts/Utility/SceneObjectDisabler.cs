using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectDisabler : MonoBehaviour
{
    List<GameObject> m_ActiveLists = new List<GameObject>();
    int m_iIndex = -1;

    [ContextMenu("Collect")]
    public void Test()
    {
        
    }

    // Use this for initialization
    void Start()
    {
        GameObject[] gos = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < gos.Length; i++)
        {
            if (!gos[i].activeSelf || gos[i] == transform.parent.gameObject)
                continue;
            m_ActiveLists.Add(gos[i]);
        }
    }
    
    void Toggle()
    {
        int iLast = m_iIndex;
        int iCurrent = m_iIndex + 1 >= m_ActiveLists.Count ? 0 : m_iIndex + 1;
        if(iLast >= 0)
            m_ActiveLists[iLast].SetActive(true);
        m_ActiveLists[iCurrent].SetActive(false);
        m_iIndex = iCurrent;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 100, 150, 50), "Toggle " + m_iIndex))
        {
            Toggle();
        }

        if (GUI.Button(new Rect(0, 150, 150, 50), "Disable All"))
        {
            foreach(GameObject go in m_ActiveLists)
            {
                go.SetActive(false);
            }
        }
    }



}