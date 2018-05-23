using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovenViewMemberInvite : CovenViewBase
{

    private void Start()
    {
        m_TabCoven.m_ListItemPool.Setup();
    }


    public override void Show()
    {
        base.Show();

        RequestInvites();
    }


    private void RequestInvites()
    {
        UIGenericLoadingPopup.ShowLoading();
        m_TabCoven.m_ListItemPool.DespawnAll();
        Action<MemberInvite> Success = (MemberInvite pInvite) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            FillList(pInvite.members);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup("Error", sError, null);
        };

        Controller.RequestList(Success, Error);
    }




    public void FillList(MemberOverview[] pCovenData)
    {
        
        for (int i = 0; i < pCovenData.Length; i++)
        {
            CovenScrollViewItemMember pView = m_TabCoven.m_ListItemPool.Spawn<CovenScrollViewItemMember>();
            pView.SetupMemberItem(pCovenData[i]);
            pView.SetBackgound(i % 2 == 0);
            // callbacks
            pView.OnClickCovenAccept += View_OnClickCovenAccept;
            pView.OnClickCovenReject += View_OnClickCovenReject;
            // scale it
            pView.transform.localScale = Vector3.zero;
            LeanTween.scale(pView.gameObject, Vector3.one, .2f).setDelay(0.05f * i).setEase(LeanTweenType.easeOutBack);
        }

        // set the scrollbar to top
        Vector3 vPosition = m_TabCoven.m_ScrollRect.content.localPosition;
        vPosition.y = 0;
        m_TabCoven.m_ScrollRect.content.localPosition = vPosition;
        //m_TabCoven.m_ScrollRect.verticalScrollbar.value = 1;

        UIGenericLoadingPopup.CloseLoading();
    }


    #region button callback

    private void View_OnClickCovenAccept(CovenScrollViewItemMember pItem)
    {
        MemberAcceptInvite(pItem.CovenName);
    }
    private void View_OnClickCovenReject(CovenScrollViewItemMember pItem)
    {
        MemberRejectInvite(pItem.CovenName);
    }


    #endregion


    public void MemberAcceptInvite(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        System.Action<CovenData> Success = (CovenData pCovenData) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            //CovenView.Instance.ShowTabMembers(CovenController.Player);
            UIGenericPopup.ShowConfirmPopup("Success", sCovenName + " was invited", null);
        };
        System.Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup("Error", "Error: " + sError, null);
        };
        Controller.AcceptMember(sCovenName, Success, Error);
    }
    public void MemberRejectInvite(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        System.Action<string> Success = (string sOk) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup("Success", sCovenName + " was rejected", null);
        };
        System.Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup("Error", "Error: " + sError, null);
        };
        Controller.RejectMember(sCovenName, Success, Error);
    }
}
