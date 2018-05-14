using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// stores the scroll view item data
/// </summary>
public class CovenScrollViewItem : MonoBehaviour
{
    public Text m_txtLevel;
    public Text m_txtName;
    public Text m_txtTitle;
    public Text m_txtStatus;
    public GameObject m_imgBackground;

    [Header("Editor Access")]
    public GameObject m_EditorRemove;
    public GameObject m_EditorChangeTitle;


    private CovenController.CovenTitle m_eTitle;


    public CovenController.CovenTitle CurrentTitle
    {
        get
        {
            return m_eTitle;
        }
    }


    public void Setup(string sLevel, string sName, CovenController.CovenTitle eTitle, string sStatus)
    {
        m_txtLevel.text = sLevel;
        m_txtName.text = sName;
        m_txtTitle.text = eTitle.ToString();    // tostring for now
        m_txtStatus.text = sStatus;
        
        m_eTitle = eTitle;
        SetEditorModeEnabled(false);
    }

    public void SetBackgound(bool bEnabled)
    {
        m_imgBackground.SetActive(bEnabled);
    }

    public void SetEditorModeEnabled(bool bEnabled, bool bAnimate = false, int iIdx = 0)
    {
        CovenController.CovenPlayerActions ePossibleActions = CovenController.GetActionsByTitle(m_eTitle);
        if ((ePossibleActions & CovenController.CovenPlayerActions.Promote) != 0)
        {
             SetEnabled(m_EditorChangeTitle, bEnabled, bAnimate, iIdx);
        }
        if ((ePossibleActions & CovenController.CovenPlayerActions.Remove) != 0)
        {
            SetEnabled(m_EditorRemove, bEnabled, bAnimate, iIdx);
        }
    }

    void SetEnabled(GameObject pGo, bool bEnabled, bool bAnimate, int iIdx)
    {
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