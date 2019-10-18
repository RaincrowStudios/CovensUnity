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
    private static float m_LastRequestTime = 0;
    private static float m_RequestCooldown = 60;
    private static bool m_IsRetrieving = false;

    public static List<UINearbyLocationItem.LocationData> CachedLocations { get; private set; }
    public static bool IsOpen => m_Instance != null && m_Instance.m_InputRaycaster.enabled;

    private int m_AnimTweenId;
    private SimplePool<UINearbyLocationItem> m_ItemPool;

    public static void Open(System.Action onLoad = null)
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
            onLoad?.Invoke();
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
                    onLoad?.Invoke();
                    LoadingOverlay.Hide();
                });
        }
    }

    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance.Hide();
    }

    public static void GetLocations(System.Action<List<UINearbyLocationItem.LocationData>> onComplete, bool showLoading = true)
    {
        //if (CachedLocations == null || Time.unscaledTime - m_LastRequestTime > m_RequestCooldown)
        //{
        m_LastRequestTime = Time.unscaledTime;
        m_IsRetrieving = true;
        if (showLoading)
            LoadingOverlay.Show();
        APIManager.Instance.Get("place-of-power/near", (response, result) =>
        {
            if (showLoading)
                LoadingOverlay.Hide();

            if (result == 200)
            {
                CachedLocations = JsonConvert.DeserializeObject<List<UINearbyLocationItem.LocationData>>(response);
                onComplete?.Invoke(CachedLocations);
            }
            else
            {
                UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
                onComplete?.Invoke(null);
            }
            m_IsRetrieving = false;
        });
        //}
        //else
        //{
        //    onComplete?.Invoke(CachedLocations);
        //}
    }

    public static void Refresh()
    {
        if (m_Instance == null || m_Instance.m_Canvas.enabled == false)
            return;

        if (m_IsRetrieving)
            return;

        Debug.Log("Refreshing nearby pops");
        CachedLocations = null;
        GetLocations(pops =>
        {
            var items = m_Instance.m_ItemPool.GetInstances();
            for (int i = pops.Count - 1; i >= 0; i--)
            {
                var data = pops[i];
                foreach (var item in items)
                {
                    if (item.m_Data.id == data.id)
                    {
                        item.Setup(data, () =>
                        {
                            PlayerManager.Instance.FlyTo(data.longitude, data.latitude);
                            m_Instance.Hide();
                        });
                        pops.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int i = 0; i < pops.Count; i++)
            {
                var data = pops[i];
                UINearbyLocationItem item = m_Instance.m_ItemPool.Spawn(m_Instance.m_Container.transform);
                item.Setup(data, () =>
                {
                    PlayerManager.Instance.FlyTo(data.longitude, data.latitude);
                    m_Instance.Hide();
                });
            }
        },
        false);
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CloseButton.onClick.AddListener(Hide);
        m_ItemPool = new SimplePool<UINearbyLocationItem>(m_Prefab, 50);

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        LeanTween.cancel(m_AnimTweenId);
        m_ItemPool?.DestroyAll();

        SceneManager.UnloadScene(SceneManager.Scene.NEARBY_POPS, null, null);
    }

    //private void OnDestroy()
    //{
    //    if (m_ItemPool != null)
    //    {
    //        m_ItemPool.DestroyAll();
    //        m_ItemPool = null;
    //    }
    //}

    private void Show()
    {
        BackButtonListener.AddCloseAction(Close);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        AnimShow(null);
        GetLocations(locations => SetupLocations(locations));
    }

    [ContextMenu("Close")]
    private void Hide()
    {
        BackButtonListener.RemoveCloseAction();
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

    private void SetupLocations(List<UINearbyLocationItem.LocationData> locations)
    {
        m_ItemPool.DespawnAll();
        CachedLocations = locations;
        foreach (var location in locations)
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
