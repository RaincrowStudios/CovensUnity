using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using TMPro;
public class ChatUI : UIAnimationManager
{
    public static ChatUI Instance { get; set; }

    public TextMeshProUGUI worldButton;
    public TextMeshProUGUI newsButton;
    public TextMeshProUGUI dominionButton;
    public TextMeshProUGUI covenButton;
    public Button helpButton;
    public GameObject helpcrowNotification;
    public TextMeshProUGUI worldButtonNotification;
    public TextMeshProUGUI newsButtonNotification;
    public TextMeshProUGUI dominionButtonNotification;
    public TextMeshProUGUI covenButtonNotification;
    public TextMeshProUGUI helpNotification;

    private List<GameObject> chatItems = new List<GameObject>();

    //	public Sprite[] profilePics;
    public GameObject locationPrefab;
    public GameObject helpMessageCrow;
    public GameObject helpMessageYou;
    public GameObject chatPrefab;
    public GameObject covenChatPrefab;
    public Transform container;
    public GameObject ChatParentObject;

    public GameObject HeaderTitle;
    public Button SendScreenShotButton;
    public TextMeshProUGUI HeaderTitleText;

    // public InputField inputMessage;
    public TMP_InputField inputMessage;
    public Button shareLocation;
    public Button sendButton;
    public Action<string> ReceiveTranslation;

    int newsNoti, worldNoti, covenNoti, dominionNoti, helpNoti = 0;

    public static int currentCount = 0;
    private int playerAvatar;

    public Button chatButton;
    public Button closeButton;
    public Button shoutButton;
    public float speed = 1;
    public LeanTweenType easeType = LeanTweenType.easeInOutSine;
    private Dictionary<string, ChatCovenData> covensDict = new Dictionary<string, ChatCovenData>();
    private string currentWindow = "world";

    string lastHelpcrowMessage
    {
        get
        {
            return PlayerPrefs.GetString("helpcrow", "");
        }
        set { PlayerPrefs.SetString("helpcrow", value); }
    }

    public enum ChatWindows
    {
        News,
        World,
        Covens,
        Dominion,
        Help,
    };

    public ChatWindows ActiveWindow = ChatWindows.World;
    public GameObject loadingFullscreen;

    public void SetChatInteraction(bool canInteract)
    {
        chatButton.interactable = canInteract;
        shoutButton.interactable = canInteract;
    }

    void Awake()
    {
        Instance = this;
        closeButton.onClick.AddListener(HideChat);
        SendScreenShotButton.onClick.AddListener(SendEmail);
    }

    public void Init()
    {
        // SwitchWindow("world");
        SetAvatar();
        GetAllCovens();
        var helpchat = ChatConnectionManager.AllChat.HelpChat;
        Debug.Log(lastHelpcrowMessage + "|||||||");
        if (helpchat != null && helpchat.Count > 0 && lastHelpcrowMessage != "" && helpchat[helpchat.Count - 1].Content != lastHelpcrowMessage)
        {
            helpcrowNotification.SetActive(true);
            helpcrowNotification.transform.localScale = Vector3.zero;
            LeanTween.scale(helpcrowNotification, Vector3.one, .5f).setEase(LeanTweenType.easeInElastic);
        }
        else
        {
            helpcrowNotification.SetActive(false);

        }
    }

    void GetAllCovens()
    {
        APIManager.Instance.Get("coven/all", (string s, int r) =>
        {
            covensDict.Clear();
            Debug.Log(s);
            var k = JsonConvert.DeserializeObject<List<ChatCovenData>>(s);
            foreach (var item in k)
            {
                covensDict[item.name] = item;
            }
        });
    }

    public void initNotifications()
    {
        worldButtonNotification.text = covenButtonNotification.text = newsButtonNotification.text = dominionButtonNotification.text = helpNotification.text = "";
        newsNoti = worldNoti = covenNoti = dominionNoti = 0;
    }

