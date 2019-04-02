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
                (new GameObject("PhysicalPositionHelper")).AddComponent<PhysicalPositionHelper>();
            }
            return m_Instance;
        }
    }

    private float m_LastLongitude;
    private float m_LastLatitude;

    public event System.Action OnPositionChange;

    private void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(m_Instance.gameObject);
        m_Instance.StartCoroutine(m_Instance.UpdatePosition());
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
                OnPositionChange?.Invoke();
                m_LastLongitude = GetGPS.longitude;
                m_LastLatitude = GetGPS.latitude;
                changed = false;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
}
