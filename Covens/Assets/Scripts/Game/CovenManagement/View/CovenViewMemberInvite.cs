using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CovenViewMemberInvite : UIBaseAnimated
{
    public CovenView.TabCoven m_TabCoven;


    // this controller can not be a singleton because we will use it to load other's screens
    private CovenController Controller
    {
        get { return CovenController.Instance; }
    }



}
