using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class LocationManagerUI : MonoBehaviour
{
    [Header("Static Items")]
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_ownedBy;
    [SerializeField] private List<LocationManagerItemData> m_popData;
    [SerializeField] private GameObject m_popItem;
    [SerializeField] private Transform m_itemContainer;
    [SerializeField] private Sprite[] m_sprites;


    private string m_popEndpoint;

    // Start is called before the first frame update
    void Start()
    {
        //setup animations, scaling, yada yada here
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

}

public class LocationManagerItemData
{
    public string popName { get; set; }
    public double claimedStamp { get; set; }
    public double rewardStamp { get; set; }
    public string guardianSpirit { get; set; }
    public int popTier { get; set; }
    public string enhancement { get; set; }
    public double lat { get; set; }
    public double lng { get; set; }
    public bool playersShown { get; set; }
    public int activePlayers { get; set; }
}