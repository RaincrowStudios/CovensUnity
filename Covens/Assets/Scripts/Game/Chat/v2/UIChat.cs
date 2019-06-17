using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace Raincrow.Chat.UI
{
    public class UIChat : MonoBehaviour
    {
        public static bool IsOpen { get; private set; }
        private static UIChat _instance;

        [Header("UI")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _inputRaycaster;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private CanvasGroup _loading;        
        [SerializeField] private Button _closeButton;        

        [Header("Header UI")]
        [SerializeField] private Text _covenName;
        [SerializeField] private GameObject _sendScreenshotButton;

        [Header("Input UI")]
        [SerializeField] private EnableChatInputUI _enableInputUI;
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Button _shareLocationButton;

        [Header("Animation")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _containerCanvasGroup;
        [SerializeField] private RectTransform _windowTransform;

        [Header("Header")]
        [SerializeField] private Button _newsButton;
        [SerializeField] private Button _worldButton;
        [SerializeField] private Button _covenButton;
        [SerializeField] private Button _dominionButton;
        [SerializeField] private Button _helpButton;

        [Header("Prefabs")]
        [SerializeField] private UIChatItem _chatMessagePrefab;
        [SerializeField] private UIChatItem _chatLocationPrefab;
        [SerializeField] private UIChatItem _chatHelpPlayerPrefab;
        [SerializeField] private UIChatItem _chatHelpCrowPrefab;
        [SerializeField] private UIChatItem _chatImagePrefab;
        [SerializeField] private UIChatCoven _chatCovenPrefab;

        [Header("Unread Messages UI")]
        [SerializeField] private TMPro.TextMeshProUGUI _newsUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _worldUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _covenUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _dominionUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _supportUnreadText;

        [Header("Settings")]
        [SerializeField] private int _maxItems = 10;
        [SerializeField] private float _shareLocationCooldown = 10f;

        private SimplePool<UIChatItem> _chatMessagePool;
        private SimplePool<UIChatItem> _chatLocationPool;
        private SimplePool<UIChatItem> _chatHelpPlayerPool;
        private SimplePool<UIChatItem> _chatHelpCrowPool;
        private SimplePool<UIChatItem> _chatImagePool;
        private SimplePool<UIChatCoven> _chatCovenPool;

        private List<ChatMessage> _messages;
        private List<UIChatItem> _items = new List<UIChatItem>();
        private ChatCategory _currentCategory = ChatCategory.NONE;

        private int _loadingTweenId;
        private double _updateTimestampIntervalSeconds = 1.0;

        public static void Show(ChatCategory category = ChatCategory.NONE)
        {
            if (_instance == null)
            {
                Debug.LogError("Chat not initialized");
                return;
            }

            if (category == ChatCategory.NONE && _instance._currentCategory != ChatCategory.NONE)
            {
                category = _instance._currentCategory;
            }

            _instance.AnimateShow(null);
            _instance.SetCategory(category);

            _instance.UpdateCategoryUnreadMessages(ChatCategory.COVEN);
            _instance.UpdateCategoryUnreadMessages(ChatCategory.DOMINION);
            _instance.UpdateCategoryUnreadMessages(ChatCategory.WORLD);
            _instance.UpdateCategoryUnreadMessages(ChatCategory.SUPPORT);
            _instance.UpdateCategoryUnreadMessages(ChatCategory.NEWS);

            _instance.StartCoroutine(_instance.UpdateTimestamps());
        }

        private IEnumerator UpdateTimestamps()
        {
            while (enabled)
            {                
                for (int i = 0; i < _items.Count; i++)
                {
                    UIChatAvatarItem avatarItem = _items[i].GetComponent<UIChatAvatarItem>();
                    if (avatarItem != null)
                    {
                        avatarItem.RefreshTimeAgo();
                        yield return null;
                    }
                }

                // Wait until a full second has elapsed
                double totalSeconds = System.Math.Floor(System.DateTime.UtcNow.TimeOfDay.TotalSeconds);
                yield return new WaitUntil(() => System.DateTime.UtcNow.TimeOfDay.TotalSeconds >= totalSeconds + _updateTimestampIntervalSeconds);
            }
        }

        private static readonly string CovenUnreadMessagesCountKey = "CovenUnreadMessagesCount";
        private static readonly string DominionUnreadMessagesCountKey = "DominionUnreadMessagesCount";
        private static readonly string WorldUnreadMessagesCountKey = "WorldUnreadMessagesCount";
        private static readonly string SupportUnreadMessagesCountKey = "SupportUnreadMessagesCount";
        private static readonly string NewsUnreadMessagesCountKey = "NewsUnreadMessagesCount";

        private void AddCategoryUnreadMessages(ChatCategory chatCategory, int unreadMessagesToAdd)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string unreadTextKey = string.Empty;
            switch (chatCategory)
            {
                case ChatCategory.COVEN:
                    unreadText = _covenUnreadText;
                    unreadTextKey = CovenUnreadMessagesCountKey;
                    break;
                case ChatCategory.DOMINION:
                    unreadText = _dominionUnreadText;
                    unreadTextKey = DominionUnreadMessagesCountKey;
                    break;
                case ChatCategory.WORLD:
                    unreadText = _worldUnreadText;
                    unreadTextKey = WorldUnreadMessagesCountKey;
                    break;
                case ChatCategory.SUPPORT:
                    unreadText = _supportUnreadText;
                    unreadTextKey = SupportUnreadMessagesCountKey;
                    break;
                case ChatCategory.NEWS:
                    unreadText = _newsUnreadText;
                    unreadTextKey = NewsUnreadMessagesCountKey;
                    break;
            }

            if (unreadText != null && int.TryParse(unreadText.text, out int unreadMessagesCount))
            {
                unreadMessagesCount += unreadMessagesToAdd;                

                if (unreadMessagesCount > 0)
                {
                    unreadText.gameObject.SetActive(true);
                    unreadText.text = unreadMessagesCount.ToString();
                }
                else
                {
                    unreadText.gameObject.SetActive(false);
                }

                PlayerPrefs.SetInt(unreadTextKey, unreadMessagesCount);
            }
        }

        private void UpdateCategoryUnreadMessages(ChatCategory chatCategory)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string unreadTextKey = string.Empty;
            switch (chatCategory)
            {
                case ChatCategory.COVEN:
                    unreadText = _covenUnreadText;
                    unreadTextKey = CovenUnreadMessagesCountKey;
                    break;
                case ChatCategory.DOMINION:
                    unreadText = _dominionUnreadText;
                    unreadTextKey = DominionUnreadMessagesCountKey;
                    break;
                case ChatCategory.WORLD:
                    unreadText = _worldUnreadText;
                    unreadTextKey = WorldUnreadMessagesCountKey;
                    break;
                case ChatCategory.SUPPORT:
                    unreadText = _supportUnreadText;
                    unreadTextKey = SupportUnreadMessagesCountKey;
                    break;
                case ChatCategory.NEWS:
                    unreadText = _newsUnreadText;
                    unreadTextKey = NewsUnreadMessagesCountKey;
                    break;
            }

            if (unreadText != null)
            {
                int unreadMessagesCount = PlayerPrefs.GetInt(unreadTextKey, 0);
                if (unreadMessagesCount > 0)
                {
                    unreadText.gameObject.SetActive(true);
                    unreadText.text = unreadMessagesCount.ToString();
                }                
                else
                {
                    unreadText.gameObject.SetActive(false);
                }
                PlayerPrefs.SetInt(unreadTextKey, unreadMessagesCount);
            }
        }

        private void ClearCategoryUnreadMessages(ChatCategory chatCategory)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string unreadTextKey = string.Empty;
            switch (chatCategory)
            {
                case ChatCategory.COVEN:
                    unreadText = _covenUnreadText;
                    unreadTextKey = CovenUnreadMessagesCountKey;
                    break;
                case ChatCategory.DOMINION:
                    unreadText = _dominionUnreadText;
                    unreadTextKey = DominionUnreadMessagesCountKey;
                    break;
                case ChatCategory.WORLD:
                    unreadText = _worldUnreadText;
                    unreadTextKey = WorldUnreadMessagesCountKey;
                    break;
                case ChatCategory.SUPPORT:
                    unreadText = _supportUnreadText;
                    unreadTextKey = SupportUnreadMessagesCountKey;
                    break;
                case ChatCategory.NEWS:
                    unreadText = _newsUnreadText;
                    unreadTextKey = NewsUnreadMessagesCountKey;
                    break;
            }

            if (unreadText != null)
            {
                unreadText.gameObject.SetActive(false);
                unreadText.text = "0";                
                PlayerPrefs.SetInt(unreadTextKey, 0);
            }
        }

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            //setup UI to default disabled state
            _loading.gameObject.SetActive(false);
            _loading.alpha = 0;
            _canvas.enabled = false;
            _inputRaycaster.enabled = false;            
            //_inputField.enabled = false;
            _canvasGroup.alpha = 0;
            _containerCanvasGroup.alpha = 0;
            _windowTransform.anchoredPosition = new Vector3(0, -_windowTransform.sizeDelta.y);

            //spawn pools
            _chatMessagePool = new SimplePool<UIChatItem>(_chatMessagePrefab, 1);
            _chatLocationPool = new SimplePool<UIChatItem>(_chatLocationPrefab, 1);
            _chatHelpPlayerPool = new SimplePool<UIChatItem>(_chatHelpPlayerPrefab, 1);
            _chatHelpCrowPool = new SimplePool<UIChatItem>(_chatHelpCrowPrefab, 1);
            _chatImagePool = new SimplePool<UIChatItem>(_chatImagePrefab, 1);
            _chatCovenPool = new SimplePool<UIChatCoven>(_chatCovenPrefab, 1);

            //button listeners
            _newsButton.onClick.AddListener(_OnClickNews);
            _worldButton.onClick.AddListener(_OnClickWorld);
            _covenButton.onClick.AddListener(_OnClickCoven);
            _dominionButton.onClick.AddListener(_OnClickDominion);
            _helpButton.onClick.AddListener(_OnClickSupport);
            _closeButton.onClick.AddListener(_OnClickClose);
            _sendButton.onClick.AddListener(_OnClickSend);
            _shareLocationButton.onClick.AddListener(_OnClickShareLocation);

            //chat listeners
            ChatManager.OnReceiveMessage += OnReceiveMessage;
            ChatManager.OnConnected += OnConnected;
            ChatManager.OnLeaveChatRequested += OnLeaveChatRequested;
        }

        private void AnimateShow(System.Action onComplete)
        {
            //todo: properly animate
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1;
            _inputRaycaster.enabled = true;
            _canvas.enabled = true;
            _windowTransform.anchoredPosition = Vector2.zero;

            IsOpen = true;
            onComplete?.Invoke();
        }

        private void AnimateHide()
        {
            IsOpen = false;

            _inputRaycaster.enabled = false;
            _canvas.enabled = false;
            _instance._canvasGroup.alpha = 0;
            _instance.gameObject.SetActive(false);
        }

        public void SetCategory(ChatCategory category, bool force = false)
        {
            if (!force && _currentCategory == category)
            {
                return;
            }

            _enableInputUI.gameObject.SetActive(false);
            _covenName.gameObject.SetActive(false);
            _sendScreenshotButton.SetActive(false);
            ClearCategoryUnreadMessages(category);
            //_inputField.enabled = false;

            Debug.Log("[Chat] SetCategory: " + category);
            _currentCategory = category;

            if (category == ChatCategory.COVEN)
            {
                _covenName.gameObject.SetActive(true);

                if (!ChatManager.IsConnected(ChatCategory.COVEN))
                {
                    ShowAvailableCovens();

                    _covenName.text = LocalizeLookUp.GetText("card_witch_noCoven");
                }                
                else
                {
                    _covenName.text = PlayerDataManager.playerData.covenName;
                }                
            }
            else if (category == ChatCategory.SUPPORT)
            {
                // show screenshot button only on support
                _sendScreenshotButton.SetActive(true);
            }

            //hide the container
            _containerCanvasGroup.alpha = 0;

            //despawn previous items
            ClearItems();

            if (ChatManager.IsConnected(category))
            {
                //setup the UI with the available messages
                _messages = ChatManager.GetMessages(category);
                StartCoroutine("SpawnChatItems");

                LeanTween.alphaCanvas(_containerCanvasGroup, 1, 0.5f).setEaseOutCubic();

                //hide the loading overlay (in case it was visible)
                ShowLoading(false);

                _enableInputUI.gameObject.SetActive(true);
                //_inputField.enabled = true;
            }
            else
            {
                //show the loading screen
                ShowLoading(true);
            }
        }

        private void ShowAvailableCovens()
        {
            APIManager.Instance.GetData("coven/all", (string payload, int response) =>
            {
                if (response == 200)
                {
                    List<ChatCovenData> chatCovenDatas = JsonConvert.DeserializeObject<List<ChatCovenData>>(payload);
                    foreach (var chatCovenData in chatCovenDatas)
                    {
                        UIChatCoven uiChatCoven = _chatCovenPool.Spawn();
                        uiChatCoven.SetupCoven(chatCovenData, onRequestChatClose: _OnClickClose);
                        uiChatCoven.transform.SetParent(_itemContainer);
                        uiChatCoven.transform.localScale = Vector3.one;
                    }
                }

                ShowLoading(false);
            });
        }

        private void ClearItems()
        {
            StopCoroutine("SpawnChatItems");
            //StopAllCoroutines();
            _chatCovenPool.DespawnAll();
            _chatLocationPool.DespawnAll();
            _chatImagePool.DespawnAll();
            _chatMessagePool.DespawnAll();
            _chatHelpPlayerPool.DespawnAll();
            _chatHelpCrowPool.DespawnAll();
            //foreach (var item in m_Items)
            //{
            //    item.Despawn();
            //}
            _items.Clear();
            _messages = new List<ChatMessage>();
        }

        private IEnumerator SpawnChatItems()
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>(_messages);
            chatMessages.Reverse();
            foreach (var message in chatMessages)
            {
                SpawnItem(_currentCategory, message).transform.SetAsFirstSibling();
                yield return null;
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
                    pool = _chatHelpPlayerPool;
                }
                else
                {
                    pool = _chatHelpCrowPool;
                }
            }
            else
            {
                if (message.type == MessageType.TEXT)
                {
                    pool = _chatMessagePool;
                }
                else if (message.type == MessageType.LOCATION)
                {
                    pool = _chatLocationPool;
                }
                else if (message.type == MessageType.IMAGE)
                {
                    pool = _chatImagePool;
                }
            }

            if (pool == null)
            {
                Debug.LogError("No prefab set for " + category + " : " + message.type);
                return null;
            }

            //setup the message and add it to the scrollview
            UIChatItem item = pool.Spawn();
            item.SetupMessage(message, ShowLoading, _OnClickClose);
            item.transform.SetParent(_itemContainer);
            item.transform.localScale = Vector3.one;
            _items.Add(item);
            return item;
        }

        private void ShowLoading(bool show)
        {
            LeanTween.cancel(_loadingTweenId);
            _loading.gameObject.SetActive(true);
            _loadingTweenId = LeanTween.alphaCanvas(_loading, show ? 1 : 0, show ? 0.25f : 0.75f)
                .setEaseOutCubic()
                .setOnComplete(() => _loading.gameObject.SetActive(show))
                .uniqueId;
        }

        //EVENT LISTENERS
        private void OnReceiveMessage(ChatCategory category, ChatMessage message)
        {
            if (IsOpen == false)
            {
                return;
            }

            if (_currentCategory != category)
            {
                AddCategoryUnreadMessages(category, 1);
                return;
            }

            if (_items.Count >= 50)
            {
                _items[0].Despawn();
                _items.RemoveAt(0);
            }

            SpawnItem(category, message);
        }

        private void OnConnected(ChatCategory category)
        {
            if (category == _currentCategory)
                SetCategory(_currentCategory, true);
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
            string text = _inputField.text;

            if (string.IsNullOrEmpty(text))
                return;

            _inputField.text = "";

            //build message data
            ChatMessage message = new ChatMessage();
            message.type = MessageType.TEXT;
            message.data.message = text;

            //send
            ChatManager.SendMessage(_currentCategory, message);
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
            ChatManager.SendMessage(_currentCategory, message);
        }

        private void _OnClickClose()
        {
            AnimateHide();

            //PlayerManager.onQuickFlight -= m_Instance._OnClickClose;
        }

        private void OnLeaveChatRequested(ChatCategory category)
        {
            if (category == ChatCategory.COVEN)
            {
                ClearItems();
                // refresh
                _currentCategory = ChatCategory.NONE;
                _enableInputUI.gameObject.SetActive(false);
                //SetCategory(ChatCategory.COVEN);
            }
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
}