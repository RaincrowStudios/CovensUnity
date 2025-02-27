﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;


public class CovenViewCovenInvite : CovenViewBase
{

    //public CovenView.TabCoven m_TabCoven;

    [Header("Botton Buttons")]
    public GameObject m_btnCreate;
    public GameObject m_btnRequest;
    public GameObject m_btnRequestAlly;
    public GameObject m_btnBack;


    private void Start()
    {
        m_TabCoven.m_ListItemPool.Setup();
    }


    public override void Show()
    {
        base.Show();

        // disable all
        Utilities.SetActiveList(false, m_btnCreate, m_btnRequest, m_btnRequestAlly, m_btnBack);
        m_TabCoven.m_ListItemPool.DespawnAll();
        UIGenericLoadingPopup.ShowLoading();
        Controller.OnCovenDataChanged -= Controller_OnCovenDataChanged;
        if (Controller.IsInCoven)
        {
            SetupForCovenDisplay();
        }
        else
        {
            SetupForNonCovenDisplay();
        }
    }


    #region not a coven display

    /// <summary>
    /// sets up the coven display for users that are not in coven yet
    /// </summary>
    private void SetupForNonCovenDisplay()
    {
        m_TabCoven.m_Title.text = "Invites";
        Utilities.SetActiveList(true, m_btnCreate, m_btnRequest);

        // tests
        Controller.CharacterInvites(ResponseCovenInvites);
    }

    public void ResponseCovenInvites(CovenOverview[] pInvites, string sError)
    {
        if (pInvites != null && pInvites != null)
        {
            FillList(pInvites);
        }
        else
        {
            UIGenericLoadingPopup.CloseLoading();
        }
    }

    #endregion



    #region coven ally manager

    /// <summary>
    /// sets up the coven display for users that ARE in a coven and may join to alliances
    /// </summary>
    private void SetupForCovenDisplay()
    {
        m_TabCoven.m_Title.text = Lokaki.GetText("Coven_TitleAlliances");
        Utilities.SetActiveList(true, m_btnBack);
        if(Controller.CanManageAlliance)
            Utilities.SetActiveList(true, m_btnBack, m_btnRequestAlly);

        // add events to player coven
        if (Controller.IsPlayerCoven)
        {
            Controller.OnCovenDataChanged -= Controller_OnCovenDataChanged;
            Controller.OnCovenDataChanged += Controller_OnCovenDataChanged;
        }

        FillList( Controller.GetAllianceRequestsList().ToArray());
    }

    private void Controller_OnCovenDataChanged(string sReason)
    {
        // I know this is a lazy way to reload the data, let's see how it will perform
        FillList(Controller.GetAllianceRequestsList().ToArray(), false);
    }
    #endregion



    public void FillList(CovenOverview[] pCovenData, bool bAnimate = true)
    {
        m_TabCoven.m_ListItemPool.DespawnAll();
        for (int i = 0; i < pCovenData.Length; i++)
        {
            CovenScrollViewItemCoven pView = m_TabCoven.m_ListItemPool.Spawn<CovenScrollViewItemCoven>();
            CovenController pController = new CovenController(pCovenData[i].coven);
            //pController.IsInCoven = true;
            pController.Setup(pCovenData[i]);
            pView.ResetItem();
            pView.SetupCovenItem(pController, pCovenData[i]);
            pView.SetBackgound(i % 2 == 0);
            // callbacks
            pView.OnClickCovenAccept += View_OnClickCovenAccept;
            pView.OnClickCovenReject += View_OnClickCovenReject;
            pView.OnClickCovenAlly += View_OnClickCovenAlly;
            pView.OnClickCovenUnally += View_OnClickCovenUnally;
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
        //m_TabCoven.m_ScrollRect.verticalScrollbar.value = 1;

        UIGenericLoadingPopup.CloseLoading();
    }

    private void CovenTipRequest(string sText)
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
        Controller.FindCoven(sText, Success, Error);
    }

    #region button callbacks

