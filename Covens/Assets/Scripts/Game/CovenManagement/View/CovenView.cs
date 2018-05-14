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

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;


    // internal
    private bool m_bEditorModeEnabled = false;



    private void Start()
    {
        m_CovenItemPool.Setup();

        // test itens
        TestAddItens();
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
                    pView.SetBackgound(iCounter % 2 == 0);
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
        SetupEditMode();
    }
    public void OnClickInvite()
    {

    }
    public void OnClickAccept()
    {

    }

    public void OnClickKickUser(CovenScrollViewItem pItem)
    {
        UIGenericPopup.ShowYesNoPopup("Info", "Click Yes to remove <name> form the Coven.".Replace("<name>", pItem.m_txtName.text),
            () => {
                Debug.Log("Will kick the player Here. remember to notify the serverside");
                m_CovenItemPool.Despawn(pItem.gameObject);
            },
            () => {
                Debug.Log("Canceled");
            }
        );
    }

    public void OnClickEditUserTitle(CovenScrollViewItem pItem)
    {
        m_EditPopup.Show(pItem.CurrentTitle, (RectTransform)pItem.m_EditorChangeTitle.transform);
        
    }

    #endregion


    
    void SetupEditMode()
    {
        m_bEditorModeEnabled = !m_bEditorModeEnabled;
        List<CovenScrollViewItem> vList = m_CovenItemPool.GetActiveGameObjectList<CovenScrollViewItem>();
        for (int i = 0; i < vList.Count; i++)
        {
            vList[i].SetEditorModeEnabled(m_bEditorModeEnabled, true, i);
        }
    }



    #region test cases

    [ContextMenu("Add 15 itens")]
    public void TestAddItens()
    {
        string[] vNames = new string[] { "Hugo ", "Lucas", "Diogo" };
        string[] vSurNames = new string[] { "Matsumoto ", "Penhas", "Conchal" };
        string[] vStatus = new string[] { "On Line", "Battling in Arena", "Off Line" };
        for (int i = 0; i < 15; i++)
        {
            m_CovenItemPool.Spawn<CovenScrollViewItem>().Setup(
                Random.Range(0, 99).ToString(),
                vNames[Random.Range(0, vNames.Length)] + " " + vSurNames[Random.Range(0, vSurNames.Length)],
                CovenController.CovenTitle.Owner,
                vStatus[Random.Range(0, vStatus.Length)]
                );
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
        m_CovenItemPool.DespawnAll();
        for (int i = 0; i < 7; i++)
        {
            m_CovenItemPool.Spawn<CovenScrollViewItem>().SetEditorModeEnabled(false);
            UpdateList();
        }
    }

    #endregion

}
