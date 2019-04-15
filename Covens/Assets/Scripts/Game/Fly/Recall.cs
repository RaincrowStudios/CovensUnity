using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class Recall : MonoBehaviour
{
    Vector2 pos, oldPos;
    float t;
    public float speed = 1;
    bool move;



    public void RecallHome()
    {
        //if (PlayerManager.physicalMarker != null)
        //      {
        //pos = PlayerManager.physicalMarker.position;
        //oldPos = MapsAPI.Instance.position;

        //         //MapsAPI.Instance.position = PlayerManager.physicalMarker.position;
        //         //MapsAPI.Instance.RemoveMarker(PlayerManager.physicalMarker);
        //         //PlayerDataManager.playerPos = MapsAPI.Instance.position;

        //         Vector2 newPos = PlayerManager.physicalMarker.position;
        //         MapsAPI.Instance.ShowStreetMap(newPos.x, newPos.y);
        //         MapsAPI.Instance.RemoveMarker(PlayerManager.physicalMarker);
        //         PlayerDataManager.playerPos = newPos;

        //         PlayerManager.inSpiritForm = false;
        //PlayerManager.physicalMarker = null;
        //PlayerManager.Instance.returnphysicalSound ();
        //PlayerManager.Instance.ReSnapMap ();
        //GetComponent<PlayerManagerUI> ().home ();
        //MarkerManagerAPI.GetMarkers (true);
        MarkerManager.DeleteAllMarkers();
        MapsAPI.Instance.RemoveMarker(PlayerManager.physicalMarker);
        PlayerManager.physicalMarker = null;
        //}
        //else
        {
            MarkerManagerAPI.GetMarkers(true, true, () =>
            {
                //PlayerManager.Instance.ReSnapMap();
                //MapsAPI.Instance.position = PlayerManager.marker.position;

                //MapsAPI.Instance.ShowStreetMap(null);
                //PlayerDataManager.playerPos = new Vector2(GetGPS.longitude, GetGPS.latitude);
            });
        }
    }
}
