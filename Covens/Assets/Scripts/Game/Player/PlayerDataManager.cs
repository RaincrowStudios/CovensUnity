using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PlayerManager))] 

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; set; }
    public static MarkerDataDetail playerData;
    public static Vector2 playerPos;
    public static float attackRadius = .5f;
    public static float DisplayRadius = .5f;
    public static int idleTimeOut;
    public static string currentDominion = "Virginia";
    public static int zone = 0;
    public static MoonData moonData;
    private ConsumableItemModel[] m_ConsumableItemModel;
    public static Dictionary<string, string> SpiritToolsDict = new Dictionary<string, string>();
    public static Dictionary<string, string> ToolsSpiritDict = new Dictionary<string, string>();
    public static Dictionary<string, SummoningMatrix> summonMatrixDict = new Dictionary<string, SummoningMatrix>();
    public static Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>();
    public static Config config;
    public static Dailies currentQuests;

    void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public EnumGender Gender
    {
        get
        {
            //return EnumGender.Male;
            if (playerData != null && playerData.male)
                return EnumGender.Male;
            return EnumGender.Female;
        }
    }

    public Equipped EquippedChar
    {
        get
        {
            if (playerData == null)
                return null;
            return new Equipped();
        }
    }
    public string[] Cosmetics
    {
        get
        {
            if (playerData == null)
                return null;
            return null;
        }
    }
    public ConsumableItemModel[] Consumables;

    public void OnPlayerJoinCoven(string sCovenId, string covenName)
    {
        playerData.coven = sCovenId;
        playerData.covenName = covenName;

    }
    public void OnPlayerLeaveCoven()
    {
        playerData.coven = null;
        playerData.covenName = "";

    }
    public void OnPurchaseItem(string sId)
    {
        //        List<string> vList = new List<string>(playerData.inventory.cosmetics); 
        //        vList.Add(sId);
        //        playerData.inventory.cosmetics = vList.ToArray();
    }
}