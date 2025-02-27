﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class FTFManager : MonoBehaviour
{
    public static FTFManager Instance { get; set; }



    private int m_CurrentIndex = 0;
    public int curIndex
    {
        get { return m_CurrentIndex; }
        set
        {
            // Debug.Log("FTF: " + value);
            m_CurrentIndex = value;
        }
    }
    public GameObject daddy;

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

    //public GameObject HitFXWhite;
    Coroutine FTFcontinue;

    public CanvasGroup InterceptAttack;

    public GameObject SpiritContainer;

    public CanvasGroup savannahCG;
    public CanvasGroup dialogueCG;
    //public GameObject portalSummonObject;
    public GameObject summonButton;
    public GameObject moreInfoButton;

    public GameObject spiritDeck;
    public Animator spiritDeckAnim;
    public CanvasGroup brigidCG;
    public GameObject SpiritDiscoveredBarghest;
    //public CanvasGroup BrigidDialogueCG;
    //public Text BrigidDialogueText;
    //public GameObject brigidContinueButton;
    //public CanvasGroup conditionHex;


    //public GameObject immunityText;
    public CanvasGroup silencedObject;

    //public GameObject silenceSpellFX;
    //public Text silenceTitle;
    //public Image silenceGlyph;

    //public GameObject dispelSpellFX;

    public CanvasGroup dispelObject;
    //public Transform mirrors;
    //	List<SpellData> spells = new List<SpellData>();

    public GameObject trueSight;
    //public Light spotlight;

    public CanvasGroup deathMsg;
    public GameObject brigidBanishMsg;
    public CanvasGroup brigidBanishMsgCG;
    public Text brigidBanishMsgtext;
    //public CanvasGroup attackFrame;
    //public GameObject attackFX;
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
    public GameObject brigidPrefabInstance;
    public Animator brigidPrefabAnim;
    public GameObject storePrefab;
    public GameObject wildBarghest;
    public GameObject wildBarghestInstance;
    public GameObject ownedBarghest;
    public GameObject ownedBarghestInstance;
    public GameObject spellbookOpenBarghest;
    public GameObject spellbookOpenWFBarghest;
    public GameObject spellbookOpenBarghestOnCast;
    public Animator spellbookOpenBrigidCastOnCastOut;
    public GameObject spellbookOpenBrigid;
    public GameObject spellbookOpenBrigidCast;
    public GameObject spellbookOpenBrigidImmune;
    public Animator spellbookOpenBrigidImmuneOut;

    public GameObject silenceGlyph;
    public GameObject dispelGlyph;
    public GameObject twilightDusk;
    public GameObject blessingParticle;

    public GameObject mirrors;
    public GameObject mirrorsInstance;

    public List<string> dialogues = new List<string>();
    public int dialogueIndex = 0;

    //dialogue slide in stuff
    public GameObject continueButton;
    public GameObject dialogueFX;
    public TextMeshProUGUI dialogueText;


    private Transform cameraTransform;
    private Transform camRotTransform;
    private Transform camCenterPoint;
    public float rotSpeed = 1;
    public LeanTweenType easeType;
    private IEnumerator rotateCoroutin;

    public AudioClip barghestHowl;
    public AudioClip openSpellbook;
    public AudioClip savannahSpell;
    public AudioClip whiteFlameSpell;
    public AudioClip brigidLand;
    public AudioClip hexOnBrigid;
    public AudioClip fowlerNoise;
    public AudioClip dispelledNoise;
    public AudioClip mirrorsNoise;
    public AudioClip banishSound;
    public AudioClip brigidLandGuitar;
    public AudioSource soundSource;
    private PlayerCompass playerCompass;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerCompass = PlayerCompass.instance;
        soundSource = gameObject.AddComponent<AudioSource>();
        ChatUI.Instance.SetChatInteraction(false);
        cameraTransform = MapController.Instance.m_StreetMap.camera.transform;
        camRotTransform = cameraTransform.parent;
        camCenterPoint = camRotTransform.parent;
        cameraTransform.GetComponent<Camera>().backgroundColor = new Color(200, 69, 50);
        Utilities.allowMapControl(false);
        currentDominion.text = LocalizeLookUp.GetText("dominion_location") + " " + PlayerDataManager.config.dominion;
        strongestWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + PlayerDataManager.config.strongestWitch;
        strongestCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + PlayerDataManager.config.strongestCoven;
        dialogues = DownloadedAssets.ftfDialogues;
        StartRotation();
        zoomCamera(-440, 15);

    }
    void rotateCamera(float endValue, float time)
    {
        LeanTween.cancel(camRotTransform.gameObject);
        LeanTween.rotateY(camRotTransform.gameObject, endValue, time).setEase(easeType).setOnUpdate((float f) =>
        {

            playerCompass.FTFCompass(camRotTransform.localEulerAngles.y);
        });
    }

    void zoomCamera(float endValue, float time)
    {
        LeanTween.cancel(cameraTransform.gameObject);
        LeanTween.moveLocalZ(cameraTransform.gameObject, endValue, time).setEase(easeType);
    }

    void moveCamera(Vector3 endPos, float time, System.Action onComplete = null)
    {
        LeanTween.cancel(camCenterPoint.gameObject);
        LeanTween.move(camCenterPoint.gameObject, endPos, time).setEase(easeType).setOnComplete(() =>
        {
            if (onComplete != null)
                onComplete();
        }); ;
    }


    void StartRotation()
    {
        rotateCoroutin = RotationHelper();
        StartCoroutine(rotateCoroutin);
    }

    IEnumerator RotationHelper()
    {
        while (true)
        {
            playerCompass.FTFCompass(camRotTransform.localEulerAngles.y);
            camRotTransform.localEulerAngles = new Vector3(camRotTransform.localEulerAngles.x, camRotTransform.localEulerAngles.y + rotSpeed * Time.deltaTime, 0);
            yield return 0;
        }
    }

    void StopRotation(float stopTime = 1)
    {
        float tempRotSpeed = rotSpeed;
        LeanTween.value(rotSpeed, 0, stopTime).setOnUpdate((float v) =>
        {
            rotSpeed = v;
        }).setOnComplete(() =>
        {
            rotSpeed = tempRotSpeed;
            StopCoroutine(rotateCoroutin);
        });
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


    private bool CanClickNext()
    {
        return Time.unscaledTime - m_LastClick > 0.5f;
    }

    public IEnumerator SavannahSpellBarghest()
    {
        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutFocus(dialogueCG));
        StartCoroutine(FadeOutFocus(highlight2));
        yield return new WaitForSeconds(.2f);
        wildBarghestInstance.transform.GetChild(3).gameObject.SetActive(true);
        PlayFTFSound(savannahSpell);
        OnContinue(true);
    }


    IEnumerator MattContinueHelper(bool nextHasDialogue)
    {
        //thinking about putting a boolean parameter to control dialogue

        curIndex++;

        if (nextHasDialogue)
        {
            dialogueIndex++;
            SetDialogue();
        }

        //   print(dialogueIndex);

        if (curIndex == 1)
        {
            StopRotation();
            //StartCoroutine (FadeOutFocus (highlight1));

            Transform trans = PlayerManager.marker.gameObject.transform;
            wildBarghestInstance = Utilities.InstantiateObject(wildBarghest, trans, 0);
            PlayFTFSound(barghestHowl);
            wildBarghestInstance.transform.Translate(new Vector3((trans.position.x - 36f), trans.position.y, (trans.position.z + 36f)));
            LeanTween.scale(wildBarghestInstance, Vector3.one, .5f).setEase(easeType);
            StopRotation();
            zoomCamera(-200, 2.4f);
            moveCamera(new Vector3((trans.position.x - 38.1f), trans.position.y, (trans.position.z + 30.1f)), 2.4f, () =>
            {
                rotSpeed = 2;
                StartRotation();
            });
            rotateCamera(-120, 2.4f);
            //    StartCoroutine(FadeOutFocus(highlight1));
            //wildBarghest.SetActive (true);

        }
        else if (curIndex == 2)
        {

        }
        else if (curIndex == 3)
        {
            continueButton.SetActive(false);
            StartCoroutine(FadeInFocus(highlight2));
            wildBarghestInstance.transform.GetChild(2).gameObject.SetActive(true);

        }
        else if (curIndex == 4)
        {
            StartCoroutine(FadeOutFocus(highlight2));
            //wildBarghestInstance
            StopRotation();
            zoomCamera(-340, 2.4f);
            moveCamera(new Vector3(-30, 0, 40f), 2.4f);
            rotateCamera(-90, 2.4f);

            wildBarghestInstance.transform.GetChild(2).gameObject.SetActive(false);
            spellbookOpenBarghest.SetActive(true);
            PlayFTFSound(openSpellbook);
            StartCoroutine(SavannahSpellBarghest());

        }
        else if (curIndex == 5)
        {
            //dialogueMidText.text = dialogueText.text;
            //			wildBarghest.transform.GetChild (0).gameObject.SetActive (true);

            //CHANGING THE TEXT
            TextMeshPro energy = wildBarghestInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
            TextMeshProUGUI energy2 = spellbookOpenBarghest.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
            LeanTween.value(440, 36, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                energy.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <b><color=#4C80FD>" + f.ToString() + "</color></b>";
                energy2.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <color=black>" + f.ToString();
            });
            yield return new WaitForSeconds(1.2f);
            continueButton.SetActive(false);
            StartCoroutine(FadeInFocus(dialogueCG));
            //will replace this
            yield return new WaitForSeconds(4f);

            StartCoroutine(FadeOutFocus(dialogueCG));

            yield return new WaitForSeconds(1.2f);

            StartCoroutine(FadeInFocus(highlight3));
            //take next button way
            //highlight button to go to white spells in spell book

            //do a cinematic for moving to the next page
        }
        else if (curIndex == 6)
        {
            StartCoroutine(FadeOutFocus(highlight3));
            StartCoroutine(FadeOutFocus(dialogueCG));
            wildBarghestInstance.transform.GetChild(3).gameObject.SetActive(false);
            //			wildBarghest.transform.GetChild (0).gameObject.SetActive (false);
            spellbookOpenWFBarghest.SetActive(true);
            //spellbookOpenBarghest.SetActive (false);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeInFocus(highlight4));
            spellbookOpenBarghest.SetActive(false);
        }
        else if (curIndex == 7)
        {

            StartCoroutine(FadeOutFocus(highlight4));
            spellbookOpenBarghestOnCast.SetActive(true);
            var t = wildBarghestInstance.transform.GetChild(3);
            t.GetChild(1).gameObject.SetActive(false);
            //t.GetChild (4).gameObject.SetActive (false);
            t.GetChild(5).gameObject.SetActive(false);
            yield return new WaitForSeconds(1.8f);
            spellbookOpenWFBarghest.SetActive(false);
            yield return new WaitForSeconds(1f);

            spellbookOpenWFBarghest.SetActive(false);
            t.gameObject.SetActive(true);
            PlayFTFSound(whiteFlameSpell);
            yield return new WaitForSeconds(1.4f);
            TextMeshPro energy = wildBarghestInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
            TextMeshProUGUI energy2 = spellbookOpenBarghest.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
            LeanTween.value(36, 0, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                energy.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <b><color=#4C80FD>" + f.ToString() + "</color></b>";
                energy2.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <color=black>" + f.ToString();
            });
            StartCoroutine(BarghestWildDefeat());
            moveCamera(PlayerManager.marker.gameObject.transform.position, 1f);
            yield return new WaitForSeconds(1f);
            StartRotation();
            SpiritDiscoveredBarghest.SetActive(true);
            yield return new WaitForSeconds(5.2f);
            SpiritDiscoveredBarghest.SetActive(false);
            continueButton.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
        }
        else if (curIndex == 8)
        {

            //HitFXManager.Instance.SpiritDiscovered.SetActive(false);
            spiritDeck.SetActive(true);

            //pull new spirit book up
        }
        else if (curIndex == 10)
        {
            spellbookOpenBarghestOnCast.SetActive(false);
            //spirit bood end animation here
            //spiritDeckAnim.SetBool("SpiritDeckClose");
            spiritDeckAnim.SetBool("SpiritDeckClose", true);
            //spiritDeck.SetActive (false);
            continueButton.SetActive(false);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeInFocus(highlight5));
            //highlight summoning button
            //this is already done
        }
        else if (curIndex == 11)
        {
            spiritDeck.SetActive(false);
            ShowSummoning();
            ownedBarghest.SetActive(true);

            //slide 13

        }
        else if (curIndex == 12)
        {
            continueButton.SetActive(false);
            StopRotation();
            //back to map and add a portal
            Debug.Log("summoning barghest");
            StartCoroutine(FadeOutFocus(highlightSummonScreen));



            PlayerDataManager.playerPos = MapsAPI.Instance.physicalPosition;
            MapsAPI.Instance.position = PlayerDataManager.playerPos;
            SummoningManager.Instance.FTFCastSummon();
            yield return new WaitForSeconds(1f);
            //SummoningManager.Instance.Close ();
            Transform trans = PlayerManager.marker.gameObject.transform;
            ownedBarghestInstance = Utilities.InstantiateObject(ownedBarghest, trans);
            ownedBarghestInstance.transform.Translate(new Vector3((trans.position.x - 24f), trans.position.y, (trans.position.z - 20f)));
            moveCamera(ownedBarghestInstance.transform.position, 2f);
            zoomCamera(-180, 2f);
            //Invoke("SpawnBarghestSummon", 5);

            //		SoundManagerOneShot.Instance.MenuSound ();
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //OnContinue();
            //SpawnPortal();
            summonButton.SetActive(false);
            moreInfoButton.SetActive(false);
            yield return new WaitForSeconds(4f);
            zoomCamera(-260, 2f);
            StartRotation();
            //this continue needs to be delayed
            continueButton.SetActive(true);

        }
        else if (curIndex == 15)
        {

            continueButton.SetActive(false);
            //    print(dialogues[dialogueIndex]);
            dialogueText.text = dialogues[dialogueIndex].Replace("{{Location}}", "<color=#FF8400>" + PlayerDataManager.playerData.dominion + "</color>");
            //      print(dialogueText.text);
            //brigidPrefab.SetActive (true);
            //continueButton.SetActive(false);
            Transform trans = PlayerManager.marker.gameObject.transform;
            Vector3 brigPos = new Vector3((trans.position.x + 30f), trans.position.y, (trans.position.z - 10f));
            moveCamera(brigPos, 2f);
            rotateCamera(390, 2f);
            zoomCamera(-360f, 2f);
            yield return new WaitForSeconds(2f);

            brigidPrefabInstance = Utilities.InstantiateObject(brigidPrefab, trans);
            brigidPrefabAnim = brigidPrefabInstance.GetComponent<Animator>();
            brigidPrefabInstance.transform.Translate(brigPos);
            brigidPrefabInstance.SetActive(true);
            yield return new WaitForSeconds(0.45f);
            PlayFTFSound(brigidLand);
            ////  **MAKE A NEW LANDING SOUND ORRY GODDAMMIT**
            //yield return new WaitForSeconds (0.1f);
            //AudioSource blam = new AudioSource ();
            //blam.clip = mirrorsNoise;
            //blam.Play ();
            //continueButton.SetActive(true);
            //spawnh brigid with vfx landing then transition to model
            //highlight her landing after the coroutine with the vfx or whatever
            continueButton.SetActive(true);
        }
        else if (curIndex == 16)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));

            //slide brigid in and savannah out
        }
        else if (curIndex == 17)
        {
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            //slide savannah in and brigid out
        }
        else if (curIndex == 18)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            //slide brigid in and savannah out
        }
        else if (curIndex == 19)
        {

            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(highlight6));
            highlight6.transform.GetChild(0).GetComponent<Button>().enabled = true;
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(true);
            continueButton.SetActive(false);

        }
        else if (curIndex == 20)
        {
            StopRotation();
            continueButton.SetActive(false);
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(false);
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(highlight6));
            Vector3 npos = brigidPrefabInstance.transform.position;
            npos.x += 10;
            npos.y += 20;
            rotateCamera(0, 1.6f);
            zoomCamera(-200, 1.6f);
            moveCamera(npos, 1.6f);
            yield return new WaitForSeconds(1.6f);


            spellbookOpenBrigid.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeInFocus(highlight7));

            //slide 23
        }
        else if (curIndex == 21)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(highlight7));
            StartCoroutine(FadeOutFocus(dialogueCG));
            brigidPrefabInstance.transform.GetChild(4).gameObject.SetActive(true);
            PlayFTFSound(hexOnBrigid);
            StartCoroutine(CastingHexAnimation());
            yield return new WaitForSeconds(1f);

            TextMeshPro energy = brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshPro>();
            TextMeshProUGUI energy2 = spellbookOpenBrigidImmune.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>();
            LeanTween.value(22244, 22224, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                energy.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <b><color=#F48D00>" + f.ToString() + "</color></b>\nlvl: <b><color=#F48D00>8</color></b>";
                energy2.text = DownloadedAssets.localizedText[LocalizationManager.lt_energy] + " <color=black>" + f.ToString();
            });



            continueButton.SetActive(true);


        }
        else if (curIndex == 22)
        {
            StartCoroutine(FadeOutFocus(highlight8));
            spellbookOpenBrigidCastOnCastOut.SetBool("OnCastOut", true);
            yield return new WaitForSeconds(2f);
            StartCoroutine(FadeInFocus(dialogueCG));
            StartCoroutine(FadeInFocus(savannahCG));
            //add immunity over brigid here or start a coroutine on the previous one

        }
        else if (curIndex == 23)
        {
            //FIX ISSUES HERE
            spellbookOpenBrigidCast.SetActive(false);
            //brigidPrefab.transform.GetChild (1).gameObject.SetActive (true);
            yield return new WaitForSeconds(1.5f);
            //brigidPrefab.transform.GetChild (1).gameObject.SetActive (false);

        }
        else if (curIndex == 24)
        {

            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            spellbookOpenBrigidImmuneOut.SetBool("ImmuneOut", true);
            //spellbookOpenBrigidImmune.SetActive (false);
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            //slide brigid in and savannah out
        }
        else if (curIndex == 25)
        {
            Transform trans = PlayerManager.marker.gameObject.transform;
            Vector3 brigPos = new Vector3((trans.position.x + 52f), trans.position.y, (trans.position.z - 10f));
            moveCamera(new Vector3((brigPos.x - 40), brigPos.y + 10, brigPos.z + 20), 2f);
            rotateCamera(360, 2f);
            zoomCamera(-300f, 2f);
            spellbookOpenBrigidImmune.SetActive(false);
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeInFocus(InterceptAttack));
            PlayFTFSound(fowlerNoise);
            //slide brigid and text out
            //bring up fowler screen which we already have
        }
        else if (curIndex == 26)
        {
            StartRotation();
            StartCoroutine(FadeOutFocus(InterceptAttack));
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //slide savannah in with text bottom
        }
        else if (curIndex == 27)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            //slide brigid in and savannah out
        }
        else if (curIndex == 28)
        {
            // not dialogue on this one
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(brigidCG));
            var temp = Instantiate(silenceGlyph, PlayerManager.marker.gameObject.transform);
            temp.transform.Translate(new Vector3(temp.transform.position.x, temp.transform.position.y + 20f, temp.transform.position.z));
            yield return new WaitForSeconds(2.5f);
            StartCoroutine(FadeInFocus(silencedObject));
            Destroy(temp);
            //SetDialogue();
            //slide brigid out and bring up silenced screen which we have... with a continue button?
        }
        else if (curIndex == 29)
        {
            StartCoroutine(FadeOutFocus(silencedObject));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{him/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_him])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_he]);
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{him/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_she]);
            }
            //slide savannah in with bottom text and next arrow active
        }
        else if (curIndex == 30)
        {
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_his]);
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her]);
            }
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeInFocus(brigidCG));
            //slide brigid in and savannah out
        }
        else if (curIndex == 31)
        {
            //no dialogue on this one
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            var temp = Instantiate(dispelGlyph, PlayerManager.marker.gameObject.transform);
            temp.transform.Translate(new Vector3(temp.transform.position.x, temp.transform.position.y + 20f, temp.transform.position.z));
            yield return new WaitForSeconds(2.5f);
            PlayFTFSound(dispelledNoise);
            StartCoroutine(FadeInFocus(dispelObject));
            Destroy(temp);
            //bring up dispelled screen with continue button active which we have
        }
        else if (curIndex == 32)
        {
            StopRotation();
            StartCoroutine(FadeOutFocus(dispelObject));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //slide savannah in with interupted text on the bottom
        }
        else if (curIndex == 33)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            moveCamera(PlayerManager.marker.gameObject.transform.position, 1f);
            rotSpeed = 50;
            StartRotation();
            zoomCamera(-500, 8f);
            continueButton.SetActive(false);
            LeanTween.value(camRotTransform.localEulerAngles.x, 12, 8).setEase(easeType).setOnUpdate((float v) =>
            {
                camRotTransform.localEulerAngles = new Vector3(v, camRotTransform.localEulerAngles.y, 0);
            }).setOnComplete(() =>
            {
                StopRotation(4);
                zoomCamera(-340, 4f);
                LeanTween.value(camRotTransform.localEulerAngles.x, 20, 4).setEase(easeType).setOnUpdate((float v) =>
                    {
                        camRotTransform.localEulerAngles = new Vector3(v, camRotTransform.localEulerAngles.y, 0);
                    }).setOnComplete(() =>
                    {
                        rotSpeed = 2;
                        //continueButton.SetActive(true);
                    });
            });
            //   tiltCamera(-500, 8f);
            //StartCoroutine (FadeInFocus (brigidCG));
            //brigidMirrors.SetActive (true);
            brigidPrefabAnim.SetBool("fade", true);
            brigidPrefabInstance.transform.GetChild(5).gameObject.SetActive(true);
            yield return new WaitForSeconds(1.8f);
            brigidPrefabInstance.transform.GetChild(1).gameObject.SetActive(false);
            //may remove above line

            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(false);
            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(false);
            //PlayFTFSound(mirrorsNoise);
            StartCoroutine(SpawnMirrors());
            //slide brigid in and savannah out
            //cast mirror thing with models, not icons
        }
        else if (curIndex == 34)
        {
            continueButton.SetActive(true);
            //slide 37
            //slide savannah in and brigid out
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            //just continued dialogue from savannah on next - explains jump below
        }
        else if (curIndex == 35)
        {
            //slide 38
            dialogueText.text = dialogues[dialogueIndex].Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            //player name here
        }
        else if (curIndex == 36)
        {
            continueButton.SetActive(false);
            ownedBarghestInstance.transform.GetChild(3).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            //StartCoroutine(DestroyMirrors());
            StartCoroutine(RevealMirrors());
            //brigidPrefabInstance.transform.GetChild (1).gameObject.SetActive (true);
            //brigidPrefabAnim.SetBool("reappear",true);
            yield return new WaitForSeconds(2f);
            //brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(true);

            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(2).gameObject.SetActive(true);
            //trueSight.SetActive(false);
            //more savannah text and then play the truesight vfx
            //then play the shadow vfx on the real brigid
            yield return new WaitForSeconds(2f);
            brigidPrefabInstance.transform.GetChild(1).gameObject.SetActive(true);
            brigidPrefabAnim.SetBool("reappear", true);
            yield return new WaitForSeconds(0.5f);
            continueButton.SetActive(true);

        }
        else if (curIndex == 37)
        {
            StartRotation();
            StartCoroutine(DestroyMirrors());
            var td = Instantiate(twilightDusk, PlayerManager.marker.gameObject.transform);
            //slide savannah out here, or do it somewhere in the coroutine
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            yield return new WaitForSeconds(1.8f);
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(false);
            DeathState.Instance.FTFDeathState(true);
            PlayerDataManager.playerData.energy = 0;
            PlayerManagerUI.Instance.UpdateEnergy();

            yield return new WaitForSeconds(3.2f);
            Destroy(td);
            StartCoroutine(FadeInFocus(deathMsg));
            //show spell from brigid and then bring up death screen
        }
        else if (curIndex == 38)
        {
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeOutFocus(deathMsg));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //slide savannah in with bottom text and arrow enabled
        }
        else if (curIndex == 39)
        {
            //brigidMirrors.SetActive (false);
            //slide savannah in with bottom text and arrow enabled
        }
        else if (curIndex == 40)
        {
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));

            DeathState.Instance.FTFDeathState(false);
            Blessing bs = new Blessing();
            bs.daily = PlayerDataManager.playerData.baseEnergy;
            PlayerDataManager.playerData.blessing = bs;
            var bless = Instantiate(blessingParticle, PlayerManager.marker.gameObject.transform);
            yield return new WaitForSeconds(3f);
            Destroy(bless);
            PlayerManagerUI.Instance.ShowBlessing();
            PlayerDataManager.playerData.energy = PlayerDataManager.playerData.baseEnergy;
            PlayerManagerUI.Instance.UpdateEnergy();
            StartCoroutine(FadeOutFocus(savannahCG));

            //forcing continue here.
            StartCoroutine(ForceContinue());
            //display grey hand coven message with energy gift
        }
        else if (curIndex == 41)
        {
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));

        }
        else if (curIndex == 42)
        {
            dialogueCG.gameObject.SetActive(false);
            StartCoroutine(FadeOutFocus(savannahCG));
            //StartCoroutine(FadeOutFocus(dialogueCG));
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);

            //brigidPrefabInstance.transform.GetChild (5).gameObject.SetActive (true);
            PlayFTFSound(banishSound);
            yield return new WaitForSeconds(1f);
            brigidPrefabAnim.SetBool("banish", true);
            yield return new WaitForSeconds(2f);
            //StartCoroutine (FadeInFocus (savannahCG));
            brigidBanishMsg.SetActive(true);
            StartCoroutine(FadeInFocus(brigidBanishMsgCG));
            dialogueCG.gameObject.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
            Destroy(brigidPrefabInstance);
            //slide savannah in with bottom text and arrow enabled
        }
        else if (curIndex == 43)
        {
            StartCoroutine(FadeOutFocus(brigidBanishMsgCG));
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeInFocus(savannahCG));
            //StartCoroutine (FadeInFocus (brigidCG));
            StartCoroutine(FadeInFocus(highlight9));
            continueButton.SetActive(false);
            //slide brigid in and savannah out
            //replace player name with your name
        }
        else if (curIndex == 44)
        {
            StopRotation();
            //slide 46
            brigidBanishMsg.SetActive(false);
            StartCoroutine(FadeOutFocus(highlight9));
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            continueButton.SetActive(true);
            storePrefab.SetActive(true);
            //slide savannah in and brigid out
            //disable next button and highlight store
        }
        else if (curIndex == 45)
        {
            //slide 47
            StartCoroutine(FadeOutFocus(highlight9));
            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            continueButton.SetActive(true);
            storePrefab.SetActive(true);
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_him])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_he]);
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_she]);
            }
            //slide savannah in and brigid out
            //disable next button and highlight store
        }
        else if (curIndex == 46)
        {
            //slide 48
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_him])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_he]);
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", DownloadedAssets.localizedText[LocalizationManager.ftf_her])
                    .Replace("{{he/she}}", DownloadedAssets.localizedText[LocalizationManager.ftf_she]);
            }

        }
        else if (curIndex == 47)
        {
            //highlight ingredients button
            //slide out text box and savannah
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeInFocus(highlight10));
            //no dialogue
        }
        else if (curIndex == 48)
        {
            //change store screen to ingredients and highlight abondia's best
            StartCoroutine(FadeOutFocus(highlight10));
            LeanTween.alphaCanvas(storePrefab.transform.GetChild(4).GetComponent<CanvasGroup>(), 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    //will have to set this up
                    storePrefab.transform.GetChild(6).gameObject.SetActive(true);
                    StartCoroutine(FadeInFocus(highlight11));
                });

        }
        else if (curIndex == 49)
        {
            StartCoroutine(FadeOutFocus(highlight11));
            buyAbondias.SetActive(true);
            //transition to claim abondia's best
        }
        else if (curIndex == 50)
        {
            //purchase successful for abondia's best
            buyAbondias.SetActive(false);
            abondiaBought.gameObject.SetActive(true);
            LeanTween.alphaCanvas(storePrefab.transform.GetChild(4).GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            storePrefab.transform.GetChild(6).gameObject.SetActive(false);
            LeanTween.alphaCanvas(abondiaBought, 1f, 1f).setEase(LeanTweenType.easeInOutQuad);
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            continueButton.SetActive(true);
            yield return new WaitForSeconds(4f);
            LeanTween.alphaCanvas(abondiaBought, 0f, .5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => { abondiaBought.gameObject.SetActive(false); });
        }
        else if (curIndex == 51)
        {
            //LeanTween.alphaCanvas (abondiaBought, 0f, 1f).setEase (LeanTweenType.easeInOutQuad).setOnComplete(() => {abondiaBought.gameObject.SetActive (false);});

            if (abondiaBought.gameObject.activeSelf)
                LeanTween.alphaCanvas(abondiaBought, 0f, .5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => { abondiaBought.gameObject.SetActive(false); });
            LeanTween.alphaCanvas(storePrefab.GetComponent<CanvasGroup>(), 0f, 0.5f).setOnComplete(() =>
            {
                storePrefab.SetActive(false);
            });
            //storePrefab.SetActive(false);

            StartRotation();
            string tribunal = "";

            //  print("replacing season and days here");
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
            //exit out of store and purchase screen.
            //slide 55
        }
        else if (curIndex == 54)
        {
            //show witch school screen here..
            //chooseSchool.gameObject.SetActive(true);
            StartCoroutine(FadeInFocus(chooseSchool));
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            brigidPrefab.SetActive(false);
            Destroy(ownedBarghestInstance);
            StopRotation();
        }

        yield return null;
    }

    private void PlayFTFSound(AudioClip clip)
    {
        soundSource.clip = clip;
        soundSource.Play();
    }

    IEnumerator SpawnMirrors()
    {
        mirrorsInstance = Utilities.InstantiateObject(mirrors, PlayerManager.marker.gameObject.transform);
        var mT = mirrorsInstance.transform;
        var mPrefab = mT.GetChild(0).gameObject;
        for (int i = 0; i < 20; i++)
        {
            var m = Utilities.InstantiateObject(mPrefab, mT);
            m.SetActive(true);
            m.transform.Translate(Random.Range(-100, 100.0f), 0, Random.Range(-100, 100.0f));

            //   LeanTween.moveLocal(m, m.transform.position + new Vector3(Random.Range(-40, 20.0f), 0, Random.Range(-40, 40.0f)), Random.Range(20, 25)).setEase(LeanTweenType.easeInOutSine);
            yield return new WaitForSeconds(.5f);
        }
        OnContinue(true);
    }

    IEnumerator DestroyMirrors()
    {
        for (int i = 0; i < mirrorsInstance.transform.childCount; i++)
        {
            if (mirrorsInstance.transform.GetChild(i).GetComponent<Animator>() != null)
                mirrorsInstance.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("out", true);
            yield return new WaitForSeconds(.1f);
        }
        continueButton.SetActive(true);
        Destroy(mirrorsInstance);
    }

    IEnumerator RevealMirrors()
    {
        for (int i = 0; i < mirrorsInstance.transform.childCount; i++)
        {
            //mirrorsInstance.transform.GetChild (i).gameObject.SetActive (false);
            if (mirrorsInstance.transform.GetChild(i).GetComponent<Animator>() != null)
                mirrorsInstance.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("reveal", true);
            else
                mirrorsInstance.transform.GetChild(i).gameObject.SetActive(false);
            yield return new WaitForSeconds(.1f);
        }
        //continueButton.SetActive(true);
    }


    IEnumerator BarghestWildDefeat()
    {
        yield return new WaitForSeconds(1.4f);

        yield return new WaitForSeconds(0.4f);
        LeanTween.scale(wildBarghestInstance, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            Destroy(wildBarghestInstance);

        });
    }

    IEnumerator ForceContinue()
    {
        dialogueIndex = 36;
        //  print("got here");
        yield return new WaitForSeconds(1);
        //   print("got here");
        OnContinue(true);

    }


    IEnumerator CastingHexAnimation()
    {
        spellbookOpenBrigidCast.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        spellbookOpenBrigid.SetActive(false);
        spellbookOpenBrigidImmune.SetActive(true);
        //might have to shift these values around a bit (WFS)
        LeanTween.alphaCanvas(spellbookOpenBrigidCast.transform.GetChild(2).GetComponent<CanvasGroup>(), 1f, 1.2f);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(FadeInFocus(highlight8));
    }

    public void EndFTF()
    {
        LeanTween.alphaCanvas(statsScreen, 0f, 1f).setOnComplete(() =>
        {

            Destroy(daddy);
            camRotTransform.localEulerAngles = new Vector3(20, 0, 0);
            LoginUIManager.isInFTF = false;
            Destroy(camCenterPoint.GetChild(0));
            cameraTransform.localPosition = new Vector3(0, 0, -300);
            MarkerManagerAPI.GetMarkers(true);
            APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
            {

                //			Debug.Log(s + " FTF RES");
                LoginAPIManager.FTFComplete = true;
                APIManager.Instance.GetData("character/get", (string ss, int rr) =>
                {
                    //   print("reinit");
                    var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
                    PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                    LoginAPIManager.loggedIn = true;
                    PlayerManager.Instance.initStart();
                    ChatUI.Instance.SetChatInteraction(true);

                    Utilities.allowMapControl(true);

                });
            });
        });
    }


    public void chooseSchoolResult(bool isSchool)
    {
        if (isSchool)
        {
            StartCoroutine(FadeOutFocus(chooseSchool));
            //  ContinueToGame();
            WitchSchoolManager.Instance.Open();
            EndFTF();
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
        // camRotTransform.localEulerAngles
        // GetComponent<Image>().raycastTarget = false;
        LoginAPIManager.isInFTF = false;
        MarkerManagerAPI.GetMarkers(true);
        APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
        {
            //			Debug.Log(s + " FTF RES");
            LoginAPIManager.FTFComplete = true;
            APIManager.Instance.GetData("character/get", (string ss, int rr) =>
            {
                //     print("reinit");
                var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
                PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                LoginAPIManager.loggedIn = true;
                PlayerManager.Instance.initStart();
                Utilities.allowMapControl(true);
            });
        });

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
        moreInfoButton.SetActive(false);
    }

    void SetDialogue()
    {
        dialogueText.text = dialogues[dialogueIndex];
        //dialogueText.text = dialogues[dialogueIndex];
        if (curIndex != 0)
            ButtonPress();
        ShowFX();
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
                //  print("Disabling Interaction");
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



    #endregion

}
