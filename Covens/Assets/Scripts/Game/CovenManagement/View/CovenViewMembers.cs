using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oktagon.Localization;


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
        // disable all buttons
        Utilities.SetActiveList(false, ButtonList);
        //Utilities.SetEnableButtonList(true, ButtonList);
        m_TabCoven.m_Title.text = Controller.CovenName;
        m_TabCoven.m_SubTitle.text = "not defined sub title";
        m_TabCoven.m_ListItemPool.DespawnAll();

        // add events to player coven
        if (Controller.IsPlayerCoven)
        {
            Controller.OnCovenDataChanged += Controller_OnCovenDataChanged;
        }

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

    private void Controller_OnCovenDataChanged(string sReason)
    {
        // I know this is a lazy way to reload the data, let's see how it will perform
        SetupDataList(false);
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
            UIGenericPopup.ShowErrorPopupLocalized(sError, CovenView.Instance.Close);
        };

        Controller.RequestDisplayCoven(Success, Error);
    }

    private void SetupDataList(bool bAnimate = true)
    {
        // setup first props
        m_TabCoven.m_Title.text = Controller.CovenName;
        m_TabCoven.m_SubTitle.text = "not defined sub title";
        m_TabCoven.m_ListItemPool.DespawnAll();

        // setup members
        FillList(Controller.Data, bAnimate);
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

    public void FillList(CovenData pCovenData, bool bAnimate)
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
            if (bAnimate)
            {
                pView.transform.localScale = Vector3.zero;
                LeanTween.scale(pView.gameObject, Vector3.one, .2f).setDelay(0.05f * i).setEase(LeanTweenType.easeOutBack);
            }
        }
        // set the scrollbar to top
        Vector3 vPosition = m_TabCoven.m_ScrollRect.content.localPosition;
        vPosition.y = 0;
        m_TabCoven.m_ScrollRect.content.localPosition = vPosition;
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
            Utilities.SetActiveList(pData.requests != null && pData.requests.Length > 0, m_MemberRequest.m_Root);
            if (pData.requests != null && pData.requests.Length > 0)
            {
                m_MemberRequest.m_Text.text = pData.requests.Length.ToString();
                m_MemberRequest.m_Root.transform.localScale = Vector3.zero;
                LeanTween.scale(m_MemberRequest.m_Root, Vector3.one, .4f).setEase(LeanTweenType.easeOutBack);
            }
        };

        Controller.CovenViewPending(Success, null);
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



    #region buttons callback


    public void OnClickChat()
    {
        Debug.Log("OnClickChat");
    }
    public void OnClickLeave()
    {
        UIGenericPopup.ShowYesNoPopupLocalized("Coven_LeaveTitle", "Coven_LeaveDescription", LeaveCoven, null);
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
        var pUI = UIGenericInputPopup.ShowPopupLocalized("Coven_TitleInvite", "", InviteUser, null);
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
        UIGenericPopup.ShowYesNoPopup(
            Lokaki.GetText("General_Info"),
            Lokaki.GetText("Coven_KickUserDesc").Replace("<name>", pItem.m_txtName.text),
            () => {
                //m_TabCoven.m_ListItemPool.Despawn(pItem.gameObject);
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
   /* public void OnClickPromoteMember(CovenScrollViewItem pItem)
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
    }*/

    private void View_OnClickChangeTitle(CovenScrollViewItemMember obj)
    {
        Controller.UpdateCovensTitles(obj.UserName, obj.UserTitle, null, null);
    }
    private void View_OnClickPromote(CovenScrollViewItemMember obj)
    {
        var eRole = CovenController.GetNextRole(obj.m_Role);
        Action Promote = () =>
        {
            PromoteMember(eRole, obj.UserName, obj);
        };

        UIGenericPopup.ShowYesNoPopup(
            Lokaki.GetText("Coven_PromoteTitle"),
            Lokaki.GetText("Coven_PromoteDesc").Replace("<name>", obj.UserName).Replace("<role>", Lokaki.GetEnumLokakiText(eRole)),
            Promote, null
            );
    }
    #endregion



    private void MemberRequest(string sText)
    {
        UIGenericInputPopup.Instance.SetLoading(true);
        Action<FindResponse> Success = (FindResponse pItens) =>
        {
            UIGenericInputPopup.Instance.SetLoading(false);
            UIGenericInputPopup.Instance.SetTipList(pItens.matches);
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
            // <name> was invited to coven.
            UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_InviteSuccessDesc").Replace("<name>", sUserName), null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowErrorPopupLocalized( sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };

        Controller.InvitePlayer(sUserName, Success, Error);
    }


    public void KickUser(string sUserName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string pCovenData) =>
        {
            UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_KickSuccess").Replace("<name>", sUserName), null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };

        Controller.Kick(sUserName, Success, Error);
    }

    void LeaveCoven()
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string pCovenData) =>
        {
            UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_LeaveSuccessDesc"), null);
            UIGenericLoadingPopup.CloseLoading();
            CovenView.Instance.Close();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
            UIGenericLoadingPopup.CloseLoading();
        };
        Controller.LeaveCoven(Success, Error);
    }


    private void PromoteMember(CovenController.CovenRole eToRole, string sUserName, CovenScrollViewItemMember obj)
    {
        Action<string> Success = (string sOK) =>
        {
            obj.SetNewRole(eToRole, true);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        Controller.PromoteMember(sUserName, eToRole, Success, Error);
    }
}