using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// stores the scroll view item data of a member
/// should we split them to coven and member?
/// </summary>
public class CovenScrollViewItemMember : CovenScrollViewItem
{

    [Header("Editor Access")]
    public GameObject m_EditorRemove;
    public GameObject m_EditorChangeTitle;

    [Header("Actionable buttons")]
    public GameObject m_btnAccept;
    public GameObject m_btnReject;

    private CovenItem m_pUserItem;

    public event Action<CovenScrollViewItemMember> OnClickCovenAccept;
    public event Action<CovenScrollViewItemMember> OnClickCovenReject;

    public CovenItem CurrentUser
    {
        get { return m_pUserItem; }
    }

    public CovenController CurrentCovenController;



    public override void ResetItem()
    {
        base.ResetItem();
        OnClickCovenAccept = null;
        OnClickCovenReject = null;
    }


    #region setup



    public void SetupMemberItem(CovenItem pUser)
    {
        var eRole = CovenController.ParseRole(pUser.rank);
        m_pUserItem = pUser;
        Setup(pUser.playerLevel, pUser.playerName, pUser.title, pUser.status);
    }
    public void Setup(int iLevel, string sName, string sTitle, string sStatus)
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

        // that's for members
        SetEditorModeEnabled(false);
    }

    public void SetEditorModeEnabled(bool bEnabled, bool bAnimate = false, int iIdx = 0)
    {
        CovenController.CovenPlayerActions ePossibleActions = CovenController.Player.GetPossibleActions();
        if ((ePossibleActions & CovenController.CovenPlayerActions.ChangeTitle) != 0)
        {
            SetEnabled(m_EditorChangeTitle, bEnabled, bAnimate, iIdx);
        }
        if ((ePossibleActions & CovenController.CovenPlayerActions.Remove) != 0)
        {
            SetEnabled(m_EditorRemove, bEnabled, bAnimate, iIdx);
        }
    }

    #endregion


    #region button callbacks

    public void OnClickItem()
    {
        //CovenView.Instance.ShowTabMembers(CurrentCovenController);
    }

    public void OnClickAccept()
    {
        if (OnClickCovenAccept != null)
            OnClickCovenAccept(this);

        //UIGenericLoadingPopup.ShowLoading();
        //System.Action<string> Success = (string sOk) =>
        //{
        //    CovenView.Instance.ShowTabMembers(CovenController.Player);
        //    UIGenericLoadingPopup.CloseLoading();
        //};
        //System.Action<string> Error = (string sError) =>
        //{
        //    UIGenericPopup.ShowConfirmPopup("Error", "Error: " + sError, null);
        //    UIGenericLoadingPopup.CloseLoading();
        //};
        //CovenController.Player.JoinCoven(CurrentCovenController.CovenName, Success, Error);
    }
    public void OnClickReject()
    {
        UIGenericPopup.ShowYesNoPopup(
           "Reject Request",
           "Do you wanna reject '<player>' invitation?".Replace("<player>",
           m_pUserItem.playerName), () => {
               if (OnClickCovenReject != null)
                   OnClickCovenReject(this);
               gameObject.SetActive(false);
           }, null
           );
    }


    #endregion


    public void Reject()
    {
        // TODO: reject join to coven
        // disable me for now
        gameObject.SetActive(false);
    }

}