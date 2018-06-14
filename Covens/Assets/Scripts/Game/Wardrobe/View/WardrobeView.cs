using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeView : UIBase
{
    public Animator anim;

    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
        anim.SetBool("animate", true);
        Invoke("OnShowFinish", 1f);
    }
    public override void DoCloseAnimation()
    {
        base.DoCloseAnimation();
        anim.SetBool("animate", false);
        Invoke("OnCloseFinish", 1f);
    }

}