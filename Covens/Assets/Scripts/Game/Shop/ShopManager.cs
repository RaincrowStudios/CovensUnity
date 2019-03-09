using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using Newtonsoft.Json;

public class ShopManager : ShopBase
{

    [SerializeField] private GameObject maskContainer;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopContainer;
    [SerializeField] private RectTransform fortuna;
    [SerializeField] private Transform wheel;
    [SerializeField] private CanvasGroup darkenCG;
    [SerializeField] private CanvasGroup gearFilterContainer;
    [SerializeField] private Transform TitleContainer;
    [SerializeField] private GameObject ingredientCharmsPrefab;
    [SerializeField] private GameObject silverPrefab;
    [SerializeField] private GameObject cosmeticsPrefab;
    [SerializeField] private TextMeshProUGUI playerSilver;
    [SerializeField] private TextMeshProUGUI playerGold;

    [Header("BuyPopup")]
    [SerializeField] private GameObject buyObject;
    [SerializeField] private Image buyObjectIcon;
    [SerializeField] private TextMeshProUGUI buyObjectDesc;
    [SerializeField] private TextMeshProUGUI buyObjectTitle;
    [SerializeField] private TextMeshProUGUI buyObjectPrice;
    [SerializeField] private Button buyObjectCloseButton;
    [SerializeField] private Button buyObjectButton;

    [Header("BuySuccess")]
    [SerializeField] private GameObject buySuccessObject;
    [SerializeField] private Image buySuccessIcon;
    [SerializeField] private TextMeshProUGUI buySuccessTitle;
    [SerializeField] private TextMeshProUGUI buySuccessSubTitle;

    [Header("Style")]
    [SerializeField] private GameObject styleContainer;
    [SerializeField] private Transform styleNavContainer;
    [SerializeField] private GameObject navCircle;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI buyWithSilver;
    [SerializeField] private TextMeshProUGUI buyWithGold;
    [SerializeField] private Image styleIcon;


    [Header("Easing")]
    [SerializeField] private LeanTweenType easeTypeStore;
    [SerializeField] private float easeTimeStore;
    [SerializeField] private float easeWheelStoreOut;
    [SerializeField] private LeanTweenType easeTypeFortuna;
    [SerializeField] private float startAngle = -50;
    [SerializeField] private LeanTweenType easeTypeWheel;
    [SerializeField] private float easeTimeWheel;


    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button storeButton;
    [SerializeField] private Button ingredientButton;
    [SerializeField] private Button charmsButton;
    [SerializeField] private Button silverButton;
    [SerializeField] private Button gearButton;
    [SerializeField] private Button gearClothingFilter;
    [SerializeField] private Button gearAccessoriesFilter;
    [SerializeField] private Button gearSkinArtFilter;
    [SerializeField] private Button gearHairStylesFilter;

    private TextMeshProUGUI hairStylesText;
    private TextMeshProUGUI accessoriesText;
    private TextMeshProUGUI clothingText;
    private TextMeshProUGUI skinArtText;

    private CanvasGroup maskCG;
    private CanvasGroup storeCG;
    private CanvasGroup wheelCG;
    private CanvasGroup styleCG;
    private CanvasGroup buyObjectCG;

    private CanvasGroup title1CG;
    private CanvasGroup title2CG;
    private TextMeshProUGUI title1;
    private TextMeshProUGUI title2;
    private GearFilter currentFilter = GearFilter.clothing;

    private SwipeDetector SD;

    public static Action animationFinished;
    private int currentStyle;

    private enum GearFilter
    {
        clothing, hairstyles, accessories, skinart
    }

