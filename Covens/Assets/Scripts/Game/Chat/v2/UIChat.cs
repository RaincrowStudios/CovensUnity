using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow.Chat;
using TMPro;

public class UIChat : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private Transform m_ItemContainer;
    [SerializeField] private CanvasGroup m_Loading;
    [SerializeField] private TMP_InputField m_InputField;
    [SerializeField] private Button m_SendButton;
    [SerializeField] private Button m_ShareLocationButton;
    [SerializeField] private Button m_CloseButton;

    [Header("Animation")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private CanvasGroup m_ContainerCanvasGroup;
    [SerializeField] private RectTransform m_WindowTransform;

    [Header("Header")]
    [SerializeField] private Button m_NewsButton;
    [SerializeField] private Button m_WorldButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_DominionButton;
    [SerializeField] private Button m_HelpButton;

    [Header("Prefabs")]
    [SerializeField] private UIChatItem m_ChatMessagePrefab;
    [SerializeField] private UIChatItem m_ChatLocationPrefab;
    [SerializeField] private UIChatItem m_ChatHelpPlayerPrefab;
    [SerializeField] private UIChatItem m_ChatHelpCrowPrefab;
    [SerializeField] private UIChatItem m_ChatImagePrefab;

    [Header("Settings")]
    [SerializeField] private int m_MaxItems = 10;
    [SerializeField] private float m_ShareLocationCooldown = 10f;


    public static bool IsOpen { get; private set; }
    private static UIChat m_Instance;

    private SimplePool<UIChatItem> m_ChatMessagePool;
    private SimplePool<UIChatItem> m_ChatLocationPool;
    private SimplePool<UIChatItem> m_ChatHelpPlayerPool;
    private SimplePool<UIChatItem> m_ChatHelpCrowPool;
    private SimplePool<UIChatItem> m_ChatImagePool;

    private List<ChatMessage> m_Messages;
    private List<UIChatItem> m_Items = new List<UIChatItem>();
    private ChatCategory m_CurrentCategory = ChatCategory.NONE;

    private int m_LoadingTweenId;
    
    public static void Show(ChatCategory category = ChatCategory.NONE)
    {
        if (m_Instance == null)
        {
            Debug.LogError("Chat not initialized");
            return;
        }

        if (category == ChatCategory.NONE && m_Instance.m_CurrentCategory != ChatCategory.NONE)
        {
            category = m_Instance.m_CurrentCategory;
        }

        m_Instance.AnimateShow(null);
        m_Instance.SetCategory(category);

        //PlayerManager.onQuickFlight += m_Instance._OnClickClose;
    }
    

    private void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        //setup UI to default disabled state
        m_Loading.gameObject.SetActive(false);
        m_Loading.alpha = 0;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_ContainerCanvasGroup.alpha = 0;
        m_WindowTransform.anchoredPosition = new Vector3(0, -m_WindowTransform.sizeDelta.y);

        //spawn pools
        m_ChatMessagePool = new SimplePool<UIChatItem>(m_ChatMessagePrefab, 1);
        m_ChatLocationPool = new SimplePool<UIChatItem>(m_ChatLocationPrefab, 1);
        m_ChatHelpPlayerPool = new SimplePool<UIChatItem>(m_ChatHelpPlayerPrefab, 1);
        m_ChatHelpCrowPool = new SimplePool<UIChatItem>(m_ChatHelpCrowPrefab, 1);
        m_ChatImagePool = new SimplePool<UIChatItem>(m_ChatImagePrefab, 1);

        //button listeners
        m_NewsButton.onClick.AddListener(_OnClickNews);
        m_WorldButton.onClick.AddListener(_OnClickWorld);
        m_CovenButton.onClick.AddListener(_OnClickCoven);
        m_DominionButton.onClick.AddListener(_OnClickDominion);
        m_HelpButton.onClick.AddListener(_OnClickSupport);
        m_CloseButton.onClick.AddListener(_OnClickClose);
        m_SendButton.onClick.AddListener(_OnClickSend);
        m_ShareLocationButton.onClick.AddListener(_OnClickShareLocation);

        //chat listeners
        ChatManager.OnReceiveMessage += OnReceiveMessage;
        ChatManager.OnConnected += OnConnected;
    }

    private void AnimateShow(System.Action onComplete)
    {
        //todo: properly animate
        gameObject.SetActive(true);
        m_CanvasGroup.alpha = 1;
        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;
        m_WindowTransform.anchoredPosition = Vector2.zero;

        IsOpen = true;
        onComplete?.Invoke();
    }

    private void AnimateHide()
    {
        IsOpen = false;

        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
        m_Instance.m_CanvasGroup.alpha = 0;
        m_Instance.gameObject.SetActive(false);
    }

    public void SetCategory(ChatCategory category, bool force = false)
    {
        if (!force && m_CurrentCategory == category)
            return;

        Debug.Log("[Chat] SetCategory: " + category);
        m_CurrentCategory = category;

        if (category == ChatCategory.COVEN && ChatManager.IsConnected(ChatCategory.COVEN) == false)
        {
            //todo: show available covens
            throw new System.NotImplementedException();
        }

        //hide the container
        m_ContainerCanvasGroup.alpha = 0;

        //despawn previous items
        ClearItems();
        
        if (ChatManager.IsConnected(category))
        {
            //setup the UI with the available messages
            m_Messages = ChatManager.GetMessages(category);
            StartCoroutine(SpawnChatItems());

            LeanTween.alphaCanvas(m_ContainerCanvasGroup, 1, 0.5f).setEaseOutCubic();

            //hide the loading overlay (in case it was visible)
            ShowLoading(false);
        }
        else
        {
            //show the loading screen
            ShowLoading(true);
        }
    }
    
    private void ClearItems()
    {
        StopAllCoroutines();
        foreach (var item in m_Items)
        {
            item.Despawn();
        }
        m_Items.Clear();
        m_Messages = new List<ChatMessage>();
    }

    private IEnumerator SpawnChatItems()
    {
        for (int i = m_Messages.Count - 1; i >= 0; i--)
        {
            SpawnItem(m_CurrentCategory, m_Messages[i]).transform.SetAsFirstSibling();
            yield return 0;
        }
    }

    private UIChatItem SpawnItem(ChatCategory category, ChatMessage message)
    {
        SimplePool<UIChatItem> pool = null;

        //use special prefabs for support ui
        if (category == ChatCategory.SUPPORT)
        {
            if (message.player.name == ChatManager.Player.name)
            {
                pool = m_ChatHelpPlayerPool;
            }
            else
            {
                pool = m_ChatHelpCrowPool;
            }
        }
        else
        {
            if (message.type == MessageType.TEXT)
            {
                pool = m_ChatMessagePool;
            }
            else if (message.type == MessageType.LOCATION)
            {
                pool = m_ChatLocationPool;
            }
            else if (message.type == MessageType.IMAGE)
            {
                pool = m_ChatImagePool;
            }
        }

        if (pool == null)
        {
            Debug.LogError("No prefab set for " + category + " : " + message.type);
            return null;
        }

        //setup the message and add it to the scrollview
        UIChatItem item = pool.Spawn();        
        item.SetupMessage(message, pool, ShowLoading, _OnClickClose);
        item.transform.SetParent(m_ItemContainer);
        item.transform.localScale = Vector3.one;
        m_Items.Add(item);
        return item;
    }

    private void ShowLoading(bool show)
    {
        LeanTween.cancel(m_LoadingTweenId);
        m_Loading.gameObject.SetActive(true);
        m_LoadingTweenId = LeanTween.alphaCanvas(m_Loading, show ? 1 : 0, show ? 0.25f : 0.75f)
            .setEaseOutCubic()
            .setOnComplete(() => m_Loading.gameObject.SetActive(show))
            .uniqueId;
    }

    //EVENT LISTENERS
    private void OnReceiveMessage(ChatCategory category, ChatMessage message)
    {
        if (IsOpen == false)
            return;

        if (m_CurrentCategory != category)
            return;

        if (m_Items.Count >= 50)
        {
            m_Items[0].Despawn();
            m_Items.RemoveAt(0);
        }

        SpawnItem(category, message);
    }

    private void OnConnected(ChatCategory category)
    {
        if (category == m_CurrentCategory)
            SetCategory(m_CurrentCategory, true);
    }

    //BUTTON LISTENERS
    private void _OnClickNews()
    {
        SetCategory(ChatCategory.NEWS);
    }

    private void _OnClickWorld()
    {
        SetCategory(ChatCategory.WORLD);
    }

    private void _OnClickCoven()
    {
        SetCategory(ChatCategory.COVEN);
    }

    private void _OnClickDominion()
    {
        SetCategory(ChatCategory.DOMINION);
    }

    private void _OnClickSupport()
    {
        SetCategory(ChatCategory.SUPPORT);
    }

    private void _OnClickSend()
    {
        string text = m_InputField.text;

        if (string.IsNullOrEmpty(text))
            return;

        m_InputField.text = "";

        //build message data
        ChatMessage message = new ChatMessage();
        message.type = MessageType.TEXT;
        message.data.message = text;

        //send
        ChatManager.SendMessage(m_CurrentCategory, message);
    }

    private void _OnClickShareLocation()
    {        
        //build message
        ChatMessage message = new ChatMessage();
        message.type = MessageType.LOCATION;

        if (PlayerDataManager.playerData == null)
        {
            message.data.longitude = Random.Range(-180f, 180f);
            message.data.latitude = Random.Range(-85f, 85f);
            Debug.Log("Sharing fake location.");
        }
        else
        {
            message.data.longitude = PlayerDataManager.playerData.longitude;
            message.data.latitude = PlayerDataManager.playerData.latitude;
        }

        //send
        ChatManager.SendMessage(m_CurrentCategory, message);
    }

    private void _OnClickClose()
    {
        AnimateHide();

        //PlayerManager.onQuickFlight -= m_Instance._OnClickClose;
    }

    //[ContextMenu("take screenshot")]
    //private void TakeScreenshot()
    //{
    //    StartCoroutine(TakeScreenshotCoroutine());
    //}

    //private IEnumerator TakeScreenshotCoroutine()
    //{
    //    yield return new WaitForEndOfFrame();
    //    Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

    //    //resize the screenshot
    //    if (screenshot.height > 720)
    //    {
    //        int width = screenshot.width;
    //        int height = screenshot.height;
    //        float ratio = 720f / screenshot.height;

    //        width = (int)(screenshot.width * ratio);
    //        height = (int)(screenshot.height * ratio);

    //        TextureScale.Bilinear(screenshot, width, height);
    //    }

    //    //compress it
    //    screenshot.Compress(false);

    //    //finally generate the bytes array
    //    byte[] byteArray = screenshot.GetRawTextureData();

    //    ChatMessage message = new ChatMessage();
    //    message.data.image = byteArray;

    //    Destroy(screenshot);
    //}
}