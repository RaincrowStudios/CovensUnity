using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerManager : MonoBehaviour
{

    public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();
    public static Dictionary<string, bool> StanceDict = new Dictionary<string, bool>();

    //public static void DeleteAllMarkers(IMarker[] markersArray = null)
    //{
    //    int i = 0;
    //    if (markersArray == null)
    //    {
    //        //SpellcastingFX.DespawnAllDeathFX();
    //        markersArray = new IMarker[Markers.Count];

    //        foreach (var item in Markers)
    //        {
    //            markersArray[i] = item.Value[0];
    //            i++;
    //        }

    //        MarkerSpawner.ImmunityMap.Clear();
    //        Markers.Clear();
    //    }
    //    else
    //    {
    //        string instance;
    //        for (i = 0; i < markersArray.Length; i++)
    //        {
    //            instance = markersArray[i].customData == null ? null : (markersArray[i].customData as Token).instance;

    //            if (string.IsNullOrEmpty(instance) == false)
    //            {
    //                //SpellcastingFX.DespawnDeathFX(instance, markersArray[i]);
    //                if (MarkerSpawner.ImmunityMap.ContainsKey(instance))
    //                    MarkerSpawner.ImmunityMap.Remove(instance);
    //                if (Markers.ContainsKey(instance))
    //                {
    //                    Markers.Remove(instance);
    //                }
    //            }
    //        }
    //    }

    //    Vector3 auxVec3;
    //    LeanTween.value(1, 0, 1f)
    //        .setEaseOutCubic()
    //        .setOnUpdate((float t) =>
    //        {
    //            auxVec3 = new Vector3(t, t, t);
    //            for (i = 0; i < markersArray.Length; i++)
    //            {
    //                markersArray[i].gameObject.transform.localScale = auxVec3;
    //            }
    //        })
    //        .setOnComplete(() =>
    //        {
    //            for (i = 0; i < markersArray.Length; i++)
    //            {
    //                MapsAPI.Instance.RemoveMarker(markersArray[i]);
    //            }
    //        });
    //}

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

    protected static void UpdateMarker(string instance, MarkerDataDetail details)
    {
        IMarker marker = GetMarker(instance);
        if (marker == null)
            return;

        Token token = marker.customData as Token;
        if (token.Type == MarkerSpawner.MarkerType.witch)
        {
            token.state = details.state;
        }
    }

    public static IMarker GetMarker(string instance)
    {
        if (Markers.ContainsKey(instance))
            return Markers[instance][0];
        return null;
    }
}

