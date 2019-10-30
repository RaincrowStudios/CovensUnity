using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPositionHelper : MonoBehaviour
{
    private static PhysicalPositionHelper m_Instance;
    public static PhysicalPositionHelper Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("PhysicalPositionHelper")).AddComponent<PhysicalPositionHelper>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    private float m_LastLongitude;
    private float m_LastLatitude;

    public event System.Action OnPositionChange;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(UpdatePosition());
    }

    private IEnumerator UpdatePosition()
    {
        bool changed = false;
        while (true)
        {
            if (m_LastLatitude != GetGPS.latitude || m_LastLongitude != GetGPS.longitude)
                changed = true;

            if (changed)
            {
                //Debug.Log("<color=magenta>moved lat" + (GetGPS.latitude - m_LastLatitude) + " lng" + (GetGPS.longitude - m_LastLongitude) + "</color>");
                OnPositionChange?.Invoke();
                m_LastLongitude = GetGPS.longitude;
                m_LastLatitude = GetGPS.latitude;
                changed = false;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
}
