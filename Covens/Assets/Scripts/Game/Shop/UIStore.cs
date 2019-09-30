using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStore : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private HorizontalLayoutGroup m_Header;

    [Header("Home")]
    [SerializeField] private CanvasGroup m_HomeWindow;
    //[SerializeField] private M

    [Header("Store")]
    [SerializeField] private CanvasGroup m_StoreWindow;
    [SerializeField] private Transform m_PageContainer;
    [SerializeField] private ScrollRect m_PageScrollView;
    [SerializeField] private UIStorePage m_PagePrefab;

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

    private void Awake()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        m_StoreWindow.gameObject.SetActive(true);
        m_StylesWindow.gameObject.SetActive(false);
        m_HomeWindow.gameObject.SetActive(false);

        m_StoreWindow.alpha = 1;
        m_StylesWindow.alpha = 0;
        m_HomeWindow.alpha = 0;
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

    public void Open()
    {

    }

    public void Close()
    {

    }
}
