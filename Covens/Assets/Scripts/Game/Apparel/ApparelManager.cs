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



        APIManager.Instance.Put("character/equip", JsonConvert.SerializeObject(PlayerDataManager.playerData.equipped), equipResult);
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
            male.InitCharacter(PlayerDataManager.playerData.equipped, false);
            ActiveViewPlayer = male;
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitCharacter(PlayerDataManager.playerData.equipped, false);
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

    [System.ComponentModel.DefaultValue("")]
    public string id;
    [System.ComponentModel.DefaultValue("")]
    public string position;
    [System.ComponentModel.DefaultValue("")]
    public string iconId;
    [JsonProperty("type"), System.ComponentModel.DefaultValue("")]
    public string gender;

    [JsonProperty("base")]
    public List<string> baseAssets;
    [JsonProperty("shadow")]
    public List<string> shadowAssets;
    [JsonProperty("grey")]
    public List<string> greyAssets;
    [JsonProperty("white")]
    public List<string> whiteAssets;

    public bool hidden;

    [JsonIgnore]
    public int silver;

    [JsonIgnore]
    public int gold;


    [JsonIgnore]
    public string tooltip;

    [JsonIgnore]
    public bool owned => PlayerDataManager.playerData.inventory.cosmetics.Exists(item => item.id == id);

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
                baseAsset = baseAssets != null ? new List<string>(baseAssets) : new List<string>(),
                shadow = shadowAssets != null ? new List<string>(shadowAssets) : new List<string>(),
                grey = greyAssets != null ? new List<string>(greyAssets) : new List<string>(),
                white = whiteAssets != null ? new List<string>(whiteAssets) : new List<string>()
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

