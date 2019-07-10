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
    public static ShopManager Instance { get; set; }
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


    [Header("BuyPopupCosmetic")]
    [SerializeField] private GameObject buyObjectCosmetic;
    [SerializeField] private Image buyObjectCosmeticIcon;
    [SerializeField] private TextMeshProUGUI buyObjectCosmeticTitle;
    [SerializeField] private TextMeshProUGUI buySilverCosmeticPrice;
    [SerializeField] private TextMeshProUGUI buyGoldCosmeticPrice;
    [SerializeField] private TextMeshProUGUI previewText;
    [SerializeField] private Button previewBtn;
    [SerializeField] private Button buyObjectCosmeticCloseButton;
    [SerializeField] private Button buyGoldCosmeticButton;
    [SerializeField] private Button buySilverCosmeticButton;
    [SerializeField] private ApparelView male;
    [SerializeField] private ApparelView female;
    [SerializeField] private GameObject cosmeticSilver;
    [SerializeField] private GameObject[] cosmeticGold;
    [SerializeField] private Button claimCosmetic;
    private ApparelView apparelView;

    [Header("BuySuccess")]
    [SerializeField] private GameObject buySuccessObject;
    [SerializeField] private Image buySuccessIcon;
    [SerializeField] private TextMeshProUGUI buySuccessTitle;
    [SerializeField] private TextMeshProUGUI buySuccessSubTitle;

    [Header("Item Locked")]
    [SerializeField] private GameObject locked;
    [SerializeField] private TextMeshProUGUI lockedName;
    [SerializeField] private TextMeshProUGUI lockedMsg;
    [SerializeField] private TextMeshProUGUI lockedDate;


    [Header("Style")]
    [SerializeField] private GameObject styleContainer;
    [SerializeField] private Transform styleNavContainer;
    [SerializeField] private GameObject navCircle;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private TextMeshProUGUI buyWithSilver;
    [SerializeField] private TextMeshProUGUI buyWithGold;
    [SerializeField] private TextMeshProUGUI styleUnlockOn;
    [SerializeField] private Image styleIcon;
    private Button buyWithSilverBtn;
    private Button buyWithGoldBtn;
    private bool isPreview = true;

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
    private CanvasGroup buyObjectCosmeticCG;

    private CanvasGroup title1CG;
    private CanvasGroup title2CG;
    private TextMeshProUGUI title1;
    private TextMeshProUGUI title2;
    private GearFilter currentFilter = GearFilter.clothing;

    [SerializeField] private Sprite bundle1;
    [SerializeField] private Sprite bundle2;
    [SerializeField] private Sprite bundle3;
    private SwipeDetector SD;

    public static Action animationFinished;
    private int currentStyle;

    private enum GearFilter
    {
        clothing, hairstyles, accessories, skinart
    }

    void Awake()
    {
        Instance = this;
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
        buyObjectCosmeticCG = buyObjectCosmetic.GetComponent<CanvasGroup>();
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
        buyObjectCosmeticCloseButton.onClick.AddListener(CloseCosmeticPopup);
        SD.SwipeLeft = SwipeLeftStyle;
        SD.SwipeRight = SwipeRightStyle;
        currentFilter = GearFilter.clothing;
        buyWithSilverBtn = buyWithSilver.GetComponent<Button>();
        buyWithGoldBtn = buyWithGold.GetComponent<Button>();

        DownloadedAssets.IconSprites["bundle_abondiasBest"] = bundle1;
        DownloadedAssets.IconSprites["bundle_sapphosChoice"] = bundle2;
        DownloadedAssets.IconSprites["bundle_hermeticCollection"] = bundle3;

        title1.GetComponent<Button>().onClick.AddListener(() =>
        {
            gearClothAction();
            maskContainer.SetActive(true);
        });


        title2.GetComponent<Button>().onClick.AddListener(ShowStyles);
        gearClothingFilter.onClick.AddListener(gearClothAction);

        gearAccessoriesFilter.onClick.AddListener(() =>
        {
            Vector2 containerPos = new Vector2(0, itemContainer.localPosition.y);
            itemContainer.localPosition = containerPos;
            currentFilter = GearFilter.accessories;
            SpawnCosmetics();
            animationFinished();
            accessoriesText.color = Color.white;
            accessoriesText.fontStyle = FontStyles.Underline;
        });

        gearSkinArtFilter.onClick.AddListener(() =>
        {
            Vector2 containerPos = new Vector2(0, itemContainer.localPosition.y);
            itemContainer.localPosition = containerPos;
            currentFilter = GearFilter.skinart;
            SpawnCosmetics();
            animationFinished();
            skinArtText.color = Color.white;
            skinArtText.fontStyle = FontStyles.Underline;
        });

        gearHairStylesFilter.onClick.AddListener(() =>
        {
            Vector2 containerPos = new Vector2(0, itemContainer.localPosition.y);
            itemContainer.localPosition = containerPos;
            currentFilter = GearFilter.hairstyles;
            SpawnCosmetics();
            animationFinished();
            hairStylesText.color = Color.white;
            hairStylesText.fontStyle = FontStyles.Underline;
        });
    }

    public void ShowLocked(string id, string date, string msg)
    {
        locked.SetActive(true);
        lockedName.text = id;
        lockedMsg.text = msg;
        lockedDate.gameObject.SetActive(date != "unknown");
        lockedDate.text = DownloadedAssets.localizedText["shop_condition_locked"] + " " + date;
        // lockedDate.text = "This item will be available for purchase on " + txt;
    }

    private void gearClothAction()
    {
        Vector2 containerPos = new Vector2(0, itemContainer.localPosition.y);
        itemContainer.localPosition = containerPos;
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
        UIStateManager.Instance.CallWindowChanged(false);

        SoundManagerOneShot.Instance.MenuSound();
        StoreManagerAPI.GetShopItems((string s, int r) =>
   {
       if (r == 200)
       {
           //    Debug.Log(s);
           PlayerDataManager.StoreData = JsonConvert.DeserializeObject<StoreApiObject>(s);
           foreach (var item in PlayerDataManager.StoreData.cosmetics)
           {
               Utilities.SetCatagoryApparel(item);
           }
       }
       else
       {
           Debug.LogError("Failed to get the store Object : " + s);
       }
   });

        if (styleNavContainer.childCount == 0)
        {
            foreach (var item in PlayerDataManager.StoreData.styles)
            {
                var g = Utilities.InstantiateObject(navCircle, styleNavContainer);
            }
        }

        if (PlayerDataManager.playerData.male)
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            apparelView = male;
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            apparelView = female;
        }
        gearFilterContainer.gameObject.SetActive(false);

        playerSilver.text = PlayerDataManager.playerData.silver.ToString();
        playerGold.text = PlayerDataManager.playerData.gold.ToString();
        styleCG.alpha = 0;
        maskCG.alpha = 0;
        storeCG.alpha = 0;
        shopContainer.SetActive(true);
        shopContainer.transform.localScale = Vector3.zero;
        LeanTween.alphaCanvas(storeCG, 1, easeTimeStore);
        LeanTween.scale(shopContainer, Vector3.one, easeTimeStore).setEase(easeTypeStore).setOnComplete(() => MapsAPI.Instance.HideMap(true));
        ShowWheel();
    }

    private void Close()
    {
        MapsAPI.Instance.HideMap(false);

        SoundManagerOneShot.Instance.MenuSound();
        LeanTween.alphaCanvas(storeCG, 0, easeTimeStore);
        LeanTween.scale(shopContainer, Vector3.zero, easeTimeStore).setEase(easeTypeStore)
            .setOnComplete(() => { shopContainer.SetActive(false); UIStateManager.Instance.CallWindowChanged(true); });
    }

    private void ShowWheel()
    {
        SoundManagerOneShot.Instance.MenuSound();
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

        }).setOnComplete(() =>
        {
            itemContainer.GetComponentInParent<RectTransform>().anchoredPosition = new Vector2(itemContainer.GetComponentInParent<RectTransform>().anchoredPosition.x, 0);
        });
    }

    private void HideWheel()
    {
        maskCG.alpha = 0;
        maskContainer.gameObject.SetActive(true);
        maskCG.transform.localScale = Vector3.one * .7f;
        LeanTween.scale(maskCG.gameObject, Vector3.one, easeWheelStoreOut).setEase(easeTypeWheel);
        LeanTween.alphaCanvas(maskCG, 1, easeWheelStoreOut);
        LeanTween.alphaCanvas(darkenCG, 1, easeWheelStoreOut);
        LeanTween.alphaCanvas(wheelCG, 0, easeWheelStoreOut);
        LeanTween.scale(wheel.gameObject, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel).setOnComplete(() => wheel.gameObject.SetActive(false));

        LeanTween.value(-282, 484, easeTimeWheel).setEase(easeTypeFortuna).setOnUpdate((float v) =>
        {
            fortuna.anchoredPosition = new Vector2(v, fortuna.anchoredPosition.y);
            //itemContainer.GetComponent<GridLayoutGroup>().padding = new RectOffset(60,0,0,0);

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
        SoundManagerOneShot.Instance.MenuSound();
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
            if (item.hidden) continue;
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
        //itemContainer.GetComponent<GridLayoutGroup>().padding = new RectOffset(60,0,285,0);

        SetCloseAction(HideIngredient);
        SetTitle(title1, LocalizeLookUp.GetText("store_ingredients"), title1CG);
        ClearContainer();
        HideWheel();

        foreach (var item in PlayerDataManager.StoreData.bundles)
        {
            GameObject g = Utilities.InstantiateObject(ingredientCharmsPrefab, itemContainer);
            g.GetComponent<ShopItem>().SetupIngredientCharm(item, OnClickItem);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject g = Utilities.InstantiateObject(ingredientCharmsPrefab, itemContainer);
            g.GetComponent<CanvasGroup>().alpha = 0;
        }
        itemContainer.GetComponentInParent<RectTransform>().anchoredPosition = new Vector2(itemContainer.GetComponentInParent<RectTransform>().anchoredPosition.x, -184);

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
    public void ShowSilver()
    {
        SetTitle(title1, LocalizeLookUp.GetText("store_silver"), title1CG);
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
        SetTitle(title1, LocalizeLookUp.GetText("store_charms"), title1CG);
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
        SetTitle(title1, LocalizeLookUp.GetText("store_cosmetics"), title1CG);
        SetTitle(title2, LocalizeLookUp.GetText("store_styles"), title2CG);
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
        styleContainer.transform.localScale = Vector3.one;
        maskContainer.SetActive(false);
        ClearContainer();
        styleContainer.SetActive(true);
        styleCG.alpha = 1;
        SetStyle();
    }

    private void SetStyle()
    {
        var st = PlayerDataManager.StoreData.styles[currentStyle];
        buyWithGoldBtn.onClick.RemoveAllListeners();
        buyWithSilverBtn.onClick.RemoveAllListeners();
        SetCloseAction(HideStyles);
        if (st.owned)
        {
            buyWithGoldBtn.gameObject.SetActive(false);
            buyWithSilver.text = LocalizeLookUp.GetText("store_gear_owned_upper");
            buyWithSilver.color = Color.white;
        }
        else
        {
            buyWithGoldBtn.gameObject.SetActive(true);
            buyWithGoldBtn.onClick.AddListener(() => { OnBuy(st, false); });
            buyWithSilverBtn.onClick.AddListener(() => { OnBuy(st, true); });
            buyWithSilver.color = st.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
            buyWithGold.color = st.gold > PlayerDataManager.playerData.gold ? Color.red : Utilities.Orange;
            buyWithGoldBtn.interactable = st.gold <= PlayerDataManager.playerData.gold;
            buyWithSilverBtn.interactable = st.silver <= PlayerDataManager.playerData.silver;
            buyWithGold.text = LocalizeLookUp.GetText("store_buy_gold") + ": " + st.gold.ToString();
            buyWithSilver.text = LocalizeLookUp.GetText("store_buy_silver") + ": " + st.silver.ToString();
        }

        title.text = LocalizeLookUp.GetStoreTitle(st.id);
        desc.text = LocalizeLookUp.GetStorePurchaseTitle(st.id);

        ResetNavButtons();
        styleNavContainer.GetChild(currentStyle).GetComponent<Image>().color = Color.white;
        styleUnlockOn.gameObject.SetActive(false);
        buyWithSilver.gameObject.SetActive(true);
        buyWithGold.gameObject.SetActive(true);
        if (st.unlockOn > 0 && (Utilities.GetTimeRemaining(st.unlockOn) != "" || Utilities.GetTimeRemaining(st.unlockOn) != "unknown"))
        {
            Debug.Log(st.iconId);
            Debug.Log(Utilities.GetTimeRemaining(st.unlockOn) + "||");
            DownloadedAssets.GetSprite(st.iconId.Replace("_M_", "_P_"), styleIcon);
            buyWithSilver.gameObject.SetActive(false);
            buyWithGold.gameObject.SetActive(false);
            styleUnlockOn.gameObject.SetActive(true);
            styleUnlockOn.text = "This style will be available on " + ShopItem.GetTimeStampDate(st.unlockOn);
        }
        else
        {
            DownloadedAssets.GetSprite(st.iconId, styleIcon);
        }
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

    private void HideStyles()
    {
        LeanTween.alphaCanvas(gearFilterContainer, 0, easeWheelStoreOut).setOnComplete(() => gearFilterContainer.gameObject.SetActive(false));
        LeanTween.scale(styleContainer, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel);
        LeanTween.alphaCanvas(styleCG, 0, easeWheelStoreOut).setOnComplete(() => styleContainer.SetActive(false));
        SetCloseAction();
        ShowWheel();
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
            // if (item.id.Contains("truesight"))
            //     buyObjectIcon.sprite = Resources.Load<Sprite>("consumable_truesight");
            // else
            DownloadedAssets.GetSprite(item.id, buyObjectIcon, true);
            buyObjectTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
            buyObjectDesc.text = LocalizeLookUp.GetStoreDesc(item.id);
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

    private void TogglePreview(ApparelData apData)
    {
        if (isPreview)
        {
            apparelView.InitializeChar(PlayerDataManager.playerData.equipped);
            if (apData.assets.baseAsset.Count > 0)
            {
                apData.apparelType = ApparelType.Base;
            }
            else if (apData.assets.white.Count > 0)
            {
                apData.apparelType = ApparelType.White;
            }
            else if (apData.assets.shadow.Count > 0)
            {
                apData.apparelType = ApparelType.Shadow;
            }
            else if (apData.assets.grey.Count > 0)
            {
                apData.apparelType = ApparelType.Grey;
            }
            apparelView.EquipApparel(apData);

        }
        else
        {
            apparelView.InitializeChar(PlayerDataManager.playerData.equipped);
        }
        isPreview = !isPreview;
    }

    private void OnClickCosmetic(ApparelData item, ShopItem buttonItem)
    {
        isPreview = true;
        TogglePreview(item);
        previewText.text = LocalizeLookUp.GetText("store_preview_on");
        previewBtn.onClick.RemoveAllListeners();
        previewBtn.onClick.AddListener(() =>
        {
            previewText.text = isPreview ? LocalizeLookUp.GetText("store_preview_on") : LocalizeLookUp.GetText("store_preview_off");
            TogglePreview(item);
        });
        buyObjectCosmetic.SetActive(true);
        buyObjectCosmeticCG.alpha = 1;
        buyObjectCosmetic.transform.localScale = Vector3.one * .7f;
        LeanTween.alphaCanvas(buyObjectCosmeticCG, 1, easeWheelStoreOut);
        LeanTween.scale(buyObjectCosmetic, Vector3.one, easeWheelStoreOut).setEase(easeTypeWheel);
        DownloadedAssets.GetSprite(item.iconId, buyObjectCosmeticIcon, true);
        buyObjectCosmeticTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        buyGoldCosmeticButton.onClick.RemoveAllListeners();
        buySilverCosmeticButton.onClick.RemoveAllListeners();

        buySilverCosmeticPrice.text = item.silver.ToString();
        buyGoldCosmeticPrice.text = item.gold.ToString();
        buyGoldCosmeticButton.onClick.AddListener(() => OnBuy(item, false, buttonItem));
        buySilverCosmeticButton.onClick.AddListener(() => OnBuy(item, true, buttonItem));
        buySilverCosmeticPrice.color = item.silver > PlayerDataManager.playerData.silver ? Color.red : Color.white;
        buyGoldCosmeticPrice.color = item.gold > PlayerDataManager.playerData.gold ? Color.red : Color.white;
        buyGoldCosmeticButton.interactable = item.gold <= PlayerDataManager.playerData.gold;
        buySilverCosmeticButton.interactable = item.silver <= PlayerDataManager.playerData.silver;

        foreach (var g in cosmeticGold)
        {
            g.SetActive(true);
        }
        cosmeticSilver.SetActive(true);
        claimCosmetic.gameObject.SetActive(false);

        if (item.gold == 0)
        {
            if (item.silver == 0)
            {
                claimCosmetic.onClick.RemoveAllListeners();
                cosmeticSilver.SetActive(false);
                foreach (var g in cosmeticGold)
                {
                    g.SetActive(false);
                }
                claimCosmetic.gameObject.SetActive(true);
                claimCosmetic.onClick.AddListener(() => OnBuy(item, true, buttonItem));

            }
            else
            {
                foreach (var g in cosmeticGold)
                {
                    g.SetActive(false);
                }
            }
        }



    }

    private void CloseCosmeticPopup()
    {
        LeanTween.alphaCanvas(buyObjectCosmeticCG, 0, easeWheelStoreOut).setOnComplete(() => buyObjectCosmetic.SetActive(false));
        LeanTween.scale(buyObjectCosmetic, Vector3.one * .7f, easeWheelStoreOut).setEase(easeTypeWheel);
    }

    private void OnBuy(StoreApiItem item, ShopItemType type)
    {
        var js = new { purchase = item.id };
        Debug.Log(item.silver);
        Debug.Log(PlayerDataManager.playerData.silver);
        APIManager.Instance.PostData("shop/purchase", JsonConvert.SerializeObject(js), (string s, int r) =>
        {
            if (r == 200)
            {
                SoundManagerOneShot.Instance.PlayReward();
                CloseBuyPopup();
                buySuccessObject.SetActive(true);
                buySuccessTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
                buySuccessSubTitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
                DownloadedAssets.GetSprite(item.id, buySuccessIcon, true);

                if (type != ShopItemType.Silver)
                {
                    int cur = PlayerDataManager.playerData.silver;
                    int dif = cur - item.silver;
                    LeanTween.value(cur, dif, 1f).setOnUpdate((float v) =>
                    {
                        playerSilver.text = ((int)v).ToString();

                    }).setOnComplete(() =>
                   {
                       Debug.Log(item.silver);
                       Debug.Log(PlayerDataManager.playerData.silver);
                       PlayerDataManager.playerData.silver = dif;
                       PlayerManagerUI.Instance.UpdateDrachs(false);
                       playerSilver.text = PlayerDataManager.playerData.silver.ToString();

                   });

                    Debug.LogError("TODO: GET CHAR AFTER PURCHASE???");
                    //APIManager.Instance.GetData("character/get", (string res, int resp) =>
                    //{
                    //    if (resp == 200)
                    //    {
                    //        var rawData = JsonConvert.DeserializeObject<PlayerDataDetail>(res);
                    //        PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                    //    }
                    //});
                }

            }
        });
    }

    public void OnBuy()
    {
        Debug.Log(PlayerDataManager.playerData.silver);
        Debug.Log("buy silver");

        var item = IAPSilver.selectedSilverPackage;
        CloseBuyPopup();
        buySuccessObject.SetActive(true);
        buySuccessTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        buySuccessSubTitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
        DownloadedAssets.GetSprite(item.id, buySuccessIcon, true);
        LeanTween.value(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver + item.amount, 1f).setOnUpdate((float v) =>
                         {
                             playerSilver.text = ((int)v).ToString();
                         }).setOnComplete(() =>
                         {
                             PlayerDataManager.playerData.silver += item.amount;
                             PlayerManagerUI.Instance.UpdateDrachs();
                         });
    }

    private void OnBuy(ApparelData item, bool isBuySilver, ShopItem buttonItem = null)
    {
        var js = new { purchase = item.id, currency = isBuySilver ? "silver" : "gold" };
        Debug.Log("buy apparel");
        Debug.Log(item.silver);
        Debug.Log(PlayerDataManager.playerData.silver);
        APIManager.Instance.PostData("shop/purchase", JsonConvert.SerializeObject(js), (string s, int r) =>
       {
           Debug.Log(s);
           if (r == 200)
           {
               SoundManagerOneShot.Instance.PlayReward();
               CloseCosmeticPopup();
               buySuccessObject.SetActive(true);
               buySuccessTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
               buySuccessSubTitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
               DownloadedAssets.GetSprite(item.iconId, buySuccessIcon, true);
               PlayerDataManager.playerData.inventory.cosmetics.Add(item);
               item.owned = true;
               if (buttonItem != null)
               {
                   buttonItem.SetBought();
               }
               else
               {
                   buyWithGoldBtn.gameObject.SetActive(false);
                   buyWithSilver.text = LocalizeLookUp.GetText("store_gear_owned_upper");
                   buyWithSilver.color = Color.white;
               }
               if (isBuySilver)
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
