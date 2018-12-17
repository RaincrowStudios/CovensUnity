using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;

public class HelpUI : MonoBehaviour
{
    public static HelpUI Instance { get; set; }
    public List<GameObject> chatItems = new List<GameObject>();
    public Transform container;
    public Button sendButton;
    public InputField inputField;
    public Animator anim;
    public GameObject yourMessage;
    public GameObject cawMessage;
    public GameObject ChatParentObject;
    public GameObject popupMessage;


    public static string HasConversationStarted
    {
        get { return PlayerPrefs.GetString("Helpcrow", ""); }
        set { PlayerPrefs.SetString("Helpcrow", value); }
    }

    void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        print(ChatConnectionManager.AllChat.HelpChat.Count);
        if (ChatConnectionManager.AllChat.HelpChat == null || ChatConnectionManager.AllChat.HelpChat.Count == 0)
        {
            ChatData cd = new ChatData();
            cd.Name = "helpcrow";
            cd.Content = "State your trouble, witch.";
            cd.TimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            CreateChat(cd);
            return;
        }

        foreach (var item in ChatConnectionManager.AllChat.HelpChat)
        {
            CreateChat(item);
        }
    }

    public void CreateChat(ChatData data)
    {
        print("Creating Chat");
        var g = Utilities.InstantiateObject((data.Name == "helpcrow" ? cawMessage : yourMessage), container);
        g.GetComponent<HelpChatData>().Setup(data);
    }

    public void SendMessage()
    {
        if (!inputField.text.IsNullOrWhiteSpace())
        {
            sendButton.interactable = false;
            inputField.interactable = false;

            ChatData CD = new ChatData();
            CD.Name = PlayerDataManager.playerData.displayName;
            CD.Content = inputField.text;
            CD.CommandRaw = Commands.HelpCrowMessage.ToString();
            CD.Channel = "helpcrow_" + PlayerDataManager.playerData.displayName;
            CD.Language = LoginAPIManager.systemLanguage;
            inputField.text = "";
            ChatConnectionManager.Instance.send(CD);
            StartCoroutine(ReEnableSendButton());
        }
    }

    IEnumerator ReEnableSendButton()
    {
        yield return new WaitForSeconds(1.5f);
        sendButton.interactable = true;
        inputField.interactable = true;
    }

    public void ShowChat()
    {
        if (!ChatConnectionManager.helpConnected)
        {
            ChatConnectionManager.Instance.SendHelpChannelRequest();
        }
        if (HasConversationStarted == "")
        {
            popupMessage.SetActive(true);
        }
        else
        {
            UIStateManager.Instance.CallWindowChanged(false);
            SoundManagerOneShot.Instance.MenuSound();
            ChatParentObject.SetActive(true);
            anim.SetBool("animate", true);
        }
    }

    public void HideChat()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        SoundManagerOneShot.Instance.MenuSound();
        anim.SetBool("animate", false);
    }

    public void ConfirmPopup()
    {
        HasConversationStarted = "true";
        ShowChat();
        popupMessage.SetActive(false);
    }
}

