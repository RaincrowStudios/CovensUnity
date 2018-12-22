using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using WebSocketSharp;


public class ChatConnectionManager : MonoBehaviour
{

    public static ChatConnectionManager Instance { get; set; }
    public static ChatContainer AllChat;
    WebSocket serverChat;
    WebSocket serverCoven;
    WebSocket serverDominion;
    WebSocket serverHelp;
    bool covenConnected = false;
    bool dominionConnected = false;
    public static bool helpConnected = false;

    string address = "ws://52.1.214.93:8086/";
    string addressHttp = "http://52.1.214.93:8086";

    //string addressHttp = "http://127.0.0.1:8086/";
    //string address = "ws://127.0.0.1:8086/";

    void Awake()
    {
        Instance = this;
    }

    IEnumerator connectDominion()
    {
        try
        {
            serverDominion.Close();
        }
        catch
        {
        }
        //		print (PlayerDataManager.currentDominion);

        serverDominion = new WebSocket(new Uri(address + PlayerDataManager.currentDominion));
        yield return StartCoroutine(serverDominion.Connect());
        if (serverDominion.error == null)
        {
            dominionConnected = true;
        }
    }

    IEnumerator connectCoven()
    {
        try
        {
            serverCoven.Close();
            AllChat.CovenChat.Clear();
        }
        catch
        {
        }
        if (PlayerDataManager.playerData.covenName != "")
        {
            serverCoven = new WebSocket(new Uri(address + PlayerDataManager.playerData.covenName));

            yield return StartCoroutine(serverCoven.Connect());
            if (serverCoven.error == null)
            {
                //				print ("Coven Connected!");
                covenConnected = true;
            }
        }
    }

    IEnumerator connectHelp()
    {
        try
        {
            serverHelp.Close();
            AllChat.HelpChat.Clear();
        }
        catch
        {
        }
        serverHelp = new WebSocket(new Uri(address + "helpcrow_" + PlayerDataManager.playerData.displayName));

        yield return StartCoroutine(serverHelp.Connect());
        if (serverHelp.error == null)
        {
            //				print ("Help Connected!");
            helpConnected = true;
        }
    }

    public void InitChat()
    {
        StartCoroutine(EstablishWSConnection());
        //		StartCoroutine ( StartChart ());
    }


    IEnumerator EstablishWSConnection()
    {
        //		print ("initializing Chat!!");
        {
            using (WWW www = new WWW(addressHttp))
            {
                yield return www;
                if (www.error == null)
                {
                    //					print (www.text + "From Chat Web Socket HTTP");
                    StartCoroutine(StartChart());
                }
                else
                    Debug.LogError(www.error);
            }
        }
    }


    public void SendCovenChannelRequest()
    {
        covenConnected = false;
        ChatData CD = new ChatData
        {
            Name = PlayerDataManager.playerData.displayName,
            Coven = PlayerDataManager.playerData.covenName,
            CommandRaw = Commands.CovenConnected.ToString()
        };
        send(CD);
    }

    public void SendDominionChannelRequest()
    {
        dominionConnected = false;
        ChatData CD = new ChatData
        {
            Name = PlayerDataManager.playerData.displayName,
            Dominion = PlayerDataManager.currentDominion,
            CommandRaw = Commands.DominionConnected.ToString()
        };
        send(CD);
    }

    public void SendHelpChannelRequest()
    {
        helpConnected = false;
        ChatData CD = new ChatData
        {
            Name = PlayerDataManager.playerData.displayName,
            Channel = "helpcrow_" + PlayerDataManager.playerData.displayName,
            CommandRaw = Commands.HelpCrowConnected.ToString()
        };
        send(CD);
    }

