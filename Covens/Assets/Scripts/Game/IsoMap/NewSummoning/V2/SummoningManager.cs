using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.Maps;

[RequireComponent(typeof(SummoningIngredientManager))]
[RequireComponent(typeof(SwipeDetector))]
public class SummoningManager : MonoBehaviour
{

    public static SummoningManager Instance { get; set; }
    public string currentSpiritID = "";

    public GameObject summonObject;
    public Text FilterDesc;
    public Text spiritDesc;
    public Image spiritIcon;
    public Text spiritTitle;
    public Text summonCost;
    public Text countText;
    public GameObject maxReached;

    [Header("Spirit Info")]
    public GameObject infoObject;
    public Text spiritInfoTitle;
    public Text spiritInfoLore;
    public Text ingredientsReq;
    public Text spiritInfoTier;
    public Text legend;
    public Image SpiritInfoIcon;

    [Header("Auto Ingredients")]
    public GameObject ingredientObject;
    public Text gemText;
    public Text herbText;
    public Text toolText;
    public GameObject gemFX;
    public GameObject herbFX;
    public GameObject toolFX;
    public Text gemTextCount;
    public Text herbTextCount;
    public Text toolTextCount;

    [Space(10)]

    public Transform[] headerItems;

    public GameObject RandomizeLoading;
    // SummonFilter filter = SummonFilter.none;
    public List<string> tempSpList = new List<string>();
    public int currentIndex = 0;
    public static bool isOpen = false;
    [HideInInspector]
    public SwipeDetector SD;
    public GameObject loading;

    public GameObject summonSuccess;
    GameObject summonSuccessInstance;


    public Button summonButton;
    public GameObject[] buttonFX;

    // public Button increasePower;

    public GameObject[] disableNoSpirits;
    public GameObject noSpiritMsg;

    private int currentTier = 0;

