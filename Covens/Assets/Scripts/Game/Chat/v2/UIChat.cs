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
    [SerializeField] private UICustomScroller m_Scroller;
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

    [Header("Settings")]
    [SerializeField] private int m_MaxItems = 10;
    [SerializeField] private float m_ShareLocationCooldown = 10f;


    public static bool IsOpen { get; private set; }
    private static UIChat m_Instance;

    private SimplePool<UIChatItem> m_ChatMessagePool;
    private SimplePool<UIChatItem> m_ChatLocationPool;
    private SimplePool<UIChatItem> m_ChatHelpPlayerPool;
    private SimplePool<UIChatItem> m_ChatHelpCrowPool;

    private List<ChatMessage> m_Messages;
    private List<UIChatItem> m_Items = new List<UIChatItem>();
    private ChatCategory m_CurrentCategory = ChatCategory.NONE;
    private float m_LastLocationShareTime = 0;

    
    public static void Show()
    {
        if (m_Instance == null)
        {
            Debug.LogError("Chat not initialized");
            return;
        }

        m_Instance.AnimateShow(() => m_Instance.SetCategory(m_Instance.m_CurrentCategory));
    }
    

    private void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(this.gameObject);

        //setup UI to default disabled state
        m_Loading.gameObject.SetActive(false);
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_ContainerCanvasGroup.alpha = 0;
        m_WindowTransform.anchoredPosition = new Vector3(0, -m_WindowTransform.sizeDelta.y);

        //setup the scroller
        m_Scroller.OnBotChildExitView += Scroller_OnBotChildExitView;
        m_Scroller.OnTopChildExitView += Scroller_OnTopChildExitView;

        //spawn pools
        m_ChatMessagePool = new SimplePool<UIChatItem>(m_ChatMessagePrefab, 1);
        m_ChatLocationPool = new SimplePool<UIChatItem>(m_ChatLocationPrefab, 1);
        m_ChatHelpPlayerPool = new SimplePool<UIChatItem>(m_ChatHelpPlayerPrefab, 1);
        m_ChatHelpCrowPool = new SimplePool<UIChatItem>(m_ChatHelpCrowPrefab, 1);

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
    }

    private void Scroller_OnTopChildExitView(RectTransform obj)
    {
        if (obj == m_Items[0].rectTransform)
        {
            UIChatItem item = m_Items[0];
            int index = item.index;
            int nextIndex = m_Items[m_Items.Count - 1].index + 1;


            //check if theres a new item to be shown
            if (nextIndex < m_Messages.Count)
            {            
                //despawn it
                item.pool.Despawn(item);
                m_Items.RemoveAt(0);

                UIChatItem newItem = SpawnItem(m_CurrentCategory, m_Messages[nextIndex]);
                newItem.index = nextIndex;
                newItem.name = "item " + nextIndex;
            }

            m_Scroller.lockUp = nextIndex + 1 >= m_Messages.Count;
        }
    }

    private void Scroller_OnBotChildExitView(RectTransform obj)
    {
        if (obj == m_Items[m_Items.Count - 1].rectTransform)
        {
            UIChatItem item = m_Items[m_Items.Count - 1];
            int index = item.index;
            int nextIndex = m_Items[0].index - 1;

            //check if theres a new item to be shown
            if (nextIndex >= 0)
            {
                //despawn it
                item.pool.Despawn(item);
                m_Items.RemoveAt(m_Items.Count - 1);

                UIChatItem newItem = SpawnItem(m_CurrentCategory, m_Messages[nextIndex], false);
                newItem.index = nextIndex;
                newItem.name = "item " + nextIndex;
            }

            m_Scroller.lockDown = nextIndex - 1 < 0;
        }
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

    public void SetCategory(ChatCategory category)
    {
        if (m_CurrentCategory == category)
            return;

        //hide the container
        m_ContainerCanvasGroup.alpha = 0;

        //despawn previous items
        ClearItems();

        //setup the UI with the available messages
        m_CurrentCategory = category;

        m_Messages = ChatManager.GetMessages(category);
        int startIndex = Mathf.Max(m_Messages.Count - m_MaxItems, 0);

        for (int i = startIndex; i < m_Messages.Count; i++)
        {
            UIChatItem item = SpawnItem(category, m_Messages[i]);
            item.name = "chatitem " + i;
            item.index = i;
        }

        //show the container after spawning
        m_ContainerCanvasGroup.alpha = 1;
    }

    private void ClearItems()
    {
        foreach (var item in m_Items)
        {
            item.pool.Despawn(item);
        }
        m_Items.Clear();
        m_Scroller.OnChange();
    }

    private UIChatItem SpawnItem(ChatCategory category, ChatMessage message, bool spawnAtEnd = true)
    {
        SimplePool<UIChatItem> pool = null;

        //use special prefabs for support ui
        if (category == ChatCategory.SUPPORT)
        {
            if (message.player.name == ChatManager.Player.name)
                pool = m_ChatHelpPlayerPool;
            else
                pool = m_ChatHelpCrowPool;
        }
        else
        {
            if (message.type == MessageType.TEXT)
                pool = m_ChatMessagePool;
            else if (message.type == MessageType.LOCATION)
                pool = m_ChatLocationPool;
        }

        if (pool == null)
        {
            Debug.LogError("No prefab set for " + category + " : " + message.type);
            return null;
        }

        //setup the message and add it to the scrollview

        UIChatItem item = pool.Spawn();
        item.pool = pool;
        
        item.SetupMessage(message);
        item.transform.SetParent(m_Scroller.container);
        
        if (spawnAtEnd)
        {
            m_Items.Add(item);
            item.transform.SetAsLastSibling();
        }
        else
        {
            m_Items.Insert(0, item);
            item.transform.SetAsFirstSibling();
        }

        item.transform.localScale = Vector3.one;
        m_Scroller.OnChange();
        return item;
    }

    //MANAGER LISTENERS
    private void OnReceiveMessage(ChatCategory category, ChatMessage message)
    {
        if (IsOpen == false)
            return;

        if (m_CurrentCategory != category)
            return;

        SpawnItem(category, message);
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

        //build message data
        ChatMessage message = new ChatMessage();
        message.type = MessageType.TEXT;
        message.data.message = text;

        //send
        ChatManager.SendMessage(m_CurrentCategory, message);
    }

    private void _OnClickShareLocation()
    {
        //check cooldown
        //if (Time.time - m_LastLocationShareTime < m_ShareLocationCooldown)
        //{

        //}
        
        //build message
        ChatMessage message = new ChatMessage();
        message.type = MessageType.LOCATION;

        if (PlayerDataManager.playerData == null)
        {
            message.data.longitude = Random.Range(-180f, 0f);
            message.data.latitude = Random.Range(-85f, 0f);
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