using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovenViewCovenInvite : UIBaseAnimated
{

    public CovenView.TabCoven m_TabCoven;

    [Header("Botton Buttons")]
    public GameObject m_btnCreate;
    public GameObject m_btnRequest;
    public GameObject m_btnRequestAlly;


    // this controller can not be a singleton because we will use it to load other's screens
    private CovenController Controller
    {
        get { return CovenController.Instance; }
    }

    public override void Show()
    {
        base.Show();

        // active buttons
        Utilities.SetActiveList(!Controller.IsInCoven, m_btnCreate, m_btnRequest);
        Utilities.SetActiveList(Controller.IsInCoven, m_btnRequestAlly);
    }



    #region button callbacks

    public void OnClickNewCoven()
    {
        UIGenericInputPopup.ShowPopup("Choose coven's name:", "", CreateCoven, null);
    }
    public void OnClicRequestInvite()
    {
        UIGenericInputPopup.ShowPopup("Type Coven's name", "", RequestInviteCoven, null);
    }
    public void OnClicRequestInviteAlly()
    {
        UIGenericInputPopup.ShowPopup("Type Coven's name", "", RequestInviteCoven, null);
    }

    #endregion


    void CreateCoven(string sCovenName)
    {

    }

    void RequestInviteCoven(string sCovenName)
    {

    }
}