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
    public InventoryItems[] Consumables
    {
        get
        {
            if (playerData == null)
                return null;
            return playerData.inventory.consumables;
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

}