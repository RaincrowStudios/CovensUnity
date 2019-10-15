using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LocationActionButton : MonoBehaviour
{

    [SerializeField] private Image m_Progress;
    [SerializeField] private Image m_BtnImg;
    [SerializeField] private Button m_Btn;
    [SerializeField] private GameObject m_locked;
    // System.Action m_OnClick;
    private int id;
    public bool isInteractable
    {
        get { return m_Btn.interactable; }
    }

    void Awake()
    {
    }

    public void SetLock(bool locked)
    {
        m_locked.SetActive(locked);
    }

    public void Setup(int cooldown, Sprite sprite, System.Action onClick, bool isCloak = false)
    {
        // m_OnClick = onClick;
        SetLock(false);
        m_Progress.fillAmount = 0;
        m_BtnImg.sprite = sprite;
        m_Btn.interactable = true;
        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(() =>
        {
            onClick();
            m_Btn.interactable = false;
            id = LeanTween.value(1, 0, cooldown).setOnUpdate((float v) =>
             {
                 if (m_Progress != null)
                     m_Progress.fillAmount = v;
             }).setOnComplete(() =>
             {
                 if (isCloak)
                 {
                     LocationPlayerAction.DisableCloaking();
                     Setup(180);
                 }
                 else
                 {
                     m_Btn.interactable = true;
                 }
             }).uniqueId;
        });
    }

    public void Setup(int cooldown)
    {
        LeanTween.cancel(id);
        m_Progress.fillAmount = 0;
        LeanTween.value(1, 0, cooldown).setOnUpdate((float v) =>
        {
            if (m_Progress != null)
                m_Progress.fillAmount = v;
        }).setOnComplete(() =>
        {
            m_Btn.interactable = true;
        });
    }
}
