using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class FTFManager : MonoBehaviour
{
    public static FTFManager Instance { get; set; }



    public static bool isInFTF = false;

    private int m_CurrentIndex = 0;
    public int curIndex
    {
        get { return m_CurrentIndex; }
        set
        {
            Debug.Log("FTF: " + value);
            m_CurrentIndex = value;
        }
    }

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

    private float m_LastClick = 0;

	[Header("Matt's Keepsakes")]
	public GameObject brigidPrefab;
	public GameObject storePrefab;
	public GameObject wildBarghest;
	public GameObject ownedBarghest;
	public GameObject spellbookOpenBarghest;
	public GameObject spellbookOpenWFBarghest;
	public GameObject spellbookOpenBrigid;
	public GameObject spellbookOpenBrigidCast;
	public GameObject spellbookOpenBrigidImmune;
	public GameObject brigidMirrors;

	public List<string> dialogues = new List<string>();
	public int dialogueIndex = 0;

	//dialogue slide in stuff
	public GameObject continueButton;
	public GameObject dialogueFX;
	public Text dialogueText;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Utilities.allowMapControl(false);
        currentDominion.text = LocalizeLookUp.GetText("dominion_location") + " " + PlayerDataManager.config.dominion;
        strongestWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + PlayerDataManager.config.strongestWitch;
        strongestCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + PlayerDataManager.config.strongestCoven;
        dialogues = DownloadedAssets.ftfDialogues;
//		for (int i = 0; i < dialogues.Count; i++) {
//			if (dialogues[i].Contains("{{Player Name}}"))
//				dialogues[i].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
//		}
		//print ("player x: " + PlayerDataManager.playerPos.x.ToString () + "player y: " + PlayerDataManager.playerPos.y.ToString ()); 
    }


	public void OnContinue(bool nextHasDialogue = true)
    {
        if (CanClickNext() == false)
            return;

        m_LastClick = Time.unscaledTime;

        if (FTFcontinue != null)
            StopCoroutine(FTFcontinue);
		//FTFcontinue = StartCoroutine(OnContinueHelper());
		FTFcontinue = StartCoroutine(MattContinueHelper(nextHasDialogue));
    }

    public void OnContinueMid()
    {
        if (CanClickNext() == false)
            return;

        m_LastClick = Time.unscaledTime;

        if (FTFcontinueMid != null)
            StopCoroutine(FTFcontinueMid);
        FTFcontinueMid = StartCoroutine(OnContinueMidHelper());
    }

    public void OnContinueSpell()
    {
        if (CanClickNext() == false)
            return;

        m_LastClick = Time.unscaledTime;

        if (FTFcontinueSpell != null)
            StopCoroutine(FTFcontinueSpell);
        FTFcontinueSpell = StartCoroutine(OnContinueSpellHelper());
    }

    private bool CanClickNext()
    {
        return Time.unscaledTime - m_LastClick > 0.5f;
    }

	IEnumerator MattContinueHelper(bool nextHasDialogue)
	{
		//thinking about putting a boolean parameter to control dialogue

		curIndex++;

		if (nextHasDialogue) {
			dialogueIndex++;
			SetDialogue ();
		}

		print (dialogueIndex);

		if (curIndex == 1) {
			StartCoroutine (FadeOutFocus (highlight1));
			wildBarghest.SetActive (true);

		} else if (curIndex == 3) {
			continueButton.SetActive (false);
			StartCoroutine (FadeInFocus (highlight2));
			wildBarghest.transform.GetChild (2).gameObject.SetActive (true);

		} else if (curIndex == 4) {
			StartCoroutine (FadeOutFocus (dialogueCG));
			StartCoroutine (FadeOutFocus (highlight2));
			wildBarghest.transform.GetChild (2).gameObject.SetActive (false);
			StartCoroutine (FadeInFocus (dialogueMid));
			dialogueMidButton.SetActive (true);
			dialogueMidText.text = dialogueText.text;
			spellbookOpenBarghest.SetActive (true);

		} else if (curIndex == 5) {
			
			dialogueMidButton.SetActive (false);
			dialogueMidText.text = dialogueText.text;
			wildBarghest.transform.GetChild (0).gameObject.SetActive (true);
			wildBarghest.transform.GetChild (1).gameObject.SetActive (true);

			//will replace this
			StartCoroutine (FadeInFocus (highlight3));
			//take next button way
			//highlight button to go to white spells in spell book

			//do a cinematic for moving to the next page
		} else if (curIndex == 6) {
			StartCoroutine (FadeOutFocus (highlight3));
			StartCoroutine (FadeOutFocus (dialogueMid));
			wildBarghest.transform.GetChild (1).gameObject.SetActive (false);
			wildBarghest.transform.GetChild (0).gameObject.SetActive (false);
			spellbookOpenWFBarghest.SetActive (true);
			spellbookOpenBarghest.SetActive (false);
			StartCoroutine (FadeInFocus (highlight4));

		} else if (curIndex == 7) {
			StartCoroutine (FadeOutFocus (highlight4));
			continueButton.SetActive (true);
			StartCoroutine (FadeInFocus (dialogueCG));
			spellbookOpenWFBarghest.GetComponent<Animator> ().SetBool ("oncast", true);
			StartCoroutine (BarghestWildDefeat ());
			yield return new WaitForSeconds (2f);
			spellbookOpenWFBarghest.SetActive (false);
			//add showing the barghest reward screen to coroutine
			//activate the knowledge of summoning a barghest

			//manually add the barghest to their spell book
			//might have already done the above on launch anyways?
		} else if (curIndex == 8) {
			//HitFXManager.Instance.SpiritDiscovered.SetActive(false);
			spellbookOpenWFBarghest.SetActive (false);
			spiritDeck.SetActive (true);

			//pull new spirit book up
		} else if (curIndex == 10) {
			//spirit bood end animation here
			spiritDeck.SetActive (false);
			continueButton.SetActive (false);
			StartCoroutine (FadeInFocus (highlight5));
			//highlight summoning button
			//this is already done
		} else if (curIndex == 11) {
			ShowSummoning ();
			ownedBarghest.SetActive (true);

			//pull up summoning screen
			//highlight summoning button
			//this is already done
			//no dialogue here

			//slide 13

		} else if (curIndex == 12) {
			//back to map and add a portal
			Debug.Log ("summoning barghest");
			StartCoroutine (FadeOutFocus (highlightSummonScreen));
			PlayerDataManager.playerPos = MapsAPI.Instance.physicalPosition;
			MapsAPI.Instance.position = PlayerDataManager.playerPos;
			SummoningManager.Instance.FTFCastSummon ();

			//Invoke("SpawnBarghestSummon", 5);
			continueButton.SetActive (false);
			//		SoundManagerOneShot.Instance.MenuSound ();
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			//OnContinue();
			//SpawnPortal();
			summonButton.SetActive (false);
			moreInfoButton.SetActive (false);

			//this continue needs to be delayed
			continueButton.SetActive (true);

		} else if (curIndex == 15) {
			dialogueText.text = dialogues[dialogueIndex].Replace("{{Location}}", "<color=#FF8400>" + PlayerDataManager.playerData.dominion + "</color>");
			brigidPrefab.SetActive (true);
			StartCoroutine (FadeInFocus (highlight6));
			//spawnh brigid with vfx landing then transition to model
			//highlight her landing after the coroutine with the vfx or whatever
		} else if (curIndex == 16) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));

			//slide brigid in and savannah out
		} else if (curIndex == 17) {
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeOutFocus (highlight6));
			StartCoroutine (FadeInFocus (savannahCG));
			//slide savannah in and brigid out
		} else if (curIndex == 18) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));
			//slide brigid in and savannah out
		} else if (curIndex == 19) {
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (highlight6));
			highlight6.transform.GetChild (0).GetComponent<Button> ().enabled = true;
			brigidPrefab.transform.GetChild (2).gameObject.SetActive (true);
			continueButton.SetActive (false);

			//slide 21

			//slide savannah in and brigid out
			//make brigid glow red and highlight her
		} else if (curIndex == 20) {
			brigidPrefab.transform.GetChild (2).gameObject.SetActive (false);
			StartCoroutine (FadeOutFocus (highlight6));
			StartCoroutine (FadeOutFocus (dialogueCG));
			dialogueMidText.text = dialogueText.text;
			spellbookOpenBrigid.SetActive (true);
			StartCoroutine (FadeInFocus (dialogueMid));
			StartCoroutine (FadeInFocus (highlight7));


			//slide 23
			//dialogueMidButton.SetActive (true);

			//move dialogue from bottom to the top and disable next button
			//highlight hex 
		} else if (curIndex == 21) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (highlight7));
			StartCoroutine (FadeOutFocus (dialogueMid));
			continueButton.SetActive (true);
			StartCoroutine (CastingHexAnimation ());

			//play animation for hex hitting brigid and move dialogue to the bottom with next button active
			//comment above is invalid

			//highlight continue button?

		} else if (curIndex == 22) {
			StartCoroutine (FadeOutFocus (highlight8));
			spellbookOpenBrigidCast.SetActive (false);
			StartCoroutine (FadeInFocus (dialogueCG));
			StartCoroutine (FadeInFocus (savannahCG));
			//add immunity over brigid here or start a coroutine on the previous one

		} else if (curIndex == 23) {
			//FIX ISSUES HERE
			brigidPrefab.transform.GetChild (1).gameObject.SetActive (true);
			yield return new WaitForSeconds (1.5f);
			brigidPrefab.transform.GetChild (1).gameObject.SetActive (false);

		} else if (curIndex == 24) {
			//might have to move this to the next one
			dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			spellbookOpenBrigidImmune.SetActive (false);
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));
			//slide brigid in and savannah out
		} else if (curIndex == 25) {
			//no dialogue on this one
			//need to make sure dialogue is right before this slide
			//SetDialogue();


			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			StartCoroutine (FadeInFocus (InterceptAttack));
			//slide brigid and text out
			//bring up fowler screen which we already have
		} else if (curIndex == 26) {
			StartCoroutine (FadeOutFocus (InterceptAttack));
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			//slide savannah in with text bottom
		} else if (curIndex == 27) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));
			//slide brigid in and savannah out
		} else if (curIndex == 28) {
			// not dialogue on this one
			StartCoroutine (FadeOutFocus (dialogueCG));
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeInFocus (silencedObject));
			//SetDialogue();
			//slide brigid out and bring up silenced screen which we have... with a continue button?
		} else if (curIndex == 29) {
			if (PlayerDataManager.playerData.male) {
				dialogueText.text = dialogueText.text.Replace ("{{him/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_him])
					.Replace ("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_he]);
			} else {
				dialogueText.text = dialogueText.text.Replace ("{{him/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her])
					.Replace ("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_she]);
			}

			StartCoroutine (FadeOutFocus (silencedObject));
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			//slide savannah in with bottom text and next arrow active
		} else if (curIndex == 30) {
			if (PlayerDataManager.playerData.male) {
				dialogueText.text = dialogueText.text.Replace ("{{his/her}}", DownloadedAssets.localizedText [LocalizationManager.ftf_his]);
			} else {
				dialogueText.text = dialogueText.text.Replace ("{{his/her}}", DownloadedAssets.localizedText [LocalizationManager.ftf_her]);
			}
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));
			//slide brigid in and savannah out
		} else if (curIndex == 31) {
			//no dialogue on this one
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			StartCoroutine (FadeInFocus (dispelObject));
			//bring up dispelled screen with continue button active which we have
		} else if (curIndex == 32) {
			StartCoroutine (FadeOutFocus (dispelObject));
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			//slide savannah in with interupted text on the bottom
		} else if (curIndex == 33) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));
			brigidMirrors.SetActive (true);
