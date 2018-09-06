using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PlayerManager))] 
[RequireComponent(typeof(PlayerManagerUI))]

public class PlayerDataManager : Patterns.SingletonComponent<PlayerDataManager>
{

    public static MarkerDataDetail playerData;
    public static Vector2 playerPos;
    public static float attackRadius = .5f;
    public static float DisplayRadius = .5f;
	public static string currentDominion = "Virginia";
    private ConsumableItemModel[] m_ConsumableItemModel;
	public static Dictionary<string,string> SpiritToolsDict = new Dictionary<string, string>();
	public static Dictionary<string,string> ToolsSpiritDict= new Dictionary<string, string>(); 
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
            return playerData.equipped;
        }
    }
    public string[] Cosmetics
    {
        get
        {
            if (playerData == null)
                return null;
            return playerData.inventory.cosmetics;
        }
    }
    public ConsumableItemModel[] Consumables
    {
        get
        {
            if (playerData == null)
                return null;
            if(m_ConsumableItemModel == null)
            {
                m_ConsumableItemModel = new ConsumableItemModel[playerData.inventory.consumables.Length];
                for (int i = 0; i < m_ConsumableItemModel.Length; i++)
                    m_ConsumableItemModel[i] = new ConsumableItemModel(playerData.inventory.consumables[i]);
            }
            return m_ConsumableItemModel;
        }
    }
    public void OnPlayerJoinCoven(string sCovenId)
    {
        playerData.coven = sCovenId;
    }
    public void OnPlayerLeaveCoven()
    {
        playerData.coven = null;
    }
    public void OnPurchaseItem(string sId)
    {
        List<string> vList = new List<string>(playerData.inventory.cosmetics);
        vList.Add(sId);
        playerData.inventory.cosmetics = vList.ToArray();
    }
}