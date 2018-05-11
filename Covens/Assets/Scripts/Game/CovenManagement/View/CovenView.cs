using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The coven's UI view 
/// </summary>
public class CovenView : MonoBehaviour
{
    [Header("Top")]
    public Text m_txtTitle;
    public Text m_txtLocation;

    [Header("Body")]
    public SimpleObjectPool m_CovenItemPool;

    //[Header("Botton")]


    private void Start()
    {
        m_CovenItemPool.Setup();
    }

    /// <summary>
    /// updates the list's background. Everytime the list has changed, it needs to be updated
    /// </summary>
    public void UpdateList()
    {
        int iCounter = 0;
        for (int i = 0; i < m_CovenItemPool.GameObjectList.Count; i++)
        {
            if (m_CovenItemPool.GameObjectList[i].activeSelf)
            {
                CovenScrollViewItem pView = m_CovenItemPool.GameObjectList[i].GetComponent<CovenScrollViewItem>();
                if(pView != null)
                {
                    pView.m_imgBackground.SetActive(iCounter % 2 == 0);
                    iCounter++;
                }
            }
        }
    }


    #region buttons callback

    public void OnClickChat()
    {

    }
    public void OnClickLeave()
    {

    }
    public void OnClickClose()
    {

    }
    public void OnClickEdit()
    {

    }
    public void OnClickInvite()
    {

    }
    public void OnClickAccept()
    {

    }

    #endregion



    #region test cases

    [ContextMenu("Add 15 itens")]
    public void TestAddItens()
    {
        for(int i = 0; i < 15; i++)
        {
            m_CovenItemPool.Spawn();
        }
        UpdateList();
    }
    [ContextMenu("Reset")]
    public void ResetItems()
    {
        m_CovenItemPool.DespawnAll();
    }
    [ContextMenu("Reset to 7")]
    public void Respawn()
    {
        for (int i = 0; i < 7; i++)
        {
            m_CovenItemPool.Spawn();
            UpdateList();
        }
    }

    #endregion

}