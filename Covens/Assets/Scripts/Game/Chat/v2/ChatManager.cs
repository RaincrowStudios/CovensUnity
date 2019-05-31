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
        private static Socket m_CovenSocket;
        private static Socket m_DominionSocket;
        private static Socket m_SupportSocket;

        //
        public static ChatPlayer Player { get; private set; }
        public static bool Connected { get { return m_SocketManager != null && m_SocketManager.Socket != null && m_SocketManager.Socket.IsOpen; } }
        private static Dictionary<ChatCategory, int> m_NewMessages = new Dictionary<ChatCategory, int>
        {
            { ChatCategory.NONE, 0 },
            { ChatCategory.SUPPORT, 0 },
            { ChatCategory.WORLD, 0 },
            { ChatCategory.COVEN, 0 },
            { ChatCategory.DOMINION, 0 },
            { ChatCategory.NEWS, 0 },
        };
        private static Dictionary<ChatCategory, List<ChatMessage>> m_Messages = new Dictionary<ChatCategory, List<ChatMessage>>
        {
            { ChatCategory.NONE, new List<ChatMessage>() },
            { ChatCategory.SUPPORT, new List<ChatMessage>() },
            { ChatCategory.WORLD, new List<ChatMessage>() },
            { ChatCategory.COVEN, new List<ChatMessage>() },
            { ChatCategory.DOMINION, new List<ChatMessage>() },
            { ChatCategory.NEWS, new List<ChatMessage>() },
        };

        //events
        public static event System.Action<ChatCategory, ChatMessage> OnReceiveMessage;
        public static event System.Action<string> OnSocketError;
        public static event System.Action<ChatCategory> OnConnected;




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
            if (m_SocketManager != null)
            {
                Debug.LogError("Chat is initializing");
                return;
            }

            Player = player;

            string chatAddress = CovenConstants.chatAddress;

            Debug.Log("Initializing chat\n" + chatAddress);

            m_SocketManager = new SocketManager(new System.Uri(chatAddress));
            m_SocketManager.Encoder = new JsonDotNetEncoder();
            m_SocketManager.Socket.On(SocketIOEventTypes.Error, (a,b,c) => OnError(ChatCategory.NONE, a, b, c));
            m_SocketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);

            m_SocketManager.Open();
        }

        public static void InitCoven(string covenName, string covenId)
        {
            if (string.IsNullOrEmpty(covenName))
            {
                Debug.LogError("Invalid coven name");
                return;
            }

            if (!Connected)
            {
                Debug.LogError("Chat not initialized");
                return;
            }

            if (m_CovenSocket == null)
            {
                m_CovenSocket = m_SocketManager["/coven"];
                m_CovenSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.COVEN, _args));
                m_CovenSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.COVEN, _args));
                m_CovenSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.COVEN, a, b, c));
            }
            m_CovenSocket.Emit("join.chat", Player);
        }

        public static void InitDominion(string dominion)
        {
            if (!Connected)
            {
                Debug.LogError("Chat not initialized");
                return;
            }

            if (m_DominionSocket == null)
            {
                Debug.Log("Initalizing dominion socket");
                m_DominionSocket = m_SocketManager["/dominion"];
                m_DominionSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.DOMINION, _args));
                m_DominionSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.DOMINION, _args));
                m_DominionSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.DOMINION, a, b, c));
            }
            Debug.Log("Joining dominion " + dominion);
            m_DominionSocket.Emit("join.chat", Player, new { id = dominion });
        }

        //MAIN SOCKET EVENTS
        private static void OnError(ChatCategory category, Socket socket, Packet packet, object[] args)
        {
            string errorMessage = args[0].ToString();
            Debug.LogError("[" + category + "] Chat error: " + errorMessage);
            OnSocketError?.Invoke(errorMessage);
        }

        private static void OnConnect(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Chat connected");

            Application.quitting -= OnApplicationQuitting;
            Application.quitting += OnApplicationQuitting;

            //set up the worldchat
            m_WorldSocket = m_SocketManager["/world"];
            m_WorldSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.WORLD, _args));
            m_WorldSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.WORLD, _args));
            m_WorldSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.WORLD, a, b, c));
            m_WorldSocket.Emit("join.chat", Player);
            
            //support
            m_SupportSocket = m_SocketManager["/help"];
            m_SupportSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.SUPPORT, _args));
            m_SupportSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.SUPPORT, _args));
            m_SupportSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.SUPPORT, a, b, c));
            m_SupportSocket.Emit("join.chat", Player);

            InitDominion("Washington");
        }

        private static void OnSocketJoinChat(ChatCategory category, object[] args)
        {
            Debug.Log("Joined " + category + " chat");

            List<ChatMessage> messages = JsonConvert.DeserializeObject<List<ChatMessage>>(args[0].ToString());
            m_Messages[category] = messages;

            OnConnected?.Invoke(category);
        }

        private static void OnSocketReceiveMessage(ChatCategory category, object[] args)
        {
            ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(args[0].ToString());

            if (m_Messages[category].Count >= 50)
                m_Messages[category].RemoveAt(0);

            m_Messages[category].Add(msg);
            m_NewMessages[category] += 1;
            OnReceiveMessage(category, msg);
        }




        public static void SendMessage(ChatCategory category, ChatMessage message)
        {
            Socket socket = null;

            switch (category)
            {
                case ChatCategory.WORLD: socket = m_WorldSocket; break;
                case ChatCategory.SUPPORT: socket = m_SupportSocket; break ;
                case ChatCategory.DOMINION: socket = m_DominionSocket; break;
                case ChatCategory.COVEN: socket = m_CovenSocket; break;
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

            m_WorldSocket.Emit("send.message", data);
        }


        public static List<ChatMessage> GetMessages(ChatCategory category)
        {
            return m_Messages[category];
        }

        public static bool IsConnected(ChatCategory category)
        {
            switch (category)
            {
                case ChatCategory.COVEN: return m_CovenSocket != null && m_CovenSocket.IsOpen;
                case ChatCategory.DOMINION: return m_DominionSocket != null && m_DominionSocket.IsOpen;
                case ChatCategory.NEWS: return false;
                case ChatCategory.SUPPORT: return m_SupportSocket != null && m_SupportSocket.IsOpen;
                case ChatCategory.WORLD: return m_WorldSocket != null && m_WorldSocket.IsOpen;
                default: return false;
            }
        }

        public static int NewMessagesCount(ChatCategory category)
        {
            return m_NewMessages[category];
        }

        public static void ResetNewMessagesCount(ChatCategory category)
        {
            m_NewMessages[category] = 0;
        }


        private static void OnApplicationQuitting()
        {
            Debug.Log("Disconnecting chat");
            m_SocketManager.Socket.Disconnect();
        }
    }
}
