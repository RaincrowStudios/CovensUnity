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
    public static Dictionary<string, bool> StanceDict = new Dictionary<string, bool>();
    
    protected static void UpdateMarkerData(string instance, CharacterMarkerDetail details)
    {
        IMarker marker = GetMarker(instance);
        if (marker == null)
            return;

        if (details.energy <= 0)
            details.state = "dead";

        Token token = marker.customData as Token;
        if (token.Type == MarkerSpawner.MarkerType.WITCH)
        {
            WitchMarker witch = marker as WitchMarker;
            WitchToken witchToken = token as WitchToken;

            witchToken.state = details.state;

            if (witchToken.energy <= 0 || witchToken.state == "dead")
                witch.AddDeathFX();
            else
                witch.RemoveDeathFX();
        }

        marker.customData = token;
    }

    public static IMarker GetMarker(string instance)
    {
        if (Markers.ContainsKey(instance))
            return Markers[instance][0];
        return null;
    }
}

