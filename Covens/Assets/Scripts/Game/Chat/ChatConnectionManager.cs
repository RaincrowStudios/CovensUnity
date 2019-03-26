using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BestHTTP.SocketIO;

public class ChatConnectionManager : MonoBehaviour
{

    public static ChatConnectionManager Instance { get; set; }
    public static ChatContainer AllChat;

    Socket dominionChat;
    Socket worldChat;
    Socket helpChat;
    Socket covenChat;
    bool isChatConnected = false;
    private SocketManager Manager;
    private string initString = "";
    string address = "http://127.0.0.1:8083/socket.io/";


    void Awake()
    {
        Instance = this;
    }

    public void InitChat()
    {
        print("InitChat");
        Manager = new SocketManager(new Uri("http://127.0.0.1:8083/socket.io/"));
        Manager.Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => Debug.LogError(string.Format("Error: {0}", args[0].ToString())));
        Manager.Open();
        var data = new { coven = (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName : "No Coven"), name = PlayerDataManager.playerData.displayName, dominion = PlayerDataManager.currentDominion };
        initString = JsonConvert.SerializeObject(data);
        Manager.Socket.On(SocketIOEventTypes.Connect, (socket, packet, args) =>
        {
            Manager.Socket.Emit("Join", initString);
            print("chatConnected");
        });

        Manager.Socket.On("SuccessAll", (socket, packet, args) =>
        {
            Debug.Log("got all chat data");
            isChatConnected = true;
            AllChat = Parse<ChatContainer>(args[0].ToString());
            ChatUI.Instance.initNotifications();
            ChatUI.Instance.Init();
            InitSocket(worldChat, "world");
            InitSocket(helpChat, "helpcrow" + PlayerDataManager.playerData.displayName);
            SendDominionChange();
            SendCovenChange();
            worldChat = Manager["/world"];
            worldChat.On("WorldMessage", ProcessJsonString);
            worldChat.On("WorldLocation", ProcessJsonString);
        });

        Manager.Socket.On("SuccessDominion", (socket, packet, args) =>
        {
            dominionChat = Manager["/" + PlayerDataManager.currentDominion];
            AllChat.DominionChat = Parse<ChatContainer>(args[0].ToString()).DominionChat;
            ChatUI.Instance.Init();
            dominionChat.On("DominionMessage", ProcessJsonString);
            dominionChat.On("DominionLocation", ProcessJsonString);
        });

        Manager.Socket.On("SuccessCoven", (socket, packet, args) =>
        {
            covenChat = Manager["/" + (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName : "No Coven")];
            //   InitSocket(covenChat, PlayerDataManager.playerData.covenName);
            AllChat.CovenChat = Parse<ChatContainer>(args[0].ToString()).CovenChat;
            ChatUI.Instance.Init();
            covenChat.On("CovenMessage", ProcessJsonString);
            covenChat.On("CovenLocation", ProcessJsonString);
        });

    }

    private void InitSocket(Socket socket, string endpoint)
    {
        socket = Manager["/" + endpoint];
        Debug.Log(socket.Namespace);
    }

    public void SendCovenChange()
    {
        if (!isChatConnected)
            return;
        if (covenChat != null && covenChat.IsOpen)
            covenChat.Disconnect();

        if (AllChat != null && AllChat.CovenChat != null)
            AllChat.CovenChat.Clear();
        var data = new { coven = (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName : "No Coven") };
        Manager.Socket.Emit("CovenChange", JsonConvert.SerializeObject(data));
    }

    public void SendDominionChange()
    {
        if (!isChatConnected)
            return;
        if (dominionChat != null && dominionChat.IsOpen)
            dominionChat.Disconnect();
        if (AllChat != null && AllChat.DominionChat != null)
            AllChat.DominionChat.Clear();
        var data = new { dominion = PlayerDataManager.currentDominion };
        Manager.Socket.Emit("DominionChange", JsonConvert.SerializeObject(data));
    }

    public void SendCoven(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        covenChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }

    public void SendWorld(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        worldChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }

    public void SendDominion(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        dominionChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }

    public void SendHelpcrow(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        helpChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }



    private T Parse<T>(string s)
    {
        Debug.Log(s);
        return JsonConvert.DeserializeObject<T>(s);
    }

    private void ProcessJsonString(Socket socket, Packet packet, params object[] args)
    {
        var Data = Parse<ChatData>(args[0].ToString());
        ChatUI.Instance.AddItemHelper(Data);
        ChatUI.Instance.addNotification(Data);
        // if (Data.CommandRaw == "HelpCrowMessage")
        // 
        //     HelpUI.Instance.CreateChat(Data);
        //     AllChat.HelpChat.Add(Data);
        //     return;
        // }
        // else if (Data.CommandRaw == "TranslateMessage")
        // {
        //     ChatUI.Instance.ReceiveTranslation(Data.Content);
        //     return;
        // }
    }

    void OnApplicationQuit()
    {
        Manager.Socket.Disconnect();
    }
}

public enum Commands
{
    Connected, WorldLocation, CovenLocation, WorldMessage, CovenMessage, NewsMessage, NewsLocation, DominionMessage, DominionLocation, CovenConnected, DominionConnected, HelpCrowConnected, HelpCrowMessage, TranslateMessage
}

public class ChatData
{
    public string _id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public bool Location { get; set; }
    public string Dominion { get; set; }
    public int Level { get; set; }
    public string Coven { get; set; }
    public string Title { get; set; }
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
