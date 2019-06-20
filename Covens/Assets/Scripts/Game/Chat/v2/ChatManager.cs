using BestHTTP.SocketIO;
using Newtonsoft.Json;
using Raincrow.Chat.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Chat
{
    public static class ChatManager
    {
        private static UIChat ChatInstance;
        private static SocketManager SocketManager;
        private static Socket WorldSocket;
        private static Socket CovenSocket;
        private static Socket DominionSocket;
        private static Socket SupportSocket;
        private static List<ChatCategory> JoinedChats = new List<ChatCategory>();

        public static ChatPlayer Player { get; private set; }
        private static string CovenId;
        private static string CovenName;
        public static readonly int MaxMessages = 50;

        public static bool Connected { get { return SocketManager != null && SocketManager.Socket != null && SocketManager.Socket.IsOpen; } }        

        private static Dictionary<ChatCategory, int> m_NewMessages = new Dictionary<ChatCategory, int>
        {
            //{ ChatCategory.NONE, 0 },
            { ChatCategory.SUPPORT, 0 },
            { ChatCategory.WORLD, 0 },
            { ChatCategory.COVEN, 0 },
            { ChatCategory.DOMINION, 0 },
            { ChatCategory.NEWS, 0 },
        };
        private static Dictionary<ChatCategory, List<ChatMessage>> m_Messages = new Dictionary<ChatCategory, List<ChatMessage>>
        {
            //{ ChatCategory.NONE, new List<ChatMessage>() },
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
        public static event System.Action<ChatCategory> OnLeaveChatRequested;
        public static event System.Action<ChatCategory> OnLeaveChatSuccess;
        public static event System.Action<string, string> OnEnterCovenChat;

        public static void InitChat(ChatPlayer player, string covenId = null, string covenName = null)
        {
            if (!Connected && SocketManager == null)
            {
                if (ChatInstance == null)
                {
                    ChatInstance = Object.FindObjectOfType<UIChat>();
                }

                Player = player;
                CovenName = covenName;
                CovenId = covenId;

                string chatAddress = CovenConstants.chatAddress;

                Debug.Log("Initializing chat\n" + chatAddress);

                SocketManager = new SocketManager(new System.Uri(chatAddress))
                {
                    Encoder = new JsonDotNetEncoder()
                };
                SocketManager.Socket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.WORLD, a, b, c));
                SocketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);

                SocketManager.Open();
            }           
        }        

        public static void InitCoven(string covenName, string covenId)
        {
            CovenId = covenId;
            CovenName = covenName;

            if (CovenSocket == null)
            {
                Debug.Log("Initializing coven socket");
                CovenSocket = SocketManager["/coven"];
                CovenSocket.On(SocketIOEventTypes.Connect, (a, b, c) => CovenSocket.Emit("join.chat", Player, new { id = covenId, name = covenName }));
                CovenSocket.On(SocketIOEventTypes.Disconnect, (_socket, _packet, _args) =>
                {
                    LeaveChat(ChatCategory.COVEN);
                    CovenSocket = null;
                });
                CovenSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.COVEN, _args));
                CovenSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.COVEN, _args));
                CovenSocket.On("left.success", (_socket, _packet, _args) => 
                    {
                        OnSocketLeaveChat(ChatCategory.COVEN, _args);
                        CovenSocket = null;
                    });
                CovenSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.COVEN, a, b, c));
            }
        }

        public static void InitDominion(string dominion)
        {
            if (DominionSocket == null)
            {
                Debug.Log("Initalizing dominion socket");
                DominionSocket = SocketManager["/dominion"];
                DominionSocket.On(SocketIOEventTypes.Connect, (_socket, _packet, _args) => 
                {
                    DominionSocket.Emit("join.chat", Player, new { id = dominion });
                });
                DominionSocket.On(SocketIOEventTypes.Disconnect, (_socket, _packet, _args) => 
                    {
                        LeaveChat(ChatCategory.DOMINION);
                        DominionSocket = null;
                    });
                DominionSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.DOMINION, _args));
                DominionSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.DOMINION, _args));
                DominionSocket.On("left.success", (_socket, _packet, _args) => 
                    {
                        OnSocketLeaveChat(ChatCategory.DOMINION, _args);
                        DominionSocket = null;
                    });
                DominionSocket.On(SocketIOEventTypes.Error, (_socket, _packet, _args) => OnError(ChatCategory.DOMINION, _socket, _packet, _args));
            }
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
            if (WorldSocket == null)
            {
                WorldSocket = SocketManager["/world"];
                WorldSocket.On(SocketIOEventTypes.Connect, (a, b, c) => WorldSocket.Emit("join.chat", Player));
                WorldSocket.On(SocketIOEventTypes.Disconnect, (_socket, _packet, _args) =>
                {
                    LeaveChat(ChatCategory.WORLD);
                    WorldSocket = null;
                });
                WorldSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.WORLD, _args));
                WorldSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.WORLD, _args));
                WorldSocket.On("left.success", (_socket, _packet, _args) => 
                    {
                        OnSocketLeaveChat(ChatCategory.WORLD, _args);
                        WorldSocket = null;
                    });
                WorldSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.WORLD, a, b, c));
            }            
            //Debug.Log("Joining World chat");
            //WorldSocket.Emit("join.chat", Player);

            //support
            if (SupportSocket == null)
            {
                SupportSocket = SocketManager["/help"];
                SupportSocket.On(SocketIOEventTypes.Connect, (a, b, c) => SupportSocket.Emit("join.chat", Player));
                SupportSocket.On(SocketIOEventTypes.Disconnect, (_socket, _packet, _args) =>
                {
                    LeaveChat(ChatCategory.SUPPORT);
                    SupportSocket = null;
                });
                SupportSocket.On("join.success", (_socket, _packet, _args) => OnSocketJoinChat(ChatCategory.SUPPORT, _args));
                SupportSocket.On("new.message", (_socket, _packet, _args) => OnSocketReceiveMessage(ChatCategory.SUPPORT, _args));
                SupportSocket.On("left.success", (_socket, _packet, _args) => 
                    {
                        OnSocketLeaveChat(ChatCategory.SUPPORT, _args);
                        SupportSocket = null;
                    });
                SupportSocket.On(SocketIOEventTypes.Error, (a, b, c) => OnError(ChatCategory.SUPPORT, a, b, c));
            }

            if (CovenSocket == null)
            {
                if (!string.IsNullOrEmpty(CovenId))
                {
                    InitCoven(CovenName, CovenId);
                }
                TeamManager.OnCovenCreated += OnJoinCoven;
                TeamManager.OnJoinCoven += OnJoinCoven;
                TeamManager.OnLeaveCovenRequested += LeaveCovenChatRequested;
            }            

            if (DominionSocket == null)
            {
                if (!string.IsNullOrEmpty(PlayerDataManager.currentDominion))
                {
                    InitDominion(PlayerDataManager.currentDominion);
                }
                MarkerManagerAPI.OnChangeDominion += OnChangeDominion;
            }            
        }

        private static void OnSocketJoinChat(ChatCategory category, object[] args)
        {
            Debug.Log("Joined " + category + " chat");

            List<ChatMessage> messages = JsonConvert.DeserializeObject<List<ChatMessage>>(args[0].ToString());
            m_Messages[category] = messages;

            JoinChat(category);

            OnConnected?.Invoke(category);
        }

        private static void OnSocketLeaveChat(ChatCategory category, object[] args)
        {
            Debug.Log("Left " + category + " chat");

            LeaveChat(category);

            Socket socket = GetSocket(category);
            socket = null; // disposing socket

            OnLeaveChatSuccess?.Invoke(category);
        }

        private static void OnSocketReceiveMessage(ChatCategory category, object[] args)
        {
            ChatMessage msg = JsonConvert.DeserializeObject<ChatMessage>(args[0].ToString());

            if (!m_Messages[category].Contains(msg))
            {
                if (m_Messages[category].Count >= MaxMessages)
                {
                    m_Messages[category].RemoveAt(0);
                }

                m_Messages[category].Add(msg);
                m_NewMessages[category] += 1;
                OnReceiveMessage(category, msg);
            }  
            else
            {
                Debug.LogWarningFormat("Received Duplicate Message: {0}", msg._id);
            }
        }

        public static void LeaveChatRequested(ChatCategory chatCategory)
        {
            if (IsConnected(chatCategory))
            {
                Socket socket = GetSocket(chatCategory);
                if (socket == null)
                {
                    Debug.LogError("Coven chat not initialized");
                    return;
                }

                socket.Emit("left.chat");
                socket.Disconnect();

                OnLeaveChatRequested?.Invoke(chatCategory);
            }            
        }

        //GAME EVENTS
        private static void LeaveCovenChatRequested()
        {
            LeaveChatRequested(ChatCategory.COVEN);
        }

        private static void OnJoinCoven(string covenId, string covenName)
        {
            if (Connected)
            {
                InitCoven(covenName, covenId);

                OnEnterCovenChat?.Invoke(covenId, covenName);
            }            
        }

        private static void OnChangeDominion(string dominion)
        {
            if (Connected)
            {
                InitDominion(dominion);
            }
        }

        private static Socket GetSocket(ChatCategory category)
        {
            switch (category)
            {
                case ChatCategory.WORLD:
                {
                    return WorldSocket;
                }
                case ChatCategory.SUPPORT:
                {
                    return SupportSocket;
                }
                case ChatCategory.DOMINION:
                {
                    return DominionSocket;
                }
                case ChatCategory.COVEN:
                {
                    return CovenSocket;
                }
                default:
                {
                    return null;
                }
            }
        }

        public static void SendMessage(ChatCategory category, ChatMessage message)
        {
            Socket socket = GetSocket(category);

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

            socket.Emit("send.message", data);
        }



        public static List<ChatMessage> GetMessages(ChatCategory category)
        {
            return m_Messages[category];
        }

        private static void JoinChat(ChatCategory category)
        {
            if (!JoinedChats.Contains(category))
            {
                JoinedChats.Add(category);
            }
        }

        private static void LeaveChat(ChatCategory category)
        {
            if (JoinedChats.Contains(category))
            {
                JoinedChats.Remove(category);
            }
        }

        public static bool HasJoinedChat(ChatCategory category)
        {
            return JoinedChats.Contains(category);
        }

        public static bool IsConnected(ChatCategory category)
        {
            switch (category)
            {
                case ChatCategory.COVEN:
                {
                    return CovenSocket != null && CovenSocket.IsOpen;
                }
                case ChatCategory.DOMINION:
                {
                    return DominionSocket != null && DominionSocket.IsOpen;
                }
                case ChatCategory.NEWS:
                {
                    return false;
                }
                case ChatCategory.SUPPORT:
                {
                    return SupportSocket != null && SupportSocket.IsOpen;
                }
                case ChatCategory.WORLD:
                {
                    return WorldSocket != null && WorldSocket.IsOpen;
                }
                default:
                {
                    return false;
                }
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
            SocketManager.Socket.Disconnect();
        }
    }
}
