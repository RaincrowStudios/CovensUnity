using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A base coven scroll view item
/// </summary>
public class CovenScrollViewItem : MonoBehaviour
{
    public Text m_txtLevel;
    public Text m_txtName;
    public Text m_txtTitle;
    public Text m_txtStatus;
    public GameObject m_imgBackground;


    public virtual void ResetItem()
    {
        SetBackgound(false);
    }

    public void SetBackgound(bool bEnabled)
    {
        m_imgBackground.SetActive(bEnabled);
    }
    

    protected void SetEnabled(GameObject pGo, bool bEnabled, bool bAnimate, int iIdx)
    {
        if (pGo == null)
            return;

        // no animation behavior
        if (!bAnimate)
        {
            pGo.SetActive(bEnabled);
            return;
        }

        // animate behavior
        if (bEnabled)
        {
            pGo.SetActive(true);
            pGo.transform.localScale = Vector3.zero;
            LeanTween.scale(pGo, Vector3.one, .3f).setEase(LeanTweenType.easeOutBack).setDelay(iIdx * 0.01f);
        }
        else
        {
            LeanTween.scale(pGo, Vector3.zero, .3f).setEase(LeanTweenType.easeInBack).setDelay(iIdx * 0.01f).setOnComplete(
                () => { pGo.SetActive(false); }
                );
        }
        
    }


}