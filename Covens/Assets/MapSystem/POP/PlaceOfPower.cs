using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPower : MonoBehaviour
{
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

    }
    
    public void Show(MarkerDataDetail locationData)
    {
        MapsAPI.Instance.ScaleBuildings(0);
    }

    public void Close()
    {
        MapsAPI.Instance.ScaleBuildings(1);
    }
}