    void Start()
    {
        SD = GetComponent<SwipeDetector>();
        hairStylesText = gearHairStylesFilter.GetComponent<TextMeshProUGUI>();
        accessoriesText = gearAccessoriesFilter.GetComponent<TextMeshProUGUI>();
        clothingText = gearClothingFilter.GetComponent<TextMeshProUGUI>();
        skinArtText = gearSkinArtFilter.GetComponent<TextMeshProUGUI>();
        SD.enabled = false;
        storeCG = shopContainer.GetComponent<CanvasGroup>();
        styleCG = styleContainer.GetComponent<CanvasGroup>();
        wheelCG = wheel.GetComponent<CanvasGroup>();
        maskCG = maskContainer.GetComponent<CanvasGroup>();
        buyObjectCG = buyObject.GetComponent<CanvasGroup>();
        title1 = TitleContainer.GetChild(0).GetComponent<TextMeshProUGUI>();
        title2 = TitleContainer.GetChild(1).GetComponent<TextMeshProUGUI>();
        title1CG = title1.GetComponent<CanvasGroup>();
        title2CG = title2.GetComponent<CanvasGroup>();
        silverButton.onClick.AddListener(ShowSilver);
        gearButton.onClick.AddListener(ShowGear);
        ingredientButton.onClick.AddListener(ShowIngredient);
        charmsButton.onClick.AddListener(ShowCharms);
        closeButton.onClick.AddListener(Close);
        storeButton.onClick.AddListener(Open);
        buyObjectCloseButton.onClick.AddListener(CloseBuyPopup);
        SD.SwipeLeft = SwipeLeftStyle;
        SD.SwipeRight = SwipeRightStyle;
        currentFilter = GearFilter.clothing;


        foreach (var item in PlayerDataManager.StoreData.styles)
        {
            var g = Utilities.InstantiateObject(navCircle, styleNavContainer);
        }

        title1.GetComponent<Button>().onClick.AddListener(() =>
        {
            gearClothAction();
            maskContainer.SetActive(true);
        });


        title2.GetComponent<Button>().onClick.AddListener(ShowStyles);
        gearClothingFilter.onClick.AddListener(gearClothAction);

        gearAccessoriesFilter.onClick.AddListener(() =>
        {
            currentFilter = GearFilter.accessories;
            SpawnCosmetics();
            animationFinished();
            accessoriesText.color = Color.white;
            accessoriesText.fontStyle = FontStyles.Underline;
        });

        gearSkinArtFilter.onClick.AddListener(() =>
        {
            currentFilter = GearFilter.skinart;
            SpawnCosmetics();
            animationFinished();
            skinArtText.color = Color.white;
            skinArtText.fontStyle = FontStyles.Underline;
        });

        gearHairStylesFilter.onClick.AddListener(() =>
        {
            currentFilter = GearFilter.hairstyles;
            SpawnCosmetics();
            animationFinished();
            hairStylesText.color = Color.white;
            hairStylesText.fontStyle = FontStyles.Underline;
        });
    }

    private void gearClothAction()
    {
        gearFilterContainer.gameObject.SetActive(true);
        styleContainer.SetActive(false);
        title1.color = Color.white;
        title2.color = Color.grey;
        currentFilter = GearFilter.clothing;
        SpawnCosmetics();
        animationFinished();
        clothingText.color = Color.white;
        clothingText.fontStyle = FontStyles.Underline;
    }

    #region MainStoreUI


    public void Open()
    {
        playerSilver.text = PlayerDataManager.playerData.silver.ToString();
        playerGold.text = PlayerDataManager.playerData.gold.ToString();
        styleCG.alpha = 0;
        maskCG.alpha = 0;
        storeCG.alpha = 0;
        shopContainer.SetActive(true);
        shopContainer.transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(storeCG, 1, easeTimeStore);
        LeanTween.scale(shopContainer, Vector3.one, easeTimeStore).setEase(easeTypeStore);
        ShowWheel();
    }

    private void Close()
    {
        LeanTween.alphaCanvas(storeCG, 0, easeTimeStore);
        LeanTween.scale(shopContainer, Vector3.zero, easeTimeStore).setEase(easeTypeStore).setOnComplete(() => { shopContainer.SetActive(false); });
    }

