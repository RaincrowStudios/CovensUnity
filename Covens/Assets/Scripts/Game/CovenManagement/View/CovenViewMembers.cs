using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CovenViewMembers : CovenViewBase
{
    [Serializable]
    public struct PendingRequestNotification
    {
        public GameObject m_Root;
        public Text m_Text;
    }

    [Header("Extarnals")]
    public CovenTitleEditPopup m_EditPopup;

    [Header("Buttons")]
    public GameObject m_btnChat;
    public GameObject m_btnEdit;
    public GameObject m_btnLeave;
    public GameObject m_btnInvite;
    public GameObject m_btnRequests;
    public GameObject m_btnAlliances;
    public GameObject m_btnBack;
    public GameObject m_btnAcceptJoinCoven;
    public GameObject m_btnAcceptAlliance;
    public GameObject m_btnRejectAlliance;

    [Header("Pending Request Notification")]
    public PendingRequestNotification m_MemberRequest;
    public PendingRequestNotification m_AlliancesRequest;


    // inner
    private bool m_bEditorModeEnabled = false;
    private GameObject[] ButtonList;

    private void Awake()
    {
        ButtonList = new GameObject[] { m_btnChat, m_btnEdit, m_btnLeave, m_btnInvite, m_btnRequests, m_btnBack, m_btnAcceptJoinCoven, m_btnAcceptAlliance, m_btnRejectAlliance, m_btnAlliances, m_MemberRequest.m_Root, m_AlliancesRequest.m_Root };
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
        Debug.Log("SetupUI Here");
        // disable all buttons
        Utilities.SetActiveList(false, ButtonList);
        //Utilities.SetEnableButtonList(true, ButtonList);
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
            UIGenericLoadingPopup.CloseLoading();
            SetupDataList();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup("Error", "RequestCovensData Error: " + sError + ".\nCoven will be closed", CovenView.Instance.Close);
        };

        Controller.RequestDisplayCoven(Success, Error);
    }

    private void SetupDataList()
    {
        FillList(Controller.Data);
        Debug.Log("ok, data filled");

        // non player means we are just displaying someone's coven
        if (!Controller.IsPlayerCoven)
        {
            Utilities.SetActiveList(true, m_btnBack);
            if (!CovenController.Player.IsInCoven)
                Utilities.SetActiveList(true, m_btnAcceptJoinCoven);
            else if (Controller.IsCovenAnAlly)
                Utilities.SetActiveList(true, m_btnAcceptAlliance);
        }
        else
        {
            switch (Controller.CurrentRole)
            {
                case CovenController.CovenRole.Moderator:
                case CovenController.CovenRole.Administrator:
                    Utilities.SetActiveList(true, m_btnChat, /*m_btnInvite,*/ m_btnEdit, m_btnRequests, m_btnAlliances, m_btnLeave);
                    break;
                case CovenController.CovenRole.Member:
                    Utilities.SetActiveList(true, m_btnChat, m_btnInvite, m_btnLeave, m_btnAlliances);
                    break;
            }

            UpdateMembersRequest();
            UpdateAlliancesRequest();
        }
    }

    public void FillList(CovenData pCovenData)
    {
        
        for (int i = 0; i < pCovenData.members.Length; i++)
        {
            CovenScrollViewItemMember pView = m_TabCoven.m_ListItemPool.Spawn<CovenScrollViewItemMember>();
            var eRole = CovenController.ParseRole(pCovenData.members[i].role);
            pView.SetupMemberItem(pCovenData.members[i]);
            pView.SetBackgound(i % 2 == 0);
            // callbacks
            pView.OnClickChangeTitle += View_OnClickChangeTitle;
            pView.OnClickPromote += View_OnClickPromote;
    
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
        // disable other buttons
        Utilities.SetEnableButtonList(!m_bEditorModeEnabled, ButtonList);
        Utilities.SetEnableButtonList(true, m_btnEdit);

        List<CovenScrollViewItemMember> vList = m_TabCoven.m_ListItemPool.GetActiveGameObjectList<CovenScrollViewItemMember>();
        for (int i = 0; i < vList.Count; i++)
        {
            vList[i].SetEditorModeEnabled(m_bEditorModeEnabled, true, i);
        }
    }


    public void UpdateMembersRequest()
    {
        Action<MemberInvite> Success = (MemberInvite pData) =>
        {
            Utilities.SetActiveList(pData.members.Length > 0, m_MemberRequest.m_Root);
            if (pData.members != null && pData.members.Length > 0)
            {
                m_MemberRequest.m_Text.text = pData.members.Length.ToString();
                m_MemberRequest.m_Root.transform.localScale = Vector3.zero;
                LeanTween.scale(m_MemberRequest.m_Root, Vector3.one, .4f).setEase(LeanTweenType.easeOutBack);
            }
        };

        Controller.CovenViewPending(Success, null);
        //Utilities.SetActiveList(Controller.MembersRequest > 0, m_MemberRequest.m_Root);
        //if (Controller.MembersRequest > 0)
        //{
        //    m_MemberRequest.m_Text.text = Controller.MembersRequest.ToString();
        //    m_MemberRequest.m_Root.transform.localScale = Vector3.zero;
        //    LeanTween.scale(m_MemberRequest.m_Root, Vector3.one, .4f).setEase(LeanTweenType.easeOutBack);
        //}
    }
    public void UpdateAlliancesRequest()
    {
        Utilities.SetActiveList(Controller.AlliancesRequest > 0, m_AlliancesRequest.m_Root);
        if (Controller.AlliancesRequest > 0)
        {
            m_AlliancesRequest.m_Text.text = Controller.AlliancesRequest.ToString();
            m_AlliancesRequest.m_Root.transform.localScale = Vector3.zero;
            LeanTween.scale(m_AlliancesRequest.m_Root, Vector3.one, .4f).setEase(LeanTweenType.easeOutBack);
        }
    }
    private void View_OnClickChangeTitle(CovenScrollViewItemMember obj)
    {
        Controller.UpdateCovensTitles(obj.UserName, obj.UserTitle, null, null);
    }
    private void View_OnClickPromote(CovenScrollViewItemMember obj)
    {
        Action<string> Success = (string sOK) =>
        {
            var eRole = CovenController.GetNextRole(obj.m_Role);
            obj.SetNetRole(eRole, true);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowConfirmPopup("Error", sError, null);
        };
        Controller.PromoteMember(obj.UserName, Success, Error);
    }


    #region buttons callback


    public void OnClickChat()
    {
        Debug.Log("OnClickChat");
    }
    public void OnClickLeave()
    {
        UIGenericPopup.ShowYesNoPopup("Leave Coven", "Do you really wanna leave from the coven?", LeaveCoven, null);
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
        var pUI = UIGenericInputPopup.ShowPopup("Type User's name", "", InviteUser, null);
        pUI.SetInputChangedCallback(MemberRequest);
    }
    public void OnClickRequests()
    {
        CovenView.Instance.ShowTabMembeRequests();
    }
    public void OnClickAlliances()
    {
        CovenView.Instance.ShowTabCovensRequests();
    }
    public void OnClickAcceptInvite()
    {
        CovenView.Instance.TabCovenInvite.CovenAcceptInvite(Controller.CovenName);
    }


    public void OnClickKickUser(CovenScrollViewItem pItem)
    {
        UIGenericPopup.ShowYesNoPopup("Info", "Click Yes to remove <name> form the Coven.".Replace("<name>", pItem.m_txtName.text),
            () => {
                Debug.Log("Will kick the player Here. remember to notify the serverside");
                m_TabCoven.m_ListItemPool.Despawn(pItem.gameObject);
                KickUser(pItem.CovenName);
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
                Controller.UpdateCovensTitles(pMemberItem.CovenName, sTitle, null, null);
            }
        });
    }
    public void OnClickPromoteMember(CovenScrollViewItem pItem)
    {
        UIGenericPopup.ShowYesNoPopup(
            "Promote user", "You are about to promote '<name>' to '<role>'.\nYes to confirm.(wip)".Replace("<name>", pItem.m_txtName.text).Replace("<role>", "Admin") ,
            () => {
                Debug.Log("Will promote the player Here");
            },
            () => {
                Debug.Log("Canceled");
            }
        );
    }
    #endregion



    private void MemberRequest(string sText)
    {
        UIGenericInputPopup.Instance.SetLoading(true);
        Action<StringItens> Success = (StringItens pItens) =>
        {
            UIGenericInputPopup.Instance.SetLoading(false);
            UIGenericInputPopup.Instance.SetTipList(pItens.itens);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericInputPopup.Instance.SetLoading(false);
        };
        Controller.FindPlayer(sText, Success, Error);
    }

    private void InviteUser(string sUserName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string sOk) =>
        {
            UIGenericPopup.ShowConfirmPopup("Invite success", sUserName + " was invited to coven", null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowConfirmPopup("Invite Error", sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };

        Controller.InvitePlayer(sUserName, Success, Error);
    }


    public void KickUser(string sUserName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string pCovenData) =>
        {
            UIGenericPopup.ShowConfirmPopup("Kick success", sUserName + " was kicked out from the coven", null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowConfirmPopup("Kick Error", sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };

        Controller.Kick(sUserName, Success, Error);
    }

    void LeaveCoven()
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string pCovenData) =>
        {
            UIGenericPopup.ShowConfirmPopup("Kick success", "You have no coven anymore", null);
            UIGenericLoadingPopup.CloseLoading();
            CovenView.Instance.Close();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowConfirmPopup("Leave Coven Error", sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Controller.LeaveCoven(Success, Error);
    }
}