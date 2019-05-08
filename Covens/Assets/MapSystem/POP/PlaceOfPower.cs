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


    [SerializeField] private UIPOPOptions m_OptionsMenu;
    [SerializeField] private PlaceOfPowerPosition m_SpiritPosition;
    [SerializeField] private PlaceOfPowerPosition[] m_WitchPositions;
    

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void Show(LocationData locationData)
    {
        //subscribe events
        OnMapTokenAdd.OnMarkerAdd += OnAddMarker;

        //hide buildings
        MapsAPI.Instance.ScaleBuildings(0);
    }

    public void Close()
    {
        //unsubscribe events
        OnMapTokenAdd.OnMarkerAdd -= OnAddMarker;

        //show buildings
        MapsAPI.Instance.ScaleBuildings(1);
    }

    private void OnAddMarker(IMarker marker)
    {

    }

    public static void EnterPoP(string location, System.Action<int, string> callback)
    {
        var data = new { location };
        APIManager.Instance.PostData(
            "/location/enter",
            JsonConvert.SerializeObject(data), 
            (response, result) =>
            {
                callback?.Invoke(result, response);
                if (result == 200)
                {
                    LocationData responseData = JsonConvert.DeserializeObject<LocationData>(response);
                    Instance.Show(responseData);
                }
                else
                {
                    LogError("\"location/enter\" error " + response);
                    //UIGlobalErrorPopup.ShowError(null, "Error entering location: " + response);
                }
            });
    }

    private static void LogError(string txt)
    {
#if UNITY_EDITOR
        Debug.LogError("[PlaceOfPower] " + txt);
#endif
    }
}
