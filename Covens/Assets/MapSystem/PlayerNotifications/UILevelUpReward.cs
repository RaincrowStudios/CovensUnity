using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelUpReward : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;

    [SerializeField] private Image m_Art;

    [Space(10)]
    [SerializeField] private Sprite m_SilverDrachsArt;
    
    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
    }

    public void SetupSilver(int amount)
    {
        m_Title.text = LocalizeLookUp.GetText("ftf_reward_upper");
        m_Subtitle.text = LocalizeLookUp.GetText("store_bought_drachs_upper").Replace("{{Number}}", amount.ToString());
        m_Art.overrideSprite = m_SilverDrachsArt;
        //Show();

        //DownloadedAssets.GetSprite("silver1", m_Art, true);
    }

    public void SetupSpell(string id)
    {
        m_Title.text = LocalizeLookUp.GetText("store_gear_unlocked_upper");
        m_Subtitle.text = LocalizeLookUp.GetSpellName(id);

        m_Art.color = new Color(1, 1, 1, 0);
        DownloadedAssets.GetSprite(id, (spr) =>
        {
            m_Art.overrideSprite = spr;
            LeanTween.color(m_Art.rectTransform, Color.white, 1f);
        });
        //Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 1);
    }
}
