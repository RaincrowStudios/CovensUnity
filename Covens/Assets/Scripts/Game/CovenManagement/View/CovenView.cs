using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The coven's main UI view 
/// </summary>
public class CovenView : UIBaseAnimated
{
    public static CovenView Instance;

    [Header("Tabs")]
    public CovenViewBase m_CovenInviteTab;
    public CovenViewBase m_MemberInviteTab;
    public CovenViewBase m_MembersTab;
    public CovenViewBase m_CurrentTab;
    



    // we should link then with a controller to make sure it has the correct context
    private List<CovenViewBase> m_TabHistory = new List<CovenViewBase>();


    [System.Serializable]
    public struct TabCoven
    {
        public Text m_Title;
        public Text m_SubTitle;
        public SimpleObjectPool m_ListItemPool;
        public ScrollRect m_ScrollRect;
    }


    // this controller can not be a singleton because we will use it to load other's screens
    private CovenController Controller
    {
        get { return CovenController.Player; }
    }
    public CovenViewMemberInvite TabMemberInvite
    {
        get { return (CovenViewMemberInvite)m_MemberInviteTab; }
    }
    public CovenViewMembers TabMembers
    {
        get { return (CovenViewMembers)m_MembersTab; }
    }
    public CovenViewCovenInvite TabCovenInvite
    {
        get { return (CovenViewCovenInvite)m_CovenInviteTab; }
    }



    private void Start()
    {
        Instance = this;
        m_CovenInviteTab.Hide();
        m_MembersTab.Hide();
        m_MemberInviteTab.Hide();
    }

    public override void Show()
    {
        base.Show();
        ShowMain();
    }

    public void ShowTabMembers(CovenController pController)
    {
        ShowTab(m_MembersTab, pController);
    }
    public void ShowTabMembers()
    {
        ShowTab(m_MembersTab);
    }
    public void ShowTabMembeRequests()
    {
        ShowTab(m_MemberInviteTab);
    }
    public void ShowTabCovensRequests()
    {
        ShowTab(m_CovenInviteTab);
    }

    public void ShowMain()
    {
        if (!Controller.IsInCoven)
        {
            ShowTab(m_CovenInviteTab);
        }
        else
        {
            ShowTab(m_MembersTab);
        }
    }



    void ShowTab(CovenViewBase pUI, CovenController pController = null)
    {
        if (!IsVisible)
            Show();
        if (m_CurrentTab != null && m_CurrentTab != pUI)
        {
            m_CurrentTab.Close();
        }
        // setting player controller as a default controller
        if (pController == null)
            pController = Controller;

        pUI.Show(pController);
        m_CurrentTab = pUI;
        m_TabHistory.Add(pUI);
    }

    public void BackTab()
    {
        if(m_TabHistory.Count >= 2)
        {
            // shows the latest ui shown
            m_TabHistory.RemoveAt(m_TabHistory.Count - 1);
            ShowTab(m_TabHistory[m_TabHistory.Count - 1]);
            m_TabHistory.RemoveAt(m_TabHistory.Count - 1);
        }
    }






    #region buttons callback

    public void OnClickClose()
    {
        Close();
    }

    public void OnClickOpenCovenInvite()
    {
        ShowTab(m_CovenInviteTab);
    }
    public void OnClickOpenMemberInvite()
    {
        ShowTab(m_MemberInviteTab);
    }
    public void OnClickOpenMembers()
    {
        ShowTab(m_MembersTab);
    }

    #endregion



}
