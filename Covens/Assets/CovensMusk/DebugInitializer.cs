using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInitializer : MonoBehaviour
{
    [SerializeField] private bool m_InitializeMap = false;
    [SerializeField] private float m_Longitude = -122.3224f;
    [SerializeField] private float m_Latitude = 47.70168f;
    [SerializeField] private bool m_Login = false;
    [SerializeField] private bool m_IAP = false;

    private void Awake()
    {
#if PRODUCTION
        Destroy(this.gameObject);
        return;
#else
        if (string.IsNullOrEmpty(LoginAPIManager.StoredUserName) || string.IsNullOrEmpty(LoginAPIManager.StoredUserPassword))
        {
            LoginAPIManager.StoredUserName = "lucas002";
            LoginAPIManager.StoredUserPassword = "password";
        }

        if (m_InitializeMap)
        {
            MapsAPI.Instance.InstantiateMap();
        }

        if (m_Login)
        {
            if (!LoginAPIManager.characterLoggedIn)
            {
                DownloadManager.OnDownloadsComplete += () => LoginAPIManager.Login((result, response) => LoginAPIManager.GetCharacter(null));
                DownloadManager.DownloadAssets(() =>
                {
                    if (m_IAP && IAPSilver.instance == null)
                    {
                        new GameObject("IAPManager", typeof(IAPSilver));
                    }
                });
            }
            else
            {
                if (m_IAP && IAPSilver.instance == null)
                {
                    new GameObject("IAPManager", typeof(IAPSilver));
                }
            }
        }
#endif
    }

    private void Start()
    {
        if (m_InitializeMap)
            LeanTween.delayedCall(1f, () => MapsAPI.Instance.InitMap(m_Longitude, m_Latitude, 0.98f, null, true));

        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        if (BackButtonListener.Instance == null)
            new GameObject("BackButtonListener", typeof(BackButtonListener));
    }
}
