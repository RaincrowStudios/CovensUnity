using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Chat.UI;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using Raincrow.Chat;

public class UICovenSearcher : MonoBehaviour
{
    private class ChatCovenDataSearchQuery
    {
        private IEnumerable<ChatCovenData> _covens { get; set; }
        private string _searchQuery { get; set; }
        private int _maxCovensQuery { get; set; }

        public ChatCovenDataSearchQuery(List<ChatCovenData> covens, string searchQuery = "", int maxCovens = 10)
        {
            _covens = covens;
            _searchQuery = searchQuery;
            _maxCovensQuery = maxCovens;
        }

        public IEnumerable<ChatCovenData> GetCovens()
        {
            IEnumerable<ChatCovenData> covensToRetrieve = _covens.OrderBy((coven1) => coven1.worldRank);

            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                //covensToRetrieve = covensToRetrieve.Where(coven => coven.name.StartsWith(_searchQuery, System.StringComparison.OrdinalIgnoreCase));
                covensToRetrieve = covensToRetrieve.Where(coven => coven.name.IndexOf(_searchQuery, System.StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return covensToRetrieve.Take(_maxCovensQuery);
        }
    }


    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Window;

    [Header("Header")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_RecentCovensButton;
    [SerializeField] private Button m_TopCovensButton;
    [SerializeField] private RectTransform m_HighlightObj;

    [Header("Container")]
    [SerializeField] private CanvasGroup m_ContainerCanvasGroup;
    [SerializeField] private LayoutGroup m_Container;
    [SerializeField] private UIChatCoven m_ItemPrefab;
    [SerializeField] private GameObject m_LoadingOverlay;

    [Header("Search")]
    [SerializeField] private TMP_InputField m_SearchField;

    private SimplePool<UIChatCoven> m_ItemPool;
    private List<ChatCovenData> m_RecentCovens = new List<ChatCovenData>();
    private List<ChatCovenData> m_TopCovens = new List<ChatCovenData>();
    private Coroutine m_ShowCovensCoroutine;

    private int m_MaxCovensAvailable = 20;
    private float m_RequestCovensCooldown = 300;
    private bool m_RecentTab = true;

    private float m_LastTopCovensRequestTime = Mathf.NegativeInfinity;
    private float m_LastRecentCovensRequestTime = Mathf.NegativeInfinity;
    private int m_TweenId;
    private int m_ContainerTweenId;

    private System.Action m_OnClose;
    private System.Action<string> m_OnSelectCoven;

    private void Awake()
    {
        //setup initial state
        m_CanvasGroup.alpha = 0;
        m_ContainerCanvasGroup.alpha = 0;
        m_LoadingOverlay.SetActive(false);
        m_ItemPool = new SimplePool<UIChatCoven>(m_ItemPrefab, m_MaxCovensAvailable);

        TextMeshProUGUI placeholderText = m_SearchField.placeholder as TextMeshProUGUI;
        placeholderText.text = LocalizeLookUp.GetText("coven_search");

        m_RecentTab = true;
        m_HighlightObj.SetParent(m_RecentCovensButton.transform);
        m_HighlightObj.localPosition = Vector3.zero;

        //setup button listeners
        m_RecentCovensButton.onClick.AddListener(() =>
        {
            if (m_RecentTab)
                return;

            m_RecentTab = true;
            RequestAvailableCovens(m_SearchField.text, m_RecentTab);
            m_HighlightObj.SetParent(m_RecentCovensButton.transform);
            m_HighlightObj.localPosition = Vector3.zero;
            //m_HighlightObj.transform.localPosition = m_RecentCovensButton.transform.localPosition;
        });

        m_TopCovensButton.onClick.AddListener(() =>
        {
            if (m_RecentTab == false)
                return;

            m_RecentTab = false;
            RequestAvailableCovens(m_SearchField.text, m_RecentTab);
            m_HighlightObj.SetParent(m_TopCovensButton.transform);
            m_HighlightObj.localPosition = Vector3.zero;
            //m_HighlightObj.transform.localPosition = m_TopCovensButton.transform.localPosition;
        });

        m_CloseButton.onClick.AddListener(OnClickClose);

        m_SearchField.onValueChanged.AddListener(OnSearchStringChange);

        m_Window.gameObject.SetActive(false);
    }
    
    public void Show(System.Action<string> onSelect, System.Action onClose)
    {
        Show();

        m_OnSelectCoven = onSelect;
        m_OnClose = onClose;
    }

    private void OnClickClose()
    {
        Close();
        
        m_OnClose?.Invoke();
        m_OnClose = null;
        m_OnSelectCoven = null;
    }

    [ContextMenu("Show")]
    private void Show()
    {
        BackButtonListener.AddCloseAction(OnClickClose);

        //update the list
        RequestAvailableCovens(m_SearchField.text, m_RecentTab);
        //m_HighlightObj.transform.localPosition = m_RecentTab ? m_RecentCovensButton.transform.localPosition : m_TopCovensButton.transform.localPosition;

        m_Window.gameObject.SetActive(true);

        //animate the ui
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    [ContextMenu("Close")]
    public void Close()
    {
        BackButtonListener.RemoveCloseAction();

        //animat ethe ui
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, .5f)
            .setEaseOutCubic()
            .setOnComplete(() => m_Window.gameObject.SetActive(false))
            .uniqueId;

        LeanTween.cancel(m_ContainerTweenId);
        m_ContainerTweenId = LeanTween.alphaCanvas(m_ContainerCanvasGroup, 0, 1).uniqueId;
    }

    private void OnSearchStringChange(string searchQuery)
    {
        List<ChatCovenData> covens = m_RecentTab ? m_RecentCovens : m_TopCovens;
        if (covens.Count > 0)
        {
            searchQuery = m_SearchField.text;

            if (m_ShowCovensCoroutine != null)
                StopCoroutine(m_ShowCovensCoroutine);

            ChatCovenDataSearchQuery chatCovenDataQuery = new ChatCovenDataSearchQuery(covens, searchQuery, m_MaxCovensAvailable);
            m_ShowCovensCoroutine = StartCoroutine(ShowAvailableCovensCoroutine(chatCovenDataQuery));
        }
    }

    private void RequestAvailableCovens(string searchQuery, bool recentCovens)
    {
        float lastRequestTime = recentCovens ? m_LastRecentCovensRequestTime : m_LastTopCovensRequestTime;
        
        //setup the cached list
        if (Time.realtimeSinceStartup - lastRequestTime < m_RequestCovensCooldown)
        {
            Debug.Log($"showing {(recentCovens ? "recent" : "top")} covens list");

            if (m_ShowCovensCoroutine != null)
                StopCoroutine(m_ShowCovensCoroutine);

            ChatCovenDataSearchQuery chatCovenDataQuery = new ChatCovenDataSearchQuery(recentCovens ? m_RecentCovens : m_TopCovens, searchQuery, m_MaxCovensAvailable);
            m_ShowCovensCoroutine = StartCoroutine(ShowAvailableCovensCoroutine(chatCovenDataQuery));
            return;
        }

        Debug.Log($"request {(recentCovens ? "recent" : "top")} covens list");

        //disable container while waiting the list form the server
        m_ContainerCanvasGroup.alpha = 0;
        m_ContainerCanvasGroup.interactable = false;

        //request new list
        m_LoadingOverlay.SetActive(true);
        APIManager.Instance.Get("coven?recent=" + recentCovens, (string payload, int response) =>
        {
            if (recentCovens)
                m_LastRecentCovensRequestTime = Time.realtimeSinceStartup;
            else
                m_LastTopCovensRequestTime = Time.realtimeSinceStartup;

            if (response == 200)
            {
                if (m_ShowCovensCoroutine != null)
                    StopCoroutine(m_ShowCovensCoroutine);

                //cache the list
                if (recentCovens)
                {
                    m_RecentCovens.Clear();
                    m_RecentCovens = JsonConvert.DeserializeObject<List<ChatCovenData>>(payload);
                }
                else
                {
                    m_TopCovens.Clear();
                    m_TopCovens = JsonConvert.DeserializeObject<List<ChatCovenData>>(payload);
                }

                //setup the list
                ChatCovenDataSearchQuery chatCovenDataQuery = new ChatCovenDataSearchQuery(recentCovens ? m_RecentCovens : m_TopCovens, searchQuery, m_MaxCovensAvailable);
                m_ShowCovensCoroutine = StartCoroutine(ShowAvailableCovensCoroutine(chatCovenDataQuery));
            }
        });
    }

    private IEnumerator ShowAvailableCovensCoroutine(ChatCovenDataSearchQuery chatCovenDataQuery)
    {
        //hide the container
        LeanTween.cancel(m_ContainerTweenId);
        m_ContainerTweenId = LeanTween.alphaCanvas(m_ContainerCanvasGroup, 0, m_ContainerCanvasGroup.alpha / 5f).uniqueId;
        m_ContainerCanvasGroup.interactable = false;


        yield return new WaitForSeconds(m_ContainerCanvasGroup.alpha / 5f);

        m_LoadingOverlay.SetActive(true);
        m_ItemPool.DespawnAll();

        IEnumerable<ChatCovenData> chatCovenDatas = chatCovenDataQuery.GetCovens();

        //prepare the ui
        List<UIChatCoven> items = new List<UIChatCoven>();
        for (int i = 0; i < chatCovenDatas.Count(); i++)
        {
            ChatCovenData data = chatCovenDatas.ElementAt(i);
            UIChatCoven uiChatCoven = m_ItemPool.Spawn();

            uiChatCoven.transform.SetParent(m_Container.transform);
            uiChatCoven.transform.localScale = Vector3.one;
            uiChatCoven.SetupCoven(
                data, 
                () =>
                {
                    //Close();
                    //TeamManagerUI.OpenName(data.name, () => Show());
                    m_OnSelectCoven?.Invoke(data.name);
                }
            );
            items.Add(uiChatCoven);
        }

        m_LoadingOverlay.SetActive(false);

        //show the container
        m_ContainerTweenId = LeanTween.alphaCanvas(m_ContainerCanvasGroup, 1, 1)
            .setEaseOutCubic()
            .uniqueId;

        m_ContainerCanvasGroup.interactable = true;
    }

    private void OnDestroy()
    {
        m_ItemPool.DestroyAll();
    }
}