    IEnumerator StartChart()
    {
        try
        {
            serverChat.Close();
            AllChat = new ChatContainer();
        }
        catch
        {
        }
        serverChat = new WebSocket(new Uri(address + "Chat"));
        ChatData CD = new ChatData
        {
            Name = PlayerDataManager.playerData.displayName,
            Coven = PlayerDataManager.playerData.covenName,
            Dominion = PlayerDataManager.currentDominion,
            CommandRaw = Commands.Connected.ToString()
        };

        yield return StartCoroutine(serverChat.Connect());
        send(CD);

        if (PlayerDataManager.playerData.covenName != "")
            SendCovenChannelRequest();
        SendDominionChannelRequest();

        while (true)
        {
            string reply = serverChat.RecvString();
            if (reply != null)
            {
                //				print (reply);
                ProcessJsonString(reply);
            }
            if (serverChat.error != null)
            {
                Debug.LogError("Error: " + serverChat.error);
                InitChat();
                break;
            }

            if (PlayerDataManager.playerData.covenName != "" && covenConnected)
            {
                string replyc = serverCoven.RecvString();
                if (replyc != null)
                {
                    //					print (replyc + "Coven");
                    ProcessJsonString(replyc);
                }
                if (serverCoven.error != null)
                {
                    Debug.LogError("Error: " + serverCoven.error);
                    InitChat();
                    break;
                }
            }
            if (dominionConnected)
            {
                string replyd = serverDominion.RecvString();
                if (replyd != null)
                {
                    //					print (replyd + "dom");
                    ProcessJsonString(replyd);
                }
                if (serverDominion.error != null)
                {
                    Debug.LogError("Error: " + serverDominion.error);
                    InitChat();
                    break;
                }
            }

            if (helpConnected)
            {
                string replyd = serverHelp.RecvString();
                if (replyd != null)
                {
                    //					print (replyd + "help");
                    ProcessJsonString(replyd);
                }
                if (serverHelp.error != null)
                {
                    Debug.LogError("Error: " + serverHelp.error);
                    InitChat();
                    break;
                }
            }
            yield return 0;
        }
        serverChat.Close();
    }

    public void send(ChatData data)
    {
        //		print ("Sending " + JsonConvert.SerializeObject (data));
        serverChat.Send(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
    }

    public void ProcessJsonString(string rawData)
    {
        try
        {
            print(rawData);
            var Data = JsonConvert.DeserializeObject<ChatData>(rawData);
            if (Data.CommandRaw == "HelpCrowMessage")
            {
                HelpUI.Instance.CreateChat(Data);
                AllChat.HelpChat.Add(Data);
                return;
            }
            else if (Data.CommandRaw == "TranslateMessage")
            {
                ChatUI.Instance.ReceiveTranslation(Data.Content);
                return;
            }
            ChatUI.Instance.AddItemHelper(Data);
            ChatUI.Instance.addNotification(Data);
            return;
        }
        catch (Exception ex)
        {
        }
        try
        {
            //			print(rawData);
            var chatObject = JsonConvert.DeserializeObject<ChatContainer>(rawData);
            if (chatObject.CommandRaw == "all")
            {
                AllChat = chatObject;
                ChatUI.Instance.initNotifications();
                ChatUI.Instance.Init();
            }
            else if (chatObject.CommandRaw == "coven")
            {
                AllChat.CovenChat = chatObject.CovenChat;
                ChatUI.Instance.Init();
                StartCoroutine(connectCoven());
            }
            else if (chatObject.CommandRaw == "dominion")
            {
                AllChat.DominionChat = chatObject.DominionChat;
                ChatUI.Instance.Init();
                StartCoroutine(connectDominion());
            }
            else if (chatObject.CommandRaw == "help")
            {
                AllChat.HelpChat = chatObject.HelpChat;
                HelpUI.Instance.Init();
                StartCoroutine(connectHelp());
            }
        }
        catch (Exception ex)
        {

        }
    }

    void OnApplicationQuit()
    {
        try
        {
            serverChat.Close();
            serverCoven.Close();
            serverDominion.Close();
            serverHelp.Close();
        }
        catch
        {
        }
    }

}

public enum Commands
{
    Connected, WorldLocation, CovenLocation, WorldMessage, CovenMessage, NewsMessage, NewsLocation, DominionMessage, DominionLocation, CovenConnected, DominionConnected, HelpCrowConnected, HelpCrowMessage, TranslateMessage
}

public class ChatData
{
    public string Name { get; set; }
    public string Content { get; set; }
    public string Receiver { get; set; }
    public string Dominion { get; set; }
    public int Level { get; set; }
    public string Coven { get; set; }
    public double TimeStamp { get; set; }
    public int Avatar { get; set; }
    public int Degree { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string CommandRaw { get; set; }
    public string Language { get; set; }
    public string Channel { get; set; }
    public Commands Command;
}

public class ChatContainer
{
    public string CommandRaw { get; set; }
    public List<ChatData> WorldChat { get; set; }
    public List<ChatData> CovenChat { get; set; }
    public List<ChatData> DominionChat { get; set; }
    public List<ChatData> News { get; set; }
    public List<ChatData> HelpChat { get; set; }
}
