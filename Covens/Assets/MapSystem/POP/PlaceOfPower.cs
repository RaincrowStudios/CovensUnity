using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPower : MonoBehaviour
{
    public class LocationData
    {
        public int position { get; set; }
        public List<Token> tokens { get; set; }
    }

    private static PlaceOfPower m_Instance;
    private static PlaceOfPower Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<PlaceOfPower>("PlaceOfPower"));
            return m_Instance;
        }
    }


    public static bool IsInsideLocation { get; private set; }

    public static event System.Action OnEnterPlaceOfPower;
    public static event System.Action OnLeavePlaceOfPower;


    [SerializeField] private PlaceOfPowerAnimation m_PopArena;
    [SerializeField] private UIPOPOptions m_OptionsMenu;
    [SerializeField] private PlaceOfPowerPosition m_SpiritPosition;
    [SerializeField] private PlaceOfPowerPosition[] m_WitchPositions;
       
    private IMarker m_Marker;
    private LocationData m_LocationData;
        
    private void Show(IMarker marker, LocationData locationData)
    {
        m_LocationData = locationData;
        m_Marker = marker;
        
        //hide buildings
        MapsAPI.Instance.ScaleBuildings(0);
                
        //hide all markers
        MarkerSpawner.HideVisibleMarkers(0.25f, true);

        MapsAPI.Instance.allowControl = false;
        transform.position = m_Marker.gameObject.transform.position;
        MapCameraUtils.FocusOnPosition(transform.position, false, 1);
        MapCameraUtils.SetZoom(1, 1f, false);
        MapCameraUtils.SetRotation(25f, 1f, false, null);

        //animate the place of power
        LeanTween.value(0, 0, 0.3f).setOnComplete(m_PopArena.Show);

        //show the player marker
        LeanTween.value(0, 0, 1f)
            .setOnComplete(() =>
            {
                //put the player on its slot
                if (locationData.position <= m_WitchPositions.Length)
                    m_WitchPositions[locationData.position - 1].AddMarker(PlayerManager.marker);
                else
                    PlayerManager.marker.SetAlpha(1f, 1f);

                //show the other players
                foreach (Token token in locationData.tokens)
                    OnMapTokenAdd.ForceEvent(token); //forcing a map_token_add event will trigger PlaceOfPower.OnAddMarker.

                //load the spirit

                m_OptionsMenu.Show(locationData);
            });
    }

    private void Close()
    {
        Debug.Log("closing place of power");
        m_LocationData = null;
        m_Marker = null;
        MapsAPI.Instance.allowControl = true;

        m_OptionsMenu.Close();

        m_PopArena.Hide();

        //hide the markers
        //also destroy it, let it be added later by map_token_add
        foreach (var pos in m_WitchPositions)
        {
            if (pos.marker != null && pos.marker != PlayerManager.marker)
            {
                string instance = pos.marker.token.instance;
                pos.marker.SetAlpha(0, 0.5f, () =>
                {
                    MarkerSpawner.DeleteMarker(instance);
                });
                pos.marker = null;
            }
        }

        LeanTween.value(0, 0, 0.5f).setOnComplete(() =>
        {
            PlayerManager.marker.SetWorldPosition(MapsAPI.Instance.GetWorldPosition(PlayerManager.marker.coords.x, PlayerManager.marker.coords.y));
            PlayerManager.marker.SetAlpha(1);
            MarkerSpawner.Instance.UpdateMarkers();
        });
    }
        
    private void OnMapUpdate(bool position, bool scale, bool rotation)
    {
        foreach(PlaceOfPowerPosition pos in m_WitchPositions)
        {
            if (pos.marker == null || pos.marker.isNull)
                continue;
            MarkerSpawner.UpdateMarker(pos.marker, false, true, MarkerSpawner.m_MarkerScale);
        }
    }

    private void OnAddMarker(IMarker marker)
    {
        Token token = marker.token;

        if (token.position == 0)
            return;

        if (token.position <= m_WitchPositions.Length)
        {
            Debug.Log(Time.time + " >> " + token.displayName+ " > " + token.position);
            m_WitchPositions[token.position - 1].AddMarker(marker);
        }
    }

    private void OnRemoveMarker(IMarker marker)
    {
        //find the marker 
        foreach(PlaceOfPowerPosition pos in m_WitchPositions)
        {
            if (pos.marker != null && pos.marker == marker)
            {
                pos.marker.SetAlpha(0, 1f, () => MarkerSpawner.DeleteMarker(marker.token.instance));
                break;
            }
        }
    }


    private void OnClickOffering()
    {

    }


    public static void EnterPoP(IMarker location, System.Action<int, string> callback)
    {
        var data = new { location = location.token.instance };
        APIManager.Instance.PostData(
            "/location/enter",
            JsonConvert.SerializeObject(data), 
            (response, result) =>
            {
                callback?.Invoke(result, response);
                if (result == 200)
                {
                    IsInsideLocation = true;
                    LocationData responseData = JsonConvert.DeserializeObject<LocationData>(response);

                    OnEnterPlaceOfPower?.Invoke();

                    //subscribe events
                    OnMapTokenAdd.OnMarkerAdd += Instance.OnAddMarker;
                    OnMapTokenRemove.OnMarkerRemove += Instance.OnRemoveMarker;
                    MapsAPI.Instance.OnCameraUpdate += Instance.OnMapUpdate;

                    //show the place of power
                    Instance.Show(location, responseData);
                }
                else
                {
                    LogError("\"location/enter\" error " + response);
                }
            });
    }

    public static void LeavePoP()
    {
        /*
         {
            "location":
            {
                "latitude":47.6973152,
                "longitude":-122.332771,
                "music":7,
                "dominion":"Washington",
                "garden":"",
                "strongest":"",
                "zone":0
            }
        }
        */

        System.Action leaveRequest = () => { };
        leaveRequest = () =>
        {
            APIManager.Instance.GetData(
                "/location/leave",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        //var data = JsonConvert.DeserializeObject<MarkerAPI>(response);
                        //Debug.Log("data: " + data.location.longitude + " - " + data.location.latitude + "\n" + "player: " + PlayerManager.marker.coords);
                    }
                    else
                    {
                        LeanTween.value(0, 0, 0.1f).setOnComplete(leaveRequest);
                        leaveRequest();
                    }
                });
        };

        leaveRequest();        
        IsInsideLocation = false;
        OnLeavePlaceOfPower?.Invoke();

        if (m_Instance != null)
        {
            //unsubscribe events
            OnMapTokenAdd.OnMarkerAdd -= Instance.OnAddMarker;
            OnMapTokenRemove.OnMarkerRemove -= Instance.OnRemoveMarker;
            MapsAPI.Instance.OnCameraUpdate -= Instance.OnMapUpdate;

            m_Instance.Close();

            OnLeavePlaceOfPower?.Invoke();
        }
    }

    private static void Log(string txt)
    {
        //if (Application.isEditor || Debug.isDebugBuild)
            Debug.Log("[PlaceOfPower] " + txt);
    }

    private static void LogError(string txt)
    {
        //if (Application.isEditor || Debug.isDebugBuild)
            Debug.LogError("[PlaceOfPower] " + txt);
    }

    [ContextMenu("LeavePOP")]
    private void Debug_LeavePOP()
    {
        if (m_LocationData == null)
            return;

        LeavePoP();
    }
}
