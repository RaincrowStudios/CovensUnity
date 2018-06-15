using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeView : UIBase
{
    public Animator anim;
    public CharacterView m_Character;

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



    public void OnClickRandomize()
    {
        m_Character.RandomItens(WardrobeController.Instance.Itens);
    }
    public void OnClickEquip(WardrobeItemModel pItem)
    {
        m_Character.SetItem(pItem);
    }
}