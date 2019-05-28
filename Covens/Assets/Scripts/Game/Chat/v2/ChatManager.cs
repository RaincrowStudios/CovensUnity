using BestHTTP.SocketIO;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Chat
{
    public static class ChatManager
    {
        private static UIChat m_ChatInstance;
        private static SocketManager m_SocketManager;
        private static Socket m_WorldSocket;
        //private static Socket 

        //
        public static bool Connected { get { return m_SocketManager != null && m_SocketManager.Socket != null && m_SocketManager.Socket.IsOpen; } }
        public static bool ConnectedToWorld { get { return m_WorldSocket != null && m_WorldSocket.IsOpen; } }

        //events
        public static event System.Action<ChatCategory, ChatMessage> OnReceiveMessage;
        public static event System.Action<string> OnSocketError;
        
        public static ChatPlayer Player { get; private set; }

        private static List<ChatMessage> m_NewsMessages = new List<ChatMessage>();
        private static List<ChatMessage> m_WorldMessages = new List<ChatMessage>();
        private static List<ChatMessage> m_CovenMessages = new List<ChatMessage>();
        private static List<ChatMessage> m_DominionMessages = new List<ChatMessage>();
        private static List<ChatMessage> m_SupportMessages = new List<ChatMessage>();

        public static void InitChat(ChatPlayer player)
        {
            //spawn new ui instance if necessary
            if (m_ChatInstance == null)
            {
                m_ChatInstance = GameObject.FindObjectOfType<UIChat>();
                if (m_ChatInstance == null)
                    m_ChatInstance = GameObject.Instantiate(Resources.Load<UIChat>("ChatCanvas"));
            }

            if (Connected)
            {
                Debug.LogError("Chat already initialized");
                return;
            }

            Player = player;

            string chatAddress = CovenConstants.chatAddress;

            Debug.Log("Initializing chat\n" + chatAddress);

            m_SocketManager = new SocketManager(new System.Uri(chatAddress));
            m_SocketManager.Encoder = new JsonDotNetEncoder();
            m_SocketManager.Socket.On(SocketIOEventTypes.Error, OnError);
            m_SocketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);

            m_SocketManager.Open();
        }

        //MAIN SOCKET EVENTS
        private static void OnError(Socket socket, Packet packet, object[] args)
        {
            string errorMessage = args[0].ToString();
            Debug.LogError("Chat socket error: " + errorMessage);

            OnSocketError?.Invoke(errorMessage);
        }

        private static void OnConnect(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Chat connected");

            Application.quitting -= OnApplicationQuitting;
            Application.quitting += OnApplicationQuitting;

            //set up the worldchat
            m_WorldSocket = m_SocketManager["/world"];
            m_WorldSocket.On("join.success", OnWorldJoin);
            m_WorldSocket.On("new.message", OnWorldMessage);
            
            //connect to the world chat
            m_WorldSocket.Emit("join.chat", Player);
        }

        //WORLD SOCKET EVENTS
        private static void OnWorldJoin(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Joined world chat");
            LogSerialized("OnWorldJoin", args);

            List<ChatMessage> messages = JsonConvert.DeserializeObject<List<ChatMessage>>(args[0].ToString());
            if (messages != null)
                m_WorldMessages = messages;
            else
                m_WorldMessages = messages;
        }

        private static void OnWorldMessage(Socket socket, Packet packet, object[] args)
        {
            LogSerialized("OnWorldMEssage", args);

            ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(args[0].ToString());

            m_WorldMessages.Add(msg);

            OnReceiveMessage(ChatCategory.WORLD, msg);
        }

        //COVEN SOCKET EVENTS
        private static void OnCovenJoin(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Joined coven chat");
            LogSerialized("OnCovenJoin", args);
        }



        public static void SendMessage(ChatCategory category, ChatMessage message)
        {
            Socket socket = null;

            switch (category)
            {
                case ChatCategory.WORLD: socket = m_WorldSocket; break;
                //case ChatCategory.SUPPORT: socket = m_sock
                //case ChatCategory.DOMINION: socket = 
                //case ChatCategory.COVEN: 
                case ChatCategory.NEWS:
                default: socket = null; break;
            }

            if (socket == null)
            {
                Debug.LogError("Socket not initialized [" + category + "]");
                return;
            }
            if (socket.IsOpen == false)
            {
                Debug.LogError("Socket not open [" + category + "]");
                return;
            }

            SendMessage(socket, message);
        }

        private static void SendMessage(Socket socket, ChatMessage message)
        {
            //filtering unnecessary properties
            object data = null;
            if (message.type == MessageType.TEXT)
            {
                data = new
                {
                    message.type,
                    data = new { message.data.message }
                };

            }
            else if (message.type == MessageType.LOCATION)
            {
                data = new
                {
                    message.type,
                    data = new
                    {
                        message.data.longitude,
                        message.data.latitude
                    }
                };
            }
            else if (message.type == MessageType.IMAGE)
            {
                data = new
                {
                    message.type,
                    data = new { message.data.image }
                };
            }

            LogSerialized("SendMessage", data);
            m_WorldSocket.Emit("send.message", data);
        }

        public static List<ChatMessage> GetMessages(ChatCategory category)
        {
            switch (category)
            {
                case ChatCategory.COVEN: return m_CovenMessages;
                case ChatCategory.DOMINION: return m_DominionMessages;
                case ChatCategory.NEWS: return m_NewsMessages;
                case ChatCategory.SUPPORT: return m_SupportMessages;
                case ChatCategory.WORLD: return m_WorldMessages;
                default: return new List<ChatMessage>();
            }
        }

        private static void LogSerialized(string title, object obj)
        {
#if UNITY_EDITOR
            Debug.Log("[" + title + "]" + JsonConvert.SerializeObject(obj, Formatting.Indented));
#endif
        }

        private static void OnApplicationQuitting()
        {
            Debug.Log("Disconnecting chat");
            m_SocketManager.Socket.Disconnect();
        }
    }
}