    private void ShowWheel()
    {
        wheel.gameObject.SetActive(true);
        title1.gameObject.SetActive(false);
        title2.gameObject.SetActive(false);
        title1CG.alpha = title2CG.alpha = 0;
        LeanTween.alphaCanvas(maskCG, 0, easeWheelStoreOut);
        LeanTween.scale(maskCG.gameObject, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel);
        LeanTween.alphaCanvas(darkenCG, 0, easeTimeWheel);
        wheelCG.alpha = 0;
        LeanTween.alphaCanvas(wheelCG, 1, easeTimeWheel);
        wheel.localScale = Vector3.one * .7f;
        LeanTween.scale(wheel.gameObject, Vector3.one, easeTimeWheel).setEase(easeTypeWheel);
        LeanTween.rotateAround(wheel.gameObject, Vector3.forward, 0, easeTimeWheel).setEase(easeTypeWheel);
        foreach (Transform item in wheel.transform)
        {
            LeanTween.value(-startAngle, 0, easeTimeWheel).setEase(easeTypeWheel).setOnUpdate((float v) =>
                {
                    item.localEulerAngles = new Vector3(0, 0, v);
                });
        }
        LeanTween.value(484, -282, easeTimeWheel).setEase(easeTypeFortuna).setOnUpdate((float v) =>
        {
            fortuna.anchoredPosition = new Vector2(v, fortuna.anchoredPosition.y);
        });
    }

    private void HideWheel()
    {
        maskCG.alpha = 0;
        maskContainer.gameObject.SetActive(true);
        maskCG.transform.localScale = Vector3.one * .7f;
        LeanTween.scale(maskCG.gameObject, Vector3.one, easeWheelStoreOut).setEase(easeTypeWheel);
        LeanTween.alphaCanvas(styleCG, 1, easeWheelStoreOut).setOnComplete(() => styleContainer.SetActive(false));
        LeanTween.alphaCanvas(maskCG, 1, easeWheelStoreOut);
        LeanTween.alphaCanvas(darkenCG, 1, easeWheelStoreOut);
        LeanTween.alphaCanvas(wheelCG, 0, easeWheelStoreOut);
        LeanTween.scale(wheel.gameObject, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel).setOnComplete(() => wheel.gameObject.SetActive(false));

        LeanTween.value(-282, 484, easeTimeWheel).setEase(easeTypeFortuna).setOnUpdate((float v) =>
        {
            fortuna.anchoredPosition = new Vector2(v, fortuna.anchoredPosition.y);
        }).setOnComplete(() => { animationFinished(); });
        SD.enabled = false;
    }

    #endregion

    #region Utils

    private void ResetGearButtons()
    {
        hairStylesText.fontStyle = accessoriesText.fontStyle = skinArtText.fontStyle = clothingText.fontStyle = FontStyles.Normal;
        hairStylesText.color = accessoriesText.color = skinArtText.color = clothingText.color = Color.grey;
    }

