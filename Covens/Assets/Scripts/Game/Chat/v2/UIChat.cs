using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using TMPro;

namespace Raincrow.Chat.UI
{
    public class UIChat : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _inputRaycaster;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private CanvasGroup _loading;
        [SerializeField] private Button _closeButton;

        [Header("Disconnect Overlay UI")]
        [SerializeField] private GameObject _disconnectOverlay;
        [SerializeField] private Button _reconnectChatButton;

        [Header("Header UI")]
        [SerializeField] private TextMeshProUGUI _covenName;
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

        [Header("Unread Messages UI")]
        [SerializeField] private TMPro.TextMeshProUGUI _newsUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _worldUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _covenUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _dominionUnreadText;
        [SerializeField] private TMPro.TextMeshProUGUI _supportUnreadText;

        [Header("Settings")]
        [SerializeField] private float _sendMessageCooldown = 1f; // seconds
        [SerializeField] private int _maxCovensAvailable = 10;

        private SimplePool<UIChatItem> _chatMessagePool;
        private SimplePool<UIChatItem> _chatLocationPool;
        private SimplePool<UIChatItem> _chatHelpPlayerPool;
        private SimplePool<UIChatItem> _chatHelpCrowPool;
        private SimplePool<UIChatItem> _chatImagePool;

        private List<ChatMessage> _messages;
        private List<UIChatItem> _items = new List<UIChatItem>();
        private ChatCategory _currentCategory = ChatCategory.WORLD;

        private int _loadingTweenId;
        private double _updateTimestampIntervalSeconds = 1.0;
        private bool _isOpen;
        private float _lastMessageSentTime = 0f;

        private Coroutine m_SpawnCoroutine;
        private Coroutine m_UpdateTimestampCoroutine;
        private Coroutine m_WaitInputCooldownCoroutine;

        private static UIChat m_Instance;

        public static void Open(ChatCategory category = ChatCategory.WORLD)
        {
            if (m_Instance != null)
            {
                m_Instance.Show(category);
            }
            else
            {
                //load the coven scene
                LoadingOverlay.Show();
                SceneManager.LoadSceneAsync(
                    SceneManager.Scene.CHAT,
                    UnityEngine.SceneManagement.LoadSceneMode.Additive,
                    (progress) => { },
                    () =>
                    {
                        LoadingOverlay.Hide();
                        m_Instance.Show(category);
                    });
            }
        }

        public void Show(ChatCategory category = ChatCategory.WORLD)
        {
            SetCategory(category);
            AnimateShow(() => MapsAPI.Instance.HideMap(true));
            if (!ChatManager.Connected)
            {
                EnableReconnectOverlay(true);
            }
            else
            {
                RefreshView(true);
            }
        }

        private void RefreshView(bool repopulateChatItems = false)
        {
            isReconnecting = false;
            EnableReconnectOverlay(false);
            ShowLoading(false);

            if (!repopulateChatItems)
            {
                int unreadMessages = GetCategoryUnreadMessages(_currentCategory);
                repopulateChatItems = unreadMessages > 0;
            }

            SetCategory(_currentCategory, repopulateChatItems);


            UpdateCategoryUnreadMessages(ChatCategory.COVEN);
            UpdateCategoryUnreadMessages(ChatCategory.DOMINION);
            UpdateCategoryUnreadMessages(ChatCategory.WORLD);
            UpdateCategoryUnreadMessages(ChatCategory.SUPPORT);
            UpdateCategoryUnreadMessages(ChatCategory.NEWS);

            // Added all listeners to buttons
            _newsButton.onClick.RemoveListener(_OnClickNews);
            _newsButton.onClick.AddListener(_OnClickNews);

            _worldButton.onClick.RemoveListener(_OnClickWorld);
            _worldButton.onClick.AddListener(_OnClickWorld);

            _covenButton.onClick.RemoveListener(_OnClickCoven);
            _covenButton.onClick.AddListener(_OnClickCoven);

            _dominionButton.onClick.RemoveListener(_OnClickDominion);
            _dominionButton.onClick.AddListener(_OnClickDominion);

            _helpButton.onClick.RemoveListener(_OnClickSupport);
            _helpButton.onClick.AddListener(_OnClickSupport);

            if (m_UpdateTimestampCoroutine != null)
                StopCoroutine(m_UpdateTimestampCoroutine);
            m_UpdateTimestampCoroutine = StartCoroutine(UpdateTimestamps());

            if (m_WaitInputCooldownCoroutine != null)
                StopCoroutine(m_WaitInputCooldownCoroutine);
            m_WaitInputCooldownCoroutine = StartCoroutine(WaitCooldownInput());
        }

