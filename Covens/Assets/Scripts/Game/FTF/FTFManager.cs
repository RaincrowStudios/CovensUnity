using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using Raincrow.Maps;

public class FTFManager : MonoBehaviour
{
    public static FTFManager Instance { get; set; }



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
    public AudioClip truesight;
    public AudioClip fowlerNoise;
    public AudioClip dispelledNoise;
    public AudioClip mirrorsNoise;
    public AudioClip banishSound;
    public AudioClip brigidLandGuitar;
    public AudioSource soundSource;
    private PlayerCompass playerCompass;

    public Image energyRing;
    private float zoomMulti = 3f;

    public GameObject gypsyHandPrefab;
    public RectTransform gypsyHandInstance;
    public CanvasGroup gypsyHandCG;

    public RectTransform[] pointerArray = new RectTransform[11];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerCompass = PlayerCompass.instance;
        soundSource = gameObject.AddComponent<AudioSource>();
        ChatUI.Instance.SetChatInteraction(false);
        cameraTransform = MapsAPI.Instance.camera.transform;
        camRotTransform = cameraTransform.parent;
        camCenterPoint = cameraTransform.parent.parent;
        Utilities.allowMapControl(false);
        currentDominion.text = LocalizeLookUp.GetText("dominion_location") + " " + PlayerDataManager.config.dominion;
        strongestWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + PlayerDataManager.config.strongestWitch;
        strongestCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + PlayerDataManager.config.strongestCoven;
        dialogues = DownloadedAssets.ftfDialogues;
        //StartRotation();
        MapCameraUtils.SetRotation(-180f, 90f, true, () => { });
        zoomCamera(-360f, 60f);
        UIStateManager.Instance.CallWindowChanged(true);
        LoginUIManager.Instance.mainUI.SetActive(true);

        InventoryItems iC = new InventoryItems();
        iC.id = "coll_ironCollar";
        iC.rarity = 1;
        iC.count = 1;
        iC.displayName = "Iron Collar";
        //PlayerDataManager.playerData.ingredients.tools.Add(iC);
        PlayerDataManager.playerData.ingredients.toolsDict.Add(iC.id, iC);

        silencedObject.transform.GetChild(1).GetComponent<Text>().text = LocalizeLookUp.GetText("ftf_silenced_by");
        dispelObject.transform.GetChild(1).GetComponent<Text>().text = LocalizeLookUp.GetText("ftf_silence_dispel");

