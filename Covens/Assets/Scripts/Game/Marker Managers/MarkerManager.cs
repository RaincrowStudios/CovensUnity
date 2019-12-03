using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

//public class MarkerSpawner : MonoBehaviour
//{
//    public enum MarkerType
//    {
//        NONE = 0,
//        PORTAL = 1,
//        SPIRIT = 2,
//        DUKE = 3,
//        PLACE_OF_POWER = 4,
//        WITCH = 5,
//        SUMMONING_EVENT = 6,
//        GEM = 7,
//        HERB = 8,
//        TOOL = 9,
//        SILVER = 10,
//        LORE = 11,
//        ENERGY = 12,
//        BOSS = 13,
//        LOOT = 14,
//    }

//    public enum MarkerSchool
//    {
//        SHADOW = -1,
//        GREY = 0,
//        WHITE = 1
//    }

//    public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();

//    public static IMarker GetMarker(string instance)
//    {
//        if (string.IsNullOrEmpty(instance))
//            return null;

//        if (!LocationIslandController.isInBattle)
//        {
//            if (PlayerDataManager.playerData.instance == instance)
//                return PlayerManager.witchMarker;

//            if (Markers.ContainsKey(instance))
//                return Markers[instance][0];
//        }
//        else
//        {
//            if (PlayerDataManager.playerData.instance == instance)
//                return LocationPlayerAction.playerMarker;

//            if (LocationUnitSpawner.Markers.ContainsKey(instance))
//                return LocationUnitSpawner.Markers[instance];
//        }
//        return null;
//    }
//}