    private void SetCloseAction()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);
    }

    private void SetCloseAction(UnityAction buttonAction)
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(buttonAction);
    }

    private void SetTitle(TextMeshProUGUI t, string title, CanvasGroup cg)
    {
        t.gameObject.SetActive(true);
        t.text = title;
        LeanTween.alphaCanvas(cg, 1, easeWheelStoreOut);
    }

    private void SpawnCosmetics()
    {
        ResetGearButtons();
        ClearContainer();
        foreach (var item in PlayerDataManager.StoreData.cosmetics)
        {
            if (item.storeCatagory == currentFilter.ToString())
            {
                GameObject g = Utilities.InstantiateObject(cosmeticsPrefab, itemContainer);
                g.GetComponent<ShopItem>().SetupCosmetics(item, OnClickCosmetic);
            }
        }

    }

    private void ResetNavButtons()
    {
        foreach (Transform item in styleNavContainer)
        {
            item.GetComponent<Image>().color = new Color(1, 1, 1, .3f);
        }
    }
    #endregion

    #region Buttons UI
    private void ShowIngredient()
    {
        SetCloseAction(HideIngredient);
        SetTitle(title1, "Ingredients", title1CG);
        ClearContainer();
        HideWheel();

        foreach (var item in PlayerDataManager.StoreData.bundles)
        {
            GameObject g = Utilities.InstantiateObject(ingredientCharmsPrefab, itemContainer);
            g.GetComponent<ShopItem>().SetupIngredientCharm(item, OnClickItem);
        }
    }

    private void HideIngredient()
    {
        ShowWheel();

        SetCloseAction();
    }

    private void ClearContainer()
    {
        foreach (Transform item in itemContainer)
        {
            Destroy(item.gameObject);
        }
    }
    private void ShowSilver()
    {
        SetTitle(title1, "Silver", title1CG);
        ClearContainer();
        HideWheel();

        foreach (var item in PlayerDataManager.StoreData.silver)
        {
            GameObject g = Utilities.InstantiateObject(silverPrefab, itemContainer);
            g.GetComponent<ShopItem>().SetupSilver(item, OnClickItem);
        }
        SetCloseAction(HideSilver);
    }
    private void HideSilver()
    {
        SetCloseAction();
        ShowWheel();
    }

    private void ShowCharms()
    {
        SetTitle(title1, "Charms", title1CG);
        SetCloseAction(HideCharms);
        HideWheel();

        ClearContainer();
        foreach (var item in PlayerDataManager.StoreData.consumables)
        {
            GameObject g = Utilities.InstantiateObject(ingredientCharmsPrefab, itemContainer);
            g.GetComponent<ShopItem>().SetupIngredientCharm(item, OnClickItem);
        }
    }
    private void HideCharms()
    {
        SetCloseAction();
        ShowWheel();
    }

    private void ShowGear()
    {
        title1.color = Color.white;
        title2.color = Color.grey;
        gearFilterContainer.alpha = 0;
        HideWheel();
        gearFilterContainer.gameObject.SetActive(true);
        LeanTween.alphaCanvas(gearFilterContainer, 1, easeWheelStoreOut);
        SetTitle(title1, "Cosmetics", title1CG);
        SetTitle(title2, "Styles", title2CG);
        SetCloseAction(HideGear);
        SpawnCosmetics();
        clothingText.color = Color.white;
        clothingText.fontStyle = FontStyles.Underline;
    }

    private void HideGear()
    {
        LeanTween.alphaCanvas(gearFilterContainer, 0, easeWheelStoreOut).setOnComplete(() => gearFilterContainer.gameObject.SetActive(false));
        SetCloseAction();
        ShowWheel();
    }

    private void ShowStyles()
    {
        title1.color = Color.grey;
        title2.color = Color.white;

        gearFilterContainer.gameObject.SetActive(false);
        SD.enabled = true;
        maskContainer.SetActive(false);
        ClearContainer();
        styleContainer.SetActive(true);
        styleCG.alpha = 1;
        SetStyle();
    }

    private void SetStyle()
    {

        var st = PlayerDataManager.StoreData.styles[currentStyle];
        title.text = DownloadedAssets.storeDict[st.id].title;
        desc.text = DownloadedAssets.storeDict[st.id].onBuyTitle;
        buyWithGold.text = "Buy with Gold: " + st.gold.ToString();
        buyWithSilver.text = "Buy with Silver: " + st.silver.ToString();
        ResetNavButtons();
        styleNavContainer.GetChild(currentStyle).GetComponent<Image>().color = Color.white;
        DownloadedAssets.GetSprite(st.iconId, styleIcon);
    }

    private void SwipeRightStyle()
    {
        if (currentStyle < PlayerDataManager.StoreData.styles.Count - 1)
            currentStyle++;
        else
            currentStyle = 0;
        SetStyle();

    }

    private void SwipeLeftStyle()
    {
        if (currentStyle > 0)
            currentStyle--;
        else
            currentStyle = PlayerDataManager.StoreData.styles.Count - 1;
        SetStyle();
    }


    #endregion

    #region purchase
    private void OnClickItem(ShopItemType type, StoreApiItem item)
    {
        if (type == ShopItemType.IngredientCharms)
        {
            buyObject.SetActive(true);
            buyObjectCG.alpha = 1;
            buyObject.transform.localScale = Vector3.one * .7f;
            LeanTween.alphaCanvas(buyObjectCG, 1, easeWheelStoreOut);
            LeanTween.scale(buyObject, Vector3.one, easeWheelStoreOut).setEase(easeTypeWheel);
            DownloadedAssets.GetSprite(item.id, buyObjectIcon, true);
            buyObjectTitle.text = DownloadedAssets.storeDict[item.id].title;
            buyObjectDesc.text = DownloadedAssets.storeDict[item.id].onBuyDescription;
            buyObjectPrice.text = item.silver.ToString();
            buyObjectButton.onClick.RemoveAllListeners();
            buyObjectButton.onClick.AddListener(() => OnBuy(item, type));
        }
    }

    private void CloseBuyPopup()
    {
        LeanTween.alphaCanvas(buyObjectCG, 0, easeWheelStoreOut).setOnComplete(() => buyObject.SetActive(false));
        LeanTween.scale(buyObject, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel);
    }

    private void OnClickCosmetic(ApparelData item)
    {

        buyObject.SetActive(true);
        buyObjectCG.alpha = 1;
        buyObject.transform.localScale = Vector3.one * .7f;
        LeanTween.alphaCanvas(buyObjectCG, 1, easeWheelStoreOut);
        LeanTween.scale(buyObject, Vector3.one, easeWheelStoreOut).setEase(easeTypeWheel);
        DownloadedAssets.GetSprite(item.id, buyObjectIcon, true);
        buyObjectTitle.text = DownloadedAssets.storeDict[item.id].title;
        buyObjectDesc.text = DownloadedAssets.storeDict[item.id].onBuyDescription;
        buyObjectPrice.text = item.silver.ToString();
        buyObjectButton.onClick.RemoveAllListeners();
        buyObjectButton.onClick.AddListener(() => OnBuy(item));
    }

    private void CloseCosmeticPopup()
    {
        LeanTween.alphaCanvas(buyObjectCG, 0, easeWheelStoreOut).setOnComplete(() => buyObject.SetActive(false));
        LeanTween.scale(buyObject, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel);
    }

    private void OnBuy(StoreApiItem item, ShopItemType type)
    {
        var js = new { purchase = item.id };

        APIManager.Instance.PostData("shop/purchase", JsonConvert.SerializeObject(js), (string s, int r) =>
        {
            if (r == 200)
            {
                CloseBuyPopup();
                buySuccessObject.SetActive(true);
                buySuccessTitle.text = DownloadedAssets.storeDict[item.id].title;
                buySuccessSubTitle.text = DownloadedAssets.storeDict[item.id].subtitle;
                DownloadedAssets.GetSprite(item.id, buySuccessIcon, true);
                if (type != ShopItemType.Silver)
                {
                    LeanTween.value(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - item.silver, 1f).setOnUpdate((float v) =>
                    {
                        playerSilver.text = ((int)v).ToString();
                    }).setOnComplete(() =>
                   {
                       PlayerDataManager.playerData.silver -= item.silver;
                       PlayerManagerUI.Instance.UpdateDrachs();
                   });
                }
                else
                {
                    LeanTween.value(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver + item.amount, 1f).setOnUpdate((float v) =>
                   {
                       playerSilver.text = ((int)v).ToString();
                   }).setOnComplete(() =>
                   {
                       PlayerDataManager.playerData.silver += item.amount;
                       PlayerManagerUI.Instance.UpdateDrachs();
                   });
                }
            }
        });
    }

    private void OnBuy(ApparelData item)
    {
        var js = new { purchase = item.id, currency = item.isBuySilver ? "silver" : "gold" };
        APIManager.Instance.PostData("shop/purchase", JsonConvert.SerializeObject(js), (string s, int r) =>
       {
           if (r == 200)
           {
               CloseBuyPopup();
               buySuccessObject.SetActive(true);
               buySuccessTitle.text = DownloadedAssets.storeDict[item.id].title;
               DownloadedAssets.GetSprite(item.iconId, buySuccessIcon, true);

               if (item.isBuySilver)
               {
                   LeanTween.value(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - item.silver, 1f).setOnUpdate((float v) =>
                   {
                       playerSilver.text = ((int)v).ToString();
                   }).setOnComplete(() =>
                   {
                       PlayerDataManager.playerData.silver -= item.silver;
                       PlayerManagerUI.Instance.UpdateDrachs();
                   });
               }
               else
               {
                   LeanTween.value(PlayerDataManager.playerData.gold, PlayerDataManager.playerData.gold - item.gold, 1f).setOnUpdate((float v) =>
                   {
                       playerGold.text = ((int)v).ToString();
                   }).setOnComplete(() =>
                   {
                       PlayerDataManager.playerData.gold -= item.gold;
                       PlayerManagerUI.Instance.UpdateDrachs();
                   }); ;
               }
           }
       });
    }

    #endregion
}
