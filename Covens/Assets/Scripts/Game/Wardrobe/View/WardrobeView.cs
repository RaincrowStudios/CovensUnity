using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeView : UIBase
{
    public Animator anim;
    public Text subtitle;

    [Header("Character")]
    public CharacterView m_Character;


    [Header("Item Buttons")]
    public GameObject m_Container;
    public SimpleObjectPool m_ItemPool;


    protected WardrobeController Controller
    {
        get { return WardrobeController.Instance; }
    }


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

    private void Start()
    {
        m_ItemPool.Setup();
    }


    public override void Show()
    {
        base.Show();



    }


    public void SetupItens()
    {
        m_ItemPool.DespawnAll();
        foreach(var pItem in Controller.Itens)
        {

        }
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