using Raincrow.Store;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager
{
    private static PlayerDataManager m_Instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new PlayerDataManager();
            return m_Instance;
        }
    }

    //player
    public static PlayerData playerData;
    public static bool IsFTF => playerData?.tutorial == false;

    //game settings
    public static float DisplayRadius = .5f;
    public static int[] summoningCosts;
    public static long[] alignmentPerDegree;
    public static int[] baseEnergyPerLevel;
    public static int[] forbiddenValue;
    public static ulong[] xpToLevelup;
    public static MoonData moonData;
    public static Sun sunData;
    public static int tribunal;
    public static double endOfTribunal;

    public static int idleTimeOut;
    public static string currentDominion;
    public static int zone = 0;    
    public static int soundTrack = 0;

    //public static double[] tribunalStamps => new double[] { 1553040000, 1561075200, 1569196800, 1576972800, 1584662400, 1592697600 };
    
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
        //playerData.coven = sCovenId;
        //playerData.covenName = covenName;

    }
    public void OnPlayerLeaveCoven()
    {
        //playerData.coven = null;
        //playerData.covenName = "";

    }
    public void OnPurchaseItem(string sId)
    {
        //        List<string> vList = new List<string>(playerData.inventory.cosmetics); 
        //        vList.Add(sId);
        //        playerData.inventory.cosmetics = vList.ToArray();
    }
}