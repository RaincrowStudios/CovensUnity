using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
public class LocationUIManager : UIAnimationManager
{
    public static LocationUIManager Instance { get; set; }
    public static string locationID { get; set; }
    public static bool isLocation = false;
    public GameObject locationPrefab;
    GameObject locRune;
    public static string controlledBy = "";
    public List<RectTransform> cards = new List<RectTransform>();

    public GameObject spiritCard;
    public GameObject emptyCard;
    public RectTransform container;
    public GameObject SpiritSummonUI;
    public CanvasGroup[] CG;
    public CanvasGroup[] CGPartial;
    public SpellTraceManager STM;
    public Text requiredTool;
    public Text Desc;
    public Text SummonButtonText;
    public Button SummonButton;

    public Animator locAnim;
    public Text title;
    public Text ownedBy;
    public Text timer;
    public Image timerProgress;
    int counter = 60;
    float[] distances;
    float[] distanceReposition;
    public float snapSpeed = 1;
    public RectTransform center;
    int minButtonNum;
    bool dragging = false;

    int lastNum = 999;

    public GameObject[] EnabledObjects;
    public GameObject spellCanvas;
    public GameObject ingredient;
    public GameObject spellContainer;

    public List<SpriteRenderer> players = new List<SpriteRenderer>();
    public List<GameObject> spirits = new List<GameObject>();
    public Dictionary<string, Token> ActiveTokens = new Dictionary<string, Token>();




    public Sprite maleBlack;
    public Sprite maleWhite;
    public Sprite maleAsian;

    public Sprite femaleWhite;
    public Sprite femaleAsian;
    public Sprite femaleBlack;



    public Image closeButton;
    public GameObject boundText;

    public CanvasGroup[] DisableInteraction;

    public Vector2 ini;
    Vector2 final;
    public bool isSummon = false;

    void Awake()
    {
        Instance = this;
    }

    void OnEnter()
    {
        this.StopAllCoroutines();
        ActiveTokens.Clear();
        players.Clear();
        spirits.Clear();
        foreach (var item in DisableInteraction)
        {
            item.blocksRaycasts = false;
        }
    }

