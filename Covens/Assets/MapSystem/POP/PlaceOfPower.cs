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
        public Token spirit { get; set; }
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
    [SerializeField] private UIPOPBattle m_BattleMenu;
    [SerializeField] private PlaceOfPowerPosition m_SpiritPosition;
    [SerializeField] private PlaceOfPowerPosition[] m_WitchPositions;
       
    private IMarker m_Marker;
    private LocationData m_LocationData;

    private void Awake()
    {
        m_OptionsMenu.onSelectChallenge += StartBattle;
        m_OptionsMenu.onSelectOferring += StartOffering;
    }

    private void Show(IMarker marker, LocationData locationData)
    {
        m_LocationData = locationData;
        m_Marker = marker;
                        
        //hide all markers
        MarkerSpawner.HideVisibleMarkers(0.25f, true);

        Vector3 offset = new Vector3(Mathf.Sin(Mathf.Deg2Rad * 25), 0, Mathf.Cos(Mathf.Deg2Rad * 25)) * 30;
        transform.position = m_Marker.gameObject.transform.position + offset;
        MapCameraUtils.FocusOnPosition(transform.position + offset, false, 1);
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
                    OnMapTokenAdd.ForceEvent(token, true); //forcing a map_token_add event will trigger PlaceOfPower.OnAddMarker.

                //load the spirit
                OnMapTokenAdd.ForceEvent(locationData.spirit, true);

                m_OptionsMenu.Show(locationData);
            });
    }

    private void Close()
    {
        Debug.Log("closing place of power");
        m_LocationData = null;
        m_Marker = null;

        m_OptionsMenu.Close();
        m_BattleMenu.Close();

        m_PopArena.Hide();

        //hide the markers
        //also destroy it, let it be added later by map_token_add
        foreach (var pos in m_WitchPositions)
        {
            if (pos.marker != null)
            {
                if (pos.marker == PlayerManager.marker)
                {
                    pos.marker.SetAlpha(0, 0.5f);
                }
                else
                {
                    string instance = pos.marker.token.instance;
                    pos.marker.SetAlpha(0, 0.5f, () =>
                    {
                        MarkerSpawner.DeleteMarker(instance);
                    });
                    pos.marker = null;
                }
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
                
        if (token.Type == MarkerSpawner.MarkerType.witch)
        {
            if (token.position > 0 && token.position <= m_WitchPositions.Length)
            {
                m_WitchPositions[token.position - 1].AddMarker(marker);
                return;
            }
        }
        else if (token.Type == MarkerSpawner.MarkerType.spirit)
        {
            m_SpiritPosition.AddMarker(marker);
            m_PopArena.AnimateSpirit(marker);
            return;
        }

        marker.inMapView = false;
        marker.gameObject.SetActive(false);
        return;
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


    public void StartOffering()
    {
        APIManager.Instance.PostData(
            "/location/offer",
            "{ }",
            (response, result) =>
            {
                Debug.Log(result + "\n" + response);
            });
    }

    public void StartBattle()
    {
        m_OptionsMenu.Close();
        m_BattleMenu.Open();
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
                    /*{
                        "type":"location",
                        "displayName":"5th Ave NE & NE 100th St",
                        "locationType":"pub",
                        "physicalOnly":false,
                        "full":false,
                        "controlledBy":"",
                        "isCoven":false,
                        "herb":"coll_willow",
                        "gem":"",
                        "tool":"coll_onyxAmulet",
                        "buff":
                        {
                            "id":"duration",
                            "type":"spells",
                            "spellId":"spell_clarity",
                            "buff":7
                        },
                        "level":1
                    }*/
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
        System.Action leaveRequest = () => { };
        leaveRequest = () =>
        {
            APIManager.Instance.GetData(
                "/location/leave",
                (response, result) =>
                {
                    if (result == 200)
                    {
                        /*{
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
                        }*/

                        //var data = JsonConvert.DeserializeObject<MarkerAPI>(response);
                        //Debug.Log("data: " + data.location.longitude + " - " + data.location.latitude + "\n" + "player: " + PlayerManager.marker.coords);
                    }


                    if (result == 0 || response == "")
                    {
                        Debug.LogError("/location/leave failed with code:" + result + ", response: " + response + "\nRetrying...");
                        LeanTween.value(0, 0, 0.1f).setOnComplete(leaveRequest);
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
