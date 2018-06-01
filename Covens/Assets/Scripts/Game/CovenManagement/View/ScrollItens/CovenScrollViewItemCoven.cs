using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Oktagon.Localization;

/// <summary>
/// coven item data. Does not do any action
/// </summary>
public class CovenScrollViewItemCoven : CovenScrollViewItem
{
    public Text m_txtDate;
    public Text m_txtRank;

    [Header("Actionable buttons")]
    public GameObject m_btnAccept;
    public GameObject m_btnReject;
    public GameObject m_btnUnally;
    public GameObject m_btnAlly;

    private CovenController.CovenActions m_eCovenActions;
    private CovenMember m_pUserItem;
    private CovenOverview m_pCovenOverview;

    public CovenController CurrentCovenController;

    public event Action<CovenScrollViewItemCoven> OnClickCovenAccept;
    public event Action<CovenScrollViewItemCoven> OnClickCovenReject;
    public event Action<CovenScrollViewItemCoven> OnClickCovenUnally;
    public event Action<CovenScrollViewItemCoven> OnClickCovenAlly;


    public override void ResetItem()
    {
        base.ResetItem();
        OnClickCovenAccept = null;
        OnClickCovenReject = null;
        OnClickCovenUnally = null;
        OnClickCovenAlly = null;
    }

    #region setup

    public void SetupCovenItem(CovenController pController, CovenOverview pCovenOverview)
    {
        CurrentCovenController = pController;
        m_pCovenOverview = pCovenOverview;
        Setup(0, pController.CovenName, null, null, pController.GetCovenAvailableActions());
    }
    public void Setup(int iLevel, string sName, string sTitle, string sStatus, CovenController.CovenActions eCovenActions)
    {
        ResetItem();
        if (m_txtLevel)
            m_txtLevel.text = iLevel.ToString();
        if (m_txtName)
            m_txtName.text = sName;
        if (m_txtTitle)
            m_txtTitle.text = sTitle;    // tostring for now
        if (m_txtStatus)
            m_txtStatus.text = sStatus;

        //CurrentCovenController.CurrentRole
        DateTime myDate = new DateTime(m_pCovenOverview.date);
        m_txtDate.text = myDate.ToString("MMMM dd, yyyy");
        m_txtRank.text = m_pCovenOverview.rank.ToString();

        // that's for covens
        m_eCovenActions = eCovenActions;
        SetCovenActions();
    }


    public void SetCovenActions()
    {
        // enable buttons by its required functionalities
        SetEnabled(m_btnAccept, ((m_eCovenActions & CovenController.CovenActions.Accept) != 0), false, 0);
        SetEnabled(m_btnAlly, ((m_eCovenActions & CovenController.CovenActions.Ally) != 0), false, 0);
        SetEnabled(m_btnReject, ((m_eCovenActions & CovenController.CovenActions.Reject) != 0), false, 0);
        SetEnabled(m_btnUnally, ((m_eCovenActions & CovenController.CovenActions.Unally) != 0), false, 0);
    }
    #endregion


    #region button callbacks

    public void OnClickItem()
    {
        CovenView.Instance.ShowTabMembers(CurrentCovenController);
    }
    public void OnClickAccept()
    {
        if (OnClickCovenAccept != null)
            OnClickCovenAccept(this);
    }
    public void OnClickReject()
    {
        //Do you wanna reject '<coven>' invitation?
        UIGenericPopup.ShowYesNoPopup(
           Lokaki.GetText("Coven_AllyRejectTitle"),
           Lokaki.GetText("Coven_AllyRejectDesc").Replace("<coven>",
           CurrentCovenController.CovenName), ()=> {
               if (OnClickCovenReject != null)
                   OnClickCovenReject(this);
               //gameObject.SetActive(false);
           }, null
           );
    }
    public void OnClickUnally()
    {
        // Do you wanna unally to coven<coven>?
        UIGenericPopup.ShowYesNoPopup(
            Lokaki.GetText("Coven_UnallyTitle"),
            Lokaki.GetText("Coven_UnallyDesc").Replace("<coven>",
            CurrentCovenController.CovenName), () =>
            {
                if (OnClickCovenUnally != null)
                    OnClickCovenUnally(this);
                //gameObject.SetActive(false);
            }, null);
    }
    public void OnClickAlly()
    {
        if (OnClickCovenAlly != null)
            OnClickCovenAlly(this);
    }


    #endregion

}