    IEnumerator CountDown()
    {
        while (counter > 0)
        {
            counter--;
            timer.text = counter.ToString();
            timerProgress.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, PlayerDataManager.idleTimeOut, counter));
            yield return new WaitForSeconds(1);
        }
    }

    public void Bind(bool isBind)
    {
        if (isBind)
        {
            closeButton.color = new Color(0, 0, 0, 0);
            closeButton.GetComponent<Button>().enabled = false;
            boundText.SetActive(true);
        }
        else
        {
            closeButton.color = Color.white;
            closeButton.GetComponent<Button>().enabled = true;
            boundText.SetActive(false);
        }
    }

    public void AddToken(Token data)
    {
        data.position--;
        if (data.type == "witch")
        {
            SpriteRenderer sp = players[data.position];
            sp.gameObject.SetActive(true);
            sp.color = Color.white;
            sp.GetComponentInChildren<Text>().text = data.displayName;
            data.Object = sp.gameObject;
            sp.gameObject.GetComponent<LocationTokenData>().token = data;
            if (data.race.Contains("m_"))
            {
                if (data.race.Contains("A"))
                {
                    sp.sprite = maleBlack;
                }
                else if (data.race.Contains("O"))
                {
                    sp.sprite = maleAsian;
                }
                else
                {
                    sp.sprite = maleWhite;
                }
            }
            else
            {
                if (data.race.Contains("A"))
                {
                    sp.sprite = femaleBlack;
                }
                else if (data.race.Contains("O"))
                {
                    sp.sprite = femaleAsian;
                }
                else
                {
                    sp.sprite = femaleWhite;
                }
            }
        }
        else if (data.type == "spirit")
        {
            spirits[data.position].SetActive(true);
            data.Object = spirits[data.position];
            spirits[data.position].GetComponent<LocationTokenData>().token = data;
        }

        ActiveTokens[data.instance] = data;
    }

    public void RemoveToken(string id)
    {
        if (ActiveTokens.ContainsKey(id))
        {
            if (ActiveTokens[id].type == "witch")
            {
                players[ActiveTokens[id].position].gameObject.SetActive(false);
            }
            else
            {
                print("Disabling Spirit at " + ActiveTokens[id].position);
                spirits[ActiveTokens[id].position].SetActive(false);
            }
            ActiveTokens.Remove(id);
        }
        print(controlledBy + "  " + controlledBy.Length);
        if (controlledBy == "")
        {
            print("Location Controlled by No one");
            foreach (var item in ActiveTokens)
            {
                if (item.Value.type == "spirit")
                {
                    print("Spirit Found");
                    return;
                }
            }
            print("Token Remove Activate No spirit");
            locRune.GetComponent<LocationRuneData>().DisableButton(true);
        }
    }

    public void Escape()
    {
        print("Escaping");
        locAnim.Play("out");
        locRune.GetComponent<Animator>().enabled = true;
        locRune.GetComponent<Animator>().SetTrigger("back");
        Destroy(locRune, 1f);
        StartCoroutine(MoveBack());

        PlayerManager.marker.instance.SetActive(true);
        if (PlayerManager.physicalMarker != null)
            PlayerManager.physicalMarker.instance.SetActive(true);
        isSummon = false;
        APIManager.Instance.GetData("/location/leave", ReceiveDataExit);
        foreach (var item in DisableInteraction)
        {
            item.blocksRaycasts = true;
        }
        Utilities.allowMapControl(true);
        print(PlayerDataManager.playerData.state);
        Invoke("ShowDead", 1.4f);
        STM.enabled = false;
        isLocation = false;
    }

    void ShowDead()
    {
        if (PlayerDataManager.playerData.state == "dead")
        {
            DeathState.Instance.ShowDeath();
        }
    }

    void OnEnterLocation(LocationData LD)
    {

        Utilities.allowMapControl(false);
        OnEnter();
        isLocation = true;
        StartCoroutine(CountDown());
        counter = PlayerDataManager.idleTimeOut;
        OnlineMaps.instance.zoom = 16;
        PlayerManager.marker.instance.SetActive(false);
        title.text = MarkerSpawner.SelectedMarker.displayName;
        if (controlledBy != "")
        {
            ownedBy.text = "Owned By : " + controlledBy;
        }
        else
        {
            ownedBy.text = "Unclaimed";
        }
        if (PlayerManager.physicalMarker != null)
            PlayerManager.physicalMarker.instance.SetActive(false);

        locRune = Utilities.InstantiateObject(locationPrefab, MarkerSpawner.SelectedMarker3DT);
        var lData = locRune.GetComponent<LocationRuneData>();

        spirits = lData.spirits;
        players = lData.players;
        if (PlayerDataManager.playerData.covenName != "")
        {
            if (MarkerSpawner.SelectedMarker.isCoven)
            {
                if (PlayerDataManager.playerData.covenName == controlledBy)
                {
                    print("Turning On Summoning");
                    lData.DisableButton(true);
                }
                else
                {
                    lData.DisableButton(false);
                }
            }
        }
        else
        {
            if (!MarkerSpawner.SelectedMarker.isCoven)
            {
                if (PlayerDataManager.playerData.displayName == controlledBy)
                {
                    print("Turning On Summoning No Coven preowned");
                    lData.DisableButton(true);
                }
                else
                {
                    lData.DisableButton(false);
                }
            }
        }

        Token t = new Token();
        t.instance = PlayerDataManager.playerData.instance;
        t.male = PlayerDataManager.playerData.male;
        t.degree = PlayerDataManager.playerData.degree;
        t.race = PlayerDataManager.playerData.race;
        t.position = LD.position;
        t.type = "witch";
        t.displayName = PlayerDataManager.playerData.displayName;
        AddToken(t);

        foreach (var item in LD.tokens)
        {
            AddToken(item);
        }

        locRune.transform.localRotation = Quaternion.Euler(90, 0, 0);
        locAnim.Play("in");
        StartCoroutine(MoveMap());
        Invoke("DisableRuneAnimator", 1.3f);

        if (controlledBy == "")
        {
            foreach (var item in LD.tokens)
            {
                if (item.type == "spirit")
                {
                    return;
                }
            }
            print("LocationEnabled no owner!");
            lData.DisableButton(true);
        }
    }

    void DisableRuneAnimator()
    {
        locRune.GetComponent<Animator>().enabled = false;
    }

    public void CharacterLocationGained(string instanceID)
    {
        if (isLocation && instanceID == locationID)
        {
            if (PlayerDataManager.playerData.covenName != "")
            {
                ownedBy.text = "Owned By : " + PlayerDataManager.playerData.covenName;
            }
            else
            {
                ownedBy.text = "Owned By : " + PlayerDataManager.playerData.displayName;
            }
            locRune.GetComponent<LocationRuneData>().DisableButton(true);
        }
    }

    public void CharacterLocationLost(string instanceID)
    {
        if (isLocation && instanceID == locationID)
        {
            ownedBy.text = "Unclaimed";
            locRune.GetComponent<LocationRuneData>().DisableButton(true);
        }
    }

    public void LocationLost(WSData data)
    {
        if (isLocation && data.location == locationID)
        {
            ownedBy.text = "Unclaimed";
            locRune.GetComponent<LocationRuneData>().DisableButton(true);
        }
    }

    public void LocationGained(WSData data)
    {
        print("LocationGained");
        print(locationID);
        if (isLocation && data.location == locationID)
        {
            print("Setting Up Gain");
            ownedBy.text = "Owned By : " + data.controlledBy;
            locRune.GetComponent<LocationRuneData>().DisableButton(true);
        }
    }

    IEnumerator MoveMap()
    {
        var OM = OnlineMaps.instance;
        float t = 0;
        ini = OnlineMaps.instance.position;
        final = MarkerSpawner.SelectedMarkerPos;
        final.x += 0.00043027191f;
        final.y += 0.00035482578f;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            OM.position = Vector2.Lerp(ini, final, t);


            foreach (var item in CG)
            {
                item.alpha = Mathf.SmoothStep(1, 0, t);
            }
            foreach (var item in CGPartial)
            {
                item.alpha = Mathf.SmoothStep(1, .3f, t);
            }
            yield return 0;
        }
    }

    IEnumerator MoveBack()
    {
        var OM = OnlineMaps.instance;
        float t = 1;

        while (t >= 0)
        {
            t -= Time.deltaTime;
            OM.position = Vector2.Lerp(ini, final, t * 1.5f);

            foreach (var item in CG)
            {
                item.alpha = Mathf.SmoothStep(1, 0, t);
            }
            foreach (var item in CGPartial)
            {
                item.alpha = Mathf.SmoothStep(1, .3f, t);
            }
            yield return 0;
        }
    }

    public void OnSummon()
    {
        SummoningManager.Instance.Open();
    }

    public void TryEnterLocation()
    {

#if !DEBUG_LOCATION
        if (OnlineMapsUtils.DistanceBetweenPointsD(PlayerManager.physicalMarker.position, MarkerSpawner.SelectedMarkerPos) > PlayerDataManager.attackRadius)
        {
            Debug.Log("Physically too far from the PoP");
            MarkerSpawner.Instance.onClickMarkerFar(MarkerSpawner.SelectedMarker, true);
            return;
        }
#endif

        var k = new { location = MarkerSpawner.instanceID };
        controlledBy = MarkerSpawner.SelectedMarker.controlledBy;
        APIManager.Instance.PostData("/location/enter", JsonConvert.SerializeObject(k), ReceiveData);
    }

    public void ReceiveData(string response, int code)
    {
        if (code == 200)
        {
            //			print ("EnteringLocation");
            OnEnterLocation(JsonConvert.DeserializeObject<LocationData>(response));
        }
        else
        {
            print(response);
        }
    }

    public void ReceiveDataExit(string response, int code)
    {
        if (code == 200)
        {
            print(response);
        }
        else
        {
            Debug.LogError("Location Leaving Error : " + response);
        }
    }
}

public class LocationData
{
    public int position { get; set; }
    public List<Token> tokens { get; set; }
}

