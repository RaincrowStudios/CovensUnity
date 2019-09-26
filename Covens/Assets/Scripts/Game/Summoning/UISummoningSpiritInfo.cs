using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISummoningSpiritInfo : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private Image m_SpiritArt;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Lore;
    [SerializeField] private TextMeshProUGUI m_Ingredients;
    [SerializeField] private TextMeshProUGUI m_Required;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_WorldZone;

    [SerializeField] private Button m_CloseButton;

    private int m_TweenId;
    private int m_ScaleTweenId;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
    }

    public void Show(SpiritData spirit)
    {
        m_CanvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);

        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_CanvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;

        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).setEaseOutCubic().uniqueId;
        m_ScaleTweenId = LeanTween.scale(gameObject, Vector3.one, 0.5f).setEaseOutCubic().uniqueId;

        m_Title.text = spirit.Name;
        m_Lore.text = spirit.Description;
        m_WorldZone.text = string.IsNullOrEmpty(spirit.legend) ? "" : LocalizeLookUp.GetText(spirit.legend);

        if (spirit.tier == 1)
            m_Tier.text = LocalizeLookUp.GetText("rarity_common");// "Common";
        else if (spirit.tier == 2)
            m_Tier.text = LocalizeLookUp.GetText("rarity_less");//"Less Common";
        else if (spirit.tier == 3)
            m_Tier.text = LocalizeLookUp.GetText("rarity_rare");//"Rare";
        else
            m_Tier.text = LocalizeLookUp.GetText("rarity_exotic");// "Exotic";

        if (string.IsNullOrEmpty(spirit.herb) && string.IsNullOrEmpty(spirit.tool) && string.IsNullOrEmpty(spirit.gem))
        {
            m_Ingredients.text = LocalizeLookUp.GetText("lt_none");
        }
        else
        {
            m_Ingredients.text = "";

            if (string.IsNullOrEmpty(spirit.gem) == false)
                m_Ingredients.text += LocalizeLookUp.GetCollectableName(spirit.gem);

            if (string.IsNullOrEmpty(spirit.herb) == false)
                m_Ingredients.text += (string.IsNullOrEmpty(m_Ingredients.text) ? "" : ", ") + LocalizeLookUp.GetCollectableName(spirit.herb);

            if (string.IsNullOrEmpty(spirit.tool) == false)
                m_Ingredients.text += (string.IsNullOrEmpty(m_Ingredients.text) ? "" : ", ") + LocalizeLookUp.GetCollectableName(spirit.tool);
        }
        m_Required.text = LocalizeLookUp.GetText("pop_required_ingredients").Replace(" {{ingredient}}", ":");

        m_SpiritArt.overrideSprite = null;
        DownloadedAssets.GetSprite(spirit.id, spr =>
        {
            if (m_SpiritArt != null)
                m_SpiritArt.overrideSprite = spr;
        });
    }

    private void Close()
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_CanvasGroup.blocksRaycasts = false;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f).setEaseOutCubic().setOnComplete(() => gameObject.SetActive(false)).uniqueId;
        m_ScaleTweenId = LeanTween.scale(gameObject, Vector3.zero, 1f).setEaseOutCubic().uniqueId;
    }
}
