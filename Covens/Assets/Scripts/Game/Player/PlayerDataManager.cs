using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerManagerUI))]

public class PlayerDataManager : Patterns.SingletonComponent<PlayerDataManager>
{


    public static MarkerDataDetail playerData;
    public static Vector2 playerPos;
    public static float attackRadius = .5f;
    public static float DisplayRadius = .5f;





    public void OnPlayerJoinCoven(string sCovenId)
    {
        playerData.coven = sCovenId;
    }
    public void OnPlayerLeaveCoven()
    {
        playerData.coven = null;
    }

}