        private bool isReconnecting = false;

        private void EnableReconnectOverlay(bool enable)
        {
            _disconnectOverlay.gameObject.SetActive(enable);

            if (enable)
            {
                _enableInputUI.enabled = false;
                _covenName.gameObject.SetActive(false);
                _sendScreenshotButton.SetActive(false);

                _newsUnreadText.text = "0";
                _newsUnreadText.gameObject.SetActive(false);

                _covenUnreadText.text = "0";
                _covenUnreadText.gameObject.SetActive(false);

                _dominionUnreadText.text = "0";
                _dominionUnreadText.gameObject.SetActive(false);

                _worldUnreadText.text = "0";
                _worldUnreadText.gameObject.SetActive(false);

                _supportUnreadText.text = "0";
                _supportUnreadText.gameObject.SetActive(false);

                _reconnectChatButton.onClick.AddListener(ReconnectChat);
            }
            else
            {
                _reconnectChatButton.onClick.RemoveListener(ReconnectChat);
            }
        }

        private void ReconnectChat()
        {
            EnableReconnectOverlay(false);
            ShowLoading(true);

            isReconnecting = true;

            ChatManager.InitChat();
        }

        private IEnumerator WaitCooldownInput()
        {
            yield return new WaitUntil(() => Time.realtimeSinceStartup > _lastMessageSentTime + _sendMessageCooldown);
            _enableInputUI.enabled = true;
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

        #region Chat Category Texts

        private static readonly string CovenLastMessageReadIdKey = "CovenLastMessageReadId";
        private static readonly string DominionLastMessageReadIdKey = "DominionLastMessageReadId";
        private static readonly string WorldLastMessageReadIdKey = "WorldLastMessageReadId";
        private static readonly string SupportLastMessageReadIdKey = "SupportLastMessageReadId";
        private static readonly string NewsLastMessageReadIdKey = "NewsLastMessageReadId";

        private void GetCategoryTextAndLastMessageIdKey(ChatCategory chatCategory, ref TMPro.TextMeshProUGUI unreadText, ref string lastMessageIdKey)
        {
            switch (chatCategory)
            {
                case ChatCategory.COVEN:
                    unreadText = _covenUnreadText;
                    lastMessageIdKey = CovenLastMessageReadIdKey;
                    break;
                case ChatCategory.DOMINION:
                    unreadText = _dominionUnreadText;
                    lastMessageIdKey = DominionLastMessageReadIdKey;
                    break;
                case ChatCategory.WORLD:
                    unreadText = _worldUnreadText;
                    lastMessageIdKey = WorldLastMessageReadIdKey;
                    break;
                case ChatCategory.SUPPORT:
                    unreadText = _supportUnreadText;
                    lastMessageIdKey = SupportLastMessageReadIdKey;
                    break;
                default:
                    unreadText = _newsUnreadText;
                    lastMessageIdKey = NewsLastMessageReadIdKey;
                    break;
            }
        }

        private void AddCategoryUnreadMessages(ChatCategory chatCategory, int unreadMessagesToAdd)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string lastMessageIdKey = string.Empty;
            GetCategoryTextAndLastMessageIdKey(chatCategory, ref unreadText, ref lastMessageIdKey);

            if (int.TryParse(unreadText.text, out int unreadMessagesCount))
            {
                unreadMessagesCount += unreadMessagesToAdd;
                unreadMessagesCount = Mathf.Min(unreadMessagesCount, ChatManager.MaxMessages);

                if (unreadMessagesCount > 0)
                {
                    unreadText.gameObject.SetActive(true);
                    unreadText.text = unreadMessagesCount.ToString();
                }
                else
                {
                    unreadText.gameObject.SetActive(false);
                }
            }
        }

        private int GetCategoryUnreadMessages(ChatCategory chatCategory)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string lastMessageIdKey = string.Empty;
            GetCategoryTextAndLastMessageIdKey(chatCategory, ref unreadText, ref lastMessageIdKey);

