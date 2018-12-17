using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class FTFManager : MonoBehaviour
{

    public static FTFManager Instance { get; set; }

    public GameObject dialogueFX;
    public Text dialogueText;
    public List<string> dialogues = new List<string>();
    public static bool isInFTF = false;
    public int curIndex = 0;

    public CanvasGroup highlight1;
    public CanvasGroup highlight2;
    public CanvasGroup highlight3;
    public CanvasGroup highlight4;
    public CanvasGroup highlight5;
    public CanvasGroup highlight6;
    public CanvasGroup highlight7;
    public CanvasGroup highlight8;
    public CanvasGroup highlight9;
    public CanvasGroup highlight10;
    public CanvasGroup highlight11;
    public CanvasGroup highlight12;
    public CanvasGroup highlightSummonScreen;
    public GameObject continueButton;
    public GameObject HitFXWhite;
    Coroutine FTFcontinue;
    Coroutine FTFcontinueMid;
    Coroutine FTFcontinueSpell;

    public CanvasGroup HighlightSpellScreen;

    public CanvasGroup dialogueMid;
    public Text dialogueMidText;
    public GameObject dialogueMidButton;

    public CanvasGroup dialogueSpell;
    public Text dialogueSpellText;
    public GameObject dialogueSpellButton;

    public CanvasGroup dialogueSpellBrigid;
    public Text dialogueSpellTextBrigid;
    public GameObject dialogueSpellBrigidButton;

    public CanvasGroup InterceptAttack;

    public GameObject SpiritContainer;

    public CanvasGroup savannahCG;
    public CanvasGroup dialogueCG;
    public GameObject portalSummonObject;
    public GameObject summonButton;
    public GameObject moreInfoButton;

    public GameObject spiritDeck;
    public CanvasGroup brigidCG;
    public CanvasGroup BrigidDialogueCG;
    public Text BrigidDialogueText;
    public GameObject brigidContinueButton;
    public CanvasGroup conditionHex;
    public GameObject playerContainer;

    public GameObject immunityText;
    public CanvasGroup silencedObject;

    public GameObject silenceSpellFX;
    public Text silenceTitle;
    public Image silenceGlyph;

    public GameObject dispelSpellFX;

    public CanvasGroup dispelObject;
    public Transform mirrors;
    //	List<SpellData> spells = new List<SpellData>();

    public GameObject trueSight;
    public Light spotlight;

    public CanvasGroup deathMsg;
    public CanvasGroup brigidBanishMsg;
    public Text brigidBanishMsgtext;
    public CanvasGroup attackFrame;
    public GameObject attackFX;
    public CanvasGroup banishObject;
    public CanvasGroup chooseSchool;
    public CanvasGroup statsScreen;
    public Text currentDominion;
    public Text strongestWitch;
    public Text strongestCoven;

    public GameObject buyAbondias;
    public CanvasGroup abondiaBought;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Utilities.allowMapControl(false);
        currentDominion.text = "You are in the dominion of " + PlayerDataManager.config.dominion;
        strongestWitch.text = "The Strongest witch in this dominion is " + PlayerDataManager.config.strongestWitch;
        strongestCoven.text = "The Strongest coven is " + PlayerDataManager.config.strongestCoven;
        dialogues = DownloadedAssets.ftfDialogues;
    }


    void SetupStatScreen()
    {
        //		if (config.tribunal == 1) {
        //			tribunalTitle.text = "THE SUMMER TOURNAMENT OF WITCHCRAFT";
        //		} else if (config.tribunal == 2) {
        //			tribunalTitle.text = "THE SPRING TOURNAMENT OF WITCHCRAFT";
        //		} else if (config.tribunal == 3) {
        //			tribunalTitle.text = "THE AUTUMN TOURNAMENT OF WITCHCRAFT";
        //		} else {
        //			tribunalTitle.text = "THE WINTER TOURNAMENT OF WITCHCRAFT";
        //		}
        //
        //		tribunalTimer.text = config.daysRemaining.ToString();

    }

    public void OnContinue()
    {
        if (FTFcontinue != null)
            StopCoroutine(FTFcontinue);
        FTFcontinue = StartCoroutine(OnContinueHelper());
    }

    IEnumerator OnContinueHelper()
    {

        curIndex++;
        SetDialogue();

        if (curIndex == 1)
        {


            InventoryItems item = new InventoryItems();
            item.id = "coll_ironCollar";
            item.name = DownloadedAssets.ingredientDictData[item.id].name;
            item.count = 1;
            item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
            PlayerDataManager.playerData.ingredients.toolsDict[item.id] = item;

            print("Spawning Barghest");
            SpawnBarghest();
            StartCoroutine(FadeOutFocus(highlight1));

        }
        else if (curIndex == 3)
        {
            continueButton.SetActive(false);
            StartCoroutine(FadeInFocus(highlight2));
            // Can Add dialogue before 3 
        }
        else if (curIndex == 6)
        {
            continueButton.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
        }
        else if (curIndex == 7)
        {
            HitFXManager.Instance.SpiritDiscovered.SetActive(false);
            SoundManagerOneShot.Instance.MenuSound();
            SoundManagerOneShot.Instance.PlayWhisper(.25f);
            spiritDeck.SetActive(true);
        }
        else if (curIndex == 9)
        {
            continueButton.SetActive(false);
            SoundManagerOneShot.Instance.MenuSound();
            spiritDeck.GetComponent<Animator>().Play("out");
            Disable(spiritDeck);
            yield return new WaitForSeconds(2);
            StartCoroutine(FadeInFocus(highlight5));
        }
        else if (curIndex == 11)
        {
            portalSummonObject.SetActive(false);
        }
        else if (curIndex == 13)
        {
            StartCoroutine(FadeOutFocus(highlight6));
            dialogueText.text = dialogues[curIndex].Replace("$", "<color=#FF8400>" + PlayerDataManager.playerData.dominion + "</color>");
            SoundManagerOneShot.Instance.LandingSound(.3f);
            SoundManagerOneShot.Instance.PlayWhisperFX();
            SpawnBrigid();
            //			highlight7.interactable = false;
            StartCoroutine(FadeInFocus(highlight7));
            //			highlight7.interactable = false;
        }
        else if (curIndex == 14)
        {
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            StartCoroutine(FadeInFocus(BrigidDialogueCG));
            BrigidDialogueText.text = dialogues[curIndex];
        }
        else if (curIndex == 15)
        {
            StartCoroutine(FadeOutFocus(highlight7));
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeOutFocus(BrigidDialogueCG));
        }
        else if (curIndex == 16)
        {
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            StartCoroutine(FadeInFocus(BrigidDialogueCG));
            BrigidDialogueText.text = dialogues[curIndex];
        }
        else if (curIndex == 17)
        {
            highlight7.interactable = true;
            StartCoroutine(FadeInFocus(highlight7));
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeOutFocus(BrigidDialogueCG));
            continueButton.SetActive(false);
        }
        else if (curIndex == 30)
        {
            //			SoundManagerOneShot.Instance.PlayBrigidLaugh ();
            mirrors.gameObject.SetActive(true);

            MarkerManager.Markers["ftf_brigid"][0].instance.gameObject.SetActive(false);
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            StartCoroutine(FadeInFocus(BrigidDialogueCG));
            BrigidDialogueText.text = dialogues[curIndex];
            SoundManagerOneShot.Instance.PlayWhisperFX();
            foreach (Transform item in mirrors)
            {
                SoundManagerOneShot.Instance.LandingSound(.4f);
                item.gameObject.SetActive(true);
                yield return new WaitForSeconds(.4f);
            }
            yield break;
        }
        else if (curIndex == 31)
        {
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeOutFocus(BrigidDialogueCG));
            //			mirrors.gameObject.SetActive (false);
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
        }
        else if (curIndex == 32)
        {
            dialogueText.text = dialogueText.text.Replace("$", PlayerDataManager.playerData.displayName);
        }
        else if (curIndex == 33)
        {
            trueSight.SetActive(true);
            StartCoroutine(TrueSightLight());
            foreach (Transform item in mirrors)
            {
                SoundManagerOneShot.Instance.LandingSound(.4f);
                item.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, .25f);
                yield return new WaitForSeconds(.18f);
            }
            MarkerManager.Markers["ftf_brigid"][0].instance.gameObject.SetActive(true);
        }
        else if (curIndex == 34)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeInFocus(attackFrame));
            attackFX.SetActive(true);
            StartCoroutine(FadeInFocus(attackFX.GetComponentInChildren<CanvasGroup>()));
            yield return new WaitForSeconds(1.8f);
            WSData data = new WSData();
            data.casterInstance = "ftf_brigid";
            data.spell = "spell_sunEater";
            Result rs = new Result();
            rs.total = -1203;
            rs.critical = true;
            rs.effect = "success";
            data.result = rs;
            MovementManager.Instance.AttackFXSelf(data);
            SoundManagerOneShot.Instance.PlaySpellFX();
            SoundManagerOneShot.Instance.PlayWhisper();
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeOutFocus(attackFX.GetComponentInChildren<CanvasGroup>()));
            yield return new WaitForSeconds(.3f);
            StartCoroutine(FadeOutFocus(attackFrame));
            DeathState.Instance.ShowDeath();
            PlayerDataManager.playerData.energy = 0;
            PlayerManagerUI.Instance.UpdateEnergy();
            yield return new WaitForSeconds(1.2f);
            attackFX.SetActive(false);
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeInFocus(deathMsg));
        }
        else if (curIndex == 35)
        {
            dialogueText.text = dialogues[curIndex].Replace("$", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeOutFocus(deathMsg));
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
        }
        else if (curIndex == 36)
        {
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(banishObject));
            MarkerManager.DeleteMarker("ftf_brigid");
            mirrors.gameObject.SetActive(false);
            yield return new WaitForSeconds(3);

            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            yield return new WaitForSeconds(1.3f);
            banishObject.gameObject.SetActive(false);
        }
        else if (curIndex == 37)
        {
            DeathState.Instance.Revived();
            Blessing bs = new Blessing();
            bs.daily = 1000;
            PlayerDataManager.playerData.blessing = bs;
            PlayerManagerUI.Instance.ShowBlessing();
            PlayerDataManager.playerData.energy = 1000;
            PlayerManagerUI.Instance.UpdateEnergy();
            StartCoroutine(FadeOutFocus(savannahCG));
            trueSight.SetActive(false);
        }
        else if (curIndex == 38)
        {
            PlayerManagerUI.Instance.HideBlessing();
            StartCoroutine(FadeOutFocus(dialogueCG));
            yield return new WaitForSeconds(1.2f);
            StartCoroutine(FadeInFocus(brigidBanishMsg));
            brigidBanishMsgtext.text = dialogues[curIndex].Replace("$", PlayerDataManager.playerData.displayName);
        }
        else if (curIndex == 39)
        {
            StartCoroutine(FadeOutFocus(brigidBanishMsg));
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            continueButton.SetActive(false);
            StartCoroutine(FadeInFocus(highlight10));
        }
        else if (curIndex == 40)
        {
            StoreUIManager.Instance.GetStore();
            StartCoroutine(FadeOutFocus(highlight10));
            continueButton.SetActive(true);

        }
        else if (curIndex == 41)
        {
            dialogueText.text = dialogues[curIndex].Replace("$", PlayerDataManager.playerData.displayName);
            dialogueText.text = dialogueText.text.Replace("@", (PlayerDataManager.playerData.male ? "he" : "she"));
            dialogueText.text = dialogueText.text.Replace("#", (PlayerDataManager.playerData.male ? "his" : "her"));
        }
        else if (curIndex == 42)
        {
            dialogueText.text = dialogues[curIndex].Replace("$", PlayerDataManager.playerData.displayName);
        }
        else if (curIndex == 43)
        {
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(highlight11));
        }
        else if (curIndex == 44)
        {
            StartCoroutine(FadeOutFocus(highlight11));
            StoreUIManager.Instance.ShowElixir(true);
            StoreUIManager.Instance.SetElixirPage(false);
            StartCoroutine(FadeInFocus(highlight12));
        }
        else if (curIndex == 45)
        {
            StartCoroutine(FadeOutFocus(highlight12));
            buyAbondias.SetActive(true);
        }
        else if (curIndex == 46)
        {
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            buyAbondias.GetComponent<Animator>().Play("out");
            Disable(buyAbondias, 1.2f);
            abondiaBought.gameObject.SetActive(true);

        }
        else if (curIndex == 47)
        {
            StartCoroutine(FadeOutFocus(abondiaBought));
            StoreUIManager.Instance.ShowElixir(false);
            StoreUIManager.Instance.Exit();
            string tribunal = "";

            if (PlayerDataManager.config.tribunal == 1)
            {
                tribunal = "Summer";
            }
            else if (PlayerDataManager.config.tribunal == 2)
            {
                tribunal = "Spring";
            }
            else if (PlayerDataManager.config.tribunal == 3)
            {
                tribunal = "Autumn";
            }
            else
            {
                tribunal = "Winter";
            }

            dialogueText.text = dialogues[curIndex].Replace("@", tribunal);
            dialogueText.text = dialogueText.text.Replace("#", PlayerDataManager.config.daysRemaining.ToString());
        }
        else if (curIndex == 50)
        {
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(chooseSchool));
        }
        yield return 0;
    }

    public void chooseSchoolResult(bool isSchool)
    {
        if (isSchool)
        {
            StartCoroutine(FadeOutFocus(chooseSchool));
            ContinueToGame();
            WitchSchoolManager.Instance.Open();
        }
        else
        {
            StartCoroutine(FadeOutFocus(chooseSchool));
            StartCoroutine(FadeInFocus(statsScreen));
        }
    }



    public void ContinueToGame()
    {
        //		SummoningManager.Instance.SD.canSwipe = true;
        //		SummoningManager.Instance.SD.canSwipe = true;
        StartCoroutine(FadeOutFocus(statsScreen));
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<Image>().raycastTarget = false;
        FTFManager.isInFTF = false;
        MarkerManagerAPI.GetMarkers(true);
        APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
        {
            //			Debug.Log(s + " FTF RES");
            LoginAPIManager.FTFComplete = true;
            APIManager.Instance.GetData("character/get", (string ss, int rr) =>
            {
                print("reinit");
                var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
                PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                LoginAPIManager.loggedIn = true;
                PlayerManager.Instance.initStart();
                Utilities.allowMapControl(true);

            });
        });

    }



    void SpawnBarghest()
    {
        Token sp = new Token();
        sp.instance = "ftf_spirit";
        sp.position = 0;
        sp.type = "spirit";
        sp.spiritType = "familiar";
        sp.latitude = PlayerDataManager.playerPos.y + .001f;
        sp.longitude = PlayerDataManager.playerPos.x + .00285f;
        sp.tier = 1;
        sp.Type = MarkerSpawner.MarkerType.spirit;
        MarkerSpawner.selectedType = MarkerSpawner.MarkerType.spirit;
        MarkerSpawner.SelectedMarkerPos = new Vector2(sp.longitude, sp.latitude);
        MarkerSpawner.instanceID = "ftf_spirit";
        var mD = new MarkerDataDetail();
        mD.id = "spirit_barghest";
        mD.energy = 98;
        mD.state = "";
        mD.level = 1;
        MarkerSpawner.SelectedMarker = mD;
        MarkerSpawner.Instance.AddMarker(sp);
    }

    public void OnTapBarghest()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        SpiritContainer.SetActive(true);
        SpiritContainer.GetComponent<Animator>().SetTrigger("in");
        StartCoroutine(FadeOutFocus(savannahCG));
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight2));

    }

    public void OnAttack()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        SoundManagerOneShot.Instance.PlayWhisper(.5f);
        SpiritContainer.GetComponent<Animator>().SetTrigger("out");
        MapSelection.Instance.OnSelect();
        Invoke("showBarghestIsoDialogue", 2.2f);
    }

    void showBarghestIsoDialogue()
    {
        SpiritContainer.SetActive(false);
        StartCoroutine(FadeInFocus(dialogueMid));
        StartCoroutine(FadeInFocus(HighlightSpellScreen, 1.5f));
        dialogueMidText.text = dialogues[4];
        curIndex = 4;
    }

    public void OnContinueMid()
    {
        if (FTFcontinueMid != null)
            StopCoroutine(FTFcontinueMid);
        FTFcontinueMid = StartCoroutine(OnContinueMidHelper());
    }

    IEnumerator OnContinueMidHelper()
    {
        if (curIndex == 4)
        {
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            SoundManagerOneShot.Instance.PlayBarghest();
            ButtonPress();
            dialogueMidButton.SetActive(true);
            MarkerSpawner.SelectedMarker.energy = 13;
            IsoTokenSetup.Instance.ChangeEnergy();
            StartCoroutine(FadeOutFocus(dialogueMid));
            yield return new WaitForSeconds(.9f);
            yield return new WaitForSeconds(1.3f);
            HitFXWhite.SetActive(true);
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeInFocus(dialogueMid));
            dialogueMidText.text = dialogues[5];
            SpellManager.whiteSpellIndex = 1;
            //			SpellManager.Instance.SD.canSwipe = false;
            if (HighlightSpellScreen.alpha == 1)
            {
                StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            }
            StartCoroutine(FadeInFocus(highlight3));
            dialogueMidButton.SetActive(false);
            curIndex++;
        }
        yield return 0;
    }

    public void BarghestShowSpell()
    {
        StartCoroutine(FadeOutFocus(dialogueMid));
        StartCoroutine(FadeOutFocus(highlight3));

        SpellManager.Instance.ChangeFilterType(0);
        SpellManager.Instance.increasePowerButton.interactable = false;
        StartCoroutine(FadeInFocus(highlight4, 2.5f));
        SoundManagerOneShot.Instance.PlayButtonTap();
        SpellManager.Instance.SD.canSwipe = false;
    }

    public void BarghestCastSpell()
    {
        StartCoroutine(FadeOutFocus(highlight4));
        StartCoroutine(BarghestCastSpellHelper());
    }

    IEnumerator BarghestCastSpellHelper()
    {
        SpellManager.Instance.CastSpellFTF();
        WSData WD = new WSData();
        WD.command = "map_spell_cast";
        WD.casterInstance = PlayerDataManager.playerData.instance;
        WD.caster = PlayerDataManager.playerData.displayName;
        WD.targetInstance = MarkerSpawner.instanceID;
        WD.target = "spirit_barghest";
        WD.spell = "spell_whiteFlame";
        Result rs = new Result();

        rs.total = -15;
        rs.xpGain = 60;
        rs.critical = true;
        rs.effect = "success";
        WD.result = rs;
        WD.json = "Fake Spirit Hit";
        WebSocketClient.Instance.ManageData(WD);

        yield return new WaitForSeconds(4.5f);
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayWhisper(.5f);
        SpellManager.Instance.Exit();

        yield return new WaitForSeconds(1.2f);
        SoundManagerOneShot.Instance.MenuSound();
        HitFXManager.Instance.titleSpirit.text = "Barghest";
        HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon Barghest!";
        //		HitFXManager.Instance.spiritDiscSprite.sprite = DownloadedAssets.spiritArt ["spirit_barghest"];

        DownloadedAssets.GetSprite("spirit_barghest", HitFXManager.Instance.spiritDiscSprite);

        HitFXManager.Instance.SpiritDiscovered.SetActive(true);
        SpellManager.Instance.increasePowerButton.interactable = true;
        SoundManagerOneShot.Instance.SpiritDiscovered();
        yield return new WaitForSeconds(1f);
        OnContinue();
        MarkerManager.DeleteMarker("ftf_spirit");

        //		SpellManager.Instance.SD.canSwipe = true;
    }

    public void ShowSummoning()
    {
        StartCoroutine(FadeOutFocus(savannahCG));
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight5));
        SummoningManager.Instance.SD.canSwipe = false;
        SummoningManager.currentSpiritID = "spirit_barghest";
        SummoningManager.Instance.Open();
        SummoningManager.Instance.increasePower.interactable = false;
        SummoningManager.Instance.buttonFX[0].SetActive(false);
        StartCoroutine(FadeInFocus(highlightSummonScreen, 1.2f));
        Invoke("EnableSummonButton", 1.2f);
    }

    void EnableSummonButton()
    {
        summonButton.SetActive(true);
        moreInfoButton.SetActive(true);
    }

    public void Summon()
    {
        Debug.Log("summoning barghest");
        StartCoroutine(FadeOutFocus(highlightSummonScreen));
        PlayerDataManager.playerPos = OnlineMapsLocationService.instance.position;
        OnlineMaps.instance.position = PlayerDataManager.playerPos;
        SummoningManager.Instance.FTFCastSummon();

        Invoke("SpawnBarghestSummon", 5);
        continueButton.SetActive(false);
        //		SoundManagerOneShot.Instance.MenuSound ();
        StartCoroutine(FadeInFocus(savannahCG));
        StartCoroutine(FadeInFocus(dialogueCG));
        OnContinue();
        SpawnPortal();
        summonButton.SetActive(false);
        moreInfoButton.SetActive(false);
    }

    void SpawnPortal()
    {
        Token sp = new Token();
        sp.instance = "ftf_portal";
        sp.position = 0;
        sp.type = "portal";
        sp.latitude = PlayerDataManager.playerPos.y + .001f;
        sp.longitude = PlayerDataManager.playerPos.x - .00285f;
        sp.Type = MarkerSpawner.MarkerType.portal;
        sp.degree = 0;
        MarkerSpawner.Instance.AddMarker(sp);
    }

    void SpawnBarghestSummon()
    {
        MarkerManager.DeleteMarker("ftf_portal");
        SoundManagerOneShot.Instance.LandingSound(.3f);
        Token sp = new Token();
        sp.instance = "ftf_spirit_summon";
        sp.position = 0;
        sp.type = "spirit";
        sp.spiritType = "familiar";
        sp.latitude = PlayerDataManager.playerPos.y + .001f;
        sp.longitude = PlayerDataManager.playerPos.x - .00285f;
        sp.tier = 1;
        sp.Type = MarkerSpawner.MarkerType.spirit;
        MarkerSpawner.selectedType = MarkerSpawner.MarkerType.spirit;
        MarkerSpawner.SelectedMarkerPos = new Vector2(sp.longitude, sp.latitude);
        MarkerSpawner.instanceID = "ftf_spirit_summon";
        var mD = new MarkerDataDetail();
        mD.id = "spirit_barghest";
        mD.energy = 98;
        mD.state = "";
        mD.level = 1;
        MarkerSpawner.SelectedMarker = mD;
        MarkerSpawner.Instance.AddMarker(sp);
        highlight6.gameObject.SetActive(false);
        //		hasSpiritSpawned = true;
        //		OnContinue ();
        continueButton.SetActive(true);

    }

    void SetDialogue()
    {
        dialogueText.text = dialogues[curIndex];
        if (curIndex != 0)
            ButtonPress();
        ShowFX();
    }


    void SpawnBrigid()
    {
        Token sp = new Token();
        sp.instance = "ftf_brigid";
        sp.position = 0;
        sp.type = "witch";
        sp.latitude = PlayerDataManager.playerPos.y + .0005f;
        sp.longitude = PlayerDataManager.playerPos.x + .00285f;
        sp.tier = 1;
        sp.Type = MarkerSpawner.MarkerType.witch;
        MarkerSpawner.selectedType = MarkerSpawner.MarkerType.witch;
        MarkerSpawner.SelectedMarkerPos = new Vector2(sp.longitude, sp.latitude);
        MarkerSpawner.instanceID = "ftf_brigid";
        sp.immunityList = new HashSet<string>();
        var mD = new MarkerDataDetail();
        mD.displayName = "Brigid Sawyer";
        mD.energy = 2444;
        mD.state = "";
        mD.level = 8;
        mD.degree = -10;
        MarkerSpawner.SelectedMarker = mD;
        MarkerSpawner.Instance.AddMarker(sp);
    }

    public void OnTapBrigid()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        playerContainer.SetActive(true);
        playerContainer.GetComponent<Animator>().SetTrigger("in");
        StartCoroutine(FadeOutFocus(savannahCG));
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight7));
    }

    public void OnBrigidAttack()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        SoundManagerOneShot.Instance.PlayWhisper(.5f);
        playerContainer.GetComponent<Animator>().SetTrigger("out");
        MapSelection.Instance.OnSelect();
        Invoke("showBrigidIsoDialogue", 2.2f);
    }

    void showBrigidIsoDialogue()
    {
        playerContainer.SetActive(false);
        StartCoroutine(FadeInFocus(dialogueMid));
        dialogueMidButton.SetActive(false);
        StartCoroutine(FadeInFocus(highlight8, 2.7f));
        dialogueMidText.text = dialogues[18];
        curIndex = 18;
    }

    public void BrigidShowHex()
    {
        StartCoroutine(FadeOutFocus(highlight8));
        StartCoroutine(FadeOutFocus(dialogueMid));
        SpellManager.Instance.ChangeFilterType(2);
        SpellManager.Instance.increasePowerButton.interactable = false;
        StartCoroutine(FadeInFocus(highlight9, 2.5f));
        SoundManagerOneShot.Instance.PlayButtonTap();
        SpellManager.Instance.SD.canSwipe = false;
    }

    public void BrigidCastHex()
    {
        StartCoroutine(FadeOutFocus(highlight9));
        SpellManager.Instance.CastSpellFTF();
        WSData WD = new WSData();
        WD.command = "map_spell_cast";
        WD.casterInstance = PlayerDataManager.playerData.instance;
        WD.caster = PlayerDataManager.playerData.displayName;
        WD.targetInstance = MarkerSpawner.instanceID;
        WD.target = "Brigid Sawyer";
        WD.spell = "spell_hex";
        Result rs = new Result();

        rs.total = -12;
        rs.xpGain = 60;
        rs.critical = false;
        rs.effect = "success";
        WD.result = rs;
        WD.json = "Fake Spirit Hit";
        WebSocketClient.Instance.ManageData(WD);
        MarkerSpawner.SelectedMarker.energy = 2232;
        IsoTokenSetup.Instance.ChangeEnergy();

        WSData immune = new WSData();
        immune.immunity = PlayerDataManager.playerData.instance;
        immune.instance = MarkerSpawner.instanceID;
        immune.command = "map_immunity_add";
        WebSocketClient.Instance.ManageData(immune);
        StartCoroutine(FadeInFocus(conditionHex));
        Invoke("ShowSavannahDialogue", 5);
    }

    void ShowSavannahDialogue()
    {
        StartCoroutine(FadeInFocus(dialogueSpell));
        StartCoroutine(FadeInFocus(HighlightSpellScreen));
        dialogueSpellText.text = dialogues[19];
        curIndex = 19;
    }

    public void OnContinueSpell()
    {
        if (FTFcontinueSpell != null)
            StopCoroutine(FTFcontinueSpell);
        FTFcontinueSpell = StartCoroutine(OnContinueSpellHelper());
    }

    IEnumerator OnContinueSpellHelper()
    {

        curIndex++;
        ButtonPress();
        if (curIndex == 20)
        {
            dialogueSpellText.text = dialogues[20];

        }
        else if (curIndex == 21)
        {
            dialogueSpellTextBrigid.text = dialogues[21].Replace("$", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeInFocus(dialogueSpellBrigid));
            StartCoroutine(FadeOutFocus(dialogueSpell));

        }
        else if (curIndex == 22)
        {
            StartCoroutine(FadeOutFocus(conditionHex));
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            StartCoroutine(FadeOutFocus(dialogueSpellBrigid));
            SoundManagerOneShot.Instance.MenuSound();
            SoundManagerOneShot.Instance.PlaySpellFX();
            yield return new WaitForSeconds(1);
            SoundManagerOneShot.Instance.PlayFowler();
            StartCoroutine(FadeInFocus(InterceptAttack));
        }
        else if (curIndex == 23)
        {
            StartCoroutine(FadeInFocus(HighlightSpellScreen));
            StartCoroutine(FadeInFocus(dialogueSpell));
            StartCoroutine(FadeOutFocus(InterceptAttack));
            dialogueSpellText.text = dialogues[23];
        }
        else if (curIndex == 24)
        {
            StartCoroutine(FadeInFocus(HighlightSpellScreen));
            StartCoroutine(FadeInFocus(dialogueSpellBrigid));
            StartCoroutine(FadeOutFocus(dialogueSpell));
            dialogueSpellTextBrigid.text = dialogues[24];
        }
        else if (curIndex == 25)
        {
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            StartCoroutine(FadeOutFocus(dialogueSpellBrigid));
            yield return new WaitForSeconds(1.5f);
            SoundManagerOneShot.Instance.PlayWhisperFX();
            HitFXManager.Instance.HideFTFImmunity();
            immunityText.SetActive(false);
            MarkerManager.SetImmunity(false, MarkerSpawner.instanceID);
            silenceSpellFX.SetActive(true);
            silenceTitle.text = "Silence";
            DownloadedAssets.GetSprite("spell_silence", silenceGlyph);
            yield return new WaitForSeconds(1.8f);
            silencedObject.gameObject.SetActive(true);
        }
        else if (curIndex == 26)
        {
            StartCoroutine(FadeOutFocus(silencedObject));
            StartCoroutine(FadeInFocus(HighlightSpellScreen));
            StartCoroutine(FadeInFocus(dialogueSpell));
            dialogueSpellText.text = dialogues[26].Replace("$", (PlayerDataManager.playerData.male ? "him" : "her"));
            dialogueSpellText.text = dialogueSpellText.text.Replace("@", (PlayerDataManager.playerData.male ? "he" : "she"));
        }
        else if (curIndex == 27)
        {
            StartCoroutine(FadeOutFocus(dialogueSpell));
            StartCoroutine(FadeInFocus(dialogueSpellBrigid));
            dialogueSpellTextBrigid.text = dialogues[27].Replace("$", (PlayerDataManager.playerData.male ? "his" : "her"));
        }
        else if (curIndex == 28)
        {
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            StartCoroutine(FadeOutFocus(dialogueSpellBrigid));
            yield return new WaitForSeconds(1f);
            dispelSpellFX.SetActive(true);
            yield return new WaitForSeconds(1.3f);

            StartCoroutine(FadeInFocus(dispelObject));
            SoundManagerOneShot.Instance.PlayWhisperFX();
            SoundManagerOneShot.Instance.MenuSound();
            yield return new WaitForSeconds(.8f);
            StartCoroutine(FadeInFocus(HighlightSpellScreen));

        }
        else if (curIndex == 29)
        {
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            StartCoroutine(FadeOutFocus(dispelObject));
            SoundManagerOneShot.Instance.MenuSound();
            SoundManagerOneShot.Instance.PlayWhisper(.8f);

            SpellManager.Instance.Exit();
            yield return new WaitForSeconds(1.3f);
            //			PlayerDataManager.playerPos = OnlineMapsLocationService.instance.position;
            //			OnlineMaps.instance.position = PlayerDataManager.playerPos;
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            dialogueText.text = dialogues[29];
            continueButton.SetActive(true);
        }

        yield return 0;
    }



    #region utils

    void ButtonPress()
    {
        SoundManagerOneShot.Instance.PlayWhisper();
        SoundManagerOneShot.Instance.PlayButtonTap();
    }


    void ShowFX()
    {
        if (dialogueFX.activeInHierarchy)
        {
            dialogueFX.SetActive(false);
        }
        dialogueFX.SetActive(true);
    }

    IEnumerator FadeOutFocus(CanvasGroup cg)
    {
        cg.interactable = false;
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2.8f;
            cg.alpha = Mathf.SmoothStep(1, 0, t);
            yield return 0;
        }
        cg.gameObject.SetActive(false);

    }

    IEnumerator FadeInFocus(CanvasGroup cg, float delay = 0)
    {
        cg.interactable = false;
        yield return new WaitForSeconds(delay);
        cg.gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            cg.alpha = Mathf.SmoothStep(0, 1, t);
            yield return 0;
        }
        if (cg == highlight7)
        {
            if (curIndex == 13 || curIndex == 14)
            {
                print("Disabling Interaction");
                cg.interactable = false;
            }
            else
            {
                cg.interactable = true;
            }
        }
        else
        {
            cg.interactable = true;
        }

    }

    public void Disable(GameObject g, float delay = 1.5f)
    {
        StartCoroutine(disableObject(g, delay));
    }

    IEnumerator disableObject(GameObject g, float delay)
    {
        yield return new WaitForSeconds(delay);
        g.SetActive(false);
    }

    IEnumerator TrueSightLight()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * .5f;
            spotlight.spotAngle = Mathf.SmoothStep(0, 153, t);
            spotlight.intensity = Mathf.Lerp(45, 9, t);
            yield return 0;
        }
    }

    #endregion



}
