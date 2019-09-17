using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInitializer : MonoBehaviour
{
    [SerializeField] private bool m_InitializeMap = false;
    [SerializeField] private float m_Longitude = -122.3224f;
    [SerializeField] private float m_Latitude = 47.70168f;
    [SerializeField] private bool m_Login = false;

    private void Awake()
    {
#if PRODUCTION
        Destroy(this.gameObject);
        return;
#endif

#if !UNITY_EDITOR
        Destroy(this.gameObject);
        return;
#endif

        if (m_InitializeMap)
        {
            MapsAPI.Instance.InstantiateMap();
        }

        if (m_Login && !LoginAPIManager.characterLoggedIn)
        {
            DownloadManager.OnDownloadsComplete += () => LoginAPIManager.Login((result, response) => LoginAPIManager.GetCharacter(null));
            DownloadManager.DownloadAssets(null);
        }
    }

    private void Start()
    {
        if (m_InitializeMap)
            LeanTween.delayedCall(1f, () => MapsAPI.Instance.InitMap(m_Longitude, m_Latitude, 0.98f, null, true));

        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
    }
}
