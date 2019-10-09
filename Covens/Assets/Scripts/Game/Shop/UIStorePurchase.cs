using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

public class UIStorePurchase : MonoBehaviour
{
    [SerializeField] protected Canvas m_Canvas;
    [SerializeField] protected GraphicRaycaster m_InputRaycaster;
    [SerializeField] protected CanvasGroup m_CanvasGroup;

    [Space()]
    [SerializeField] protected Image m_Icon;
    [SerializeField] protected TextMeshProUGUI m_Title;
    [SerializeField] protected TextMeshProUGUI m_Description;
    [SerializeField] protected Button m_ClaimButton;
    [SerializeField] protected Button m_CloseButton;
    [SerializeField] private TextMeshProUGUI m_LockedTooltip;

    [Space()]
    [SerializeField] protected GameObject m_SilverGameObject;
    [SerializeField] protected TextMeshProUGUI m_SilverAmount;
    [SerializeField] protected Button m_SilverButton;

    [Space()]
    [SerializeField] protected GameObject m_GoldGameObject;
    [SerializeField] protected TextMeshProUGUI m_GoldAmount;
    [SerializeField] protected Button m_GoldButton;

    [Space()]
    [SerializeField] protected GameObject m_OrGameObject;

    protected StoreItem m_Item;
    protected string m_ItemType;
    protected System.Action<string> m_OnPurchase;
    private int m_AlphaTweenId;
    private int m_ScaleTweenId;

    private static UIStorePurchase m_Instance;
    protected static UIStorePurchaseCosmetic m_InstanceCosmetic;

    public static void Show(StoreItem item, string type, string title, string description, Sprite icon, string locked, System.Action<string> onPurchase)
    {
        if (m_Instance == null)
            return;

        LoadingOverlay.Show();
        m_Instance._Show(item, type, title, description, icon, locked, onPurchase);
        LoadingOverlay.Hide();
    }

    public static void Show(StoreItem item, CosmeticData data, Sprite icon, string locked, System.Action<string> onPurchase)
    {
        if (m_InstanceCosmetic == null)
            return;

        LoadingOverlay.Show();
        m_InstanceCosmetic._Show(item, data, StoreManagerAPI.TYPE_COSMETIC, LocalizeLookUp.GetStoreTitle(item.id), "", icon, locked, onPurchase);
        LoadingOverlay.Hide();
    }

    public static void Close()
    {
        if (m_Instance != null)
            m_Instance._Close();
        if (m_InstanceCosmetic != null)
            m_InstanceCosmetic._Close();
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.transform.localScale = Vector3.zero;

        m_SilverButton.onClick.AddListener(OnClickBuySilver);
        m_GoldButton.onClick.AddListener(OnClickBuyGold);
        m_ClaimButton.onClick.AddListener(OnClickBuySilver);
        m_CloseButton.onClick.AddListener(_Close);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_AlphaTweenId);
        LeanTween.cancel(m_ScaleTweenId);
    }

    protected virtual void _Show(StoreItem item, string type, string title, string description, Sprite icon, string locked, System.Action<string> onPurchase)
    {
        m_Item = item;
        m_ItemType = type;
        m_Title.text = title;
        m_Description.text = description;
        m_Icon.overrideSprite = icon;

        bool hasSilver = PlayerDataManager.playerData.silver >= item.silver;
        bool hasGold = PlayerDataManager.playerData.gold >= item.gold;

        if (string.IsNullOrEmpty(locked))
        {
            m_LockedTooltip.text = "";
            m_SilverAmount.text = item.silver.ToString();
            m_GoldAmount.text = item.gold.ToString();

            m_SilverAmount.color = m_SilverButton.GetComponent<TextMeshProUGUI>().color = hasSilver ? Color.white : Color.red;
            m_GoldAmount.color = m_GoldButton.GetComponent<TextMeshProUGUI>().color = hasGold ? Color.white : Color.red;

            m_ClaimButton.gameObject.SetActive(item.gold == 0 && item.silver == 0);
            m_OrGameObject.SetActive(item.gold > 0 && item.silver > 0);
            m_SilverGameObject.SetActive(item.silver > 0);
            m_GoldGameObject.SetActive(item.gold > 0);
            m_OnPurchase = onPurchase;
        }
        else
        {
            m_ClaimButton.gameObject.SetActive(false);
            m_OrGameObject.SetActive(false);
            m_SilverGameObject.SetActive(false);
            m_GoldGameObject.SetActive(false);
            m_LockedTooltip.text = locked;
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_AlphaTweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f).setEaseOutCubic().uniqueId;
        m_ScaleTweenId = LeanTween.scale(m_CanvasGroup.gameObject, Vector3.one, 0.5f).setEaseOutCubic().uniqueId;
    }

    protected void _Close()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_AlphaTweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 0.5f).setOnComplete(() => m_Canvas.enabled = false).setEaseOutCubic().uniqueId;
        m_ScaleTweenId = LeanTween.scale(m_CanvasGroup.gameObject, Vector3.zero, 1f).setEaseOutCubic().uniqueId;
    }

    protected void OnClickBuySilver()
    {
        bool hasSilver = PlayerDataManager.playerData.silver >= m_Item.silver;
        if (hasSilver == false)
            return;

        LoadingOverlay.Show();
        StoreManagerAPI.Purchase(
            m_Item.id,
            m_ItemType,
            "silver",
            (error) =>
            {
                m_OnPurchase?.Invoke(error);
                LoadingOverlay.Hide();
            }
        );
    }

    protected void OnClickBuyGold()
    {
        bool hasGold = PlayerDataManager.playerData.gold >= m_Item.gold;
        if (hasGold == false)
            return;

        LoadingOverlay.Show();
        StoreManagerAPI.Purchase(
            m_Item.id,
            m_ItemType,
            "gold",
            (error) =>
            {
                m_OnPurchase?.Invoke(error);
                LoadingOverlay.Hide();
            }
        );
    }
}
