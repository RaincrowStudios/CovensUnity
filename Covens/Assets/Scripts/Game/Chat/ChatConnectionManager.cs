using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using BestHTTP.SocketIO;
using UnityEngine.Networking;

public class ChatConnectionManager : MonoBehaviour
{

    public static ChatConnectionManager Instance { get; set; }
    public static ChatContainer AllChat;

    Socket dominionChat;
    Socket worldChat;
    Socket helpChat;
    Socket covenChat;
    Socket newsChat;
    bool isChatConnected = false;
    private SocketManager Manager;
    private string initString = "";
    //  string address = "http://127.0.0.1:8083/socket.io/";
    private bool helpCrowConnected = false;

    void Awake()
    {
        Instance = this;
    }

    void SendChatHTTPRequest()
    {
        Debug.Log("<b> Sending connect Chat request");
        var data = new { coven = (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName : "No Coven"), name = PlayerDataManager.playerData.displayName, dominion = PlayerDataManager.currentDominion, instance = PlayerDataManager.playerData.instance };

        PostData("connect", JsonConvert.SerializeObject(data), (string res, int r) =>
        {
            if (r == 200)
            {
                Debug.Log(res);
                Debug.Log("got all chat data");
                isChatConnected = true;
                AllChat = Parse<ChatContainer>(res);
                ChatUI.Instance.initNotifications();
                ChatUI.Instance.Init();
                //  SendDominionChange();
                //  SendCovenChange();
            }
        });
    }

    public void SendDominionHTTPRequest(string data)
    {

        PostData("dominion", data, (string res, int r) =>
        {
            if (r == 200)
            {
                Debug.Log(res);

                Debug.Log("sending Join request Dom");
                AllChat.DominionChat = Parse<ChatContainer>(res).DominionChat;
                ChatUI.Instance.Init();
            }
        });
    }

    public void SendCovenHTTPRequest(string data)
    {
        PostData("coven", data, (string res, int r) =>
        {
            if (r == 200)
            {
                Debug.Log(res);

                Debug.Log("sending Join request Dom");
                AllChat.CovenChat = Parse<ChatContainer>(res).CovenChat;
                ChatUI.Instance.Init();
            }
        });
    }


    public void InitChat()
    {
        //Debug.Log("InitChat");
        Debug.Log(CovenConstants.chatAddress);
        Manager = new SocketManager(new Uri(CovenConstants.chatAddress));
        Manager.Open();
        Manager.Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => Debug.LogError(string.Format("Error: {0}", args[0].ToString())));
        var data = new { coven = (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName : "No Coven"), name = PlayerDataManager.playerData.displayName, dominion = PlayerDataManager.currentDominion, instance = PlayerDataManager.playerData.instance };
        initString = JsonConvert.SerializeObject(data);
        Manager.Socket.On(SocketIOEventTypes.Connect, (socket, packet, args) =>
        {
            Debug.Log("InitChat");
            Manager.Socket.Emit("Join", initString);
            SendChatHTTPRequest();
            Debug.Log("chatConnected");
            worldChat = Manager["/world"];
            newsChat = Manager["/news"];
        });

        Manager.Socket.On("SuccessAll", (socket, packet, args) =>
        {
            SendDominionChange();
            SendCovenChange();
            worldChat.On("WorldMessage", ProcessJsonString);
            worldChat.On("WorldLocation", ProcessJsonString);
            newsChat.On("NewsMessage", ProcessJsonString);
            newsChat.On("NewsLocation", ProcessJsonString);
        });

        Manager.Socket.On("SuccessDominion", (socket, packet, args) =>
        {
            if (dominionChat != null && dominionChat.IsOpen)
            {
                Debug.Log("disconnecting dominion");
                dominionChat.Disconnect();
            }
            Debug.Log("SUCCESS DOMINION");
            dominionChat = Manager["/" + PlayerDataManager.currentDominion.Replace(" ", "-")];
            dominionChat.On("DominionMessage", ProcessJsonString);
            dominionChat.On("DominionLocation", ProcessJsonString);
        });

