using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LocationActionButton : MonoBehaviour
{

    [SerializeField] private Image m_Progress;
    [SerializeField] private Image m_BtnImg;
    [SerializeField] private Button m_Btn;

    public bool isInteractable
    {
        get { return m_Btn.interactable; }
    }

    void Awake()
    {
    }

    public void Setup(int cooldown, Sprite sprite, System.Action onClick)
    {
        m_Progress.fillAmount = 0;
        m_BtnImg.sprite = sprite;
        m_Btn.interactable = true;
        m_Btn.onClick.RemoveAllListeners();
        m_Btn.onClick.AddListener(() =>
        {
            onClick();
            m_Btn.interactable = false;
            LeanTween.value(1, 0, cooldown).setOnUpdate((float v) =>
            {
                m_Progress.fillAmount = v;
            }).setOnComplete(() =>
            {
                m_Btn.interactable = true;
            });
        });
    }
}
