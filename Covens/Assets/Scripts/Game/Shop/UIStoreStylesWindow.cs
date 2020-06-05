using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

[RequireComponent(typeof(SwipeDetector)), RequireComponent(typeof(CanvasGroup))]
public class UIStoreStylesWindow : MonoBehaviour
{
    [SerializeField] private ApparelView m_MaleApparel;
    [SerializeField] private ApparelView m_FemaleApparel;
    [SerializeField] private Image m_MaleStyle;
    [SerializeField] private Image m_FemaleStyle;

    [Space()]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private TextMeshProUGUI m_SilverText;
    [SerializeField] private TextMeshProUGUI m_GoldText;
    [SerializeField] private TextMeshProUGUI m_UnlockText;

    [Space()]
    [SerializeField] private Button m_SilverButton;
    [SerializeField] private Button m_GoldButton;

    [Space()]
    [SerializeField] private RectTransform m_IconContainer;
    [SerializeField] private Image m_IconPrefab;

    private List<StoreItem> m_Styles;
    private SwipeDetector m_SwipeDetector;
    private int m_CurrentIndex;
    private List<Image> m_BottomIcons;
    private ApparelView m_Apparel;
    private Image m_StyleArt;

    public CanvasGroup canvasGroup => this.GetComponent<CanvasGroup>();

    public RectTransform rectTransform => this.GetComponent<RectTransform>();

    public float alpha
    {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    private void Start()
    {
        LoadingOverlay.Show();

        alpha = 0;
        m_SwipeDetector = this.GetComponent<SwipeDetector>();
        m_SwipeDetector.SwipeLeft = OnSwipeLeft;
        m_SwipeDetector.SwipeRight = OnSwipeRight;

        m_Styles = new List<StoreItem>();
        char gender = PlayerDataManager.playerData.male ? 'm' : 'f';
        foreach (var style in StoreManagerAPI.StoreData.Styles)
        {
            CosmeticData cosmetic = DownloadedAssets.GetCosmetic(style.id);
            if (cosmetic.gender[0] == gender)
                m_Styles.Add(style);
        }

        m_BottomIcons = new List<Image>() { m_IconPrefab };
        for (int i = 0; i < m_Styles.Count - 1; i++)
            m_BottomIcons.Add(GameObject.Instantiate(m_IconPrefab, m_IconContainer));

        if (PlayerDataManager.playerData.male)
        {
            m_Apparel = m_MaleApparel;
            m_StyleArt = m_MaleStyle;
        }
        else
        {
            m_Apparel = m_FemaleApparel;
            m_StyleArt = m_FemaleStyle;
        }
        m_Apparel.gameObject.SetActive(true);

        m_CurrentIndex = 0;
        SetupStyle(m_Styles[0]);
        LoadingOverlay.Hide();
    }

    private void OnSwipeLeft()
    {
        if (m_CurrentIndex < m_Styles.Count - 1)
            m_CurrentIndex++;
        else
            m_CurrentIndex = 0;

        SetupStyle(m_Styles[m_CurrentIndex]);
    }

    private void OnSwipeRight()
    {
        if (m_CurrentIndex > 0)
            m_CurrentIndex--;
        else
            m_CurrentIndex = m_Styles.Count - 1;

        SetupStyle(m_Styles[m_CurrentIndex]);
    }

    private void SetupStyle(StoreItem item)
    {
        CosmeticData cosmetic = DownloadedAssets.GetCosmetic(item.id);

        for (int i = 0; i < m_BottomIcons.Count; i++)
        {
            if (i == m_CurrentIndex)
                m_BottomIcons[i].color = Color.white;
            else
                m_BottomIcons[i].color = Color.white * 0.35f;
        }

        m_SilverButton.onClick.RemoveAllListeners();
        m_GoldButton.onClick.RemoveAllListeners();

        if (cosmetic.assets.baseAsset.Count > 0)    cosmetic.apparelType = (ApparelType.Base);
        if (cosmetic.assets.shadow.Count > 0)       cosmetic.apparelType = (ApparelType.Shadow);
        if (cosmetic.assets.grey.Count > 0)         cosmetic.apparelType = (ApparelType.Grey);
        if (cosmetic.assets.white.Count > 0)        cosmetic.apparelType = (ApparelType.White);
        
        m_Apparel.InitCharacter(new List<EquippedApparel>());
        m_Apparel.EquipApparel(cosmetic);

        m_Title.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_Description.text = LocalizeLookUp.GetStorePurchaseTitle(item.id);

        //owned
        if (PlayerDataManager.playerData.inventory.cosmetics.Exists(cos => cos.id == item.id))
        {
            m_StyleArt.color = Color.white;
            m_SilverText.text = m_GoldText.text = "";
            m_UnlockText.text = LocalizeLookUp.GetText("store_gear_owned_upper");
        }
        //will unlock i nthe future
        else if (Utilities.TimespanFromJavaTime(item.unlockOn).TotalSeconds > 0)
        {
            m_StyleArt.color = Color.white * 0.15f;
            m_SilverText.text = m_GoldText.text = "";
            m_UnlockText.text = LocalizeLookUp.GetText(item.tooltip);
        }
        //available for purchase
        else
        {
            m_StyleArt.color = Color.white;
            m_SilverText.text = LocalizeLookUp.GetText("store_buy_silver") + ": " + item.silver;
            m_GoldText.text = LocalizeLookUp.GetText("store_buy_gold") + ": " + item.gold;
            m_UnlockText.text = "";

            m_SilverText.color = PlayerDataManager.playerData.silver >= item.silver ? Color.white * 0.8f : Color.red;
            m_GoldText.color = PlayerDataManager.playerData.gold >= item.gold ? new Color(1, 0.6f, 0) : Color.red;

            m_SilverButton.gameObject.SetActive(item.silver > 0);
            m_GoldButton.gameObject.SetActive(item.gold > 0);

            m_SilverButton.onClick.AddListener(() => Purchase(item, "silver"));
            m_GoldButton.onClick.AddListener(() => Purchase(item, "gold"));
        }
    }

    private void Purchase(StoreItem item, string currency)
    {
        if (currency == "silver" && PlayerDataManager.playerData.silver < item.silver)
        {
            UIGlobalPopup.ShowError(null, LocalizeLookUp.GetText("store_not_enough_silver"));
            return;
        }

        if (currency == "gold" && PlayerDataManager.playerData.gold < item.gold)
        {
            UIGlobalPopup.ShowError(null, LocalizeLookUp.GetText("store_not_enough_gold"));
            return;
        }

        CosmeticData cosmetic = DownloadedAssets.GetCosmetic(item.id);

        UIGlobalPopup.ShowPopUp(
            () =>
            {
                LoadingOverlay.Show();
                StoreManagerAPI.Purchase(
                    item.id,
                    StoreManagerAPI.TYPE_COSMETIC,
                    currency,
                    (error) =>
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            DownloadedAssets.GetSprite(cosmetic.iconId, spr =>
                            {
                                UIStorePurchaseSuccess.Show(m_Title.text, "", spr, () => SetupStyle(item));
                                LoadingOverlay.Hide();
                            },
                            true);
                        }
                        else
                        {
                            UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                            LoadingOverlay.Hide();
                        }
                    }
                );
            },
            () => { },
            m_Title.text + "\n" + LocalizeLookUp.GetText("store_confirm_upper")
        );
    }
}