            List<ChatMessage> reverseMessages = new List<ChatMessage>();
            reverseMessages.AddRange(ChatManager.GetMessages(chatCategory));
            reverseMessages.Reverse();

            int unreadMessagesCount = 0;
            string lastMessageId = PlayerPrefs.GetString(lastMessageIdKey, string.Empty);
            if (!string.IsNullOrEmpty(lastMessageId))
            {
                foreach (var message in reverseMessages)
                {
                    if (message._id.Equals(lastMessageId))
                    {
                        break;
                    }

                    unreadMessagesCount += 1;
                }
            }
            else
            {
                unreadMessagesCount = reverseMessages.Count;
            }

            return unreadMessagesCount;
        }

        private void UpdateCategoryUnreadMessages(ChatCategory chatCategory)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string lastMessageIdKey = string.Empty;
            GetCategoryTextAndLastMessageIdKey(chatCategory, ref unreadText, ref lastMessageIdKey);

            List<ChatMessage> reverseMessages = new List<ChatMessage>();
            reverseMessages.AddRange(ChatManager.GetMessages(chatCategory));
            reverseMessages.Reverse();

            int unreadMessagesCount = 0;
            string lastMessageId = PlayerPrefs.GetString(lastMessageIdKey, string.Empty);
            if (!string.IsNullOrEmpty(lastMessageId))
            {
                foreach (var message in reverseMessages)
                {
                    if (message._id.Equals(lastMessageId))
                    {
                        break;
                    }

                    unreadMessagesCount += 1;
                }
            }
            else
            {
                unreadMessagesCount = reverseMessages.Count;
            }

            unreadText.gameObject.SetActive(unreadMessagesCount > 0);
            unreadText.text = unreadMessagesCount.ToString();

            //if (reverseMessages.Count > 0)
            //{
            //    lastMessageId = reverseMessages[0]._id; // since we reversed this array, we are getting the last message
            //    PlayerPrefs.SetString(lastMessageIdKey, lastMessageId);
            //}
        }

        private void ClearCategoryUnreadMessages(ChatCategory chatCategory)
        {
            TMPro.TextMeshProUGUI unreadText = null;
            string lastMessageIdKey = string.Empty;
            GetCategoryTextAndLastMessageIdKey(chatCategory, ref unreadText, ref lastMessageIdKey);

            unreadText.gameObject.SetActive(false);
            unreadText.text = "0";

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.AddRange(ChatManager.GetMessages(chatCategory));
            if (messages.Count > 0)
            {
                string lastMessageId = messages[messages.Count - 1]._id; // last message
                PlayerPrefs.SetString(lastMessageIdKey, lastMessageId);
            }
        }

        #endregion

        private void Awake()
        {
            m_Instance = this;
            _SelectedGlow.gameObject.SetActive(false);

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

            //button listeners            
            _closeButton.onClick.AddListener(_OnClickClose);
            _sendButton.onClick.AddListener(_OnClickSend);
            _shareLocationButton.onClick.AddListener(_OnClickShareLocation);
            _sendScreenshotButton.GetComponent<Button>().onClick.AddListener(() => { SendEmail(); });

            m_HeaderButtons = new Dictionary<ChatCategory, TMPro.TextMeshProUGUI>
            {
                { ChatCategory.NEWS,        _newsButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() },
                { ChatCategory.WORLD,       _worldButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() },
                { ChatCategory.COVEN,       _covenButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() },
                { ChatCategory.DOMINION,    _dominionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() },
                { ChatCategory.SUPPORT,     _helpButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() }
            };

            //chat listeners
            ChatManager.OnReceiveMessage += OnReceiveMessage;
            ChatManager.OnConnected += OnConnected;
            ChatManager.OnSocketError += OnSocketError;
            ChatManager.OnDisconnected += ShowReconnectOverlay;
            ChatManager.OnLeaveChatRequested += OnLeaveChatRequested;
            ChatManager.OnEnterCovenChat += OnEnterCovenChat;

            DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
        }

        private void DownloadedAssets_OnWillUnloadAssets()
        {
            if (_isOpen)
                return;

            DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;

            _chatLocationPool.DestroyAll();
            _chatImagePool.DestroyAll();
            _chatMessagePool.DestroyAll();
            _chatHelpPlayerPool.DestroyAll();
            _chatHelpCrowPool.DestroyAll();

            SceneManager.UnloadScene(SceneManager.Scene.CHAT, null, null);
        }

        private void AnimateShow(System.Action onComplete)
        {
            //todo: properly animate
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1;
            _inputRaycaster.enabled = true;
            _canvas.enabled = true;
            _windowTransform.anchoredPosition = Vector2.zero;

            _isOpen = true;
            onComplete?.Invoke();
        }

        private void AnimateHide()
        {
            MapsAPI.Instance.HideMap(false);

            _isOpen = false;

            _inputRaycaster.enabled = false;
            _canvas.enabled = false;
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);

            // Remove All Button Listeners
            _newsButton.onClick.RemoveListener(_OnClickNews);
            _worldButton.onClick.RemoveListener(_OnClickWorld);
            _covenButton.onClick.RemoveListener(_OnClickCoven);
            _dominionButton.onClick.RemoveListener(_OnClickDominion);
            _helpButton.onClick.RemoveListener(_OnClickSupport);

            // Hide Disconnect Overlay
            EnableReconnectOverlay(false);
        }

        public void SetCategory(ChatCategory category, bool repopulateChatItems = false)
        {
            if (!repopulateChatItems && _currentCategory == category)
            {
                return;
            }

            HighlightHeader(category);
            
            _enableInputUI.gameObject.SetActive(false);
            _covenName.gameObject.SetActive(false);
            _sendScreenshotButton.SetActive(false);
            ClearCategoryUnreadMessages(category);
            //_inputField.enabled = false;

            Debug.Log("[Chat] SetCategory: " + category);
            ChatCategory previousCategory = _currentCategory;
            _currentCategory = category;

            //hide the container
            _containerCanvasGroup.alpha = 0;

            //despawn previous items
            ClearItems();

            if (ChatManager.IsConnected(category) && ChatManager.HasJoinedChat(category))
            {
                //setup the UI with the available messages
                _messages = new List<ChatMessage>();
                _messages.AddRange(ChatManager.GetMessages(category));
                m_SpawnCoroutine = StartCoroutine(SpawnChatItems());

                LeanTween.alphaCanvas(_containerCanvasGroup, 1, 0.5f).setEaseOutCubic();

                //hide the loading overlay (in case it was visible)
                ShowLoading(false);

                _enableInputUI.gameObject.SetActive(true);

                if (category == ChatCategory.COVEN)
                {
                    _covenName.text = PlayerDataManager.playerData.covenInfo.name;
                    _covenName.gameObject.SetActive(true);
                }
                else if (category == ChatCategory.DOMINION)
                {
                    _covenName.text = LocalizeLookUp.GetText("show_dominion").Replace("{{Dominion Name}}", PlayerDataManager.currentDominion);
                    _covenName.gameObject.SetActive(true);
                }
                else if (category == ChatCategory.SUPPORT)
                {
                    // show screenshot button only on support
                    _sendScreenshotButton.SetActive(true);
                }
            }
            else
            {
                if (category == ChatCategory.COVEN && string.IsNullOrEmpty(PlayerDataManager.playerData.covenInfo.coven))
                {
                    UICovenSearcher.Instance.Show(() => SetCategory(previousCategory));
                }
                else
                {
                    //show the loading screen
                    ShowLoading(true);
                }
            }
        }

        private void ClearItems()
        {
            if (m_SpawnCoroutine != null)
            {
                StopCoroutine(m_SpawnCoroutine);
                m_SpawnCoroutine = null;
            }
            _chatLocationPool.DespawnAll();
            _chatImagePool.DespawnAll();
            _chatMessagePool.DespawnAll();
            _chatHelpPlayerPool.DespawnAll();
            _chatHelpCrowPool.DespawnAll();
            _items.Clear();
            _messages = new List<ChatMessage>();
        }

        //private void SpawnChatItems()
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
            if (_currentCategory != category)
            {
                AddCategoryUnreadMessages(category, 1);
                return;
            }

            if (_isOpen)
            {
                if (_items.Count >= ChatManager.MaxMessages)
                {
                    _items[0].Despawn();
                    _items.RemoveAt(0);
                }

                SpawnItem(category, message);

                TMPro.TextMeshProUGUI unreadText = null;
                string lastMessageIdKey = string.Empty;
                GetCategoryTextAndLastMessageIdKey(category, ref unreadText, ref lastMessageIdKey);

                PlayerPrefs.SetString(lastMessageIdKey, message._id);
            }
        }

        private void OnConnected(ChatCategory category)
        {
            //bool repopulateChatItems = category == _currentCategory;
            if (_isOpen)
            {
                if (category == _currentCategory)
                {
                    RefreshView(true);
                }
            }
        }

        private void OnSocketError(string errorMessage)
        {
            if (isReconnecting)
            {
                ShowReconnectOverlay();
            }
        }

        private void ShowReconnectOverlay()
        {
            isReconnecting = false;
            ShowLoading(false);
            ClearItems();

            EnableReconnectOverlay(true);
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

        public void _OnClickSupport()
        {
            SetCategory(ChatCategory.SUPPORT);
        }

        private void _OnClickSend()
        {
            if (_currentCategory != ChatCategory.COVEN || ChatManager.IsConnected(ChatCategory.COVEN))
            {
                string text = _inputField.text;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    _inputField.text = "";

                    //build message data
                    ChatMessage message = new ChatMessage
                    {
                        type = MessageType.TEXT
                    };
                    message.data.message = text;

                    //send
                    ChatManager.SendMessage(_currentCategory, message);
                    _lastMessageSentTime = Time.realtimeSinceStartup;
                    _enableInputUI.enabled = false;

                    m_WaitInputCooldownCoroutine = StartCoroutine(WaitCooldownInput());
                }
            }
        }

        private void _OnClickShareLocation()
        {
            if (_currentCategory != ChatCategory.COVEN || ChatManager.IsConnected(ChatCategory.COVEN))
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
                _lastMessageSentTime = Time.realtimeSinceStartup;
                _enableInputUI.enabled = false;

                m_WaitInputCooldownCoroutine = StartCoroutine(WaitCooldownInput());
            }
        }

        private void _OnClickClose()
        {
            AnimateHide();

            //PlayerManager.onQuickFlight -= m_Instance._OnClickClose;
        }

        private void OnEnterCovenChat(string covenId, string covenName)
        {
            if (_currentCategory == ChatCategory.COVEN)
            {
                SetCategory(_currentCategory, _isOpen);
            }
        }

        private void OnLeaveChatRequested(ChatCategory category)
        {
            if (category == ChatCategory.COVEN)
            {
                SetCategory(_currentCategory, _isOpen);
            }
        }

        private void SendEmail()
        {
            string email = "help@raincrowgames.com";
            string subject = MyEscapeURL("Covens Bug #" + PlayerDataManager.playerData.name);
            string body = MyEscapeURL($"Version: {Application.version} \n Platform: {Application.platform} \n  _id: {PlayerDataManager.playerData.instance} \n  displayName: {PlayerDataManager.playerData.name}  \n  GameToken:{LoginAPIManager.loginToken}\n\n\n ***Your Message*** +\n\n\n ***Screenshot***\n\n\n");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

        }
        private string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }


        [SerializeField] private Image _SelectedGlow;
        private Dictionary<ChatCategory, TMPro.TextMeshProUGUI> m_HeaderButtons;

        /// <summary>
        /// If chat category is null, hide the glow
        /// </summary>
        /// <param name="category"></param>
        private void HighlightHeader(ChatCategory category)
        {
            //if (!category.HasValue)
            //{
            //    _SelectedGlow.gameObject.SetActive(false);
            //    return;
            //}            

            foreach (KeyValuePair<ChatCategory, TMPro.TextMeshProUGUI> entry in m_HeaderButtons)
            {
                if (entry.Key == category)
                {
                    _SelectedGlow.transform.SetParent(entry.Value.transform);
                    _SelectedGlow.transform.SetAsFirstSibling();
                    _SelectedGlow.transform.localPosition = Vector3.zero;
                    entry.Value.fontStyle = TMPro.FontStyles.Bold;
                }
                else
                {
                    entry.Value.fontStyle = TMPro.FontStyles.Normal;
                }
            }
            _SelectedGlow.gameObject.SetActive(true);
        }
    }
}