        Manager.Socket.On("SuccessCoven", (socket, packet, args) =>
        {
            Debug.Log("SUCCESS COVEN");
            if (covenChat != null && covenChat.IsOpen)
            {
                covenChat.Disconnect();
                Debug.Log("coven Disconnected");
            }
            covenChat = Manager["/" + (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName.Replace(" ", "-") : "No-Coven")];
            covenChat.On("CovenMessage", ProcessJsonString);
            covenChat.On("CovenLocation", ProcessJsonString);
        });

    }

    public void ConnectHelpCrow()
    {
        if (!helpCrowConnected)
        {
            Debug.Log("HelpcrowConneted");
            helpChat = Manager["/helpcrow" + PlayerDataManager.playerData.displayName.Replace(" ", "-")];
            helpChat.On("HelpCrowMessage", ProcessJsonString);
            helpCrowConnected = true;
        }
    }

    public void SendCovenChange()
    {
        if (!isChatConnected)
            return;
        Debug.Log("covenChanged get socket");
        if (covenChat != null && covenChat.IsOpen)
        {
            covenChat.Disconnect();
            Debug.Log("coven Disconnected");
        }

        if (AllChat != null && AllChat.CovenChat != null)
            AllChat.CovenChat.Clear();
        var data = new { coven = (PlayerDataManager.playerData.covenName != "" ? PlayerDataManager.playerData.covenName.Replace(" ", "-") : "No-Coven") };
        SendCovenHTTPRequest(JsonConvert.SerializeObject(data));
        Manager.Socket.Emit("CovenChange", JsonConvert.SerializeObject(data));
    }

    public void SendDominionChange()
    {
        if (!isChatConnected)
            return;
        if (dominionChat != null && dominionChat.IsOpen)
        {
            Debug.Log("disconnecting dominion");
            dominionChat.Disconnect();
        }
        if (AllChat != null && AllChat.DominionChat != null)
            AllChat.DominionChat.Clear();
        var data = new { dominion = PlayerDataManager.currentDominion.Replace(" ", "-") };
        SendDominionHTTPRequest(JsonConvert.SerializeObject(data));
        Manager.Socket.Emit("DominionChange", JsonConvert.SerializeObject(data));
    }

    public void SendCoven(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        Debug.Log(covenChat.Namespace + "  " + covenChat.IsOpen);

        Manager.Socket.Emit("msg", JsonConvert.SerializeObject(data));
    }

    public void SendWorld(ChatData data)
    {
        // Debug.Log(JsonConvert.SerializeObject(data));

        // Debug.Log(worldChat.Namespace + "  " + worldChat.IsOpen);
        worldChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }

    public void SendDominion(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        Debug.Log(dominionChat.Namespace + "  " + dominionChat.IsOpen);
        Manager.Socket.Emit("msg", JsonConvert.SerializeObject(data));
    }

    public void SendHelpcrow(ChatData data)
    {
        Debug.Log(JsonConvert.SerializeObject(data));
        // Debug.Log(helpChat.Namespace + "  " + helpChat.IsOpen);
        helpChat.Emit(data.CommandRaw, JsonConvert.SerializeObject(data));
    }



    private T Parse<T>(string s)
    {
        Debug.Log(s);
        return JsonConvert.DeserializeObject<T>(s);
    }

    private void ProcessJsonString(Socket socket, Packet packet, params object[] args)
    {
        var t = JsonConvert.DeserializeObject<List<string>>(packet.Payload);
        var Data = Parse<ChatData>(t[1]);
        ChatUI.Instance.AddItemHelper(Data);
        ChatUI.Instance.addNotification(Data);
    }

    void OnApplicationQuit()
    {
        Manager.Socket.Disconnect();
    }


    public void PostData(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(PostDataHelper(endpoint, data, CallBack));
    }

    IEnumerator PostDataHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(CovenConstants.chatAddressHTTP + endpoint, data);

        www.method = "POST";

        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        string sRequest = "==> BakeRequest for: " + endpoint;
        sRequest += "\n  endpoint: " + CovenConstants.chatAddressHTTP + "covens/" + endpoint;
        sRequest += "\n  method: " + ("POST");
        sRequest += "\n  data: " + data;
        sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
        Debug.Log(sRequest);


        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
            CallBack(www.error, Convert.ToInt32(www.responseCode));
        }
        else
        {
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }


    }
}




public enum Commands
{
    Connected, WorldLocation, CovenLocation, WorldMessage, CovenMessage, NewsMessage, NewsLocation, DominionMessage, DominionLocation, CovenConnected, DominionConnected, HelpCrowConnected, HelpCrowMessage, TranslateMessage, BugsMessage
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
    public List<ChatData> Bugs { get; set; }
}