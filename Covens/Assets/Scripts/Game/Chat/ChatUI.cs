using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class ChatUI : UIAnimationManager
{
    public static ChatUI Instance { get; set; }

    public Text worldButton;
    public Text newsButton;
    public Text dominionButton;
    public Text covenButton;
    public Text CovenUIText;

    public Text worldButtonNotification;
    public Text newsButtonNotification;
    public Text dominionButtonNotification;
    public Text covenButtonNotification;

    public List<GameObject> chatItems = new List<GameObject>();

    //	public Sprite[] profilePics;
    public GameObject locationPrefab;
    public GameObject chatPrefab;
    public Transform container;
    public GameObject ChatParentObject;

    public GameObject Header;

    public InputField inputMessage;
    public Button shareLocation;
    public Button sendButton;
    public Action<string> ReceiveTranslation;
    public Sprite[] chatHeads;

    int newsNoti, worldNoti, covenNoti, dominionNoti = 0;

    public static int currentCount = 0;
    int playerAvatar;
    public Animator anim;

    public ApparelView maleApparel;
    public ApparelView femaleApparel;

    public GameObject playerInfo;
    public Text playerName;
    public Text playerLevel;
    public Text playerDegree;
    public Text playerEnergy;
    public GameObject InviteToCoven;
    public GameObject playerLoading;
    public Button inviteButton;
    public Text InviteText;
    public Text playerCoven;
    public GameObject inviteLoading;

    public enum ChatWindows
    {
        News,
        World,
        Covens,
        Dominion,
    };

    public ChatWindows ActiveWindow = ChatWindows.World;

    void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        SwitchWindow("world");
        SetAvatar();
    }

    public void initNotifications()
    {
        worldButtonNotification.text = covenButtonNotification.text = newsButtonNotification.text = dominionButtonNotification.text = "";
        newsNoti = worldNoti = covenNoti = dominionNoti = 0;
    }

    public void addNotification(ChatData data)
    {
        var c = (Commands)Enum.Parse(typeof(Commands), data.CommandRaw);
        data.Command = c;
        //		print ("Adding stuff");
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
    }

    public void SwitchWindow(string type)
    {
        currentCount = 0;
        worldButton.transform.localScale = newsButton.transform.localScale = dominionButton.transform.localScale = covenButton.transform.localScale = Vector3.one;
        worldButton.color = newsButton.color = dominionButton.color = covenButton.color = Utilities.Grey;
        playerLoading.SetActive(false);
        playerInfo.SetActive(false);
        CovenUIText.gameObject.SetActive(false);
        inputMessage.interactable = true;
        sendButton.interactable = true;
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
            ActiveWindow = ChatWindows.Covens;
            CovenUIText.gameObject.SetActive(true);
            if (PlayerDataManager.playerData.covenName != "")
            {
                CovenUIText.text = PlayerDataManager.playerData.covenName;
                populateChat(ChatConnectionManager.AllChat.CovenChat);

            }
            else
            {
                CovenUIText.text = "No Coven";
                clearChat();
                inputMessage.interactable = false;
                sendButton.interactable = false;
                shareLocation.interactable = false;
            }
            covenButton.transform.localScale = Vector3.one * 1.2f;
            covenButton.color = Color.white;
            covenNoti = 0;
            covenButtonNotification.text = "";

        }
        else
        {
            ActiveWindow = ChatWindows.Dominion;
            populateChat(ChatConnectionManager.AllChat.DominionChat);
            dominionButton.transform.localScale = Vector3.one * 1.2f;
            dominionButton.color = Color.white;
            dominionNoti = 0;
            dominionButtonNotification.text = "";
        }
    }

    void populateChat(List<ChatData> CD)
    {
        clearChat();
        if (CD == null)
            return;
        foreach (var item in CD)
        {
            AddItemHelper(item);
        }
    }

    public void AddItemHelper(ChatData CD)
    {
        #region newsScroll
        if (CD.Command == Commands.CovenMessage)
        {
            NewsScroll.Instance.ShowText("(Coven) " + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.CovenLocation)
        {
            NewsScroll.Instance.ShowText("(Coven) " + CD.Name + " shared location.", true);
        }
        else if (CD.Command == Commands.WorldMessage)
        {
            NewsScroll.Instance.ShowText("(World) " + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.WorldLocation)
        {
            NewsScroll.Instance.ShowText("(World) " + CD.Name + " shared location.", true);
        }
        else if (CD.Command == Commands.NewsMessage)
        {
            NewsScroll.Instance.ShowText("(News) " + CD.Content, true);
        }
        else if (CD.Command == Commands.DominionMessage)
        {
            NewsScroll.Instance.ShowText("(" + CD.Dominion + ") " + CD.Name + " : " + CD.Content, true);
        }
        else if (CD.Command == Commands.DominionLocation)
        {
            NewsScroll.Instance.ShowText("(" + CD.Dominion + ") " + CD.Name + " shared location.", true);
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
        else
        {
            if (CD.Command == Commands.DominionMessage || CD.Command == Commands.DominionLocation)
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
            CD.Name = PlayerDataManager.playerData.displayName;
            CD.Content = inputMessage.text;
            CD.Degree = PlayerDataManager.playerData.degree;
            CD.Level = PlayerDataManager.playerData.level;
            CD.Language = LoginAPIManager.systemLanguage;
            if (ActiveWindow == ChatWindows.World)
            {
                CD.CommandRaw = Commands.WorldMessage.ToString();
            }
            else if (ActiveWindow == ChatWindows.Covens)
            {
                CD.CommandRaw = Commands.CovenMessage.ToString();
                CD.Coven = PlayerDataManager.playerData.covenName;
            }
            else if (ActiveWindow == ChatWindows.Dominion)
            {
                CD.CommandRaw = Commands.DominionMessage.ToString();
                CD.Dominion = PlayerDataManager.currentDominion;
            }
            //			inputMessage.Select ();
            inputMessage.text = "";
            ChatConnectionManager.Instance.send(CD);
            StartCoroutine(ReEnableSendButton());
        }
    }

    public void SendTranslate(string text, Action<string> OnReceiveTranslation)
    {
        ChatData CD = new ChatData();
        CD.Content = text;
        CD.CommandRaw = Commands.TranslateMessage.ToString();
        CD.Language = LoginAPIManager.systemLanguage;
        ReceiveTranslation = OnReceiveTranslation;
        ChatConnectionManager.Instance.send(CD);
    }

    public void SendLocation()
    {
        sendButton.interactable = false;
        shareLocation.interactable = false;
        ChatData CD = new ChatData();
        CD.Name = PlayerDataManager.playerData.displayName;
        CD.Degree = PlayerDataManager.playerData.degree;
        CD.Level = PlayerDataManager.playerData.level;
        CD.Latitude = MapsAPI.Instance.position.y;
        CD.Longitude = MapsAPI.Instance.position.x;
        CD.Avatar = playerAvatar;
        if (ActiveWindow == ChatWindows.World)
        {
            CD.CommandRaw = Commands.WorldLocation.ToString();
        }
        else if (ActiveWindow == ChatWindows.Covens)
        {
            CD.CommandRaw = Commands.CovenLocation.ToString();
            CD.Coven = PlayerDataManager.playerData.covenName;
        }
        else if (ActiveWindow == ChatWindows.Dominion)
        {
            CD.CommandRaw = Commands.DominionLocation.ToString();
            CD.Dominion = PlayerDataManager.currentDominion;
        }
        //			inputMessage.Select ();
        //			inputMessage.text = "";
        //			print (JsonConvert.SerializeObject (CD));
        ChatConnectionManager.Instance.send(CD);
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
        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();

        ChatParentObject.SetActive(true);
        anim.SetBool("animate", true);
    }

    public void HideChat()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        SoundManagerOneShot.Instance.MenuSound();

        anim.SetBool("animate", false);
    }

    void SetAvatar()
    {
        var data = PlayerDataManager.playerData;
        if (data.male)
        {
            if (data.race.Contains("A"))
            {
                playerAvatar = 0;
            }
            else if (data.race.Contains("O"))
            {
                playerAvatar = 1;
            }
            else
            {
                playerAvatar = 2;
            }
        }
        else
        {
            if (data.race.Contains("A"))
            {
                playerAvatar = 3;

            }
            else if (data.race.Contains("O"))
            {
                playerAvatar = 4;

            }
            else
            {
                playerAvatar = 5;

            }
        }
    }

    public void LogCovenNotification(string message)
    {
        ChatData data = new ChatData();
        data.Avatar = -1;
        data.Command = Commands.CovenMessage;
        data.CommandRaw = Commands.CovenMessage.ToString();
        data.Name = message;
        data.Content = "";
        data.Language = LoginAPIManager.systemLanguage;
        data.TimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

        try
        {
            AddItemHelper(data);
            addNotification(data);
        }
        //catch any exception in case the chat is not properly initialized to avoid the websocketmanaget breaking
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }

    public void GetPlayerDetails(String playerID)
    {
        var data = new { target = playerID };
        playerLoading.SetActive(true);

        APIManager.Instance.PostData("chat/select", JsonConvert.SerializeObject(data), (string s, int r) =>
        {
            playerLoading.SetActive(false);

            if (r == 200)
            {

                playerInfo.SetActive(true);
                var jsonData = JsonConvert.DeserializeObject<MarkerDataDetail>(s);
                playerName.text = playerID;
                playerCoven.text = jsonData.covenName;
                if (jsonData.equipped[0].id.Contains("_m_"))
                {
                    femaleApparel.gameObject.SetActive(false);
                    maleApparel.gameObject.SetActive(true);
                    maleApparel.InitializeChar(jsonData.equipped);
                }
                else
                {
                    femaleApparel.gameObject.SetActive(true);
                    maleApparel.gameObject.SetActive(false);
                    femaleApparel.InitializeChar(jsonData.equipped);
                }
                if (PlayerDataManager.playerData.covenName != "")
                {
                    InviteToCoven.SetActive(jsonData.covenName == "");
                }
                else
                {
                    InviteToCoven.SetActive(false);
                }
                playerLevel.text = "Level: " + jsonData.level.ToString();
                playerDegree.text = Utilities.witchTypeControlSmallCaps(jsonData.degree);
                playerEnergy.text = "Energy: " + jsonData.energy.ToString();
            }
            else
            {

            }
        });

    }

    public void SendInviteRequest()
    {
        var data = new { invited = MarkerSpawner.instanceID };
        inviteLoading.SetActive(true);
        APIManager.Instance.PostData("coven/invite", JsonConvert.SerializeObject(data), requestResponse);
    }
    public void requestResponse(string s, int r)
    {
        inviteLoading.SetActive(false);
        Debug.Log(s);
        if (r == 200)
        {
            inviteButton.onClick.RemoveListener(SendInviteRequest);
            InviteText.text = "Invitation Sent!";
        }
        else
        {
            Debug.Log(s);
            if (s == "4803")
            {
                InviteText.text = "Invitation already Sent!";
                InviteText.color = Color.red;
            }
            else
            {
                InviteText.text = "Invite Failed...";
                InviteText.color = Color.red;
            }
            inviteButton.onClick.RemoveListener(SendInviteRequest);
        }
    }

}

