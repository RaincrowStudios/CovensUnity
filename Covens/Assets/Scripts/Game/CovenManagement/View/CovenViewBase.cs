using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CovenViewBase : UIBaseAnimated
{
    public CovenView.TabCoven m_TabCoven;
    // this controller can not be a singleton because we will use it to load other's screens
    protected CovenController Controller
    {
        get;
        set;
    }

    public void Setup(CovenController pController)
    {
        Controller = pController;
    }
    public void Show(CovenController pController)
    {
        Setup(pController);
        Show();
    }


}