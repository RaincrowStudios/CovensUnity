using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;


public class ApparelManager : MonoBehaviour
{
    public static ApparelManager instance { get; set; }
    [HideInInspector]
    public ApparelView ActiveViewPlayer;

    public ApparelView male;
    public ApparelView female;

    void Awake()
    {
        instance = this;
    }

    public void SendEquipChar()
    {
        PlayerDataManager.playerData.equipped = ActiveViewPlayer.equippedApparel.Values.ToList();
        PlayerManager.Instance.OnUpdateEquips();
        LoadPlayerPortrait.ReloadPortrait();
        var data = new { equipped = PlayerDataManager.playerData.equipped };
        APIManager.Instance.Post("inventory/equip", JsonConvert.SerializeObject(data), equipResult);
    }

    public void equipResult(string s, int r)
    {

    }

    public void SetupApparel()
    {
        if (PlayerDataManager.playerData.male)
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            male.InitializeChar(PlayerDataManager.playerData.equipped);
            ActiveViewPlayer = male;
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitializeChar(PlayerDataManager.playerData.equipped);
            ActiveViewPlayer = female;
        }
    }
}

public class CosmeticData
{
    public struct CosmeticAsset
    {
        public List<string> baseAsset;
        public List<string> shadow;
        public List<string> grey;
        public List<string> white;
    }

    public string id;
    public string position;
    public string iconId;
    [JsonProperty("base")]
    public string[] baseAssets;
    [JsonProperty("shadow")]
    public string[] shadowAssets;
    [JsonProperty("grey")]
    public string[] greyAssets;
    [JsonProperty("white")]
    public string[] whiteAssets;

    public bool hidden;
    public int silver;
    public int gold;


    [JsonIgnore]
    public string tooltip;

    [JsonIgnore]
    public bool owned;

    [JsonIgnore]
    public double unlockOn;

    [JsonIgnore]
    public bool locked;

    [JsonIgnore]
    public string catagory { get; set; }

    [JsonIgnore]
    public string storeCatagory { get; set; }

    [JsonIgnore]
    public ApparelButtonData buttonData;

    [JsonIgnore]
    public bool isNew;
    
    [JsonIgnore]
    public ApparelType apparelType;

    [JsonIgnore]
    public CosmeticAsset assets
    {
        get
        {
            return new CosmeticAsset
            {
                baseAsset = new List<string>(baseAssets),
                shadow = new List<string>(shadowAssets),
                grey = new List<string>(greyAssets),
                white = new List<string>(whiteAssets)
            };
        }
    }
}

public enum ApparelType
{
    Base, Grey, White, Shadow
}

public class EquippedApparel
{

    public string position
    {
        get
        {
            CosmeticData data = DownloadedAssets.GetCosmetic(id);
            if (data == null)
                return "";
            else
                return data.position;
        }
    }

    public string id { get; set; }

    public List<string> assets { get; set; }

}

