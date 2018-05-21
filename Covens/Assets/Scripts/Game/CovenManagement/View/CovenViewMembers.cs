using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CovenViewMembers : CovenViewBase
{

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;

    [Header("deprecated Buttons")]
    public GameObject m_btnChat;
    public GameObject m_btnEdit;
    public GameObject m_btnLeave;
    public GameObject m_btnInvite;
    public GameObject m_btnRequests;
    public GameObject m_btnAlliances;
    public GameObject m_btnBack;
    public GameObject m_btnAcceptAlliance;
    public GameObject m_btnRejectAlliance;
    

    // inner
    private bool m_bEditorModeEnabled = false;


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
        // disable all buttons
        Utilities.SetActiveList(false, m_btnChat, m_btnEdit, m_btnLeave, m_btnInvite, m_btnRequests, m_btnBack, m_btnAcceptAlliance, m_btnRejectAlliance, m_btnAlliances);
        m_TabCoven.m_Title.text = Controller.CovenName;
        m_TabCoven.m_SubTitle.text = "not defined sub title";
        m_TabCoven.m_ListItemPool.DespawnAll();

        if (Controller.NeedsReload)
        {
            Debug.Log("Members. Reloading data");
            RequestCovensData();
        }
        else
        {
            Debug.Log("Members. settingup data");
            SetupDataList();
        }
    }


    void RequestCovensData()
    {
        // show loading 
        UIGenericLoadingPopup.ShowLoading();

        Action<CovenData> Success = (CovenData pCovenData) =>
        {
            SetupDataList();
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowConfirmPopup("Error", "RequestCovensData Error: " + sError + ".\nCoven will be closed", CovenView.Instance.Close);
            UIGenericLoadingPopup.CloseLoading();
        };

        Controller.RequestCovensData(Success, Error);
    }
    /*
    IEnumerator RequestData()
    {
        // show loading 
        UIGenericLoadingPopup.ShowLoading();
        yield return null;

        Debug.Log("Request coven Data");
        Controller.RequestCovensData(null, null);


        while (!Controller.IsDataLoaded)
        {
            yield return null;
        }

        // show loading 
        yield return null;
        UIGenericLoadingPopup.CloseLoading();
        yield return null;

        SetupDataList();
    }*/

    private void SetupDataList()
    {
        FillList(Controller.GetPlayerCovenData());
        Debug.Log("ok, data filled");

        // non player means we are just displaying someone's coven
        if (!Controller.IsPlayerCoven)
        {
            Utilities.SetActiveList(true, m_btnBack);
            if (Controller.IsCovenAnAlly)
                Utilities.SetActiveList(true, m_btnAcceptAlliance);
        }
        else
        {

            switch (Controller.CurrentRole)
            {
                case CovenController.CovenRole.Moderator:
                case CovenController.CovenRole.Administrator:
                    Utilities.SetActiveList(true, m_btnChat, m_btnInvite, m_btnEdit, m_btnRequests, m_btnAlliances);
                    break;
                case CovenController.CovenRole.Member:
                    Utilities.SetActiveList(true, m_btnChat, m_btnInvite, m_btnLeave, m_btnAlliances);
                    break;
            }
        }
    }

    public void FillList(CovenData pCovenData)
    {
        
        for (int i = 0; i < pCovenData.players.Count; i++)
        {
            CovenScrollViewItemMember pView = m_TabCoven.m_ListItemPool.Spawn<CovenScrollViewItemMember>();
            var eRole = CovenController.ParseRole(pCovenData.players[i].rank);
            pView.SetupMemberItem(pCovenData.players[i]);
            pView.SetBackgound(i % 2 == 0);
            // scale it
            pView.transform.localScale = Vector3.zero;
            LeanTween.scale(pView.gameObject, Vector3.one, .2f).setDelay(0.05f * i).setEase(LeanTweenType.easeOutBack);
        }
        // set the scrollbar to top
        Vector3 vPosition = m_TabCoven.m_ScrollRect.content.localPosition;
        vPosition.y = 0;
        m_TabCoven.m_ScrollRect.content.localPosition = vPosition;
        //m_TabCoven.m_ScrollRect.verticalScrollbar.value = 1;
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
        List<CovenScrollViewItemMember> vList = m_TabCoven.m_ListItemPool.GetActiveGameObjectList<CovenScrollViewItemMember>();
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

    }
    public void OnClickAlliances()
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
                m_TabCoven.m_ListItemPool.Despawn(pItem.gameObject);
            },
            () => {
                Debug.Log("Canceled");
            }
        );
    }

    public void OnClickEditUserTitle(CovenScrollViewItem pItem)
    {
        CovenScrollViewItemMember pMemberItem = (CovenScrollViewItemMember)pItem;
        m_EditPopup.Show(pMemberItem.m_txtTitle.text, (bool bChanged, string sTitle) =>
        {
            // TODO: notify server
            if (bChanged)
            {
                pMemberItem.CurrentUser.title = sTitle;
                pMemberItem.m_txtTitle.text = sTitle;
                Controller.UpdateCovensTitles(pMemberItem.CurrentUser);
            }
        });
    }

    #endregion


    private void InviteUser(string sUserName)
    {

    }
}