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
    

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void Show(LocationData locationData)
    {
        //hide buildings
        MapsAPI.Instance.ScaleBuildings(0);
    }

    public void Close()
    {
        //show buildings
        MapsAPI.Instance.ScaleBuildings(1);
    }

    private void OnAddMarker(IMarker marker)
    {

    }

    private void OnRemoveMarker(IMarker marker)
    {

    }



    public static void EnterPoP(string instance, System.Action<int, string> callback)
    {
        var data = new { instance };
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
                    //UIGlobalErrorPopup.ShowError(null, "Error entering location: " + response);
                }
            });
    }

    public static void LeavePoP()
    {
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

                        Instance.Close();
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
}
