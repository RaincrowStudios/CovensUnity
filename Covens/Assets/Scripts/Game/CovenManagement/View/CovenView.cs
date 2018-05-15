using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The coven's UI view 
/// </summary>
public class CovenView : UIBaseAnimated
{
    [Header("Top")]
    public Text m_txtTitle;
    public Text m_txtLocation;

    [Header("Body")]
    public SimpleObjectPool m_CovenItemPool;

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;

    [Header("Buttons")]
    public GameObject m_btnChat;
    public GameObject m_btnEdit;
    public GameObject m_btnLeave;
    public GameObject m_btnInvite;
    public GameObject m_btnCreateCoven;


    // internal
    private bool m_bEditorModeEnabled = false;



    private void Start()
    {
        m_CovenItemPool.Setup();

        // test itens
        //TestAddItens();

        SetupUI();
    }


    public void SetupUI()
    {
        ActiveActions(false, m_btnChat, m_btnEdit, m_btnLeave, m_btnInvite, m_btnCreateCoven);
        if (!CovenController.Instance.IsInCoven)
        {
            ActiveActions(true, m_btnCreateCoven);
        }
        else
        {
            StartCoroutine(RequestData());
        }
    }

    public void ActiveActions(bool bActive, params GameObject[] vGOs)
    {
        foreach(GameObject pGO in vGOs)
        {
            pGO.SetActive(bActive);
        }
    }



    IEnumerator RequestData()
    {
        CovenData pCovenData = null;
        bool bDone = false;
        yield return new WaitForSeconds(1.3f);

        CovenController.Instance.RequestCovensData(
            (CovenData pData) => { pCovenData = pData; bDone = true; },
            (string sError) => { bDone = true; }
            );

        while (!bDone)
        {
            yield return null;
        }
        if (pCovenData == null)
        {
            Debug.Log("FUUUU");
            yield break;
        }
        FillList(pCovenData);

        switch (CovenController.Instance.CurrentRole)
        {
            case CovenController.CovenRole.Administrator:
                ActiveActions(true, m_btnChat, m_btnInvite, m_btnEdit, m_btnLeave);
                break;
            case CovenController.CovenRole.Member:
                ActiveActions(true, m_btnChat, m_btnInvite, m_btnLeave);
                break;
            case CovenController.CovenRole.Moderator:
                ActiveActions(true, m_btnChat, m_btnInvite, m_btnLeave);
                break;
        }
    }

    public void FillList(CovenData pCovenData)
    {
        m_CovenItemPool.DespawnAll();
        for (int i = 0; i < pCovenData.players.Count; i++)
        {
            CovenScrollViewItem pView = m_CovenItemPool.Spawn<CovenScrollViewItem>();
            var eRole = CovenController.ParseRole(pCovenData.players[i].rank);
            pView.Setup(pCovenData.players[i]);
            pView.SetBackgound(i % 2 == 0);
        }
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
        m_EditPopup.Show(pItem.m_txtTitle.text, (bool bChanged, string sTitle) =>
        {
            // TODO: notify server
            if (bChanged)
            {
                pItem.CurrentUser.title = sTitle;
                pItem.m_txtTitle.text = sTitle;
                CovenController.Instance.UpdateCovensTitles(pItem.CurrentUser);
            }
        });
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
                Random.Range(0, 99),
                vNames[Random.Range(0, vNames.Length)] + " " + vSurNames[Random.Range(0, vSurNames.Length)],
                CovenController.CovenRole.Administrator.ToString(),
                vStatus[Random.Range(0, vStatus.Length)],
                CovenController.CovenRole.Administrator
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
