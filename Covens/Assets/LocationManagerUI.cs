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
    [SerializeField] private List<LocationManagerItemData> m_popData;
    [SerializeField] private GameObject m_popItem;
    [SerializeField] private Transform m_itemContainer;
    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private float m_fadeTime;
    private CanvasGroup m_locationManagerCG;
    private string m_popEndpoint;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //transform.localScale = Vector3.zero;
        m_locationManagerCG = GetComponent<CanvasGroup>();
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => {
            StartCoroutine(Close());
        });
        Open();
        LocationDataSetup();
    }

    void LocationDataSetup()
    {
        //setup loading animation here

        APIManager.Instance.GetData(m_popEndpoint, LocationDataSetupCallback);
    }

    void LocationDataSetupCallback(string result, int code)
    {
        if (code == 200)
        {
            m_popData = JsonConvert.DeserializeObject<List<LocationManagerItemData>>(result);
            //kill loading animation here
            PopulateLocationItems();
        }
        else
        {
            Debug.LogError("couldn't get the data");
            var temp = Instantiate(m_popItem, m_itemContainer);
            temp.GetComponent<LocationManagerItem>().Setup(new LocationManagerItemData(), m_sprites[0]);
        }
    }

    void PopulateLocationItems()
    {
        foreach(LocationManagerItemData item in m_popData)
        {
            Sprite spr = m_sprites[DownloadedAssets.spiritDictData[item.guardianSpirit].spiritTier - 1];
            var obj = Instantiate(m_popItem, m_itemContainer);
            obj.GetComponent<LocationManagerItem>().Setup(item, spr);
        }
    }

    //opening anims
    void Open()
    {
        LeanTween.alphaCanvas(m_locationManagerCG, 1, m_fadeTime);
        //LeanTween.scale(gameObject, Vector3.one, m_fadeTime).setEase(LeanTweenType.easeInOutQuad);
    }

    //closing anims
    public IEnumerator Close()
    {
        LeanTween.alphaCanvas(m_locationManagerCG, 0, m_fadeTime);
        //LeanTween.scale(gameObject, Vector3.zero, m_fadeTime).setEase(LeanTweenType.easeInOutQuad);
        //scale it down
        yield return new WaitForSeconds(m_fadeTime);
        Destroy(gameObject);
    }
}

public struct LocationManagerItemData
{
    public string popName { get; set; }
    public double claimedStamp { get; set; }
    public double rewardStamp { get; set; }
    public string guardianSpirit { get; set; }
    public int spiritEnergy { get; set; }
    public int popTier { get; set; }
    //public LocationPerkData enhancement { get; set; }
    public double lat { get; set; }
    public double lng { get; set; }
    public bool playersShown { get; set; }
    public int activePlayers { get; set; }
}

//will need to add back LocationPerkData once PoP buffs are finished
/*
public struct LocationPerkData
{
    public string id { get; set; }
    public string type { get; set; }
    public string spellId { get; set; }
    public string buff { get; set; }
    public string spiritID { get; set; }
}
*/
