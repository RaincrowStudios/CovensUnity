using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

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
    [SerializeField] private TextMeshProUGUI m_GoldDrachs;
    [SerializeField] private TextMeshProUGUI m_SilverDrachs;

    [Header("Home")]
    [SerializeField] private CanvasGroup m_HomeWindow;
    [SerializeField] private Button m_CosmeticsButton;
    [SerializeField] private Button m_CurrenciesButton;
    [SerializeField] private Button m_CharmsButton;
    [SerializeField] private Button m_IngredientsButton;

    [Header("Store")]
    [SerializeField] private UIStoreContainer m_StoreWindow;

    [Header("Styles")]
    [SerializeField] private UIStoreStylesWindow m_StylesWindow;
        
    private Screen m_CurrentScreen = Screen.NONE;
    private int m_MainTweenId;
    private int m_ScreenTweenId;
    private int m_DrachsTweenId;

    private static UIStore m_Instance;

    public static bool IsOpen => m_Instance != null && m_Instance.m_InputRaycaster.enabled;

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

    public static void UpdateDrachs()
    {
        if (m_Instance == null)
            return;

        m_Instance._UpdateDrachs();
    }

    private void Awake()
    {
        if (UnityEngine.EventSystems.EventSystem.current == null)
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        m_Instance = this;
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
        m_CosmeticsButton.onClick.AddListener(() => SetScreen(Screen.COSMETICS));
        m_CurrenciesButton.onClick.AddListener(() => SetScreen(Screen.CURRENCY));
        m_CharmsButton.onClick.AddListener(() => SetScreen(Screen.CHARMS));
        m_IngredientsButton.onClick.AddListener(() => SetScreen(Screen.INGREDIENTS));

        SetScreen(Screen.HOME);

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;

        LeanTween.cancel(m_MainTweenId);
        LeanTween.cancel(m_ScreenTweenId);
        LeanTween.cancel(m_DrachsTweenId);
        SceneManager.UnloadScene(SceneManager.Scene.STORE, null, null);
    }

    private void OnClickClose()
    {
        if (m_CurrentScreen == Screen.HOME)
            Close();
        else if (m_CurrentScreen == Screen.STYLES)
            SetScreen(Screen.COSMETICS);
        else
            SetScreen(Screen.HOME);
    }

    [ContextMenu("Open")]
    public void Open()
    {
        if (m_Canvas.enabled)
            return;

        StoreManagerAPI.OnPurchaseComplete += OnPurchaseComplete;

        LeanTween.cancel(m_MainTweenId);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_SilverDrachs.text = PlayerDataManager.playerData.silver.ToString();
        m_GoldDrachs.text = PlayerDataManager.playerData.gold.ToString();

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
        if (!m_InputRaycaster.enabled)
            return;

        StoreManagerAPI.OnPurchaseComplete -= OnPurchaseComplete;

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
        List<CanvasGroup> screens = new List<CanvasGroup>() {
            m_HomeWindow,
            m_StoreWindow.canvasGroup,
            m_StylesWindow.canvasGroup
        };

        //get the canvas group that will be shown
        switch (m_CurrentScreen)
        {
            case Screen.HOME:           toHide = m_HomeWindow;      break;
            case Screen.COSMETICS:      toHide = m_StoreWindow.canvasGroup;     break;
            case Screen.CURRENCY:       toHide = m_StoreWindow.canvasGroup;     break;
            case Screen.INGREDIENTS:    toHide = m_StoreWindow.canvasGroup;     break;
            case Screen.STYLES:         toHide = m_StylesWindow.canvasGroup;    break;
            case Screen.CHARMS:         toShow = m_StoreWindow.canvasGroup;     break;
        }
        //get the canvasgroup that wil lbe hidden
        switch (screen)
        {
            case Screen.HOME:       toShow = m_HomeWindow; break;
            case Screen.COSMETICS:  toShow = m_StoreWindow.canvasGroup; break;
            case Screen.CURRENCY:   toShow = m_StoreWindow.canvasGroup; break;
            case Screen.INGREDIENTS:toShow = m_StoreWindow.canvasGroup; break;
            case Screen.STYLES:     toShow = m_StylesWindow.canvasGroup; break;
            case Screen.CHARMS:     toShow = m_StoreWindow.canvasGroup; break;
        }
        //prepare the setup screen method
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

        if (toShow != toHide)
        {
            if (toHide)
                toHide.blocksRaycasts = toHide.interactable = false;
            if (toShow)
                toShow.blocksRaycasts = toShow.interactable = true;

            float start = toShow ? toShow.alpha : 0;

            //setup the screen and fade the canvas gorups
            m_ScreenTweenId = LeanTween.value(start, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() => 
                {
                    toShow?.gameObject.SetActive(true);
                    setupScreen();
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
        else
        {
            if (toHide)
                toHide.blocksRaycasts = toHide.interactable = false;

            //first hide the canvasgroup
            m_ScreenTweenId = LeanTween.alphaCanvas(toHide, 0, 0.2f)
                .setOnComplete(() =>
                {
                    if (toShow)
                        toShow.blocksRaycasts = toShow.interactable = true;

                    toHide?.gameObject.SetActive(false);
                    toShow?.gameObject.SetActive(true);

                    //setup the screen and show the canvasgroup
                    setupScreen();

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

    public void SetHeaderButtons(int start, params UnityEngine.Events.UnityAction[] onClick)
    {
        System.Action<int, bool> toggleHeader = (index, toggle) =>
        {
            TextMeshProUGUI t = m_Header.GetChild(index).GetComponent<TextMeshProUGUI>();
            if (toggle)
            {
                t.fontStyle = FontStyles.Underline;
                t.color = Color.white;
            }
            else
            {
                t.fontStyle = FontStyles.Normal;
                t.color = Color.white * 0.64f;
            }
        };

        for (int i = 0; i < m_Header.childCount; i++)
        {
            if (i == start)
                toggleHeader.Invoke(i, true);
            else
                toggleHeader.Invoke(i, false);
        }

        for (int i = 0; i < m_Header.childCount; i++)
        {
            Button button = m_Header.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();

            int aux = i;
            if (i < onClick?.Length)
            {
                button.gameObject.SetActive(true);
                button.onClick.AddListener(() =>
                {
                    for (int j = 0; j < m_Header.childCount; j++)
                        toggleHeader.Invoke(j, j == aux);

                    onClick[aux]?.Invoke();
                });
            }
            else
            {
                button.gameObject.SetActive(false);
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
            0,
            () => SetScreen(Screen.COSMETICS),
            () => SetScreen(Screen.STYLES));

        m_StoreWindow.SetupCosmetics(StoreManagerAPI.Store.Cosmetics);
    }

    private void SetupStyles()
    {
        SetHeaderText(
            LocalizeLookUp.GetText("store_cosmetics"),
            LocalizeLookUp.GetText("store_styles"));

        SetHeaderButtons(
            1,
            () => SetScreen(Screen.COSMETICS),
            () => SetScreen(Screen.STYLES));
    }

    private void SetupCurrency()
    {
        SetHeaderButtons(0);
        SetHeaderText(LocalizeLookUp.GetText("store_currency"));
        m_StoreWindow.SetupCurrency(StoreManagerAPI.Store.Currencies);
    }

    private void SetupCharms()
    {
        SetHeaderButtons(0);
        SetHeaderText(LocalizeLookUp.GetText("store_charms"));
        m_StoreWindow.SetupCharms(StoreManagerAPI.Store.Consumables);
    }

    private void SetupIngredients()
    {
        SetHeaderButtons(0);
        SetHeaderText(LocalizeLookUp.GetText("store_ingredients"));
        m_StoreWindow.SetupIngredients(StoreManagerAPI.Store.Bundles);
    }

    private void _UpdateDrachs(float time = 2)
    {
        LeanTween.cancel(m_DrachsTweenId);
        int startGold = PlayerDataManager.playerData.gold;
        int startSilver = PlayerDataManager.playerData.silver;

        int.TryParse(m_GoldDrachs.text, out startGold);
        int.TryParse(m_SilverDrachs.text, out startSilver);

        m_DrachsTweenId = LeanTween.value(0, 1, 2)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_GoldDrachs.text = ((int)Mathf.Lerp(startGold, PlayerDataManager.playerData.gold, t)).ToString();
                m_SilverDrachs.text = ((int)Mathf.Lerp(startSilver, PlayerDataManager.playerData.silver, t)).ToString();
            })
            .uniqueId;
    }

    private void OnPurchaseComplete(string id, string type)
    {
        _UpdateDrachs();
    }
}
