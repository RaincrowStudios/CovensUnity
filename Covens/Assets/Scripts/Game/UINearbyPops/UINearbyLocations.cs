using Newtonsoft.Json;
using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINearbyLocations : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private LayoutGroup m_Container;
    [SerializeField] private UINearbyLocationItem m_Prefab;

    [SerializeField] private Button m_CloseButton;

    private static UINearbyLocations m_Instance;
    
    public static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.NEARBY_POPS,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance.Show();
                    LoadingOverlay.Hide();
                });
        }
    }

    private int m_AnimTweenId;
    private List<UINearbyLocationItem.LocationData> m_Locations = null;
    private float m_LastRequestTime = 0;
    private float m_RequestCooldown = 60;
    private SimplePool<UINearbyLocationItem> m_ItemPool;

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CloseButton.onClick.AddListener(Hide);
        m_ItemPool = new SimplePool<UINearbyLocationItem>(m_Prefab, 50);
    }

    private void OnDestroy()
    {
        if (m_ItemPool != null)
        {
            m_ItemPool.DestroyAll();
            m_ItemPool = null;
        }
    }

    private void Show()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        AnimShow(null);
        GetLocations();
    }

    [ContextMenu("Close")]
    private void Hide()
    {
        m_InputRaycaster.enabled = false;
        AnimHide(() =>
        {
            m_Canvas.enabled = false;
        });
    }

    private void AnimShow(System.Action onComplete)
    {
        LeanTween.cancel(m_AnimTweenId);
        m_AnimTweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(onComplete)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void AnimHide(System.Action onComplete)
    {
        LeanTween.cancel(m_AnimTweenId);
        m_AnimTweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(onComplete)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void GetLocations()
    {
        if (m_Locations == null || Time.unscaledTime - m_LastRequestTime > m_RequestCooldown)
        {
            m_LastRequestTime = Time.unscaledTime;
            LoadingOverlay.Show();
            APIManager.Instance.Get("place-of-power/near", (response, result) =>
            {
                LoadingOverlay.Hide();
                if (result == 200)
                {
                    List<UINearbyLocationItem.LocationData> nearbyPops = JsonConvert.DeserializeObject<List<UINearbyLocationItem.LocationData>>(response);
                    SetupLocations(nearbyPops);
                }
                else
                {
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
                }
            });
        }
        else
        {
            SetupLocations(m_Locations);
        }
    }

    private void SetupLocations(List<UINearbyLocationItem.LocationData> locations)
    {
        m_ItemPool.DespawnAll();
        m_Locations = locations;
        foreach(var location in locations)
        {
            UINearbyLocationItem item = m_ItemPool.Spawn(m_Container.transform);
            item.Setup(location, () =>
            {
                PlayerManager.Instance.FlyTo(location.longitude, location.latitude);
                this.Hide();
            });
        }
    }
}
