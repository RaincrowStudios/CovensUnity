using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;
using Raincrow.Store;
using System.Globalization;
using System.Text.RegularExpressions;

public class UIBundlePopup : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private UIBundlePopupAnim m_Anim;

    [Space]
    [SerializeField] private Image m_PackArt;
    [SerializeField] private RectTransform m_RewardContainer;
    [SerializeField] private TextMeshProUGUI m_RewardPrefab;

    [Space]
    [SerializeField] private TextMeshProUGUI m_ExpireText;
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_PriceText;
    [SerializeField] private TextMeshProUGUI m_OldPriceText;

    [Space]
    [SerializeField] private Button m_BackgroundButton;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_BuyButton;

    private static UIBundlePopup m_Instance;
    public static bool IsOpen => m_Instance != null && m_Instance.m_InputRaycaster.enabled;
    private static System.Action m_OnPurchase;

    private int m_TweenId;
    private int m_TimerTweenId;

    public static void Open(string bundleId, System.Action onPurchase = null)
    {
        m_OnPurchase = onPurchase;

        if (m_Instance != null)
        {
            m_Instance._Show(bundleId);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.STORE_BUNDLE, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Show(bundleId);
            });
        }
    }

    private string m_BundleId;

    private void Awake()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        m_Instance = this;
        m_Canvas.enabled = false;
        m_Instance.enabled = false;

        m_BuyButton.onClick.AddListener(OnClickBuy);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_BackgroundButton.onClick.AddListener(OnClickClose);
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_TimerTweenId);
    }

    private void _Show(string bundleId)
    {
        if (m_Canvas.enabled)
            return;

        bool owned = PlayerDataManager.playerData.OwnedPacks.Contains(bundleId);
        m_BuyButton.interactable = !owned;

        BackButtonListener.AddCloseAction(OnClickClose);

        //destroy previous reward instances
        for(int i = 0; i < m_RewardContainer.childCount; i++)
            Destroy(m_RewardContainer.GetChild(i).gameObject);

        LeanTween.cancel(m_TweenId);
        m_BundleId = bundleId;

        LoadingOverlay.Show();
        DownloadedAssets.GetSprite(m_BundleId, spr =>
        {
            LoadingOverlay.Hide();

            PackData data = StoreManagerAPI.GetPackData(m_BundleId);
            char gender = PlayerDataManager.playerData.male ? 'm' : 'f';

            //title
            m_TitleText.text = LocalizeLookUp.GetStoreTitle(m_BundleId);

            //duration
            UpdateTimer(data.expiresOn);

            //content
            foreach (var item in data.content)
            {
                TextMeshProUGUI instance = Instantiate(m_RewardPrefab, m_RewardContainer);
                int amount = Mathf.Max(item.amount, 1);

                switch (item.type)
                {
                    //only show cosmetics matching the character's gender
                    case StoreManagerAPI.TYPE_COSMETIC:
                        CosmeticData cosmetic = DownloadedAssets.GetCosmetic(item.id);
                        if (cosmetic.gender[0] != gender)
                            instance.gameObject.SetActive(false);
                        else
                            instance.text = amount + "x " + LocalizeLookUp.GetStoreTitle(item.id);
                        break;

                    case StoreManagerAPI.TYPE_CURRENCY:
                        instance.text = amount + " " + LocalizeLookUp.GetText(item.id == "gold" ? "store_gold" : "store_silver");
                        break;

                    case "effect":
                        instance.text = LocalizeLookUp.GetStoreTitle(item.id);
                        break;

                    default:
                        instance.text = amount + "x " + LocalizeLookUp.GetStoreTitle(item.id);
                        break;
                }
            }

            m_OldPriceText.gameObject.SetActive(false);

            //cost
            if (owned)
            {
                m_PriceText.text = LocalizeLookUp.GetText("store_gear_owned_upper");
            }
            else if (data.isFree)
            {
                m_PriceText.text = LocalizeLookUp.GetText("ftf_claim");
            }
            else
            {
                string localizedPriceString = IAPSilver.GetLocalizedPrice(data.product);
                m_PriceText.text = localizedPriceString;

                //try
                //{
                //    m_OldPriceText.text = GetMultipliedPrice(localizedPriceString, data.fullPrice);
                //    m_OldPriceText.gameObject.SetActive(true);
                //}
                //catch(System.Exception e)
                //{
                //    Debug.LogException(new System.Exception("Failed to parse localized price " + localizedPriceString));
                    //m_OldPriceText.gameObject.SetActive(false);
                //}
            }

            m_Anim.Show();
            m_Canvas.enabled = true;
            m_InputRaycaster.enabled = true;
            m_PackArt.overrideSprite = spr;
        },
        true);
    }

    public void _Close()
    {
        if (m_InputRaycaster.enabled == false)
            return;

        BackButtonListener.RemoveCloseAction();

        m_InputRaycaster.enabled = false;
        m_Anim.Hide(() =>
        {
            m_Canvas.enabled = false;
            m_PackArt.overrideSprite = null;

            m_TweenId = LeanTween.value(0, 0, 0f).setDelay(20).setOnComplete(() =>
            {
                SceneManager.UnloadScene(SceneManager.Scene.STORE_BUNDLE, null, null);
            }).uniqueId;
        });
    }

    private void UpdateTimer(double timestamp)
    {
        System.TimeSpan expire = Utilities.TimespanFromJavaTime(timestamp);

        if(expire.TotalSeconds <= 0)
        {
            _Close();
            return;
        }

        if (expire.Days > 0)
            m_ExpireText.text = expire.Days + " " + LocalizeLookUp.GetText("lt_time_days") + ", " + expire.Hours + " " + LocalizeLookUp.GetText("lt_time_hours");
        else if (expire.Hours > 0)
            m_ExpireText.text = expire.Hours + " " + LocalizeLookUp.GetText("lt_time_hours") + ", " + expire.Minutes + " " + LocalizeLookUp.GetText("lt_time_minutes");
        else if (expire.Minutes > 0)
            m_ExpireText.text = expire.Minutes + " " + LocalizeLookUp.GetText("lt_time_minutes") + ", " + expire.Seconds + " " + LocalizeLookUp.GetText("lt_time_seconds");
        else if (expire.Seconds > 0)
            m_ExpireText.text = expire.Seconds + " " + LocalizeLookUp.GetText("lt_time_seconds");
        else
            m_ExpireText.text = LocalizeLookUp.GetText("lt_unkown");

        m_ExpireText.text = LocalizeLookUp.GetText("spirit_deck_expire").Replace("{{time}}", "<color=#E38E05>" + m_ExpireText.text + "</color>");

        m_TimerTweenId = LeanTween.value(0, 0, 0).setDelay(1).setOnComplete(() => UpdateTimer(timestamp)).uniqueId;
    }

    private void OnClickClose()
    {
        _Close();
    }

    private void OnClickBuy()
    {
        LoadingOverlay.Show();

        string bundleId = m_BundleId;

        System.Action<string> onPurchase = (error) =>
        {
            LoadingOverlay.Hide();
            if (string.IsNullOrEmpty(error))
            {
                UIStorePurchaseSuccess.Show(
                    LocalizeLookUp.GetStoreTitle(bundleId),
                    "",
                    m_PackArt.overrideSprite,
                    () => {
                        this._Close();
                        this._Show(bundleId);
                    });
            }
            else if (string.IsNullOrEmpty(error) == false)
            {
                if (error.StartsWith("PurchaseFailureReason"))
                {
                    UnityEngine.Purchasing.PurchaseFailureReason reason = (UnityEngine.Purchasing.PurchaseFailureReason)int.Parse(error.Replace("PurchaseFailureReason", ""));
                    if (reason != UnityEngine.Purchasing.PurchaseFailureReason.UserCancelled)
                        UIGlobalPopup.ShowError(null, reason.ToString());
                }
                else
                {
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                }
            }

            m_OnPurchase?.Invoke();
        };

        PackData data = StoreManagerAPI.GetPackData(m_BundleId);

        if (data.isFree)
        {
            StoreManagerAPI.Purchase(m_BundleId, StoreManagerAPI.TYPE_PACK, null, onPurchase);
        }
        else
        {
            IAPSilver.instance.BuyProductID(m_BundleId, onPurchase);
        }
    }

    //public static string GetMultipliedPrice(string localizedPrice, float multiplier)
    //{
    //    string currentPriceString = Regex.Match(localizedPrice, @"\d+[.,]+\d+").Value;
    //    float currentPriceFloat = float.Parse(currentPriceString.Replace(',','.'), NumberStyles.Any, CultureInfo.InstalledUICulture);
    //    float oldPriceFloat = currentPriceFloat * multiplier;
    //    return localizedPrice.Replace(currentPriceString, string.Format("{0:0.##}", oldPriceFloat));
    //}
}
