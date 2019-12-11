using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILootRewardItem : MonoBehaviour
{
    [SerializeField] private Image m_Art;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private RectTransform m_TitleGlow;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [Space]
    [SerializeField] private Sprite m_GoldSprite;
    [SerializeField] private Sprite m_SilverSprite;
    [SerializeField] private Sprite m_XPSprite;
    [SerializeField] private Sprite m_IngredientSprite;

    public void SetupGold(int amount)
    {
        m_Art.overrideSprite = m_GoldSprite;
        m_Title.text = LocalizeLookUp.GetText("amount_gold_drachs").Replace("{{Number}}", amount.ToString());

        Setup();
    }

    public void SetupSilver(int amount)
    {
        m_Art.overrideSprite = m_SilverSprite;
        m_Title.text = LocalizeLookUp.GetText("store_bought_drachs_upper").Replace("{{Number}}", amount.ToString());

        Setup();
    }

    public void SetupExp(ulong amount)
    {
        m_Art.overrideSprite = m_XPSprite;
        m_Title.text = LocalizeLookUp.GetText("spell_xp").Replace("{{Number}}", amount.ToString());

        Setup();
    }

    public void SetupIngredients(int amount)
    {
        m_Art.overrideSprite = m_IngredientSprite;
        m_Title.text = LocalizeLookUp.GetText("amount_ingredients").Replace("{{Number}}", amount.ToString());

        Setup();
    }

    public void SetupCosmetic(string id)
    {
        CosmeticData data = DownloadedAssets.GetCosmetic(id);
        DownloadedAssets.GetSprite(data.iconId, m_Art, true);
        m_Title.text = LocalizeLookUp.GetStoreTitle(id);

        Setup();
    }

    private void Setup()
    {
        transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        LeanTween.scale(this.gameObject, Vector3.one, 0.5f).setEaseOutCubic();
        m_TitleGlow.localScale = new Vector3(4, 3, 2);
        LeanTween.scale(m_TitleGlow.gameObject, new Vector3(2, 2, 2), 1f).setEaseOutCubic();
        m_CanvasGroup.alpha = 0;
        LeanTween.alphaCanvas(m_CanvasGroup, 1, 1).setEaseOutCubic();
        gameObject.SetActive(true);
    }
}