    public void addNotification(ChatData data)
    {
        var c = (Commands)Enum.Parse(typeof(Commands), data.CommandRaw);
        data.Command = c;
        //		Debug.Log ("Adding stuff");
        if (c == Commands.CovenLocation || c == Commands.CovenMessage)
        {
            if (ActiveWindow != ChatWindows.Covens)
            {
                covenNoti++;
                covenButtonNotification.text = covenNoti.ToString();
            }
            ChatConnectionManager.AllChat.CovenChat.Add(data);
        }
        else if (c == Commands.WorldMessage || c == Commands.WorldLocation)
        {
            if (ActiveWindow != ChatWindows.World)
            {
                worldNoti++;
                worldButtonNotification.text = worldNoti.ToString();
            }
            ChatConnectionManager.AllChat.WorldChat.Add(data);
        }
        else if (c == Commands.DominionMessage || c == Commands.DominionLocation)
        {
            if (ActiveWindow != ChatWindows.Dominion)
            {
                dominionNoti++;
                dominionButtonNotification.text = dominionNoti.ToString();
            }
            ChatConnectionManager.AllChat.DominionChat.Add(data);
        }
        else if (c == Commands.NewsMessage || c == Commands.NewsLocation)
        {
            if (ActiveWindow != ChatWindows.News)
            {
                newsNoti++;
                newsButtonNotification.text = newsNoti.ToString();
            }
            ChatConnectionManager.AllChat.News.Add(data);
        }
        else if (c == Commands.HelpCrowMessage)
        {
            if (ActiveWindow != ChatWindows.Help)
            {
                helpNoti++;
                helpNotification.text = helpNoti.ToString();
            }
            ChatConnectionManager.AllChat.HelpChat.Add(data);
        }

    }

    public void SwitchWindow(string type)
    {

        currentCount = 0;
        worldButton.transform.localScale = newsButton.transform.localScale = dominionButton.transform.localScale = covenButton.transform.localScale = helpButton.transform.localScale = Vector3.one;
        worldButton.color = newsButton.color = dominionButton.color = covenButton.color = Utilities.Grey;
        // playerLoading.SetActive(false);
        // playerInfo.SetActive(false);
        //    CovenUIText.gameObject.SetActive(false);
        HeaderTitle.SetActive(false);
        SendScreenShotButton.gameObject.SetActive(false);
        inputMessage.onValueChanged.RemoveAllListeners();
        inputMessage.interactable = true;
        sendButton.interactable = true;
        inputMessage.placeholder.GetComponent<TextMeshProUGUI>().text = "";
        shareLocation.interactable = true;
        if (type == "news")
        {
            ActiveWindow = ChatWindows.News;
            populateChat(ChatConnectionManager.AllChat.News);
            newsButton.transform.localScale = Vector3.one * 1.2f;
            newsButton.color = Color.white;
            inputMessage.interactable = false;
            sendButton.interactable = false;
            shareLocation.interactable = false;
            newsNoti = 0;
            newsButtonNotification.text = "";
        }
        else if (type == "world")
        {
            ActiveWindow = ChatWindows.World;
            populateChat(ChatConnectionManager.AllChat.WorldChat);
            worldButton.transform.localScale = Vector3.one * 1.2f;
            worldButton.color = Color.white;
            worldNoti = 0;
            worldButtonNotification.text = "";

        }
        else if (type == "coven")
        {
            HeaderTitle.SetActive(true);
            ActiveWindow = ChatWindows.Covens;
            // CovenUIText.gameObject.SetActive(true);
            if (PlayerDataManager.playerData.covenId != "")
            {
                HeaderTitleText.text = PlayerDataManager.playerData.covenId;

                // CovenUIText.text = PlayerDataManager.playerData.covenName;
                populateChat(ChatConnectionManager.AllChat.CovenChat);

            }
            else
            {
                inputMessage.onValueChanged.AddListener(onCovenSearch);
                inputMessage.interactable = true;
				inputMessage.placeholder.GetComponent<TextMeshProUGUI> ().text = LocalizeLookUp.GetText ("coven_search");// "Search for coven name here";
				HeaderTitleText.text = LocalizeLookUp.GetText("invite_request_basic");// "Send a request to join a coven";
                clearChat();
                sendButton.interactable = false;
                shareLocation.interactable = false;

                foreach (var item in covensDict)
                {
                    if (chatItems.Count < 50)
                    {
                        var chatObject = Utilities.InstantiateObject(covenChatPrefab, container);
                        chatObject.GetComponent<ChatCovenItem>().Setup(item.Value);
                        chatItems.Add(chatObject);
                    }
                    else
                    {
                        break;
                    }
                }

            }
            covenButton.transform.localScale = Vector3.one * 1.2f;
            covenButton.color = Color.white;
            covenNoti = 0;
            covenButtonNotification.text = "";

        }
        else if (type == "dominion")
        {
            HeaderTitle.SetActive(true);
            HeaderTitleText.text = PlayerDataManager.currentDominion;

            ActiveWindow = ChatWindows.Dominion;
            populateChat(ChatConnectionManager.AllChat.DominionChat);
            dominionButton.transform.localScale = Vector3.one * 1.2f;
            dominionButton.color = Color.white;
            dominionNoti = 0;
            dominionButtonNotification.text = "";
        }
        else if (type == "help")
        {
            ChatConnectionManager.Instance.ConnectHelpCrow();
            HeaderTitle.SetActive(true);
			HeaderTitleText.text = LocalizeLookUp.GetText ("chat_state_trouble");// "State your trouble, Witch.";
            SendScreenShotButton.gameObject.SetActive(true);
            ActiveWindow = ChatWindows.Help;
            populateChat(ChatConnectionManager.AllChat.HelpChat);
            helpButton.transform.localScale = Vector3.one * 1.2f;
            //  helpButton.color = Color.white;
            helpNoti = 0;
            helpNotification.text = "";
            if (ChatConnectionManager.AllChat.HelpChat.Count > 0)
            {

                lastHelpcrowMessage = ChatConnectionManager.AllChat.HelpChat[ChatConnectionManager.AllChat.HelpChat.Count - 1].Content;
                Debug.Log(lastHelpcrowMessage + "|||||||");


            }
            helpcrowNotification.SetActive(false);
        }
        currentWindow = type;
    }

