using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerManager : MonoBehaviour
{

    public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();
    public static Dictionary<string, bool> StanceDict = new Dictionary<string, bool>();
    
    public static void DeleteMarker(string ID, bool destroy = true)
    {
        if (Markers.ContainsKey(ID))
        {
            IMarker marker = Markers[ID][0];
            Markers.Remove(ID);

            if (destroy)
                MapsAPI.Instance.RemoveMarker(marker);
        }

        if (MarkerSpawner.ImmunityMap.ContainsKey(ID))
            MarkerSpawner.ImmunityMap.Remove(ID);
    }

    //public static void DestroyAllMarkers()
    //{
    //    foreach (var marker in Markers.Values)
    //    {
    //        MapsAPI.Instance.RemoveMarker(marker[0]);
    //        if (MarkerSpawner.ImmunityMap.ContainsKey(marker[0].token.instance))
    //            MarkerSpawner.ImmunityMap.Remove(marker[0].token.instance);
    //    }

    //    Markers.Clear();
    //}

    //public static void SetImmunity(bool isImmune,string id)
    //{
    //	if (isImmune) {
    //		if (Markers.ContainsKey (id)) {
    //			Markers [id] [0].gameObject.GetComponentInChildren<SpriteRenderer> ().color = new Color (1, 1, 1, .3f);
    //		}
    //	} else {
    //		if (Markers.ContainsKey (id)) {
    //			Markers [id] [0].gameObject.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
    //		}
    //	}
    //}

    protected static void UpdateMarkerData(string instance, CharacterMarkerDetail details)
    {
        IMarker marker = GetMarker(instance);
        if (marker == null)
            return;

        if (details.energy <= 0)
            details.state = "dead";

        Token token = marker.customData as Token;
        if (token.Type == MarkerSpawner.MarkerType.witch)
        {
            WitchMarker witch = marker as WitchMarker;

            token.state = details.state;

            if (token.energy <= 0 || token.state == "dead")
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

