using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CovenViewMembers : UIBaseAnimated
{
    public CovenView.TabCoven m_TabCoven;

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;

    [Header("deprecated Buttons")]
    public GameObject m_btnChat;
    public GameObject m_btnEdit;
    public GameObject m_btnLeave;
    public GameObject m_btnInvite;
    public GameObject m_btnRequests;

    // inner
    private bool m_bEditorModeEnabled = false;

    // this controller can not be a singleton because we will use it to load other's screens
    private CovenController Controller
    {
        get { return CovenController.Instance; }
    }


    private void Start()
    {
        m_TabCoven.m_ListItemPool.Setup();
        
    }


    public override void Show()
    {
        base.Show();
        SetupUI();
    }


    public void SetupUI()
    {
        if (Controller.NeedsReload)
        {
            Utilities.SetActiveList(false, m_btnChat, m_btnEdit, m_btnLeave, m_btnInvite, m_btnRequests);
            StartCoroutine(RequestData());
        }
    }


    IEnumerator RequestData()
    {
        yield return new WaitForSeconds(1.3f);

        Debug.Log("Request coven Data");
        Controller.RequestCovensData(null, null);

        while (!Controller.IsDataLoaded)
        {
            yield return null;
        }

        FillList(Controller.GetPlayerCovenData());
        Debug.Log("ok, data filled");

        switch (Controller.CurrentRole)
        {
            case CovenController.CovenRole.Moderator:
            case CovenController.CovenRole.Administrator:
                Utilities.SetActiveList(true, m_btnChat, m_btnInvite, m_btnEdit, m_btnRequests, m_btnLeave);
                break;
            case CovenController.CovenRole.Member:
                Utilities.SetActiveList(true, m_btnChat, m_btnInvite, m_btnLeave);
                break;
        }
    }

    public void FillList(CovenData pCovenData)
    {
        m_TabCoven.m_ListItemPool.DespawnAll();
        for (int i = 0; i < pCovenData.players.Count; i++)
        {
            CovenScrollViewItem pView = m_TabCoven.m_ListItemPool.Spawn<CovenScrollViewItem>();
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
        for (int i = 0; i < m_TabCoven.m_ListItemPool.GameObjectList.Count; i++)
        {
            if (m_TabCoven.m_ListItemPool.GameObjectList[i].activeSelf)
            {
                CovenScrollViewItem pView = m_TabCoven.m_ListItemPool.GameObjectList[i].GetComponent<CovenScrollViewItem>();
                if (pView != null)
                {
                    pView.SetBackgound(iCounter % 2 == 0);
                    iCounter++;
                }
            }
        }
    }

    void SetupEditMode()
    {
        m_bEditorModeEnabled = !m_bEditorModeEnabled;
        List<CovenScrollViewItem> vList = m_TabCoven.m_ListItemPool.GetActiveGameObjectList<CovenScrollViewItem>();
        for (int i = 0; i < vList.Count; i++)
        {
            vList[i].SetEditorModeEnabled(m_bEditorModeEnabled, true, i);
        }
    }



    #region buttons callback


    public void OnClickChat()
    {

    }
    public void OnClickLeave()
    {
        UIGenericPopup.ShowYesNoPopup("Leave Coven", "Do you really wanna leave from the coven?", Close, null);
    }
    public void OnClickClose()
    {
        Close();
    }
    public void OnClickEdit()
    {
        SetupEditMode();
    }
    public void OnClickInvite()
    {
        UIGenericInputPopup.ShowPopup("Type User's name", "", InviteUser, null);
    }
    public void OnClickRequests()
    {
        CovenView.Instance.ShowTabMembeRequests();
    }
    public void OnClickAccept()
    {

    }

    public void OnClickKickUser(CovenScrollViewItem pItem)
    {
        UIGenericPopup.ShowYesNoPopup("Info", "Click Yes to remove <name> form the Coven.".Replace("<name>", pItem.m_txtName.text),
            () => {
                Debug.Log("Will kick the player Here. remember to notify the serverside");
                m_TabCoven.m_ListItemPool.Despawn(pItem.gameObject);
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
                Controller.UpdateCovensTitles(pItem.CurrentUser);
            }
        });
    }

    #endregion


    private void InviteUser(string sUserName)
    {

    }
}