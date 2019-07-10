using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;


public class StoreUIManager : UIAnimationManager
{

    public static StoreUIManager Instance { get; set; }

    public GameObject loadingButton;
    public Animator storeAnim;

    public GameObject wheelContainer;
    public GameObject silverContainer;
    public GameObject elixirContainer;
    public GameObject gearContainer;
    public GearUIManager gearUIM;
    StoreApiObject storeItems;

    public StoreButtonData[] Energy;
    public StoreButtonData[] xp;
    public StoreButtonData[] align;
    public StoreButtonData[] bundle;
    public StoreButtonData[] Silver;

    //silver PurchaseA
    public GameObject selectSilver;
    public Text selectSilverTitle;
    public Text selectSilverDesc;
    public Image selectSilverImg;
    public Text selectSilverAmount;
    public GameObject selectBuy;
    public GameObject selectBuySilver;
    public GameObject SelectNoSilver;
    public Text silverDrachs;
    public Text goldDrachs;

    public GameObject purchaseSuccess;
    public Text purchaseSuccessTitle;
    public Text purchaseAmount;
    public Image purchaseSuccessDisplayImage;

    public static StoreApiItem SelectedStoreItem;

    public GameObject rightArrowElixir;
    public GameObject leftArrowElixir;
    public GameObject page1Elixir;
    public GameObject page2Elixir;


    void Awake()
    {
        Instance = this;
    }

    public void GetStore()
    {
        goldDrachs.text = PlayerDataManager.playerData.gold.ToString();
        UIStateManager.Instance.CallWindowChanged(false);
        StoreManagerAPI.GetShopItems(Callback);
        loadingButton.SetActive(true);
    }

    public void Callback(string result, int code)
    {

        if (code == 200)
        {
            storeItems = JsonConvert.DeserializeObject<StoreApiObject>(result);
            HandleResult();
            InitStore();
        }
        else
        {
            UIStateManager.Instance.CallWindowChanged(true);
            Debug.LogError(code + " | Couldnt Get Store Data : " + result);
        }
        loadingButton.SetActive(false);

    }

    void HandleResult()
    {
        foreach (var item in storeItems.silver)
        {
            item.type = "silver";
            int silverIndex = (int.Parse((item.id[6]).ToString())) - 1; //silver1
            Silver[silverIndex].Setup(item);
        }



        foreach (var item in storeItems.bundles)
        {
            item.type = "bundle";
            if (item.id == "bundle_abondiasBest")
            {
                bundle[0].Setup(item);
            }
            if (item.id == "bundle_sapphosChoice")
            {
                bundle[1].Setup(item);
            }
            if (item.id == "bundle_hermeticCollection")
            {
                bundle[2].Setup(item);
            }
        }

        foreach (var item in storeItems.consumables)
        {
            if (item.id.Contains("energy"))
            {
                item.type = "energy";
                if (item.id.Contains("1"))
                {
                    Energy[0].Setup(item);
                }
                else if (item.id.Contains("3"))
                {
                    Energy[1].Setup(item);
                }
                else if (item.id.Contains("5"))
                {
                    Energy[2].Setup(item);
                }
            }
            else if (item.id.Contains("xpBooster"))
            {
                item.type = "xp";
                if (item.id.Contains("Smaller"))
                {
                    xp[0].Setup(item);
                }
                else if (item.id.Contains("Medium"))
                {
                    xp[1].Setup(item);
                }
                else if (item.id.Contains("Greater"))
                {
                    xp[2].Setup(item);
                }
            }
            else if (item.id.Contains("alignmentBooster"))
            {
                item.type = "align";
                if (item.id.Contains("Smaller"))
                {
                    align[0].Setup(item);
                }
                else if (item.id.Contains("Medium"))
                {
                    align[1].Setup(item);
                }
                else if (item.id.Contains("Greater"))
                {
                    align[2].Setup(item);
                }
            }
            else if (item.type == "cosmetics")
            {

            }
        }
    }

    void InitStore()
    {
        SoundManagerOneShot.Instance.MenuSound();

        this.CancelInvoke();
        storeAnim.gameObject.SetActive(true);
        storeAnim.Play("enter");
        Show(wheelContainer);
    }

