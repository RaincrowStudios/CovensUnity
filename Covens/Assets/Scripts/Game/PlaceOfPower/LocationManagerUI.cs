using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class LocationManagerUI : MonoBehaviour
{
    public static LocationManagerUI Instance { get; set; }

    [Header("Static Items")]
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_ownedBy;
    [SerializeField] private TextMeshProUGUI m_nameHeader;
    [SerializeField] private TextMeshProUGUI m_guardianHeader;
    [SerializeField] private TextMeshProUGUI m_buffHeader;
    [SerializeField] private TextMeshProUGUI m_flyHeader;
    [SerializeField] private List<LocationManagerItemData> m_popData = new List<LocationManagerItemData>();
    [SerializeField] private GameObject m_popItem;
    [SerializeField] private Transform m_itemContainer;
    [SerializeField] private float m_fadeTime;

    private CanvasGroup m_locationManagerCG;
    private string m_popEndpoint;

    private int m_TweenId;

    private void Awake()
    {
        Instance = this;
        m_popEndpoint = "location/manager";
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.localScale = Vector3.zero;
        m_locationManagerCG = GetComponent<CanvasGroup>();
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(Close);
        Open();
        LocationDataSetup();
    }

    void LocationDataSetup()
    {
        //setup loading animation here

        APIManager.Instance.Get(m_popEndpoint, LocationDataSetupCallback);
    }

    void SetupUIText()
    {
        m_title.text = LocalizeLookUp.GetText("pop_title");
        Debug.Log(m_popData.Count);

        var ownedText = "";
        if (PlayerDataManager.playerData.covenId == string.Empty)
            ownedText = LocalizeLookUp.GetText("pop_you")
                .Replace("{{Pop Number}}", m_popData.Count.ToString());
        else
            ownedText = LocalizeLookUp.GetText("pop_coven")
                .Replace("{{Coven Name}}", PlayerDataManager.playerData.covenId)
                .Replace("{{Pop Number}}", m_popData.Count.ToString());
        m_ownedBy.text = ownedText;

        m_nameHeader.text = LocalizeLookUp.GetText("name");
        m_guardianHeader.text = LocalizeLookUp.GetText("pop_guardian");
        m_buffHeader.text = LocalizeLookUp.GetText("pop_enhancement");
        m_flyHeader.text = LocalizeLookUp.GetText("pop_fly_to");
    }

    void LocationDataSetupCallback(string result, int code)
    {
        if (code == 200)
        {
            m_popData = JsonConvert.DeserializeObject<List<LocationManagerItemData>>(result);
            //kill loading animation here
            
            Debug.Log("connected");
        }
        else
        {
            Debug.LogError("couldn't get the data");
            Debug.LogError("code: " + code);
            Debug.LogError("result: " + result);
        }
        AddFillerItemData();
        SetupUIText();
        PopulateLocationItems();
    }

    void PopulateLocationItems()
    {
        if (m_popData.Count == 0)
        {
            var empty = new LocationManagerItemData();
            var emptyObj = Instantiate(m_popItem, m_itemContainer);
            emptyObj.GetComponent<LocationManagerItem>().Setup(empty, false);
        }

        for (int i = 0; i < m_popData.Count; i++)
        {
            var obj = Instantiate(m_popItem, m_itemContainer);
            obj.GetComponent<LocationManagerItem>().Setup(m_popData[i]);
            if ((i % 2) == 1)
                obj.GetComponent<Image>().enabled = false;
        }
    }

    //opening anims
    void Open()
    {
        LeanTween.cancel(m_TweenId);

        m_locationManagerCG.blocksRaycasts = true;
        m_locationManagerCG.interactable = true;

        m_TweenId = LeanTween.alphaCanvas(m_locationManagerCG, 1, m_fadeTime)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => MapsAPI.Instance.HideMap(true))
            .uniqueId;
    }

    //closing anims
    public void Close()
    {
        LeanTween.cancel(m_TweenId);

        m_locationManagerCG.blocksRaycasts = false;
        m_locationManagerCG.interactable = false;
        MapsAPI.Instance.HideMap(false);

        m_TweenId = LeanTween.alphaCanvas(m_locationManagerCG, 0, m_fadeTime)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => Destroy(gameObject))
            .uniqueId;
    }

    #region
    void AddFillerItemData()
    {
        var fillerItem = new LocationManagerItemData()
        {
            name = "R.I.P.",
            rewardOn = 1562164383,
            spirit = "spirit_barghest",
            spiritEnergy = 969,
            tier = 1,
            latitude = 0,
            longitude = 0,
            silver = 1
        };
        m_popData.Add(fillerItem);
    }
    #endregion
}

public struct LocationManagerItemData
{
    public string name { get; set; }
    public double rewardOn { get; set; }
    public string spirit { get; set; }
    public int spiritEnergy { get; set; }
    public int tier { get; set; }
    public LocationBuff enhancement { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public bool playersShown { get; set; }
    public int activePlayers { get; set; }
    public int silver { get; set; }
}