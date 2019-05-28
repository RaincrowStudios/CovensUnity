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
    public static PlaceOfPower Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<PlaceOfPower>("PlaceOfPower"));
            return m_Instance;
        }
    }

    public static event System.Action OnEnterPlaceOfPower;
    public static event System.Action OnLeavePlaceOfPower;


    [SerializeField] private UIPOPOptions m_OptionsMenu;
    [SerializeField] private PlaceOfPowerPosition m_SpiritPosition;
    [SerializeField] private PlaceOfPowerPosition[] m_WitchPositions;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer m_GroundGlyph;
    [SerializeField] private SpriteRenderer[] m_Shadows;


    private LocationData m_LocationData;
    private int m_PoPTweenId;

    private void Awake()
    {
        gameObject.SetActive(false);
        m_GroundGlyph.gameObject.SetActive(false);
        foreach (SpriteRenderer shadow in m_Shadows)
            shadow.gameObject.SetActive(false);
    }
    
    private void Show(LocationData locationData)
    {
        MapsAPI.Instance.allowControl = false;
        m_LocationData = locationData;

        //hide buildings
        MapsAPI.Instance.ScaleBuildings(0);

        transform.position = MapsAPI.Instance.GetWorldPosition();

        //animate the place of power
        AnimateShow();

        //hide all markers
        MarkerSpawner.HideVisibleMarkers(0.5f);

        //show the player marker
        LeanTween.value(0, 0, 1f)
            .setOnComplete(() =>
            {
                //put it in the right slot
                if (locationData.position <= m_WitchPositions.Length)
                    m_WitchPositions[locationData.position - 1].AddMarker(PlayerManager.marker);
                else
                    PlayerManager.marker.SetAlpha(1f, 1f);
            });

        //load the spirit
    }

    private void Close()
    {
        m_LocationData = null;

        //show buildings
        MapsAPI.Instance.ScaleBuildings(1);

        AnimateHide();
        MarkerSpawner.ShowVisibleMarkers(1f);
        PlayerManager.marker.SetWorldPosition(MapsAPI.Instance.GetWorldPosition(PlayerManager.marker.coords.x, PlayerManager.marker.coords.y));
    }

    private void AnimateShow()
    {
        LeanTween.cancel(m_PoPTweenId);

        float t2;
        Color aux;

        m_PoPTweenId = LeanTween.value(0f, 1f, 2f).setEaseOutCubic()
            .setOnStart(() =>
            {
                gameObject.SetActive(true);
                m_GroundGlyph.gameObject.SetActive(true);
                foreach (SpriteRenderer shadow in m_Shadows)
                    shadow.gameObject.SetActive(true);
            })
            .setOnUpdate((float v) =>
            {
                t2 = v * v;

                foreach (SpriteRenderer _shadow in m_Shadows)
                {
                    aux = _shadow.color;
                    aux.a = t2;
                    _shadow.color = aux;
                }

                aux = m_GroundGlyph.color;
                aux.a = v;
                m_GroundGlyph.color = aux;
                m_GroundGlyph.transform.localScale = Vector3.one * v * 25;
            })
            .uniqueId;
    }

    private void AnimateHide()
    {
        Color aux;
        foreach (SpriteRenderer _shadow in m_Shadows)
        {
            aux = _shadow.color;
            aux.a = 0;
            _shadow.color = aux;
        }

        aux = m_GroundGlyph.color;
        aux.a = 0;
        m_GroundGlyph.color = aux;
        m_GroundGlyph.transform.localScale = Vector3.zero;
    }

    private void OnAddMarker(IMarker marker)
    {

    }

    private void OnRemoveMarker(IMarker marker)
    {

    }



    public static void EnterPoP(string instance, System.Action<int, string> callback)
    {
        var data = new { location = instance };
        APIManager.Instance.PostData(
            "/location/enter",
            JsonConvert.SerializeObject(data), 
            (response, result) =>
            {
                callback?.Invoke(result, response);
                if (result == 200)
                {
                    LocationData responseData = JsonConvert.DeserializeObject<LocationData>(response);

                    //show the place of power
                    Instance.Show(responseData);

                    OnEnterPlaceOfPower?.Invoke();

                    //subscribe events
                    OnMapTokenAdd.OnMarkerAdd += Instance.OnAddMarker;
                    OnMapTokenRemove.OnMarkerRemove += Instance.OnRemoveMarker;
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
        APIManager.Instance.GetData(
            "/location/leave",
            (response, result) =>
            {
                if (result == 200)
                {
                    OnLeavePlaceOfPower?.Invoke();

                    if (m_Instance != null)
                    {
                        //unsubscribe events
                        OnMapTokenAdd.OnMarkerAdd -= Instance.OnAddMarker;
                        OnMapTokenRemove.OnMarkerRemove -= Instance.OnRemoveMarker;

                        m_Instance.Close();
                    }
                    
                    Log(response);
                }
                else
                {
                    LogError("\"location/leave\" error " + response);
                }
            });
    }

    private static void Log(string txt)
    {
        if (Application.isEditor || Debug.isDebugBuild)
            Debug.Log("[PlaceOfPower] " + txt);
    }

    private static void LogError(string txt)
    {
        if (Application.isEditor || Debug.isDebugBuild)
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
