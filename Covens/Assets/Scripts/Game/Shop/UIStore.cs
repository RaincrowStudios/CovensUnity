using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStore : MonoBehaviour
{
    public enum Screen
    {
        NONE = 0,
        HOME,
        COSMETICS,
        STYLES,
        CURRENCY,
        INGREDIENTS,
        CHARMS,
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Space()]
    [SerializeField] private RectTransform m_Header;
    [SerializeField] private Button m_CloseButton;

    [Header("Home")]
    [SerializeField] private CanvasGroup m_HomeWindow;
    [SerializeField] private Button m_CosmeticsButton;
    [SerializeField] private Button m_CurrenciesButton;
    [SerializeField] private Button m_CharmsButton;
    [SerializeField] private Button m_IngredientsButton;

    [Header("Store")]
    [SerializeField] private CanvasGroup m_StoreWindow;
    [SerializeField] private Transform m_PageContainer;
    [SerializeField] private ScrollRect m_PageScrollView;
    [SerializeField] private UIStorePage m_PagePrefab;
    [SerializeField] private UIStoreItem m_ItemPrefab;
    [SerializeField] private RectTransform m_BottomBar;

    [Header("Styles")]
    [SerializeField] private UIStoreStylesWindow m_StylesWindow;

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

    private Screen m_CurrentScreen = Screen.NONE;
    private int m_CurrentPage = 0;
    private float m_PageSize;
    private RectTransform m_CanvasRectTransform;
    private RectTransform m_ContainerRectTransform;

    private SimplePool<UIStorePage> m_PagePool;
    private SimplePool<UIStoreItem> m_ItemPool;

    private int m_MainTweenId;
    private int m_ScreenTweenId;

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


        m_PagePool = new SimplePool<UIStorePage>(m_PagePrefab, 3);
        m_ItemPool = new SimplePool<UIStoreItem>(m_ItemPrefab, 20);

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CosmeticsButton.onClick.AddListener(() => SetScreen(Screen.COSMETICS));
        m_CurrenciesButton.onClick.AddListener(() => SetScreen(Screen.CURRENCY));
        m_CharmsButton.onClick.AddListener(() => SetScreen(Screen.CHARMS));
        m_IngredientsButton.onClick.AddListener(() => SetScreen(Screen.INGREDIENTS));
    }

    private void Start()
    {
        m_CanvasRectTransform = m_Canvas.GetComponent<RectTransform>();
        m_ContainerRectTransform = m_PageContainer.GetComponent<RectTransform>();
        m_PageSize = m_CanvasRectTransform.sizeDelta.x;
        SetScreen(Screen.HOME);
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
        if (m_CurrentScreen == Screen.HOME)
            Close();
        else
            SetScreen(Screen.HOME);
    }

    [ContextMenu("Open")]
    public void Open()
    {
        LeanTween.cancel(m_MainTweenId);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_MainTweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 0.25f)
            .setOnUpdate((float v) =>
            {
                m_CanvasGroup.alpha = v;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    [ContextMenu("Close")]
    public void Close()
    {
        LeanTween.cancel(m_MainTweenId);
        m_InputRaycaster.enabled = false;

        m_MainTweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 0.5f)
            .setOnUpdate((float v) =>
            {
                m_CanvasGroup.alpha = v;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    public void SetScreen(Screen screen)
    {
        if (screen == m_CurrentScreen)
            return;

        LeanTween.cancel(m_ScreenTweenId);
        CanvasGroup toShow = null;
        CanvasGroup toHide = null;
        List<CanvasGroup> screens = new List<CanvasGroup>() { m_HomeWindow, m_StoreWindow, m_StylesWindow.canvasGroup };

        switch (m_CurrentScreen)
        {
            case Screen.HOME:           toHide = m_HomeWindow;      break;
            case Screen.COSMETICS:      toHide = m_StoreWindow;     break;
            case Screen.CURRENCY:       toHide = m_StoreWindow;     break;
            case Screen.INGREDIENTS:    toHide = m_StoreWindow;     break;
            case Screen.STYLES:         toHide = m_StylesWindow.canvasGroup;    break;
            case Screen.CHARMS:         toShow = m_StoreWindow;     break;
        }
        switch (screen)
        {
            case Screen.HOME:       toShow = m_HomeWindow; break;
            case Screen.COSMETICS:  toShow = m_StoreWindow; break;
            case Screen.CURRENCY:   toShow = m_StoreWindow; break;
            case Screen.INGREDIENTS:toShow = m_StoreWindow; break;
            case Screen.STYLES:     toShow = m_StylesWindow.canvasGroup; break;
            case Screen.CHARMS:     toShow = m_StoreWindow; break;
        }
        System.Action setupScreen = () =>
        {
            switch (screen)
            {
                case Screen.HOME:       SetupHome(); break;
                case Screen.COSMETICS:  SetupCosmetics(); break;
                case Screen.CURRENCY:   SetupCurrency(); break;
                case Screen.INGREDIENTS:SetupIngredients(); break;
                case Screen.STYLES:     SetupStyles(); break;
                case Screen.CHARMS:     SetupCharms(); break;
            }
        };

        m_CurrentScreen = screen;

        //show and hide at same time
        if (toShow != toHide)
        {
            if (toHide)
                toHide.blocksRaycasts = toHide.interactable = false;
            if (toShow)
                toShow.blocksRaycasts = toShow.interactable = true;

            float start = toShow ? toShow.alpha : 0;
            m_ScreenTweenId = LeanTween.value(start, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() => 
                {
                    setupScreen();
                    toShow?.gameObject.SetActive(true);
                })
                .setOnUpdate((float v) =>
                {
                    foreach(var item in screens)
                    {
                        if (item != toShow)
                            item.alpha = 1 - v;
                        else
                            item.alpha = v;
                    }
                })
                .setOnComplete(() =>
                {
                    toHide?.gameObject.SetActive(false);
                    foreach (var item in screens)
                    {
                        if (item != toShow)
                            item.gameObject.SetActive(false);
                    }
                })
                .uniqueId;
        }
        //hide first then show
        else
        {
            if (toHide)
                toHide.blocksRaycasts = toHide.interactable = false;

            m_ScreenTweenId = LeanTween.alphaCanvas(toHide, 0, 0.2f)
                .setOnComplete(() =>
                {
                    setupScreen();

                    if (toShow)
                        toShow.blocksRaycasts = toShow.interactable = true;

                    toHide?.gameObject.SetActive(false);
                    toShow?.gameObject.SetActive(true);

                    m_ScreenTweenId = LeanTween.alphaCanvas(toShow, 1f, 0.5f)
                        .setEaseOutCubic()
                        .uniqueId;
                })
                .uniqueId;
        }
    }

    public void SetHeaderText(params string[] title)
    {
        for (int i = 0; i < m_Header.transform.childCount; i++)
        {
            if (i < title.Length)
            {
                m_Header.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = title[i];
                m_Header.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_Header.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetHeaderButtons(params UnityEngine.Events.UnityAction[] onClick)
    {
        for (int i = 0; i < m_Header.childCount; i++)
        {
            if (i < onClick.Length)
            {
                Button button = m_Header.GetChild(i).GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(onClick[i]);
                m_Header.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_Header.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetBottomText(params string[] title)
    {
        for (int i = 0; i < m_BottomBar.childCount; i++)
        {
            if (i < title.Length)
            {
                m_BottomBar.GetChild(i).GetComponent<TextMeshProUGUI>().text = title[i];
                m_BottomBar.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_BottomBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void SetBottomButtons(params UnityEngine.Events.UnityAction[] onClick)
    {
        for (int i = 0; i < m_BottomBar.childCount; i++)
        {
            if (i < onClick.Length)
            {
                Button button = m_BottomBar.GetChild(i).GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(onClick[i]);
                m_BottomBar.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                m_BottomBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void SetupHome()
    {
        SetHeaderText();
    }

    private void SetupCosmetics()
    {
        SetHeaderText(
            LocalizeLookUp.GetText("store_cosmetics"), 
            LocalizeLookUp.GetText("store_styles"));
        SetHeaderButtons(
            () => SetScreen(Screen.COSMETICS),
            () => SetScreen(Screen.STYLES));

        SetBottomText(
            LocalizeLookUp.GetText("store_gear_clothing"),
            LocalizeLookUp.GetText("store_gear_accessories"),
            LocalizeLookUp.GetText("store_gear_skin_art"),
            LocalizeLookUp.GetText("store_gear_hairstyle"));
    }

    private void SetupStyles()
    {
        SetHeaderText(
            LocalizeLookUp.GetText("store_cosmetics"),
            LocalizeLookUp.GetText("store_styles"));
        SetHeaderButtons(
            () => SetScreen(Screen.COSMETICS),
            () => SetScreen(Screen.STYLES));

        SetBottomText();
    }

    private void SetupCurrency()
    {
        SetHeaderText();
        SetBottomText();
    }

    private void SetupCharms()
    {
        SetHeaderText();
        SetBottomText();
    }

    private void SetupIngredients()
    {
        SetHeaderText();
        SetBottomText();
    }
}
