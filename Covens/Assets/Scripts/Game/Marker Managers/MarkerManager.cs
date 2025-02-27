﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class MarkerManager : MonoBehaviour {
	
	public static Dictionary<string, List<IMarker>> Markers = new Dictionary<string, List<IMarker>>();
	public static Dictionary<string,bool> StanceDict = new Dictionary<string,bool> ();
    
	public static void DeleteAllMarkers( )
	{
        IMarker[] markersArray = new IMarker[Markers.Count];

        SpellcastingFX.DespawnAllDeathFX();

        int i = 0;
        foreach (var item in Markers)
        {
            markersArray[i] = item.Value[0];
            i++;
        }

        Vector3 auxVec3;
        LeanTween.value(1, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                auxVec3 = new Vector3(t, t, t);
                for (i = 0; i < markersArray.Length; i++)
                {
                    markersArray[i].gameObject.transform.localScale = auxVec3;
                }
            })
            .setOnComplete(() =>
            {
                for (i = 0; i < markersArray.Length; i++)
                {
                    MapsAPI.Instance.RemoveMarker(markersArray[i]);
                }
            });

        //foreach (var item in Markers)
        //{
        //    foreach (var marker in item.Value)
        //    {
        //        try
        //        {
        //            //marker.control.RemoveMarker3D(marker);
        //            MapsAPI.Instance.RemoveMarker(marker);
        //        }
        //        catch (System.Exception e)
        //        {
        //            var s = marker.customData as Token;
        //            print(s.type);
        //            Debug.LogError(e.ToString());
        //        }
        //    }
        //}
        MarkerSpawner.ImmunityMap.Clear ();
		Markers.Clear ();
	}

	public static void DeleteMarker(string ID)
	{
        if (Markers.ContainsKey(ID))
        {
            foreach (var marker in Markers[ID])
            {
                LeanTween.scale(marker.gameObject, Vector3.zero, 1f)
                    .setEaseOutCubic()
                    .setOnComplete(() =>
                    {
                        MapsAPI.Instance.RemoveMarker(marker);
                    });

                SpellcastingFX.DespawnDeathFX(ID, marker);
            }
        }

        if (MarkerSpawner.ImmunityMap.ContainsKey(ID))
        {
            MarkerSpawner.ImmunityMap.Remove(ID);
        }

		Markers.Remove (ID);
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

