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
    public Image m_sptRole;
    public GameObject m_UserBG;

    [Header("Editor Access")]
    public GameObject m_EditorRemove;
    public GameObject m_EditorChangeTitle;
    public InputField m_iptTitle;

    [Header("Actionable buttons")]
    public GameObject m_btnAccept;
    public GameObject m_btnReject;

    private CovenMember m_pUserItem;
    public CovenController.CovenRole m_Role;

    public event Action<CovenScrollViewItemMember> OnClickCovenAccept;
    public event Action<CovenScrollViewItemMember> OnClickCovenReject;
    public event Action<CovenScrollViewItemMember> OnClickChangeTitle;
    public event Action<CovenScrollViewItemMember> OnClickPromote;

    public CovenMember CurrentUser
    {
        get { return m_pUserItem; }
    }
    public string UserName
    {
        get { return m_txtName.text; }
    }
    public string UserTitle
    {
        get { return m_txtTitle.text; }
    }
    public bool IsPlayerItem
    {
        get {
            return UserName == PlayerDataManager.playerData.displayName;
        }
    }
    


    public override void ResetItem()
    {
        base.ResetItem();
        OnClickCovenAccept = null;
        OnClickCovenReject = null;
        OnClickChangeTitle = null;
        OnClickPromote = null;
        m_Role = CovenController.CovenRole.Member;
    }


    #region setup



    public void SetupMemberItem(CovenMember pUser)
    {
        CovenController.CovenRole eRole = CovenController.ParseRole(pUser.role);
        m_pUserItem = pUser;
        Setup(pUser.level, pUser.displayName , pUser.title, pUser.status, eRole);
    }
    public void SetupMemberItem(MemberOverview pUser)
    {
        Setup(pUser.playerLevel, pUser.playerName, null, null, CovenController.CovenRole.None);
    }
    public void Setup(int iLevel, string sName, string sTitle, string sStatus, CovenController.CovenRole eRole)
    {
        ResetItem();
        if (m_txtLevel)
            m_txtLevel.text = iLevel.ToString();
        if (m_txtName)
            m_txtName.text = sName;
        if (m_txtTitle)
            m_txtTitle.text = sTitle;
        if (m_txtStatus)
            m_txtStatus.text = sStatus;
        // hightlight when is user item
        if(m_UserBG)
            m_UserBG.gameObject.SetActive(IsPlayerItem);
        SetNewRole(eRole);
        SetEditorModeEnabled(false);
    }

    public void SetNewRole(CovenController.CovenRole eRole, bool Animate = false)
    {
        if (m_sptRole)
        {
            m_sptRole.gameObject.SetActive(eRole != CovenController.CovenRole.None);
            m_sptRole.sprite = SpriteResources.GetSprite("Icon-Coven-" + eRole.ToString());
            m_Role = eRole;
            if (Animate)
            {
                LeanTween.scale(m_sptRole.gameObject, new Vector3(1.4f, 1.4f, 1.4f), .5f).setEase(LeanTweenType.punch);
                SetEditorModeEnabled(false);
                SetEditorModeEnabled(true);
            }
        }
    }

    public void SetEditorModeEnabled(bool bEnabled, bool bAnimate = false, int iIdx = 0)
    {
        if (!bEnabled)
        {
            SetEnabled(m_EditorChangeTitle, bEnabled, bAnimate, iIdx);
            if(m_iptTitle)
                SetEnabled(m_iptTitle.gameObject, bEnabled, bAnimate, iIdx);
            SetEnabled(m_EditorRemove, bEnabled, bAnimate, iIdx);
        }
        else
        {
            CovenController.CovenPlayerActions ePossibleActions = CovenController.Player.GetPossibleActions(m_pUserItem, IsPlayerItem);
            if ((ePossibleActions & CovenController.CovenPlayerActions.ChangeTitle) != 0)
            {
                if (m_iptTitle)
                {
                    SetEnabled(m_iptTitle.gameObject, bEnabled, bAnimate, iIdx);
                    m_iptTitle.text = UserTitle;
                }
            }
            if ((ePossibleActions & CovenController.CovenPlayerActions.Promote) != 0)
            {
                SetEnabled(m_EditorChangeTitle, bEnabled, bAnimate, iIdx);
            }
            if ((ePossibleActions & CovenController.CovenPlayerActions.Remove) != 0)
            {
                SetEnabled(m_EditorRemove, bEnabled, bAnimate, iIdx);
            }
        }
    }

    #endregion


    #region button callbacks

    public void OnClickItem()
    {
    }

    public void OnClickAccept()
    {
        if (OnClickCovenAccept != null)
            OnClickCovenAccept(this);
    }
    public void OnClickReject()
    {
        UIGenericPopup.ShowYesNoPopup(
           "Reject Request",
           "Do you wanna reject '<player>' invitation?".Replace("<player>",
           UserName), () => {
               if (OnClickCovenReject != null)
                   OnClickCovenReject(this);
               gameObject.SetActive(false);
           }, null
           );
    }
    public void OnChangeTitle()
    {
        m_txtTitle.text = m_iptTitle.text;
        if (OnClickChangeTitle != null)
            OnClickChangeTitle(this);
    }
    public void OnClickPromoteButton()
    {
        if (OnClickPromote != null)
            OnClickPromote(this);
    }

    #endregion


    public void Reject()
    {
        // TODO: reject join to coven
        // disable me for now
        gameObject.SetActive(false);
    }

}