    void onCovenSearch(string s)
    {
        clearChat();
        foreach (var item in covensDict)
        {
            if (item.Key.Contains(s))
            {
                var chatObject = Utilities.InstantiateObject(covenChatPrefab, container);
                chatObject.GetComponent<ChatCovenItem>().Setup(item.Value);
                chatItems.Add(chatObject);
            }
        }
    }

    void populateChat(List<ChatData> CD)
    {
        clearChat();
        if (CD == null)
            return;
        foreach (var item in CD)
        {
            //  Debug.Log(item.CommandRaw);
            AddItemHelper(item);
        }
    }

    public void AddItemHelper(ChatData CD)
    {
        #region newsScroll
        if (CD.Command == Commands.CovenMessage)
        {
			NewsScroll.Instance.ShowText("(" + LocalizeLookUp.GetText("chat_coven") + ") " + /*"(Coven)*/ " " + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.CovenLocation)
        {
			NewsScroll.Instance.ShowText("(" + LocalizeLookUp.GetText("chat_coven") + ") " /*"(Coven)*/ + " " + CD.Name + LocalizeLookUp.GetText("chat_share_location")/*" shared location."*/, true);
        }
        else if (CD.Command == Commands.WorldMessage)
        {
			NewsScroll.Instance.ShowText("(" + LocalizeLookUp.GetText("chat_world") + ") " /*(World) "*/ + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.WorldLocation)
        {
			NewsScroll.Instance.ShowText("(" + LocalizeLookUp.GetText("chat_world") + ") " /*"(World) "*/ + " " + CD.Name + LocalizeLookUp.GetText("chat_share_location")/*" shared location."*/, true);
        }
        else if (CD.Command == Commands.NewsMessage)
        {
			NewsScroll.Instance.ShowText("(" + LocalizeLookUp.GetText("chat_news") + /*"(News)*/") " + CD.Content, true);
        }
        else if (CD.Command == Commands.DominionMessage)
        {
            NewsScroll.Instance.ShowText("(" + CD.Dominion + ") " + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.DominionLocation)
        {
			NewsScroll.Instance.ShowText("(" + CD.Dominion + ") " + CD.Name + " " + LocalizeLookUp.GetText("chat_share_location")/*shared location."*/, true);
        }
        #endregion

        CD.Command = (Commands)Enum.Parse(typeof(Commands), CD.CommandRaw);
        if (ActiveWindow == ChatWindows.Covens)
        {
            if (CD.Command == Commands.CovenMessage || CD.Command == Commands.CovenLocation)
            {
                AddItem(CD);
            }
        }
        else if (ActiveWindow == ChatWindows.World)
        {
            if (CD.Command == Commands.WorldMessage || CD.Command == Commands.WorldLocation)
            {
                AddItem(CD);
            }
        }
        else if (ActiveWindow == ChatWindows.News)
        {
            if (CD.Command == Commands.NewsMessage || CD.Command == Commands.NewsLocation)
            {
                AddItem(CD);
            }
        }
        else if (ActiveWindow == ChatWindows.Dominion)
        {
            if (CD.Command == Commands.DominionMessage || CD.Command == Commands.DominionLocation)
            {
                AddItem(CD);
            }
        }
        else if (ActiveWindow == ChatWindows.Help)
        {
            if (CD.Command == Commands.HelpCrowMessage)
            {
                AddItem(CD);
            }
        }

    }

    void AddItem(ChatData CD, bool isPVP = false)
    {
        GameObject chatObject = null;
        if (CD.Command == Commands.CovenMessage || CD.Command == Commands.NewsMessage || CD.Command == Commands.WorldMessage || CD.Command == Commands.DominionMessage)
        {
            chatObject = Utilities.InstantiateObject(chatPrefab, container);
            chatObject.GetComponent<ChatItemData>().Setup(CD, false);
            currentCount++;
        }
        else if (CD.Command == Commands.HelpCrowMessage)
        {
            if (CD.Name.Contains("$"))
            {
                chatObject = Utilities.InstantiateObject(helpMessageCrow, container);
                chatObject.GetComponent<ChatItemData>().Setup(CD, false);
                currentCount++;
                if (ActiveWindow != ChatWindows.Help || !ChatParentObject.activeInHierarchy)
                {
                    helpcrowNotification.SetActive(true);
                    lastHelpcrowMessage = CD.Content;
                    Debug.Log(lastHelpcrowMessage + "|||||||");
                }
            }
            else
            {
                chatObject = Utilities.InstantiateObject(helpMessageYou, container);
                chatObject.GetComponent<ChatItemData>().Setup(CD, false);
                currentCount++;
            }
        }
        else
        {
            chatObject = Utilities.InstantiateObject(locationPrefab, container);
            chatObject.GetComponent<ChatItemData>().Setup(CD, true);
        }
        chatItems.Add(chatObject);
    }

    void clearChat()
    {
        foreach (var item in chatItems)
        {
            Destroy(item);
        }
        chatItems.Clear();
    }

    public void SendMessage()
    {
        if (!inputMessage.text.IsNullOrWhiteSpace())
        {
            sendButton.interactable = false;
            shareLocation.interactable = false;

            ChatData CD = new ChatData();
            CD.Avatar = playerAvatar;
            CD.Name = PlayerDataManager.playerData.name;
            CD.Content = inputMessage.text;
            CD.Degree = PlayerDataManager.playerData.degree;
            CD.Level = PlayerDataManager.playerData.level;
            CD.Language = PlayerManager.SystemLanguage;
            CD.TimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (ActiveWindow == ChatWindows.World)
            {
                CD.CommandRaw = Commands.WorldMessage.ToString();
                ChatConnectionManager.Instance.SendWorld(CD);
            }
            else if (ActiveWindow == ChatWindows.Covens)
            {
                CD.CommandRaw = Commands.CovenMessage.ToString();
                CD.Coven = PlayerDataManager.playerData.covenId.Replace(" ", "-");
                ChatConnectionManager.Instance.SendCoven(CD);
            }
            else if (ActiveWindow == ChatWindows.Dominion)
            {
                CD.CommandRaw = Commands.DominionMessage.ToString();
                CD.Dominion = PlayerDataManager.currentDominion.Replace(" ", "-"); ;
                ChatConnectionManager.Instance.SendDominion(CD);
            }
            else if (ActiveWindow == ChatWindows.Help)
            {
                CD.CommandRaw = Commands.HelpCrowMessage.ToString();
                CD.Channel = "helpcrow" + PlayerDataManager.playerData.name.Replace(" ", "-");
                ChatConnectionManager.Instance.SendHelpcrow(CD);
            }
            //			inputMessage.Select ();
            inputMessage.text = "";
            StartCoroutine(ReEnableSendButton());
        }
    }

    public void SendTranslate(string text, Action<string> OnReceiveTranslation)
    {
        ChatData CD = new ChatData();
        CD.Content = text;
        CD.CommandRaw = Commands.TranslateMessage.ToString();
        CD.Language = PlayerManager.SystemLanguage;
        ReceiveTranslation = OnReceiveTranslation;
        // ChatConnectionManager.Instance.send(CD);
    }

    public void SendLocation()
    {
        sendButton.interactable = false;
        shareLocation.interactable = false;
        ChatData CD = new ChatData();
        CD.Name = PlayerDataManager.playerData.name;
        CD.Degree = PlayerDataManager.playerData.degree;
        CD.Level = PlayerDataManager.playerData.level;
        CD.Latitude = MapsAPI.Instance.position.y;
        CD.Longitude = MapsAPI.Instance.position.x;
        CD.Location = true;
        CD.Avatar = playerAvatar;
        CD.TimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        if (ActiveWindow == ChatWindows.World)
        {
            CD.CommandRaw = Commands.WorldLocation.ToString();
            ChatConnectionManager.Instance.SendWorld(CD);
        }
        else if (ActiveWindow == ChatWindows.Covens)
        {
            CD.CommandRaw = Commands.CovenLocation.ToString();
            CD.Coven = PlayerDataManager.playerData.covenId;
            ChatConnectionManager.Instance.SendCoven(CD);
        }
        else if (ActiveWindow == ChatWindows.Dominion)
        {
            CD.CommandRaw = Commands.DominionLocation.ToString();
            CD.Dominion = PlayerDataManager.currentDominion;
            ChatConnectionManager.Instance.SendDominion(CD);
        }
        //			inputMessage.Select ();
        //			inputMessage.text = "";
        //			Debug.Log (JsonConvert.SerializeObject (CD));
        //   ChatConnectionManager.Instance.send(CD);
        StartCoroutine(ReEnableSendButton());
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(1.5f);
        sendButton.interactable = true;
        shareLocation.interactable = true;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && inputMessage.IsInteractable())
        {
            SendMessage();
        }
    }

    public void ShowChat()
    {

        SwitchWindow(currentWindow);
        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();

        ChatParentObject.SetActive(true);
        var rt = ChatParentObject.GetComponent<RectTransform>();
        LeanTween.value(1448, 0, speed).setEase(easeType).setOnUpdate((float v) =>
        {
            rt.offsetMin = new Vector2(0, -v);
            rt.offsetMax = new Vector2(0, -v);
        }).setOnComplete(() => MapsAPI.Instance.HideMap(true));
        // anim.SetBool("animate", true);
    }

    public void HideChat()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        SoundManagerOneShot.Instance.MenuSound();
        var rt = ChatParentObject.GetComponent<RectTransform>();
        MapsAPI.Instance.HideMap(false);
        LeanTween.value(0, 1448, speed).setEase(easeType).setOnUpdate((float v) =>
            {
                rt.offsetMin = new Vector2(0, -v);
                rt.offsetMax = new Vector2(0, -v);
            }).setOnComplete(() =>
            {
                ChatParentObject.SetActive(false);
                clearChat();
            });
        //   anim.SetBool("animate", false);
    }

    void SetAvatar()
    {
        playerAvatar = PlayerDataManager.playerData.bodyType;
    }

    void SendEmail()
    {
        string email = "help@raincrowgames.com";
        string subject = MyEscapeURL("Covens Bug #" + PlayerDataManager.playerData.name);
        string body = MyEscapeURL($"Version: {Application.version} \n Platform: {Application.platform} \n  _id: {PlayerDataManager.playerData.instance} \n  displayName: {PlayerDataManager.playerData.name}  \n  GameToken:{LoginAPIManager.loginToken}\n\n\n ***Your Message*** +\n\n\n ***Screenshot***\n\n\n");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void GetPlayerDetails(String playerID)
    {

        TeamPlayerView.ViewCharacter(playerID,
                 (character, error) =>
                 {
                     loadingFullscreen.SetActive(false);
                 });

    }



}

public class ChatCovenData
{
    public string name { get; set; }
    public string dominion { get; set; }
    public int members { get; set; }
    public int worldRank { get; set; }
    public int dominionRank { get; set; }
    public int xp { get; set; }
    public int alignment { get; set; }
    public int level { get; set; }
    public string founder { get; set; }
}