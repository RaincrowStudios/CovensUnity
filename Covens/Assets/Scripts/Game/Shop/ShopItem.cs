using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using Raincrow.Store;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] public TextMeshProUGUI buy;
    [SerializeField] private TextMeshProUGUI subtitle;
    [SerializeField] private TextMeshProUGUI silver;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI tagAmount;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private CanvasGroup iconCG;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image Icon;
    [SerializeField] private Image button;
    [SerializeField] private Sprite red;
    [SerializeField] private Sprite green;
    [SerializeField] private GameObject goldDrachs;
    [SerializeField] private GameObject silveDrachs;
    [SerializeField] private GameObject orText;
    private string iconID;

    public void SetBought()
    {
        StopAllCoroutines();
        buy.text = LocalizeLookUp.GetText("store_gear_owned_upper");// "OWNED";
        button.sprite = green;
        buyButton.onClick.RemoveAllListeners();
    }

    private void SetSprite()
    {
        // if (iconID.Contains("truesight"))
        //     Icon.sprite = Resources.Load<Sprite>("consumable_truesight");
        // else
        DownloadedAssets.GetSprite(iconID, Icon, true);
        LeanTween.alphaCanvas(iconCG, 1, .5f);
    }

    private void SetUp(StoreApiItem item)
    {
        iconCG.alpha = 0;
        title.text = LocalizeLookUp.GetStoreTitle(item.id);
        iconID = item.id;
        ShopManager.animationFinished += SetSprite;
    }

    private void SetUp(CosmeticData item)
    {
        if (item.owned)
            buyButton.interactable = false;

        iconCG.alpha = 0;
        title.text = LocalizeLookUp.GetStoreTitle(item.id);
        iconID = item.iconId;
        ShopManager.animationFinished += SetSprite;
    }

    public void SetupIngredientCharm(StoreApiItem item, System.Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        SetUp(item);
        subtitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
        cost.text = item.silver.ToString();
        cost.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        button.sprite = red;
        buy.text = item.silver > 0 ? LocalizeLookUp.GetText("store_buy_upper") : "CLAIM";

        buyButton.interactable = item.silver <= PlayerDataManager.playerData.silver;
        buyButton.onClick.AddListener(() => { onClick(ShopBase.ShopItemType.IngredientCharms, item); });
    }

    public void SetupSilver(StoreApiItem item, System.Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        UnityEngine.Purchasing.Product product = IAPSilver.instance.GetProduct(item.productId);


        SetUp(item);

        if (product == null)
        {
            Debug.LogError("product not found for \"" + item.productId + "\"");
            cost.text = "$??.??";
        }
        else
        {
            cost.text = product.metadata.localizedPriceString;
        }

        tagAmount.text = item.bonus.ToString();
        if (tagAmount.text == "")
            tagAmount.transform.parent.gameObject.SetActive(false);
        amount.text = item.amount.ToString();

        buyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            IAPSilver.instance.BuyProductID(item, (error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    ShopManager.OnBuySilver(item);
                }
                else if (string.IsNullOrEmpty(error) == false)
                {
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                }
            });
        });

    }

    public void SetupCosmetics(CosmeticData item, System.Action<CosmeticData, ShopItem> onClick)
    {
        SetUp(item);
        silver.text = item.silver.ToString();
        gold.text = item.gold.ToString();
        silver.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        gold.color = item.gold > PlayerDataManager.playerData.gold ? Color.red : Color.white;
        //Debug.Log(item.gold < PlayerDataManager.playerData.gold || item.silver < PlayerDataManager.playerData.silver);
        // buyButton.interactable = !item.owned;
        // buyButton.interactable = (item.gold < PlayerDataManager.playerData.gold || item.silver < PlayerDataManager.playerData.silver);
        if (!item.locked)
        {
            buy.text = item.owned ? LocalizeLookUp.GetText("store_gear_owned_upper")/*"OWNED"*/ : LocalizeLookUp.GetText("store_buy_upper");//"BUY";
            button.sprite = item.owned ? green : red;
            buyButton.onClick.AddListener(() => { onClick(item, this); });
        }
        else
        {
            button.sprite = red;
            buy.text = LocalizeLookUp.GetText("store_gear_locked_upper");//"Locked";
            buyButton.onClick.AddListener(() => 
            {
                UIGlobalPopup.ShowPopUp(
                    null,
                    DownloadedAssets.localizedText["shop_condition_locked"] + " " + GetTimeStampDate(item.unlockOn).Replace("unknown", "") + "\n" + DownloadedAssets.localizedText[item.tooltip]
                );
                    //ShopManager.Instance.ShowLocked(LocalizeLookUp.GetStoreTitle(item.id), GetTimeStampDate(item.unlockOn), DownloadedAssets.localizedText[item.tooltip]);
            });

        }



        if (item.unlockOn > 0)
        {
            string s = GetTimeRemaining(item.unlockOn);
            if (s == "unknown" || s == "")
            {
                // buyButton.enabled = true;
            }
            else
            {
                buy.text = s;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    UIGlobalPopup.ShowPopUp(
                       null,
                       DownloadedAssets.localizedText["shop_condition_locked"] + " " + GetTimeStampDate(item.unlockOn).Replace("unknown", "") + "\n" + DownloadedAssets.localizedText[item.tooltip]
                    );
                    //ShopManager.Instance.ShowLocked(LocalizeLookUp.GetStoreTitle(item.id), GetTimeStampDate(item.unlockOn), DownloadedAssets.localizedText[item.tooltip]);
                });
            }
        }
        else
        {
            if (item.gold == 0)
            {
                orText.SetActive(false);

                if (item.silver != 0)
                {
                    goldDrachs.SetActive(false);
                }
                else
                {
                    if (!item.locked)
                    {
                        goldDrachs.SetActive(false);
                        silveDrachs.SetActive(false);
                        buy.text = "CLAIM";
                    }
                }
            }
        }
    }

    public string GetTimeRemaining(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        if (DateTime.Compare(dtDateTime, DateTime.UtcNow) > 0)
        {
            TimeSpan timeSpan = dtDateTime.Subtract(DateTime.UtcNow);
            return String.Format("{0:00}d:{1:00}h", timeSpan.TotalDays, timeSpan.Hours);
        }
        else
        {
            return "";
        }
    }

    public static string GetTimeStampDate(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
        if (DateTime.Compare(dtDateTime, DateTime.UtcNow) < 0)
        {
            return "";
        }
        return dtDateTime.ToString("d");
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        ShopManager.animationFinished -= SetSprite;
    }


}