    Coroutine timerRoutine;
    void Awake()
    {
        Instance = this;
        SD = GetComponent<SwipeDetector>();
        SD.SwipeRight += OnSwipeRight;
        SD.SwipeLeft += OnSwipeLeft;

    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && isOpen && !LoginUIManager.isInFTF)
        {
            PointerEventData ped = new PointerEventData(null);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            foreach (var item in results)
            {

                if (item.gameObject.tag == "Header")
                {
                    currentTier = int.Parse(item.gameObject.name);
                    InitHeader();
                    item.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    item.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    SetPage();
                    SoundManagerOneShot.Instance.PlayButtonTap();
                }
            }
        }
    }

    void InitHeader()
    {
        foreach (var item in headerItems)
        {
            item.GetComponent<CanvasGroup>().alpha = .5f;
            item.GetChild(0).gameObject.SetActive(false);
        }
    }

    void Start()
    {
        InitSummon();
    }

    public void InitSummon()
    {
        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayWhisper(.2f);


        Show(summonObject);
        InitHeader();
        // RandomizeLoading.SetActive(false);
        summonButton.interactable = true;
        // filter = SummonFilter.none;
        if (currentSpiritID != "")
        {
            for (int i = 0; i < PlayerDataManager.playerData.knownSpirits.Count; i++)
            {
                if (PlayerDataManager.playerData.knownSpirits[i].spirit == currentSpiritID)
                {
                    currentIndex = i;
                    break;
                }
            }
        }
        else
        {
            currentIndex = 0;
        }
        SetPage(false);
        Invoke("enableBool", 1f);
    }

    void enableBool()
    {
        isOpen = true;
        MapsAPI.Instance.HideMap(true);
        if (!LoginUIManager.isInFTF)
        {
            SD.canSwipe = true;
        }
        else
        {
            SD.canSwipe = false;

        }
    }

    public void Close()
    {
        Debug.Log("setting summoning false");
        this.CancelInvoke();
        MapsAPI.Instance.HideMap(false);
        UIStateManager.Instance.CallWindowChanged(true);
        isOpen = false;
        SD.canSwipe = false;
        Hide(summonObject);
        if (UIPOPOptions.Instance != null)
        {
            UIPOPOptions.Instance.ShowUI();
        }
        // Destroy(gameObject, 2f);
    }

    void SetPage(bool isReset = true)
    {
        var knownList = PlayerDataManager.playerData.knownSpirits;
        tempSpList.Clear();
        if (isReset)
            currentIndex = 0;
        if (currentTier == 0)
        {
            tempSpList = knownList.Select(l => l.spirit).ToList();
            FilterDesc.text = "";

        }
        else
        {
            FilterDesc.text = LocalizeLookUp.GetText("summoning_tier") + " " + currentTier.ToString();
            foreach (var item in knownList)
            {
                try
                {
                    if (currentTier == DownloadedAssets.spiritDict[item.spirit].tier)
                        tempSpList.Add(item.spirit);
                }
                catch
                {

                    Debug.Log(item.spirit);
                }

            }
        }

        if (tempSpList.Count == 0)
        {
            foreach (var item in disableNoSpirits)
            {
                item.SetActive(false);
            }
            noSpiritMsg.SetActive(true);
            return;
        }
        else
        {
            foreach (var item in disableNoSpirits)
            {
                item.SetActive(true);
            }
            noSpiritMsg.SetActive(false);
        }

        currentSpiritID = tempSpList[currentIndex];
        DownloadedAssets.GetSprite(currentSpiritID, spiritIcon);
        spiritTitle.text = LocalizeLookUp.GetSpiritName(currentSpiritID);
        countText.text = (currentIndex + 1).ToString() + "/" + (tempSpList.Count).ToString();
        spiritDesc.text = LocalizeLookUp.GetSpiritBehavior(currentSpiritID);

        if (!SummoningIngredientManager.AddBaseIngredients(currentSpiritID))
        {
            summonButton.interactable = false;
            //  increasePower.interactable = false;
            foreach (var item in buttonFX)
            {
                item.SetActive(false);
            }
        }
        else
        {
            summonButton.interactable = true;
            //            increasePower.interactable = true;
            foreach (var item in buttonFX)
            {
                item.SetActive(true);
            }
        }
        summonCost.text = LocalizeLookUp.GetText("spell_data_cost").Replace("{{Energy Cost}}", PlayerDataManager.SummoningCosts[DownloadedAssets.spiritDict[currentSpiritID].tier - 1].ToString());// + " Energy";
    }

    void OnSwipeLeft()
    {
        SoundManagerOneShot.Instance.PlayWhisper();
        if (currentIndex < tempSpList.Count - 1)
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }
        SetPage(false);
    }

    void OnSwipeRight()
    {
        SoundManagerOneShot.Instance.PlayWhisper();

        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = tempSpList.Count - 1;
        }
        SetPage(false);
    }

    public void ShowMoreInfo()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        Show(infoObject);
        spiritInfoTitle.text = LocalizeLookUp.GetSpiritName(currentSpiritID);
        spiritInfoLore.text = LocalizeLookUp.GetSpiritDesc(currentSpiritID);
        DownloadedAssets.GetSprite(currentSpiritID, SpiritInfoIcon);

        SpiritData spiritData = DownloadedAssets.GetSpirit(currentSpiritID);
        

        string kind = "";
        if (DownloadedAssets.spiritDict[currentSpiritID].tier == 1)
        {
            kind = LocalizeLookUp.GetText("rarity_common");// "Common";
        }
        else if (DownloadedAssets.spiritDict[currentSpiritID].tier == 2)
        {
            kind = LocalizeLookUp.GetText("rarity_less");//"Less Common";
        }
        else if (DownloadedAssets.spiritDict[currentSpiritID].tier == 3)
        {
            kind = LocalizeLookUp.GetText("rarity_rare");//"Rare";
        }
        else
        {
            kind = LocalizeLookUp.GetText("rarity_exotic");// "Exotic";
        }
        spiritInfoTier.text = kind;
        legend.text = spiritData.legend;

        string s = "";
        s += (string.IsNullOrEmpty(spiritData.gem) ? "" : " " + LocalizeLookUp.GetCollectableName(spiritData.gem));
        s += (string.IsNullOrEmpty(spiritData.herb) ? "" : " " + LocalizeLookUp.GetCollectableName(spiritData.herb));
        s += (string.IsNullOrEmpty(spiritData.tool) ? "" : " " + LocalizeLookUp.GetCollectableName(spiritData.tool));
        ingredientsReq.text = (s == "" ? LocalizeLookUp.GetText("card_witch_noCoven") + "." /*"None."*/ : s);
    }

    public void CloseMoreInfo()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        Hide(infoObject);
    }
    public void CloseMaxReached()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        Hide(maxReached);
    }

    #region Animation

    void Show(GameObject g)
    {
        if (g.activeInHierarchy)
            g.SetActive(false);
        var anim = g.GetComponent<Animator>();
        g.SetActive(true);
        anim.Play("in");
    }

    void Hide(GameObject g, bool isDisable = true)
    {
        if (isDisable)
            StopCoroutine("DisableObject");

        if (g.activeInHierarchy)
        {
            var anim = g.GetComponent<Animator>();
            anim.Play("out");
            if (isDisable)
                StartCoroutine(DisableObject(g));
        }
    }

    IEnumerator DisableObject(GameObject g)
    {
        yield return new WaitForSeconds(.55f);
        g.SetActive(false);
    }
    #endregion


    public void FTFCastSummon()
    {
        ShowSpiritCastResult(null);
        SummoningController.Instance.Close();
    }

    public void CastSummon()
    {
        summonButton.interactable = false;
        SoundManagerOneShot.Instance.SummonRiser();

        loading.SetActive(true);
        string spiritId = currentSpiritID;
        
        //var data = new { ingredients = GetIngredients() };
        SummoningIngredientManager.ClearIngredient();

        //string endpoint = PlaceOfPower.IsInsideLocation ? "location/summon" : 
        string endpoint = "character/summon/" + currentSpiritID;
        APIManager.Instance.Post(endpoint, "{}", (string s, int r) =>
        {
            summonButton.interactable = true;
            loading.SetActive(false);

            if (r == 200)
            {
                //remove the ingredients
                RemoveIngredients(spiritId);

                SoundManagerOneShot.Instance.SpiritSummon();
                SummoningController.Instance.Close();

                //spawn the marker
                SpiritToken token = JsonConvert.DeserializeObject<SpiritToken>(s);
                SpiritMarker marker = MarkerSpawner.Instance.AddMarker(token) as SpiritMarker;

                ShowSpiritCastResult(marker);
            }
            else
            {
                UIGlobalErrorPopup.ShowError(SummoningController.Instance.Close, LocalizeLookUp.GetText("error_" + s));
            }

            //if (r == 200)
            //{
            //    bool isLocationSummon = (endpoint == "location/summon");

            //    SoundManagerOneShot.Instance.SpiritSummon();
            //    SummoningController.Instance.Close();
            //    if (UIPOPOptions.Instance != null)
            //    {
            //        UIPOPOptions.Instance.ShowUI();
            //    }

            //    if (!isLocationSummon)
            //    {
            //        JObject d = JObject.Parse(s);
            //        ShowSpiritCastResult(true, double.Parse(d["summonOn"].ToString()));
            //    }

            //    RemoveIngredients(spiritId);
            //}
            //else if (s == "4902")
            //{
            //    //you have summoned your maximum 
            //    Show(maxReached);
            //    SoundManagerOneShot.Instance.MenuSound();
            //}
            //else
            //{
            //    Debug.LogError(s);
            //}
        });
    }

    private void RemoveIngredients(string spiritId)
    {
        SpiritData spirit = DownloadedAssets.GetSpirit(spiritId);
        List<spellIngredientsData> toRemove = new List<spellIngredientsData>();

        if (string.IsNullOrEmpty(spirit.gem) == false)
            toRemove.Add(new spellIngredientsData(spirit.gem, 1));

        if (string.IsNullOrEmpty(spirit.tool) == false)
            toRemove.Add(new spellIngredientsData(spirit.tool, 1));

        if (string.IsNullOrEmpty(spirit.herb) == false)
            toRemove.Add(new spellIngredientsData(spirit.herb, 1));

        PlayerDataManager.playerData.ingredients.RemoveIngredients(toRemove);
    }

    void ShowSpiritCastResult(SpiritMarker spirit)
    {
        summonSuccessInstance = Instantiate(summonSuccess);
        var ss = summonSuccessInstance.GetComponent<SummonSuccess>();

        ss.headingText.text = LocalizeLookUp.GetText("summoning_success");//"Summoning Successful";
        ss.bodyText.text = "";//= spiritTitle.text + " " + LocalizeLookUp.GetText("summoning_time") + " " + Utilities.GetTimeRemaining(result);
        ss.summonSuccessSpirit.overrideSprite = spiritIcon.overrideSprite;
        ss.spirit = spirit;
        //try
        //{
        //    if (timerRoutine != null)
        //        StopCoroutine(timerRoutine);
        //}
        //catch
        //{
        //}
        //timerRoutine = ss.StartCoroutine(StartTimer(result, ss.bodyText));
    }



    IEnumerator StartTimer(double result, Text text)
    {
        if (LoginUIManager.isInFTF)
        {
            result = (System.DateTime.UtcNow.AddSeconds(5)).Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        while (text != null)
        {
            if (Utilities.GetTimeRemaining(result) != "null")
            {
                text.text = spiritTitle.text + " " + LocalizeLookUp.GetText("summoning_time") + " " + Utilities.GetTimeRemaining(result);
            }
            else
            {
                Destroy(summonSuccessInstance);
            }
            yield return new WaitForSeconds(1);
        }
    }

    List<spellIngredientsData> GetIngredients()
    {
        var data = new SpellTargetData();
        data.ingredients = new List<spellIngredientsData>();

        if (SummoningIngredientManager.addedTool != "")
        {
            data.ingredients.Add(new spellIngredientsData { id = SummoningIngredientManager.addedTool, count = SummoningIngredientManager.addedToolCount });
        }

        if (SummoningIngredientManager.addedHerb != "")
        {
            data.ingredients.Add(new spellIngredientsData { id = SummoningIngredientManager.addedHerb, count = SummoningIngredientManager.addedHerbCount });
        }
        if (SummoningIngredientManager.addedGem != "")
        {
            data.ingredients.Add(new spellIngredientsData { id = SummoningIngredientManager.addedGem, count = SummoningIngredientManager.addedGemCount });
        }
        return data.ingredients;
    }

}

// public enum SummonFilter
// {
//     forbidden, harvester, healer, protector, trickster, warrior, guardian, familiar, none
// }