    public void Exit()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        storeAnim.Play("exit");
        Invoke("DisableDelay", .9f);
        SoundManagerOneShot.Instance.MenuSound();

    }

    void DisableDelay()
    {
        storeAnim.gameObject.SetActive(false);
    }

    public void ShowSilver(bool isShow)
    {
        SoundManagerOneShot.Instance.MenuSound();
        if (isShow)
        {
            Hide(wheelContainer);
            Show(silverContainer);
        }
        else
        {
            Show(wheelContainer);
            Hide(silverContainer);
        }
    }

    public void ShowGear(bool isShow)
    {
        SoundManagerOneShot.Instance.MenuSound();
        if (isShow)
        {
            Hide(wheelContainer);
            Show(gearContainer);
            gearUIM.Init(storeItems.cosmetics);
        }
        else
        {
            Show(wheelContainer);
            Hide(gearContainer);
        }
    }

    public void ShowElixir(bool isShow)
    {
        SoundManagerOneShot.Instance.MenuSound();
        if (isShow)
        {
            Hide(wheelContainer);
            Show(elixirContainer);
        }
        else
        {
            Show(wheelContainer);
            Hide(elixirContainer);
        }
    }

    public void InitiatePurchase(StoreApiItem data, Sprite sp)
    {
        Show(selectSilver);
        if (PlayerDataManager.playerData.silver > data.silver)
        {
            selectBuy.SetActive(true);
            selectBuySilver.SetActive(false);
            SelectNoSilver.SetActive(false);
            silverDrachs.color = Color.white;
            selectSilverTitle.text = "Buy <color=ffffff>" + LocalizeLookUp.GetStoreTitle(data.id) + "</color>";
            selectSilverDesc.text = LocalizeLookUp.GetStoreDesc(data.id);
            selectSilverImg.sprite = sp;
            selectSilverAmount.text = data.silver.ToString();
        }
        else
        {
            selectBuy.SetActive(false);
            selectBuySilver.SetActive(true);
            SelectNoSilver.SetActive(true);
            silverDrachs.color = Color.red;
        }
    }

    public void CancelSilverPurchase()
    {
        Hide(selectSilver);
        silverDrachs.color = Color.white;
        SelectNoSilver.SetActive(false);
    }

    public void buyWithGold()
    {
        ConfirmPurchase(false);
    }

    public void buyWithSilver()
    {
        ConfirmPurchase();
    }

    void ConfirmPurchase(bool isSilver = true)
    {
        var data = new { purchaseItem = SelectedStoreItem.id, currency = (isSilver ? "silver" : "gold") };
        APIManager.Instance.PostData("shop/purchase", JsonConvert.SerializeObject(data), PurchaseCallback);
    }

    public void PurchaseCallback(string result, int code)
    {
        if (code == 200)
        {
            Raincrow.Analytics.Events.PurchaseAnalytics.PurchaseItem(SelectedStoreItem.id, false);
            PuchaseSuccess();
        }
        else
        {
            Debug.LogError("Something Went Wrong in Purchase : " + result);
        }
    }

    public void PuchaseSuccess(bool isCosmetic = false, ApparelData apData = null, bool isGold = false)
    {
        Debug.Log("purchase Success!");
        SoundManagerOneShot.Instance.PlayReward();
        Hide(selectSilver);
        purchaseSuccess.SetActive(true);
        if (!isCosmetic)
        {
            if (SelectedStoreItem.type == "silver")
            {
                purchaseSuccessTitle.text = SelectedStoreItem.amount.ToString() + " SILVER DRACHS";
                purchaseAmount.text = "";
                StartCoroutine(Countup(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver + SelectedStoreItem.amount));
                PlayerDataManager.playerData.silver += SelectedStoreItem.amount;
                PlayerManagerUI.Instance.ShowElixirOnBuy();
            }
            if (SelectedStoreItem.type == "energy")
            {
                purchaseSuccessTitle.text = LocalizeLookUp.GetStoreTitle(SelectedStoreItem.id);
                purchaseAmount.text = LocalizeLookUp.GetStoreSubtitle(SelectedStoreItem.id);
                StartCoroutine(Countup(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
                PlayerDataManager.playerData.silver -= SelectedStoreItem.silver;
                PlayerManagerUI.Instance.UpdateDrachs();
                foreach (var item in PlayerDataManager.playerData.inventory.consumables)
                {
                    if (item.id == SelectedStoreItem.contents[0].id)
                    {
                        item.count += SelectedStoreItem.contents[0].count;
                        return;
                    }
                }
                Item ci = new Item();
                ci.count = SelectedStoreItem.contents[0].count;
                ci.id = SelectedStoreItem.contents[0].id;
                PlayerDataManager.playerData.inventory.consumables.Add(ci);
                // PlayerManagerUI.Instance.ShowElixirVulnerable (false);

                purchaseSuccessDisplayImage.sprite = selectSilverImg.sprite;
            }
            if (SelectedStoreItem.type == "bundle")
            {
                purchaseSuccessTitle.text = LocalizeLookUp.GetStoreTitle(SelectedStoreItem.id);
                purchaseAmount.text = LocalizeLookUp.GetStoreSubtitle(SelectedStoreItem.id);
                StartCoroutine(Countup(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
                PlayerDataManager.playerData.silver -= SelectedStoreItem.silver;
                PlayerManagerUI.Instance.UpdateDrachs();

                foreach (var item in SelectedStoreItem.contents)
                {
                    var type = DownloadedAssets.GetCollectable(item.id).type;

                    if (type == "herb")
                    {
                        PlayerDataManager.playerData.ingredients.herbsDict[item.id].count += item.count;
                    }
                    else if (type == "gem")
                    {
                        PlayerDataManager.playerData.ingredients.gemsDict[item.id].count += item.count;
                    }
                    else if (type == "tool")
                    {
                        PlayerDataManager.playerData.ingredients.toolsDict[item.id].count += item.count;
                    }
                }

                purchaseSuccessDisplayImage.sprite = SelectedStoreItem.pic;
            }
            if (SelectedStoreItem.type == "xp" || SelectedStoreItem.type == "align")
            {
                purchaseSuccessTitle.text = LocalizeLookUp.GetStoreTitle(SelectedStoreItem.id);
                purchaseAmount.text = LocalizeLookUp.GetStoreSubtitle(SelectedStoreItem.id);
                StartCoroutine(Countup(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
                PlayerDataManager.playerData.silver -= SelectedStoreItem.silver;
                PlayerManagerUI.Instance.UpdateDrachs();


                purchaseSuccessDisplayImage.sprite = SelectedStoreItem.pic;

                foreach (var item in PlayerDataManager.playerData.inventory.consumables)
                {
                    if (item.id == SelectedStoreItem.id)
                    {
                        item.count++;
                        return;
                    }
                }
                Item ci = new Item();
                ci.count = 1;
                ci.id = SelectedStoreItem.id;
                PlayerDataManager.playerData.inventory.consumables.Add(ci);
            }
            purchaseSuccessDisplayImage.sprite = SelectedStoreItem.pic;

        }
        else
        {
            gearUIM.curButton.buttonText.text = "OWNED";
            gearUIM.curButton.button.interactable = false;
            gearUIM.curButton.button.image.sprite = gearUIM.curButton.unlockSprite;
            purchaseSuccessTitle.text = LocalizeLookUp.GetStoreTitle(apData.id);


            DownloadedAssets.GetSprite(apData.iconId, purchaseSuccessDisplayImage, true);

            if (!isGold)
            {
                StartCoroutine(Countup(PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - apData.silver));
                PlayerDataManager.playerData.silver -= apData.silver;
            }
            else
            {
                StartCoroutine(CountUpGold(PlayerDataManager.playerData.gold, PlayerDataManager.playerData.gold - apData.gold));
                PlayerDataManager.playerData.gold -= apData.gold;
            }

            PlayerManagerUI.Instance.UpdateDrachs();

        }
    }

    public void OpenSilverStore()
    {
        Hide(elixirContainer);
        Hide(gearContainer);
        Hide(selectSilver);
        Show(silverContainer);
    }

    IEnumerator Countup(int before, int after)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            silverDrachs.text = ((int)Mathf.Lerp(before, after, t)).ToString();
            yield return null;
        }
    }

    IEnumerator CountUpGold(int before, int after)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            goldDrachs.text = ((int)Mathf.Lerp(before, after, t)).ToString();
            yield return null;
        }
    }


    public void SetElixirPage(bool isPage1)
    {
        rightArrowElixir.SetActive(isPage1);
        leftArrowElixir.SetActive(!isPage1);
        page1Elixir.SetActive(isPage1);
        page2Elixir.SetActive(!isPage1);
    }
}