//			for (int i = 0; i < brigidMirrors.transform.childCount; i++) {
//				brigidMirrors[i].
//			}
			//slide brigid in and savannah out
			//cast mirror thing with models, not icons
		} else if (curIndex == 34) {
			//slide 37
			//slide savannah in and brigid out
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeInFocus (savannahCG));
			//just continued dialogue from savannah on next - explains jump below
		} else if (curIndex == 35) {
			//slide 38
			dialogueText.text = dialogues[dialogueIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			//player name here
		}else if (curIndex == 36) {
			trueSight.SetActive (true);
			//play animation for mirrors fading here instead of deactivation
			brigidMirrors.SetActive (false);
			yield return new WaitForSeconds (1.5f);
			trueSight.SetActive (false);

			//more savannah text and then play the truesight vfx
			//then play the shadow vfx on the real brigid

		} else if (curIndex == 37) {
			//slide savannah out here, or do it somewhere in the coroutine
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			DeathState.Instance.ShowDeath ();
			PlayerDataManager.playerData.energy = 0;
			PlayerManagerUI.Instance.UpdateEnergy ();
			yield return new WaitForSeconds (2f);
			StartCoroutine (FadeInFocus (deathMsg));
			//show spell from brigid and then bring up death screen
		} else if (curIndex == 38) {
			dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			StartCoroutine (FadeOutFocus (deathMsg));
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			//slide savannah in with bottom text and arrow enabled
		} else if (curIndex == 39) {
			
			//slide savannah in with bottom text and arrow enabled
		} else if (curIndex == 40) {
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			DeathState.Instance.Revived ();
			Blessing bs = new Blessing ();
			bs.daily = 1000;
			PlayerDataManager.playerData.blessing = bs;
			PlayerManagerUI.Instance.ShowBlessing ();
			PlayerDataManager.playerData.energy = 1000;
			PlayerManagerUI.Instance.UpdateEnergy ();
			StartCoroutine (FadeOutFocus (savannahCG));

			//forcing continue here.
			print("forcing continue");
			StartCoroutine(ForceContinue());
			//display grey hand coven message with energy gift
		} else if (curIndex == 41) {
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			continueButton.SetActive (true);
			//slide savannah in with bottom text and arrow enabled
		} else if (curIndex == 42) {
			dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeInFocus (brigidCG));

			//slide brigid in and savannah out
			//replace player name with your name
		} else if (curIndex == 43) {
			//slide 46
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeInFocus (highlight9));

			StartCoroutine (FadeInFocus (savannahCG));
			continueButton.SetActive (false);
			//slide savannah in and brigid out
			//disable next button and highlight store
		} else if (curIndex == 44) {
			//slide 47
			StartCoroutine (FadeOutFocus (highlight9));
			StartCoroutine (FadeOutFocus (brigidCG));
			StartCoroutine (FadeInFocus (savannahCG));
			continueButton.SetActive (true);
			storePrefab.SetActive (true);
			//slide savannah in and brigid out
			//disable next button and highlight store
		} else if (curIndex == 45) {
			//slide 48
			dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			if (PlayerDataManager.playerData.male) {
				dialogueText.text = dialogueText.text.Replace ("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_him])
					.Replace ("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_he]);
			} else {
				dialogueText.text = dialogueText.text.Replace ("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her])
					.Replace ("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_she]);
			}
			//replacing player name
		} else if (curIndex == 46) {
			//slide 49
			dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
			//replacing player name
		}else if (curIndex == 47) {
			//highlight ingredients button
			//slide out text box and savannah
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			StartCoroutine (FadeInFocus (highlight10));
			//no dialogue
		} else if (curIndex == 48) {
			//change store screen to ingredients and highlight abondia's best
			StartCoroutine (FadeOutFocus (highlight10));
			LeanTween.alphaCanvas (storePrefab.transform.GetChild (4).GetComponent<CanvasGroup> (), 0f, 0.5f).setEase (LeanTweenType.easeInOutQuad).setOnComplete (() => {
				//will have to set this up
				storePrefab.transform.GetChild (6).gameObject.SetActive (true);
				StartCoroutine (FadeInFocus (highlight11));
			});

		} else if (curIndex == 49) {
			StartCoroutine (FadeOutFocus (highlight11));
			buyAbondias.SetActive (true);
			//transition to claim abondia's best
		} else if (curIndex == 50) {
			//purchase successful for abondia's best
			buyAbondias.SetActive (false);
			abondiaBought.gameObject.SetActive (true);
			LeanTween.alphaCanvas (abondiaBought, 1f, .5f).setEase (LeanTweenType.easeInOutQuad);
			StartCoroutine (FadeInFocus (savannahCG));
			StartCoroutine (FadeInFocus (dialogueCG));
			continueButton.SetActive (true);
			yield return new WaitForSeconds (1.5f);
			LeanTween.alphaCanvas (abondiaBought, 0f, .5f).setEase (LeanTweenType.easeInOutQuad).setOnComplete(() => {abondiaBought.gameObject.SetActive (false);});
			//savannah slide in with bottom text and arrow enabled
		} else if (curIndex == 51) {
			string tribunal = "";

			print ("replacing season and days here");
			if (PlayerDataManager.config.tribunal == 1)
			{
				tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_summer];
			}
			else if (PlayerDataManager.config.tribunal == 2)
			{
				tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_spring];
			}
			else if (PlayerDataManager.config.tribunal == 3)
			{
				tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_autumn];
			}
			else
			{
				tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_winter];
			}

			dialogueText.text = dialogues[dialogueIndex].Replace("{{Season}}", tribunal);
			dialogueText.text = dialogueText.text.Replace("{{Number}}", PlayerDataManager.config.daysRemaining.ToString())
				.Replace("{{Season}}", tribunal);
		} else if (curIndex == 52) {

			storePrefab.SetActive (false);
			//exit out of store and purchase screen.
			//slide 55
		} else if (curIndex == 54) {
			//show witch school screen here..
			//chooseSchool.gameObject.SetActive(true);
			StartCoroutine(FadeInFocus(chooseSchool));
			StartCoroutine (FadeOutFocus (savannahCG));
			StartCoroutine (FadeOutFocus (dialogueCG));
			brigidPrefab.SetActive(false);
			ownedBarghest.SetActive(false);
		} 

		yield return null;
	}

	IEnumerator BarghestWildDefeat()
	{
		wildBarghest.transform.GetChild(3).gameObject.SetActive(true);
		yield return new WaitForSeconds (.6f);
		wildBarghest.transform.GetChild(3).gameObject.SetActive(false);
		LeanTween.scale (wildBarghest, Vector3.zero, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
			wildBarghest.SetActive(false);
//			HitFXManager.Instance.titleSpirit.text = "Barghest";
//			HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon Barghest!";
//			//this needs a fix
//			//HitFXManager.Instance.spiritDiscSprite.sprite = DownloadedAssets.spiritArt ["spirit_barghest"];
//			HitFXManager.Instance.SpiritDiscovered.SetActive(true);
		});

	}

	IEnumerator ForceContinue()
	{
		dialogueIndex = 36;
		print ("got here");
		yield return new WaitForSeconds (1);
		print ("got here");
		OnContinue (true);
	}


	IEnumerator CastingHexAnimation(){
		spellbookOpenBrigidCast.SetActive (true);
		yield return new WaitForSeconds (1.3f);
		spellbookOpenBrigid.SetActive (false);
		spellbookOpenBrigidImmune.SetActive (true);
		//might have to shift these values around a bit (WFS)
		LeanTween.alphaCanvas (spellbookOpenBrigidCast.transform.GetChild (2).GetComponent<CanvasGroup> (), 1f, 1.2f);
		yield return new WaitForSeconds (1.2f);
		StartCoroutine (FadeInFocus (highlight8));
	}


    IEnumerator OnContinueHelper()
    {
        curIndex++;
        SetDialogue();

        if (curIndex == 1)
        {

			//instantiate orry's prefab under the player manager


//            InventoryItems item = new InventoryItems();
//            item.id = "coll_ironCollar";
//            item.name = DownloadedAssets.ingredientDictData[item.id].name;
//            item.count = 1;
//            item.rarity = DownloadedAssets.ingredientDictData[item.id].rarity;
//            PlayerDataManager.playerData.ingredients.toolsDict[item.id] = item;

            //SpawnBarghest();
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
            dialogueText.text = dialogues[curIndex].Replace("{{Location}}", "<color=#FF8400>" + PlayerDataManager.playerData.dominion + "</color>");
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

            //MarkerManager.Markers["ftf_brigid"][0].gameObject.gameObject.SetActive(false);
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
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
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
            //MarkerManager.Markers["ftf_brigid"][0].gameObject.gameObject.SetActive(true);
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
            dialogueText.text = dialogues[curIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
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
            brigidBanishMsgtext.text = dialogues[curIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
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
			//Replace with mridul's store
            //StoreUIManager.Instance.GetStore();
			storePrefab.SetActive(true);
            StartCoroutine(FadeOutFocus(highlight10));
            continueButton.SetActive(true);

        }
        else if (curIndex == 41)
        {
            dialogueText.text = dialogues[curIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            dialogueText.text = dialogueText.text.Replace("{{he/she}}", (PlayerDataManager.playerData.male ? DownloadedAssets.localizedText[LocalizationManager.ftf_he] : DownloadedAssets.localizedText[LocalizationManager.ftf_she]));
            dialogueText.text = dialogueText.text.Replace("{{his/her}}", (PlayerDataManager.playerData.male ? DownloadedAssets.localizedText[LocalizationManager.ftf_his] : DownloadedAssets.localizedText[LocalizationManager.ftf_her]));
        }
        else if (curIndex == 42)
        {
            dialogueText.text = dialogues[curIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
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
			//replace with mridul's store nav code...
            //StoreUIManager.Instance.ShowElixir(true);
            //StoreUIManager.Instance.SetElixirPage(false);
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
                tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_summer];
            }
            else if (PlayerDataManager.config.tribunal == 2)
            {
                tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_spring];
            }
            else if (PlayerDataManager.config.tribunal == 3)
            {
                tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_autumn];
            }
            else
            {
                tribunal = DownloadedAssets.localizedText[LocalizationManager.ftf_winter];
            }

            dialogueText.text = dialogues[curIndex].Replace("{{Season}}", tribunal);
            dialogueText.text = dialogueText.text.Replace("{{Number}}", PlayerDataManager.config.daysRemaining.ToString());
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
			ContinueToGame ();
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
		sp.longitude = PlayerDataManager.playerPos.x + .000685f;
        sp.tier = 1;
		// added the below because it was having issues?
		sp.spiritId = "spirit_barghest";
		//
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

		// will set this to move forward
        SpiritContainer.SetActive(true);
        SpiritContainer.GetComponent<Animator>().SetTrigger("in");
		print (curIndex);

        StartCoroutine(FadeOutFocus(savannahCG));
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight2));

    }

    public void OnAttack()
    {
		print (curIndex);
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayButtonTap();
        SoundManagerOneShot.Instance.PlayWhisper(.5f);
        SpiritContainer.GetComponent<Animator>().SetTrigger("out");
        //MapSelection.Instance.OnSelect();
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

   IEnumerator OnContinueMidHelper()
    {
        if (curIndex == 4)
        {
            StartCoroutine(FadeOutFocus(HighlightSpellScreen));
            SoundManagerOneShot.Instance.PlayBarghest();
            ButtonPress();
            dialogueMidButton.SetActive(true);

			//here they physically change this stuff, we can setup animations for it.
            //MarkerSpawner.SelectedMarker.energy = 13;
            //IsoTokenSetup.Instance.ChangeEnergy();
            StartCoroutine(FadeOutFocus(dialogueMid));
            yield return new WaitForSeconds(.9f);
            //yield return new WaitForSeconds(1.3f);
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

        //SpellManager.Instance.ChangeFilterType(0);
        //SpellManager.Instance.increasePowerButton.interactable = false;
        StartCoroutine(FadeInFocus(highlight4, 0.5f));
        SoundManagerOneShot.Instance.PlayButtonTap();
        //SpellManager.Instance.SD.canSwipe = false;
    }

    public void BarghestCastSpell()
    {
        StartCoroutine(FadeOutFocus(highlight4));
        StartCoroutine(BarghestCastSpellHelper());
    }

    IEnumerator BarghestCastSpellHelper()
    {
        //SpellManager.Instance.CastSpellFTF();
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
        //WebSocketClient.Instance.ManageData(WD);

        yield return new WaitForSeconds(4.5f);
        SoundManagerOneShot.Instance.MenuSound();
        SoundManagerOneShot.Instance.PlayWhisper(.5f);
        //SpellManager.Instance.Exit();

        yield return new WaitForSeconds(1.2f);
        SoundManagerOneShot.Instance.MenuSound();
//        HitFXManager.Instance.titleSpirit.text = "Barghest";
//        HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon Barghest!";
//        HitFXManager.Instance.spiritDiscSprite.sprite = DownloadedAssets.spiritArt ["spirit_barghest"];

        //DownloadedAssets.GetSprite("spirit_barghest", HitFXManager.Instance.spiritDiscSprite);

//        HitFXManager.Instance.SpiritDiscovered.SetActive(true);
        //SpellManager.Instance.increasePowerButton.interactable = true;
        SoundManagerOneShot.Instance.SpiritDiscovered();
        yield return new WaitForSeconds(1f);
        OnContinue();
        //MarkerManager.DeleteMarker("ftf_spirit");

        //		SpellManager.Instance.SD.canSwipe = true;
    }

    public void ShowSummoning()
    {
        StartCoroutine(FadeOutFocus(savannahCG));
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight5));
        SummoningController.Instance.Open();
        var summon = SummoningController.summon;
        summon.SD.canSwipe = false;
        summon.currentSpiritID = "spirit_barghest";
        summon.increasePower.interactable = false;
        summon.buttonFX[0].SetActive(false);
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
        PlayerDataManager.playerPos = MapsAPI.Instance.physicalPosition;
        MapsAPI.Instance.position = PlayerDataManager.playerPos;
        SummoningManager.Instance.FTFCastSummon();

        //Invoke("SpawnBarghestSummon", 5);
        continueButton.SetActive(false);
        //		SoundManagerOneShot.Instance.MenuSound ();
        StartCoroutine(FadeInFocus(savannahCG));
        StartCoroutine(FadeInFocus(dialogueCG));
        OnContinue();
        //SpawnPortal();
        summonButton.SetActive(false);
        moreInfoButton.SetActive(false);

		//this continue needs to be delayed
		continueButton.SetActive(true);
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
		sp.latitude = PlayerDataManager.playerPos.y; // + .001f;
		sp.longitude = PlayerDataManager.playerPos.x; // - .00285f;
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
		dialogueText.text = dialogues[dialogueIndex];
		//dialogueText.text = dialogues[dialogueIndex];
        if (curIndex != 0)
            ButtonPress();
        ShowFX();
    }


    void SpawnBrigid()
    {
//		Instantiate (brigidPrefab, MarkerSpawner.Instance.transform);
//		Vector3 brigPos = new Vector3 (PlayerDataManager.playerPos.x + .00285f, PlayerDataManager.playerPos.y + .0005f, brigidPrefab.transform.position.z);


		brigidPrefab.SetActive(true);
		//Play animation for her here.


//        Token sp = new Token();
//        sp.instance = "ftf_brigid";
//        sp.position = 0;
//        sp.type = "witch";
//		sp.latitude = PlayerDataManager.playerPos.y;	// + .0005f;
//		sp.longitude = PlayerDataManager.playerPos.x; 	//+ .00285f;
//        sp.tier = 1;
//        sp.Type = MarkerSpawner.MarkerType.witch;
//        MarkerSpawner.selectedType = MarkerSpawner.MarkerType.witch;
//        MarkerSpawner.SelectedMarkerPos = new Vector2(sp.longitude, sp.latitude);
//        MarkerSpawner.instanceID = "ftf_brigid";
//        sp.immunityList = new HashSet<string>();
//        var mD = new MarkerDataDetail();
//        mD.displayName = "Brigid Sawyer";
//        mD.energy = 2444;
//        mD.state = "";
//        mD.level = 8;
//        mD.degree = -10;
//        MarkerSpawner.SelectedMarker = mD;
//        MarkerSpawner.Instance.AddMarker(sp);
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
        //MapSelection.Instance.OnSelect();
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
        //SpellManager.Instance.ChangeFilterType(2);
        //SpellManager.Instance.increasePowerButton.interactable = false;
        StartCoroutine(FadeInFocus(highlight9, 2.5f));
        SoundManagerOneShot.Instance.PlayButtonTap();
        //SpellManager.Instance.SD.canSwipe = false;
    }

    public void BrigidCastHex()
    {
        StartCoroutine(FadeOutFocus(highlight9));
//        SpellManager.Instance.CastSpellFTF();
//        WSData WD = new WSData();
//        WD.command = "map_spell_cast";
//        WD.casterInstance = PlayerDataManager.playerData.instance;
//        WD.caster = PlayerDataManager.playerData.displayName;
//        WD.targetInstance = MarkerSpawner.instanceID;
//        WD.target = "Brigid Sawyer";
//        WD.spell = "spell_hex";
//        Result rs = new Result();
//
//        rs.total = -12;
//        rs.xpGain = 60;
//        rs.critical = false;
//        rs.effect = "success";
//        WD.result = rs;
//        WD.json = "Fake Spirit Hit";
//        WebSocketClient.Instance.ManageData(WD);
//        MarkerSpawner.SelectedMarker.energy = 2232;
//        IsoTokenSetup.Instance.ChangeEnergy();
//
//        WSData immune = new WSData();
//        immune.immunity = PlayerDataManager.playerData.instance;
//        immune.instance = MarkerSpawner.instanceID;
//        immune.command = "map_immunity_add";
//        WebSocketClient.Instance.ManageData(immune);
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


	//Move this code
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
            dialogueSpellTextBrigid.text = dialogues[21].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
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
            //HitFXManager.Instance.HideFTFImmunity();

			// this is missing somewhere, replace if needed
            //immunityText.SetActive(false);
            
			//MarkerManager.SetImmunity(false, MarkerSpawner.instanceID);
            Debug.LogError("TODO: ADD IMMUNITY TO TUTORIAL FLOW");
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
            dialogueSpellText.text = dialogues[26].Replace("{{him/her}}", (PlayerDataManager.playerData.male ? DownloadedAssets.localizedText[LocalizationManager.ftf_him] : DownloadedAssets.localizedText[LocalizationManager.ftf_her]));
            dialogueSpellText.text = dialogueSpellText.text.Replace("{{he/she}}", (PlayerDataManager.playerData.male ? DownloadedAssets.localizedText[LocalizationManager.ftf_he] : DownloadedAssets.localizedText[LocalizationManager.ftf_she]));
        }
        else if (curIndex == 27)
        {
            StartCoroutine(FadeOutFocus(dialogueSpell));
            StartCoroutine(FadeInFocus(dialogueSpellBrigid));
            dialogueSpellTextBrigid.text = dialogues[27].Replace("{{his/her}}", (PlayerDataManager.playerData.male ? DownloadedAssets.localizedText[LocalizationManager.ftf_his] : DownloadedAssets.localizedText[LocalizationManager.ftf_her]));
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

            //SpellManager.Instance.Exit();
            yield return new WaitForSeconds(1.3f);
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
		try{
			SoundManagerOneShot.Instance.PlayWhisper();
			SoundManagerOneShot.Instance.PlayButtonTap();
		} catch {
			print ("Error accessing Sound Manager One Shot");

		}

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
