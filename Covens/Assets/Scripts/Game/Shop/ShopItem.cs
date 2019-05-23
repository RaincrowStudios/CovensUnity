using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private string iconID;

    public void SetBought()
    {
		buy.text = LocalizeLookUp.GetText ("store_gear_owned_upper");// "OWNED";
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
        StoreDictData itemData = DownloadedAssets.GetStoreItem(item.id);
        if (itemData == null)
        {
            Destroy(this.gameObject);
            return;
        }
        // buyButton.interactable = false;
        iconCG.alpha = 0;
        title.text = itemData.title;
        iconID = item.id;
        ShopManager.animationFinished += SetSprite;
    }

    private void SetUp(ApparelData item)
    {
        StoreDictData itemData = DownloadedAssets.GetStoreItem(item.id);
        if (itemData == null)
        {
            Destroy(this.gameObject);
            return;
        }

        iconCG.alpha = 0;
        title.text = itemData.title;
        iconID = item.iconId;
        ShopManager.animationFinished += SetSprite;
    }
    public void SetupIngredientCharm(StoreApiItem item, System.Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        StoreDictData itemData = DownloadedAssets.GetStoreItem(item.id);
        if (itemData == null)
        {
            Destroy(this.gameObject);
            return;
        }

        SetUp(item);
        subtitle.text = itemData.subtitle;
        cost.text = item.silver.ToString();
        cost.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        button.sprite = item.owned ? green : red;
        //   buy.text = item.owned ? "OWNED" : "BUY";
        buyButton.interactable = item.silver < PlayerDataManager.playerData.silver;
        buyButton.onClick.AddListener(() => { onClick(ShopBase.ShopItemType.IngredientCharms, item); });
    }

    public void SetupSilver(StoreApiItem item, System.Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        SetUp(item);

        UnityEngine.Purchasing.Product product = IAPSilver.instance.GetProduct(item.productId);
        cost.text = product.metadata.localizedPriceString;

        tagAmount.text = item.bonus.ToString();
        if (tagAmount.text == "")
            tagAmount.transform.parent.gameObject.SetActive(false);
        amount.text = item.amount.ToString();
        buyButton.onClick.AddListener(() => { IAPSilver.instance.BuyProductID(item); });

    }

    public void SetupCosmetics(ApparelData item, System.Action<ApparelData, ShopItem> onClick)
    {
        SetUp(item);
        silver.text = item.silver.ToString();
        gold.text = item.gold.ToString();
        silver.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        gold.color = item.gold > PlayerDataManager.playerData.gold ? Color.red : Color.white;
        //Debug.Log(item.gold < PlayerDataManager.playerData.gold || item.silver < PlayerDataManager.playerData.silver);
        buyButton.interactable = !item.owned;
        // buyButton.interactable = (item.gold < PlayerDataManager.playerData.gold || item.silver < PlayerDataManager.playerData.silver);
        if (item.position != "carryOnLeft" && item.position != "carryOnRight")
        {
			buy.text = item.owned ? LocalizeLookUp.GetText ("store_gear_owned_upper")/*"OWNED"*/ : LocalizeLookUp.GetText ("store_buy_upper");//"BUY";
            button.sprite = item.owned ? green : red;
            buyButton.onClick.AddListener(() => { onClick(item, this); });
        }
        else
        {
            button.sprite = red;
			buy.text = LocalizeLookUp.GetText ("store_gear_locked_upper");//"Locked";
        }
    }

    void OnDestroy()
    {
        ShopManager.animationFinished -= SetSprite;
    }
}