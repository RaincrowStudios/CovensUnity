using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private TextMeshProUGUI buy;
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



    private void SetSprite()
    {
        DownloadedAssets.GetSprite(iconID, Icon, true);
        LeanTween.alphaCanvas(iconCG, 1, .5f).setOnComplete(() =>
        {
            buyButton.interactable = true;
        });
    }

    private void SetUp(StoreApiItem item)
    {
        buyButton.interactable = false;
        iconCG.alpha = 0;
        title.text = DownloadedAssets.storeDict[item.id].title;
        iconID = item.id;
        ShopManager.animationFinished += SetSprite;
    }

    private void SetUp(ApparelData item)
    {
        iconCG.alpha = 0;
        title.text = DownloadedAssets.storeDict[item.id].title;
        iconID = item.iconId;
        ShopManager.animationFinished += SetSprite;
    }
    public void SetupIngredientCharm(StoreApiItem item, Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        SetUp(item);
        subtitle.text = DownloadedAssets.storeDict[item.id].subtitle;
        cost.text = item.silver.ToString();
        cost.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        button.sprite = item.owned ? green : red;
        buy.text = item.owned ? "OWNED" : "BUY";
        buyButton.interactable = item.silver < PlayerDataManager.playerData.silver;
        buyButton.onClick.AddListener(() => { onClick(ShopBase.ShopItemType.IngredientCharms, item); });
    }

    public void SetupSilver(StoreApiItem item, Action<ShopBase.ShopItemType, StoreApiItem> onClick)
    {
        SetUp(item);
        cost.text = item.cost.ToString();
        tagAmount.text = item.bonus.ToString();
        amount.text = item.amount.ToString();
        buyButton.onClick.AddListener(() => { onClick(ShopBase.ShopItemType.Silver, item); });

    }

    public void SetupCosmetics(ApparelData item, Action<ApparelData> onClick)
    {
        SetUp(item);
        silver.text = item.silver.ToString();
        gold.text = item.gold.ToString();
        silver.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        gold.color = item.gold > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        buyButton.interactable = (item.gold < PlayerDataManager.playerData.gold || item.silver < PlayerDataManager.playerData.silver);
        buy.text = item.owned ? "OWNED" : "BUY";
        buyButton.onClick.AddListener(() => { onClick(item); });
    }

    void OnDestroy()
    {
        ShopManager.animationFinished -= SetSprite;
    }
}