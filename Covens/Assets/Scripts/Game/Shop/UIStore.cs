using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStore : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Space()]
    [SerializeField] private HorizontalLayoutGroup m_Header;
    [SerializeField] private Button m_CloseButton;

    [Header("Home")]
    [SerializeField] private CanvasGroup m_HomeWindow;

    [Header("Store")]
    [SerializeField] private CanvasGroup m_StoreWindow;
    [SerializeField] private Transform m_PageContainer;
    [SerializeField] private ScrollRect m_PageScrollView;
    [SerializeField] private UIStorePage m_PagePrefab;
    [SerializeField] private UIStoreItem m_ItemPrefab;

    [Header("Styles")]
    [SerializeField] private CanvasGroup m_StylesWindow;

    private static UIStore m_Instance;
    
    public static void OpenStore(System.Action onLoad = null, bool showFortuna = true)
    {
        if (m_Instance != null)
        {
            //m_Instance.fortuna.gameObject.SetActive(showFortuna);
            m_Instance.Open();
            onLoad?.Invoke();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.STORE,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    LoadingOverlay.Hide();
                    //m_Instance.fortuna.gameObject.SetActive(showFortuna);
                    m_Instance.Open();
                    onLoad?.Invoke();
                });
        }
    }

    private int m_CurrentPage = 0;
    private RectTransform m_CanvasRectTransform;
    private RectTransform m_ContainerRectTransform;
    private float m_PageSize;

    private SimplePool<UIStorePage> m_PagePool;
    private SimplePool<UIStoreItem> m_ItemPool;

    private void Awake()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_StoreWindow.gameObject.SetActive(false);
        m_StylesWindow.gameObject.SetActive(false);
        m_HomeWindow.gameObject.SetActive(false);

        m_CanvasGroup.alpha = 0;
        m_StoreWindow.alpha = 0;
        m_StylesWindow.alpha = 0;
        m_HomeWindow.alpha = 0;

        m_CloseButton.onClick.AddListener(OnClickClose);

        m_PagePool = new SimplePool<UIStorePage>(m_PagePrefab, 3);
        m_ItemPool = new SimplePool<UIStoreItem>(m_ItemPrefab, 20);
    }

    private void Start()
    {
        m_CanvasRectTransform = m_Canvas.GetComponent<RectTransform>();
        m_ContainerRectTransform = m_PageContainer.GetComponent<RectTransform>();
        m_PageSize = m_CanvasRectTransform.sizeDelta.x;
    }

    private void Update()
    {
        float pos = Mathf.Abs(m_ContainerRectTransform.anchoredPosition.x - m_PageSize);
        int page = Mathf.RoundToInt(pos / m_PageSize);

        if (page != m_CurrentPage)
        {
            LoadPage(page);
        }
    }

    private void LoadPage(int page)
    {
        m_CurrentPage = page;

    }

    private void OnClickClose()
    {
        Close();
    }

    [ContextMenu("Open")]
    public void Open()
    {

    }

    [ContextMenu("Close")]
    public void Close()
    {

    }
}
