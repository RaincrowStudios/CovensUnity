using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The coven's UI view 
/// </summary>
public class CovenView : Patterns.SingletonComponent<CovenView>
{

    [Header("Tabs")]
    public UIBaseAnimated m_CovenInviteTab;
    public UIBaseAnimated m_MemberInviteTab;
    public UIBaseAnimated m_MembersTab;
    public UIBaseAnimated m_CurrentTab;
    


    [Header("Top")]
    public Text m_txtTitle;
    public Text m_txtLocation;

    [Header("Body")]
    public SimpleObjectPool m_CovenItemPool;

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;

    [Header("deprecated Buttons")]
    public GameObject m_btnChat;
    public GameObject m_btnEdit;
    public GameObject m_btnLeave;
    public GameObject m_btnInvite;
    public GameObject m_btnCreateCoven;




    // internal
    private bool m_bEditorModeEnabled = false;
    // we should link then with a controller to make sure it has the correct context
    private List<UIBaseAnimated> m_TabHistory = new List<UIBaseAnimated>();


    [System.Serializable]
    public struct TabCoven
    {
        public Text m_Title;
        public Text m_SubTitle;
        public SimpleObjectPool m_ListItemPool;
    }


    // this controller can not be a singleton because we will use it to load other's screens
    private CovenController Controller
    {
        get { return CovenController.Instance; }
    }


    private void Start()
    {
        m_CovenItemPool.Setup();

        m_CovenInviteTab.Hide();
        m_MembersTab.Hide();
        m_MemberInviteTab.Hide();

        // test itens
        //TestAddItens();

        //SetupUI();
    }


    public void ShowTabMembers()
    {
        ShowTab(m_MembersTab);
    }
    public void ShowTabMembeRequests()
    {
        ShowTab(m_MemberInviteTab);
    }
    public void ShowTabCovensRequests()
    {
        ShowTab(m_CovenInviteTab);
    }

    public void ShowMain()
    {
        if (!Controller.IsInCoven)
        {
            ShowTab(m_CovenInviteTab);
        }
        else
        {
            ShowTab(m_MembersTab);
        }
    }



    void ShowTab(UIBaseAnimated pUI)
    {
        if (m_CurrentTab != null && m_CurrentTab != pUI)
        {
            m_CurrentTab.Close();
        }

        pUI.Show();
        m_CurrentTab = pUI;
        m_TabHistory.Add(pUI);
    }

    public void BackTab()
    {
        if(m_TabHistory.Count >= 2)
        {
            // shows the latest ui shown
            m_TabHistory.RemoveAt(m_TabHistory.Count - 1);
            ShowTab(m_TabHistory[m_TabHistory.Count - 1]);
            m_TabHistory.RemoveAt(m_TabHistory.Count - 1);
        }
    }






    public void SetupUI()
    {
        ActiveActions(false, m_btnChat, m_btnEdit, m_btnLeave, m_btnInvite, m_btnCreateCoven);
        if (!Controller.IsInCoven)
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

        Controller.RequestCovensData(
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

        switch (Controller.CurrentRole)
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
    public void OnClickOpenCovenInvite()
    {
        ShowTab(m_CovenInviteTab);
    }
    public void OnClickOpenMemberInvite()
    {
        ShowTab(m_MemberInviteTab);
    }
    public void OnClickOpenMembers()
    {
        ShowTab(m_MembersTab);
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
        UIGenericInputPopup.ShowPopup(pItem.m_txtTitle.text, "",
            (string sText) =>
            {
                pItem.CurrentUser.title = sText;
                pItem.m_txtTitle.text = sText;
                Controller.UpdateCovensTitles(pItem.CurrentUser);
            }, null
        );
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


    /*
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
    */
}