        gypsyHandInstance = Utilities.InstantiateUI(gypsyHandPrefab, transform).GetComponent<RectTransform>();
        gypsyHandInstance.GetComponent<Image>().SetNativeSize();
        gypsyHandCG = gypsyHandInstance.GetComponent<CanvasGroup>();
        gypsyHandInstance.transform.localScale = new Vector3(400f, 400f);
        gypsyHandInstance.anchoredPosition = new Vector3(60f, -125f);
        Throb();
    }

    void rotateCamera(float endValue, float time)
    {
        //LeanTween.cancel(camRotTransform.gameObject);
        LeanTween.cancel(camCenterPoint.gameObject);

        LeanTween.rotateY(camCenterPoint.gameObject, endValue, time).setEase(easeType).setOnUpdate((float f) =>
        {
            MapsAPI.Instance.OnCameraUpdate?.Invoke(false, false, false);
            playerCompass.FTFCompass(camCenterPoint.localEulerAngles.y);
        });
        /*LeanTween.rotateY(camRotTransform.gameObject, endValue, time).setEase(easeType).setOnUpdate((float f) =>
        {
            playerCompass.FTFCompass(camRotTransform.localEulerAngles.y);
        });*/
    }

    void zoomCamera(float endValue, float time)
    {
        LeanTween.cancel(cameraTransform.gameObject);
        LeanTween.moveLocalZ(cameraTransform.gameObject, endValue * zoomMulti, time).setEase(easeType).setOnUpdate((float f) =>
        {
            MapsAPI.Instance.OnCameraUpdate?.Invoke(false, false, false);
        });
    }

    void moveCamera(Vector3 endPos, float time, System.Action onComplete = null)
    {
        LeanTween.cancel(camCenterPoint.gameObject);
        LeanTween.move(camCenterPoint.gameObject, endPos, time).setEase(easeType).setOnUpdate((float f) => { MapsAPI.Instance.OnCameraUpdate?.Invoke(false, false, false); }).setOnComplete(() =>
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
        //PlayFTFSound (savannahSpell);
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

        //   Debug.Log(dialogueIndex);

        if (curIndex == 1)
        {
            //StopRotation();
            //StartCoroutine (FadeOutFocus (highlight1));

            Transform trans = PlayerManager.marker.gameObject.transform;
            wildBarghestInstance = Utilities.InstantiateObject(wildBarghest, trans, 0);
            PlayFTFSound(barghestHowl);
            wildBarghestInstance.transform.Translate(new Vector3((trans.position.x - 70f), trans.position.y, (trans.position.z + 70f)));
            LeanTween.scale(wildBarghestInstance, Vector3.one, .5f).setEase(easeType);
            //StopRotation();
            zoomCamera(-180, 3.2f);
            MapCameraUtils.SetRotation(225, 3.2f, true, () => { });
            moveCamera(wildBarghestInstance.transform.position, 3.2f);
            yield return new WaitForSeconds(.3f);
            //MapCameraUtils.FocusOnTarget(wildBarghestInstance.GetComponent<MuskMarker>());
            //MapCameraUtils.
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
            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);
        }
        else if (curIndex == 4)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            StartCoroutine(FadeOutFocus(highlight2));

            zoomCamera(-210, 2.4f);
            //MapCameraUtils.FocusOnTargetCenter(wildBarghestInstance.GetComponent<MuskMarker>(), 2.4f);
            moveCamera(new Vector3(wildBarghestInstance.transform.position.x - 30f, wildBarghestInstance.transform.position.y, +wildBarghestInstance.transform.position.z), 2.4f);
            //MapCameraUtils.SetRotation(-90, 2.4f, true, () => { });
            wildBarghestInstance.transform.GetChild(2).gameObject.SetActive(false);
            spellbookOpenBarghest.SetActive(true);
            StartCoroutine(SavannahSpellBarghest());

        }
        else if (curIndex == 5)
        {
            //dialogueMidText.text = dialogueText.text;
            //			wildBarghest.transform.GetChild (0).gameObject.SetActive (true);

            //CHANGING THE TEXT

            energyRing = wildBarghestInstance.transform.GetChild(4).GetComponent<Image>();
            TextMeshPro energy = wildBarghestInstance.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
            TextMeshProUGUI energy2 = spellbookOpenBarghest.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>();
            energyRing.fillAmount = 1f;
            LeanTween.value(1f, 0.12f, 1f).setOnUpdate((float f) =>
            {
                energyRing.fillAmount = f;
            });
            LeanTween.value(440, 36, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                energy.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <b><color=#4C80FD>" + f.ToString() + "</color></b>";
                energy2.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <color=black>" + f.ToString();
            });
            yield return new WaitForSeconds(1.2f);
            continueButton.SetActive(false);
            StartCoroutine(FadeInFocus(dialogueCG));
            //will replace this
            yield return new WaitForSeconds(4f);

            StartCoroutine(FadeOutFocus(dialogueCG));

            yield return new WaitForSeconds(1.2f);
            gypsyHandInstance.position = pointerArray[0].position;
            gypsyHandInstance.localScale = new Vector3(gypsyHandInstance.localScale.x, -gypsyHandInstance.localScale.y);
            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);
            StartCoroutine(FadeInFocus(highlight3));
            //take next button way
            //highlight button to go to white spells in spell book

            //do a cinematic for moving to the next page
        }
        else if (curIndex == 6)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            PlayFTFSound(openSpellbook);
            StartCoroutine(FadeOutFocus(highlight3));
            StartCoroutine(FadeOutFocus(dialogueCG));
            wildBarghestInstance.transform.GetChild(3).gameObject.SetActive(false);
            //			wildBarghest.transform.GetChild (0).gameObject.SetActive (false);
            spellbookOpenWFBarghest.SetActive(true);
            //spellbookOpenBarghest.SetActive (false);
            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeInFocus(highlight4));
            gypsyHandInstance.position = pointerArray[1].position;
            gypsyHandInstance.localScale = new Vector3(gypsyHandInstance.localScale.x, -gypsyHandInstance.localScale.y);

            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);

            spellbookOpenBarghest.SetActive(false);
        }
        else if (curIndex == 7)
        {
            PlayFTFSound(openSpellbook);
            var q = SpiritDiscoveredBarghest.GetComponentInParent<CanvasGroup>();
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            q.alpha = 1f;
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
            LeanTween.value(0.12f, 0f, 1f).setOnUpdate((float f) =>
            {
                energyRing.fillAmount = f;
            });
            LeanTween.value(36, 0, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                energy.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <b><color=#4C80FD>" + f.ToString() + "</color></b>";
                energy2.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <color=black>" + f.ToString();
            });
            StartCoroutine(BarghestWildDefeat());
            moveCamera(PlayerManager.marker.gameObject.transform.position, 2f);
            yield return new WaitForSeconds(1f);
            //StartRotation();
            SpiritDiscoveredBarghest.SetActive(true);
            yield return new WaitForSeconds(5.2f);
            MapCameraUtils.SetRotation(45, 75f, true, () => { });
            StartCoroutine(FadeOutFocus(q));
            //SpiritDiscoveredBarghest.SetActive(false);
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
            continueButton.SetActive(false);
            spellbookOpenBarghestOnCast.SetActive(false);


            //spirit bood end animation here
            //spiritDeckAnim.SetBool("SpiritDeckClose");
            spiritDeckAnim.SetBool("SpiritDeckClose", true);
            //spiritDeck.SetActive (false);
            yield return new WaitForSeconds(1f);
            gypsyHandInstance.position = pointerArray[2].position;
            gypsyHandInstance.localScale = new Vector3(-gypsyHandInstance.localScale.x, gypsyHandInstance.localScale.y);

            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);
            StartCoroutine(FadeInFocus(highlight5));
            //highlight summoning button
            //this is already done
        }
        else if (curIndex == 11)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);


            spiritDeck.SetActive(false);
            ShowSummoning();
            PlayFTFSound(openSpellbook);
            ownedBarghest.SetActive(true);

            MapCameraUtils.SetRotation(45f, 1f, true, () => { });
            //slide 13
            yield return new WaitForSeconds(.5f);
            gypsyHandInstance.position = pointerArray[3].position;
            gypsyHandInstance.localScale = new Vector3(-gypsyHandInstance.localScale.x, gypsyHandInstance.localScale.y);
            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);


        }
        else if (curIndex == 12)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);


            continueButton.SetActive(false);
            summonButton.SetActive(false);
            moreInfoButton.SetActive(false);
            //StopRotation();
            //back to map and add a portal
            Debug.Log("summoning barghest");
            StartCoroutine(FadeOutFocus(highlightSummonScreen));

            //MapsAPI.Instance.position = PlayerDataManager.playerPos;
            //SummoningManager.Instance.FTFCastSummon();
            SummoningController.Instance.Close();
            yield return new WaitForSeconds(1.5f);
            gypsyHandInstance.position = pointerArray[4].position;

            //SummoningManager.Instance.Close ();
            Transform trans = PlayerManager.marker.gameObject.transform;
            ownedBarghestInstance = Utilities.InstantiateObject(ownedBarghest, trans);
            ownedBarghestInstance.transform.Translate(new Vector3((trans.position.x - 56f), trans.position.y, (trans.position.z - 56f)));
            moveCamera(ownedBarghestInstance.transform.position, 2f);
            zoomCamera(-180, 2f);
            //Invoke("SpawnBarghestSummon", 5);

            //		SoundManagerOneShot.Instance.MenuSound ();
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //OnContinue();
            //SpawnPortal();

            yield return new WaitForSeconds(4f);
            zoomCamera(-220, 2f);
            //StartRotation();
            MapCameraUtils.SetRotation(225, 90f, true, () => { });
            //this continue needs to be delayed
            continueButton.SetActive(true);

        }
        else if (curIndex == 15)
        {

            continueButton.SetActive(false);
            //    Debug.Log(dialogues[dialogueIndex]);
            dialogueText.text = dialogues[dialogueIndex].Replace("{{Location}}", "<color=#FF8400>" + PlayerDataManager.playerData.dominion + "</color>");
            //      Debug.Log(dialogueText.text);
            //brigidPrefab.SetActive (true);
            //continueButton.SetActive(false);
            Transform trans = PlayerManager.marker.gameObject.transform;
            Vector3 brigPos = new Vector3((trans.position.x + 30f), (trans.position.y + 20f), (trans.position.z - 10f));

            //moveCamera(brigPos, 2f);
            //MapCameraUtils.FocusOnTargetCenter(brigidPrefabInstance.GetComponent<MuskMarker>());
            //rotateCamera(390, 2f);
            MapCameraUtils.SetRotation(390, 2f, true, () => { });
            zoomCamera(-250f, 2f);
            //MapCameraUtils.FocusOnPosition(brigPos, -250, true, 2f);
            moveCamera(brigPos, 2f);
            yield return new WaitForSeconds(2f);

            brigidPrefabInstance = Utilities.InstantiateObject(brigidPrefab, trans);
            //MapCameraUtils.FocusOnTargetCenter(brigidPrefabInstance.GetComponent<MuskMarker>(), 2f);
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
            yield return new WaitForSeconds(.5f);
            continueButton.SetActive(true);
            //MapCameraUtils.SetRotation(210f, 60f, true, () => { });
            //MapCameraUtils.FocusOnTarget(PlayerManager.marker, 2f);


        }
        else if (curIndex == 16)
        {
            MapCameraUtils.SetRotation(0f, 15f, true, () => { });

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
            yield return new WaitForSeconds(1.2f);
        }
        else if (curIndex == 19)
        {
            continueButton.SetActive(false);
            gypsyHandInstance.position = pointerArray[5].position;


            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);

            StartCoroutine(FadeOutFocus(brigidCG));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(highlight6));
            //highlight6.transform.GetChild(0).GetComponent<Button>().enabled = true;
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(true);


        }
        else if (curIndex == 20)
        {
            continueButton.SetActive(false);
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            //StopRotation();
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(false);
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeOutFocus(highlight6));
            Vector3 npos = brigidPrefabInstance.transform.position;
            npos.x += 26;
            npos.y += 35;
            rotateCamera(40, 1.6f);
            zoomCamera(-180, 1.6f);
            //MapCameraUtils.FocusOnTargetCenter(brigidPrefabInstance.GetComponent<MuskMarker>(), 1.6f);
            moveCamera(npos, 1.6f);
            yield return new WaitForSeconds(1.6f);


            spellbookOpenBrigid.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
            yield return new WaitForSeconds(1f);
            gypsyHandInstance.position = pointerArray[6].position;


            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);
            StartCoroutine(FadeInFocus(highlight7));

            //slide 23
        }
        else if (curIndex == 21)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(highlight7));
            StartCoroutine(FadeOutFocus(dialogueCG));
            brigidPrefabInstance.transform.GetChild(4).gameObject.SetActive(true);
            StartCoroutine(CastingHexAnimation());
            yield return new WaitForSeconds(1f);
            gypsyHandInstance.position = new Vector3(pointerArray[0].position.x, (pointerArray[0].position.y - .5f));
            gypsyHandInstance.localScale = new Vector3(gypsyHandInstance.localScale.x, -gypsyHandInstance.localScale.y);
            //TextMeshPro energy = brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<TextMeshPro>();
            TextMeshProUGUI energy2 = spellbookOpenBrigidImmune.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>();
            LeanTween.value(22244, 22224, 1f).setOnUpdate((float f) =>
            {
                f = (int)f;
                //energy.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <b><color=#F48D00>" + f.ToString() + "</color></b>\nlvl: <b><color=#F48D00>8</color></b>";
                energy2.text = LocalizeLookUp.GetText(LocalizationManager.lt_energy) + " <color=black>" + f.ToString();
            }).setOnComplete(() => { LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f); });

            continueButton.SetActive(true);
        }
        else if (curIndex == 22)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            StartCoroutine(FadeOutFocus(highlight8));
            spellbookOpenBrigidCastOnCastOut.SetBool("OnCastOut", true);
            yield return new WaitForSeconds(2f);
            gypsyHandInstance.localScale = new Vector3(gypsyHandInstance.localScale.x, -gypsyHandInstance.localScale.y);

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
            //moveCamera(new Vector3((brigPos.x - 40), brigPos.y + 10, brigPos.z + 20), 2f);
            MapCameraUtils.FocusOnTargetCenter(PlayerManager.marker, 2f);
            //rotateCamera(360, 2f);
            spellbookOpenBrigidImmune.SetActive(false);
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeInFocus(InterceptAttack));
            PlayFTFSound(fowlerNoise);
            //slide brigid and text out
            //bring up fowler screen which we already have
            yield return new WaitForSeconds(2.5f);
            zoomCamera(-300f, 5f);
            MapCameraUtils.SetRotation(180, 45, true, () => { });

        }
        else if (curIndex == 26)
        {
            //StartRotation();
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
            temp.transform.Translate(new Vector3(temp.transform.position.x, temp.transform.position.y + 50f, temp.transform.position.z));
            Destroy(temp, 3.5f);
            yield return new WaitForSeconds(2.5f);
            StartCoroutine(FadeInFocus(silencedObject));
            yield return new WaitForSeconds(1f);
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
                dialogueText.text = dialogueText.text.Replace("{{him/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_him))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_he));
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{him/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_her))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_she));
            }
            //slide savannah in with bottom text and next arrow active
        }
        else if (curIndex == 30)
        {
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_his));
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_her));
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
            Destroy(temp, 3.5f);
            temp.transform.Translate(new Vector3(temp.transform.position.x, temp.transform.position.y + 50f, temp.transform.position.z));
            yield return new WaitForSeconds(2.5f);
            //PlayFTFSound(dispelledNoise);
            StartCoroutine(FadeInFocus(dispelObject));
            yield return new WaitForSeconds(1f);
            //bring up dispelled screen with continue button active which we have
        }
        else if (curIndex == 32)
        {
            //StopRotation();
            StartCoroutine(FadeOutFocus(dispelObject));
            StartCoroutine(FadeInFocus(savannahCG));
            StartCoroutine(FadeInFocus(dialogueCG));
            //slide savannah in with interupted text on the bottom
            MapCameraUtils.SetRotation(90f, 3f, true, () => { });
            zoomCamera(-340f, 3f);
        }
        else if (curIndex == 33)
        {
            continueButton.SetActive(false);
            StartCoroutine(FadeOutFocus(savannahCG));
            moveCamera(PlayerManager.marker.gameObject.transform.position, 1f);
            //rotSpeed = 50;
            //StartRotation();
            brigidPrefabAnim.SetBool("fade", true);
            brigidPrefabInstance.transform.GetChild(5).gameObject.SetActive(true);
            yield return new WaitForSeconds(1.8f);
            brigidPrefabInstance.transform.GetChild(1).gameObject.SetActive(false);
            //may remove above line

            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(false);
            brigidPrefabInstance.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            brigidPrefabInstance.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            brigidPrefabInstance.transform.GetChild(6).gameObject.SetActive(false);
            // brigidPrefabInstance.transform.GetChild(6).gameObject.SetActive(false);
            //PlayFTFSound(mirrorsNoise);
            StartCoroutine(SpawnMirrors());
            zoomCamera(-400f, 8f);
            MapCameraUtils.SetRotation(200, 8f, true, () => { });
            yield return new WaitForSeconds(8.2f);
            zoomCamera(-180f, 8f);
            MapCameraUtils.SetRotation(380f, 8f, true, () => { });
            yield return new WaitForSeconds(8.2f);

            /*
            LeanTween.value(camRotTransform.localEulerAngles.x, 12, 15).setEase(easeType).setOnUpdate((float v) =>
            {
                camRotTransform.localEulerAngles = new Vector3(v, camRotTransform.localEulerAngles.y, 0);
            }).setOnComplete(() =>
            {
                //StopRotation(4);
                zoomCamera(-260, 15f);
                LeanTween.value(camRotTransform.localEulerAngles.x, 20, 15).setEase(easeType).setOnUpdate((float v) =>
                {
                    camRotTransform.localEulerAngles = new Vector3(v, camRotTransform.localEulerAngles.y, 0);
                }).setOnComplete(() =>
                {
                    rotSpeed = 2;
                    //continueButton.SetActive(true);
                });
            });
            */

            //   tiltCamera(-500, 8f);
            //StartCoroutine (FadeInFocus (brigidCG));
            //brigidMirrors.SetActive (true);

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
            ownedBarghestInstance.transform.GetChild(3).position = PlayerManager.marker.gameObject.transform.position;
            ownedBarghestInstance.transform.GetChild(3).gameObject.SetActive(true);
            zoomCamera(-250f, 5f);
            MapCameraUtils.SetRotation(180f, 5f, true, () => { });
            moveCamera(new Vector3(camCenterPoint.position.x, camCenterPoint.position.y + 36f, camCenterPoint.position.z), 5f);
            yield return new WaitForSeconds(0.1f);
            //StartCoroutine(DestroyMirrors());
            PlayFTFSound(truesight);
            StartCoroutine(RevealMirrors());
            //brigidPrefabInstance.transform.GetChild (1).gameObject.SetActive (true);
            //brigidPrefabAnim.SetBool("reappear",true);
            yield return new WaitForSeconds(2f);
            //brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(true);



            //trueSight.SetActive(false);
            //more savannah text and then play the truesight vfx
            //then play the shadow vfx on the real brigid
            yield return new WaitForSeconds(2f);
            brigidPrefabInstance.transform.GetChild(1).gameObject.SetActive(true);
            brigidPrefabAnim.SetBool("reappear", true);
            yield return new WaitForSeconds(0.5f);
            continueButton.SetActive(true);
            brigidPrefabInstance.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
            brigidPrefabInstance.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
            brigidPrefabInstance.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
            brigidPrefabInstance.transform.GetChild(6).gameObject.SetActive(true);
        }
        else if (curIndex == 37)
        {
            //StartRotation();
            StartCoroutine(DestroyMirrors());
            var td = Instantiate(twilightDusk, PlayerManager.marker.gameObject.transform);
            Destroy(td, 5f);
            //slide savannah out here, or do it somewhere in the coroutine
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            yield return new WaitForSeconds(1.8f);
            brigidPrefabInstance.transform.GetChild(2).gameObject.SetActive(false);
            DeathState.Instance.FTFDeathState(true);
            PlayerDataManager.playerData.energy = 0;
            PlayerManagerUI.Instance.UpdateEnergy();
            yield return new WaitForSeconds(3.2f);
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
            Destroy(bless, 3f);
            yield return new WaitForSeconds(3f);
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
            Destroy(brigidPrefabInstance, 3f);
            dialogueCG.gameObject.SetActive(false);
            StartCoroutine(FadeOutFocus(savannahCG));
            //StartCoroutine(FadeOutFocus(dialogueCG));
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);

            //brigidPrefabInstance.transform.GetChild (5).gameObject.SetActive (true);

            yield return new WaitForSeconds(1f);
            PlayFTFSound(banishSound);
            brigidPrefabAnim.SetBool("banish", true);
            yield return new WaitForSeconds(2f);
            //StartCoroutine (FadeInFocus (savannahCG));
            brigidBanishMsg.SetActive(true);
            StartCoroutine(FadeInFocus(brigidBanishMsgCG));
            dialogueCG.gameObject.SetActive(true);
            StartCoroutine(FadeInFocus(dialogueCG));
            //slide savannah in with bottom text and arrow enabled
        }
        else if (curIndex == 43)
        {
            continueButton.SetActive(false);
            StartCoroutine(FadeOutFocus(brigidBanishMsgCG));
            gypsyHandInstance.position = pointerArray[7].position;


            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);



            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            StartCoroutine(FadeInFocus(savannahCG));
            //StartCoroutine (FadeInFocus (brigidCG));
            StartCoroutine(FadeInFocus(highlight9));
            //slide brigid in and savannah out
            //replace player name with your name
        }
        else if (curIndex == 44)
        {
            //StopRotation();
            //slide 46
            //StartCoroutine(FadeOutFocus(savannahCG));
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);
            brigidBanishMsg.SetActive(false);
            StartCoroutine(FadeOutFocus(highlight9));
            StartCoroutine(FadeOutFocus(brigidCG));
            //StartCoroutine(FadeInFocus(savannahCG));
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
            //StartCoroutine(FadeInFocus(savannahCG));
            continueButton.SetActive(true);
            storePrefab.SetActive(true);
            dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            if (PlayerDataManager.playerData.male)
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_his))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_he));
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_her))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_she));
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
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_him))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_he));
            }
            else
            {
                dialogueText.text = dialogueText.text.Replace("{{his/her}}", LocalizeLookUp.GetText(LocalizationManager.ftf_her))
                    .Replace("{{he/she}}", LocalizeLookUp.GetText(LocalizationManager.ftf_she));
            }

        }
        else if (curIndex == 47)
        {
            //highlight ingredients button
            //slide out text box and savannah
            //Debug.Log("about to pass");
            //dialogueText.text = dialogueText.text.Replace("{{Player Name}}", PlayerDataManager.playerData.displayName);
            //Debug.Log ("passed");
            gypsyHandInstance.position = pointerArray[8].position;



            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);

            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            StartCoroutine(FadeInFocus(highlight10));
            //no dialogue
        }
        else if (curIndex == 48)
        {
            //change store screen to ingredients and highlight abondia's best

            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

            StartCoroutine(FadeOutFocus(highlight10));
            LeanTween.alphaCanvas(storePrefab.transform.GetChild(4).GetComponent<CanvasGroup>(), 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                //will have to set this up
                storePrefab.transform.GetChild(6).gameObject.SetActive(true);
                StartCoroutine(FadeInFocus(highlight11));
                gypsyHandInstance.position = pointerArray[9].position;


                LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);

            });

        }
        else if (curIndex == 49)
        {
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);
            yield return new WaitForSeconds(.5f);
            gypsyHandInstance.position = pointerArray[10].position;


            StartCoroutine(FadeOutFocus(highlight11));
            buyAbondias.SetActive(true);
            //transition to claim abondia's best
            LeanTween.alphaCanvas(gypsyHandCG, 1f, .5f);

        }
        else if (curIndex == 50)
        {
            //purchase successful for abondia's best
            LeanTween.alphaCanvas(gypsyHandCG, 0f, .5f);

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

            //StartRotation();
            string tribunal = "";

            //  Debug.Log("replacing season and days here");
            if (PlayerDataManager.config.tribunal == 1)
            {
                tribunal = LocalizeLookUp.GetText(LocalizationManager.ftf_summer);
            }
            else if (PlayerDataManager.config.tribunal == 2)
            {
                tribunal = LocalizeLookUp.GetText(LocalizationManager.ftf_spring);
            }
            else if (PlayerDataManager.config.tribunal == 3)
            {
                tribunal = LocalizeLookUp.GetText(LocalizationManager.ftf_autumn);
            }
            else
            {
                tribunal = LocalizeLookUp.GetText(LocalizationManager.ftf_winter);
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

            zoomCamera(-360f, .5f);
            StartCoroutine(FadeOutFocus(savannahCG));
            StartCoroutine(FadeOutFocus(dialogueCG));
            brigidPrefab.SetActive(false);
            Destroy(ownedBarghestInstance);
            //StopRotation();

            EndFTF();
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
        for (int i = 0; i < 12; i++)
        {
            var m = Utilities.InstantiateObject(mPrefab, mT);
            m.SetActive(true);
            m.transform.Translate(Random.Range(-200, 200.0f), 0, Random.Range(-200, 200.0f));

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
            yield return new WaitForSeconds(.08f);
        }
        continueButton.SetActive(true);
        yield return new WaitForSeconds(0.5f);
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
        //  Debug.Log("got here");
        yield return new WaitForSeconds(1);
        //   Debug.Log("got here");
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

    void Throb()
    {
        LeanTween.size(gypsyHandInstance, new Vector2(1.1f, 1.1f), 1f).setOnComplete(() =>
        {
            LeanTween.size(gypsyHandInstance, new Vector2(.9f, .9f), 1f).setOnComplete(() =>
            {
                Throb();
            });
        });

    }


    public void EndFTF()
    {
        print("end ftf");

        Vector2 physCoords = MapsAPI.Instance.physicalPosition;
        MapsAPI.Instance.InitMap(physCoords.x, physCoords.y, 1, null, false);
        LoginUIManager.isInFTF = false;
        MapCameraUtils.FocusOnPosition(Vector3.zero, 1, false, 1f);
        ChatUI.Instance.SetChatInteraction(true);

        System.Action getCharacter = () => { };
        getCharacter = () =>
        {
            LoginAPIManager.GetCharacter((result, response) =>
            {
                if (response == 200)
                {
                    var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(result);
                    PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
                }
                else
                {
                    getCharacter();
                }
            });
        };

        APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
        {
            LoginAPIManager.FTFComplete = true;
            AppsFlyerAPI.CompletedFTUE();
            Utilities.allowMapControl(true);
            Debug.Log("FTF Complete!");
            MarkerManagerAPI.GetMarkers(physCoords.x, physCoords.y, true, () =>
            {
                getCharacter();
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
            LeanTween.alphaCanvas(statsScreen, 0f, 1f).setOnComplete(() => Destroy(daddy));
        }
        else
        {
            StartCoroutine(FadeOutFocus(chooseSchool));
            StartCoroutine(FadeInFocus(statsScreen));
        }
    }



    //called when clicking the close button in the statScreen
    public void ContinueToGame()
    {
        ////		SummoningManager.Instance.SD.canSwipe = true;
        ////		SummoningManager.Instance.SD.canSwipe = true;
        //StartCoroutine(FadeOutFocus(statsScreen));
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
        //// camRotTransform.localEulerAngles
        //// GetComponent<Image>().raycastTarget = false;
        //LoginAPIManager.isInFTF = false;
        //MarkerManagerAPI.GetMarkers(true);
        //APIManager.Instance.GetData("ftf/complete", (string s, int r) =>
        //{
        //    //			Debug.Log(s + " FTF RES");
        //    LoginAPIManager.FTFComplete = true;
        //    APIManager.Instance.GetData("character/get", (string ss, int rr) =>
        //    {
        //        //     Debug.Log("reinit");
        //        var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(ss);
        //        PlayerDataManager.playerData = LoginAPIManager.DictifyData(rawData);
        //        LoginAPIManager.loggedIn = true;
        //        PlayerManager.Instance.initStart();
        //        Utilities.allowMapControl(true);
        //    });
        //});

        LeanTween.alphaCanvas(statsScreen, 0f, 1f).setOnComplete(() => Destroy(daddy));
        // GetComponent<Image>().raycastTarget = false;
    }

    public void ShowSummoning()
    {
        SoundManagerOneShot.Instance.PlayButtonTap();
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
                //  Debug.Log("Disabling Interaction");
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