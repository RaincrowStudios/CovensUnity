using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.Maps;
using Raincrow;
using Raincrow.GameEventResponses;

[RequireComponent(typeof(SwipeDetector))]
public class SummoningManager : MonoBehaviour
{
    private static SummoningManager m_Instance;

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

    [Space(10)]

    public Transform[] headerItems;

    public List<string> tempSpList = new List<string>();
    public int currentIndex = 0;
    public static bool isOpen = false;

    private SwipeDetector SD;
    public GameObject loading;

    public GameObject summonSuccess;
    private GameObject summonSuccessInstance;


    public Button summonButton;
    public Button closeButton;
    public GameObject[] buttonFX;

    public GameObject[] disableNoSpirits;
    public GameObject noSpiritMsg;

    private int currentTier = 0;
    private int m_TweenId;


    public static void Open(System.Action onLoaded = null)
    {
        //wait for the video to be closed/skiped
        if (!FirstTapVideoManager.Instance.CheckSummon())
            return;

        if (m_Instance != null)
        {
            m_Instance._Open();
            onLoaded?.Invoke();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.SUMMONING, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Open();
                onLoaded?.Invoke();
            });
        }
    }

    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance._Close();
    }

    void Awake()
    {
        m_Instance = this;
        SD = GetComponent<SwipeDetector>();
        SD.SwipeRight += OnSwipeRight;
        SD.SwipeLeft += OnSwipeLeft;

        closeButton.onClick.AddListener(_Close);

        GameResyncHandler.OnResyncStart += _Close;
    }

    private void OnDestroy()
    {
        GameResyncHandler.OnResyncStart -= _Close;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && isOpen && !PlayerDataManager.IsFTF)
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

    private void _Open()
    {
        LeanTween.cancel(m_TweenId);
        StopAllCoroutines();
        m_TweenId = LeanTween.value(0, 1, 1).setOnComplete(enableBool).uniqueId;

        OnMapEnergyChange.OnPlayerDead += _Close;

        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayWhisper(.2f);

        Show(summonObject);
        InitHeader();
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
    }

    void enableBool()
    {
        isOpen = true;
        MapsAPI.Instance.HideMap(true);
        if (!PlayerDataManager.IsFTF)
        {
            SD.canSwipe = true;
        }
        else
        {
            SD.canSwipe = false;
        }
    }

    private void _Close()
    {
        LeanTween.cancel(m_TweenId);
        StopAllCoroutines();

        OnMapEnergyChange.OnPlayerDead -= _Close;

        MapsAPI.Instance.HideMap(false);
        UIStateManager.Instance.CallWindowChanged(true);
        isOpen = false;
        SD.canSwipe = false;
        Hide(summonObject);
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
        _Close();
    }

    public void CastSummon()
    {
        if (FTFManager.InFTF)
            return;

        summonButton.interactable = false;
        SoundManagerOneShot.Instance.SummonRiser();

        loading.SetActive(true);
        string spiritId = currentSpiritID;

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
                _Close();

                //spawn the marker
                SpiritToken token = JsonConvert.DeserializeObject<SpiritToken>(s);
                SpiritMarker marker = MarkerSpawner.Instance.AddMarker(token) as SpiritMarker;

                ShowSpiritCastResult(marker);

                int summonCost = PlayerDataManager.SummoningCosts[DownloadedAssets.spiritDict[currentSpiritID].tier - 1];

                OnCharacterDeath.CheckSummonDeath(spiritId, summonCost);
                PlayerDataManager.playerData.AddEnergy(-summonCost);
            }
            else
            {
                UIGlobalPopup.ShowError(_Close, APIManager.ParseError(s));
            }
        });
    }

    private void RemoveIngredients(string spiritId)
    {
        SpiritData spirit = DownloadedAssets.GetSpirit(spiritId);
        List<spellIngredientsData> toRemove = new List<spellIngredientsData>();

        if (string.IsNullOrEmpty(spirit.gem) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.gem, 1);

        if (string.IsNullOrEmpty(spirit.tool) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.tool, 1);

        if (string.IsNullOrEmpty(spirit.herb) == false)
            PlayerDataManager.playerData.SubIngredient(spirit.herb, 1);
    }

    void ShowSpiritCastResult(SpiritMarker spirit)
    {
        summonSuccessInstance = Instantiate(summonSuccess);
        var ss = summonSuccessInstance.GetComponent<SummonSuccess>();

        ss.headingText.text = LocalizeLookUp.GetText("summoning_success");//"Summoning Successful";
        ss.bodyText.text = spiritTitle.text;//= spiritTitle.text + " " + LocalizeLookUp.GetText("summoning_time") + " " + Utilities.GetTimeRemaining(result);
        ss.summonSuccessSpirit.overrideSprite = spiritIcon.overrideSprite;
        ss.spirit = spirit;
    }



    IEnumerator StartTimer(double result, Text text)
    {
        if (PlayerDataManager.IsFTF)
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
            data.ingredients.Add(new spellIngredientsData { collectible = SummoningIngredientManager.addedTool, count = SummoningIngredientManager.addedToolCount });
        }

        if (SummoningIngredientManager.addedHerb != "")
        {
            data.ingredients.Add(new spellIngredientsData { collectible = SummoningIngredientManager.addedHerb, count = SummoningIngredientManager.addedHerbCount });
        }
        if (SummoningIngredientManager.addedGem != "")
        {
            data.ingredients.Add(new spellIngredientsData { collectible = SummoningIngredientManager.addedGem, count = SummoningIngredientManager.addedGemCount });
        }
        return data.ingredients;
    }

}
