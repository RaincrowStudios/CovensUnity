using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerManager : MonoBehaviour
{
    public enum MarkerType
    {
        NONE = 0,
        PORTAL = 1,
        SPIRIT = 2,
        DUKE = 3,
        PLACE_OF_POWER = 4,
        WITCH = 5,
        SUMMONING_EVENT = 6,
        GEM = 7,
        HERB = 8,
        TOOL = 9,
        SILVER = 10,
        LORE = 11,
        ENERGY = 12
    }

    public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();

    public static IMarker GetMarker(string instance)
    {
        if (!LocationIslandController.isInBattle)
        {
            if (Markers.ContainsKey(instance))
                return Markers[instance][0];
        }
        else
        {
            if (LocationUnitSpawner.Markers.ContainsKey(instance))
                return LocationUnitSpawner.Markers[instance];
        }
        return null;
    }
}

