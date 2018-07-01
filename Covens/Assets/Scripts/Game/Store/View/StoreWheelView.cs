using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreWheelView : UIBaseAnimated
{
    public GameObject m_btnSilver;
    public GameObject m_btnElixir;
    public GameObject m_btnGear;

    private GameObject m_btnSelected;


    public void OnClickSilver()
    {
        m_btnSelected = m_btnSilver;
        UIManager.Get<StoreView>().ShowTabSilver();
        Close();
    }
    public void OnClickElixir()
    {
        m_btnSelected = m_btnElixir;
        UIManager.Get<StoreView>().ShowTabElixirs();
        Close();
    }
    public void OnClickGear()
    {
        m_btnSelected = m_btnGear;
        UIManager.Get<StoreView>().ShowTabGear();
        Close();
    }


    public override void DoCloseAnimation()
    {
        //base.DoCloseAnimation();
        m_IsAnimating = true;
        LeanTween.rotateLocal(m_Target, new Vector3(0, 0, 90), .4f).setOnComplete(OnCloseFinish);
        LeanTween.scale(m_btnSelected, new Vector3(3, 3, 3), .4f);

        //LeanTween.alpha(m_btnSelected, new Vector3(3, 3, 3), .4f);
    }

    public override void OnCloseFinish()
    {
        base.OnCloseFinish();
        m_Target.transform.localRotation = Quaternion.identity;
        m_btnSelected.transform.localScale = Vector3.one;
    }

}