    public void OnClickNewCoven()
    {
        // Choose coven's name
        UIGenericInputPopup.ShowPopup(Lokaki.GetText("Coven_CreateTitle"), "", CreateCoven, null);
    }
    public void OnClicRequestInvite()
    {
        UIGenericInputPopup pInput = UIGenericInputPopup.ShowPopupLocalized("Coven_InviteTitle", "", RequestInviteCoven, null);
        pInput.SetInputChangedCallback(CovenTipRequest, 1);
    }
    public void OnClicRequestInviteAlly()
    {
        UIGenericInputPopup pInput = UIGenericInputPopup.ShowPopupLocalized("Coven_InviteTitle", "", RequestInviteAllyCoven, null);
        pInput.SetInputChangedCallback(CovenTipRequest, 1);
    }
    public void OnClickCovenItem(CovenScrollViewItem pItem)
    {
        CovenScrollViewItemCoven pItemCoven = (CovenScrollViewItemCoven)pItem;
        CovenView.Instance.ShowTabMembers(pItemCoven.CurrentCovenController);
    }



    private void View_OnClickCovenAccept(CovenScrollViewItemCoven pItem)
    {
        CovenAcceptInvite(pItem.m_pCovenOverview.inviteToken);
    }
    private void View_OnClickCovenReject(CovenScrollViewItemCoven pItem)
    {
        // there is no way to reject the coven invitation
    }
    private void View_OnClickCovenAlly(CovenScrollViewItemCoven pItem)
    {
        Ally(pItem.CovenName);
    }
    private void View_OnClickCovenUnally(CovenScrollViewItemCoven pItem)
    {
        Unally(pItem.CovenName, pItem);
    }
    #endregion


    #region UI-controller actions

    private void Unally(string sCovenName, CovenScrollViewItemCoven pItem)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string sOK) =>
        {
            //pItem.gameObject.SetActive(false);
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        CovenController.Player.Unally(sCovenName, Success, Error);
    }
    private void Ally(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string sOK) =>
        {
            UIGenericLoadingPopup.CloseLoading();
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        CovenController.Player.Ally(sCovenName, Success, Error);
    }


    public void CovenAcceptInvite(string sCovenInviteToken)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action Success = () =>
        {
            UIGenericLoadingPopup.CloseLoading();
            CovenView.Instance.ShowTabMembers(CovenController.Player);
        };
        Action<string> Error = (string sError) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sError, null);
        };
        CovenController.Player.JoinCoven(sCovenInviteToken, Success, Error);
    }
    public void CreateCoven(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        // TODO send create coven to server
        Controller.CreateCoven(sCovenName,
            (string sOk) => {
                UIGenericLoadingPopup.CloseLoading();
                CovenView.Instance.ShowTabMembers(Controller);
            },
            (string sError) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                UIGenericPopup.ShowErrorPopupLocalized(sError, null);
                ///UIGenericPopup.ShowYesNoPopup("Error", "Couldn't create a coven.\nError: " + sError + "\nWould you like to try again?", OnClickNewCoven, null);
            }
            );
    }

    public void RequestInviteAllyCoven(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Controller.Ally(sCovenName,
            (string sOk) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                // Alliance request sent with success
                UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_AllyRequestSuccess").Replace("<name>", sCovenName), null);
            },
            (string sError) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                UIGenericPopup.ShowErrorPopupLocalized(sError, null);
                //UIGenericPopup.ShowYesNoPopup("Error", "Couldn't invitecreate a coven.\nError: " + sError + "\nWould you like to try again?", OnClickNewCoven, null);
            }
            );

    }

    public void RequestInviteCoven(string sCovenName)
    {
        UIGenericLoadingPopup.ShowLoading();
        Controller.RequestJoinCoven(sCovenName,
            (string sOk) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                //<name> was invited to coven.
                UIGenericPopup.ShowConfirmPopup(Lokaki.GetText("General_Success"), Lokaki.GetText("Coven_RequestSuccess").Replace("<name>", sCovenName), null);
            },
            (string sError) =>
            {
                UIGenericLoadingPopup.CloseLoading();
                UIGenericPopup.ShowErrorPopupLocalized(sError, null);
                //UIGenericPopup.ShowYesNoPopup("Error", "Couldn't invitecreate a coven.\nError: " + sError + "\nWould you like to try again?", OnClickNewCoven, null);
            }
            );
    }

    #endregion
}