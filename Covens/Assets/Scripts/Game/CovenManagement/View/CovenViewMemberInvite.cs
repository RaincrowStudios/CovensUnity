using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;


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
            FillList(pInvite.requests, true);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };

        Controller.CovenViewPending(Success, Error);
    }




    public void FillList(MemberOverview[] pCovenData, bool bAnimate)
    {
        m_TabCoven.m_ListItemPool.DespawnAll();
        if (pCovenData != null && pCovenData.Length > 0)
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
                if (bAnimate)
                {
                    pView.transform.localScale = Vector3.zero;
                    LeanTween.scale(pView.gameObject, Vector3.one, .2f).setDelay(0.05f * i).setEase(LeanTweenType.easeOutBack);
                }
            }
        }

        // set the scrollbar to top
        Vector3 vPosition = m_TabCoven.m_ScrollRect.content.localPosition;
        vPosition.y = 0;
        m_TabCoven.m_ScrollRect.content.localPosition = vPosition;
        UIGenericLoadingPopup.CloseLoading();
    }




    #region button callback

    private void View_OnClickCovenAccept(CovenScrollViewItemMember pItem)
    {
        MemberAcceptInvite(pItem.CovenName, pItem);
    }
    private void View_OnClickCovenReject(CovenScrollViewItemMember pItem)
    {
        MemberRejectInvite(pItem.CurrentMemberOverview.character);
        //MemberRejectInvite(pItem.CovenName);
    }

    #endregion


    public void MemberAcceptInvite(string sCovenName, CovenScrollViewItemMember pItem)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string pCovenData) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            pItem.gameObject.SetActive(false);
            //CovenView.Instance.ShowTabMembers(CovenController.Player);
            UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_InviteAccepted").Replace("<name>", sCovenName), null);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        Controller.AcceptMember(sCovenName, Success, Error);
    }
    public void MemberRejectInvite(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        System.Action<string> Success = (string sOk) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_InviteRejectDesc").Replace("<name>", sCovenName), null);
        };
        System.Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        Controller.RejectMember(sCovenName, Success, Error);
        
    }

}
