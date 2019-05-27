using BestHTTP.SocketIO;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Chat
{
    public static class ChatManager
    {
        private static SocketManager m_SocketManager;
        private static Socket m_WorldSocket;
        //private static Socket 

        //
        public static bool Connected { get { return m_SocketManager != null && m_SocketManager.Socket != null && m_SocketManager.Socket.IsOpen; } }
        public static bool ConnectedToWorld { get { return m_WorldSocket != null && m_WorldSocket.IsOpen; } }

        //events
        public static event System.Action OnReceiveNews;
        public static event System.Action OnReceiveWorld;
        public static event System.Action OnReceiveCoven;
        public static event System.Action OnReceiveDominion;
        public static event System.Action OnReceiveHelp;
        public static event System.Action<string> OnSocketError;
        

        private static ChatPlayer m_ChatPlayer;

        public static void InitChat(ChatPlayer player)
        {
            if (Connected)
            {
                Debug.LogError("Chat already initialized");
                return;
            }

            m_ChatPlayer = player;

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
            m_WorldSocket.On("world.message", OnWorldMessage);
            
            //connect to the world chat
            m_WorldSocket.Emit("join.chat", m_ChatPlayer);
        }

        private static void OnSuccessAll(Socket socket, Packet packet, object[] args)
        {

        }

        //WORLD SOCKET EVENTS
        private static void OnWorldJoin(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Joined world chat");
        }

        private static void OnWorldMessage(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Received world message");
            LogSerialized(args);
        }

        public static void SendWorldMessage(ChatMessage message)
        {
            if (ConnectedToWorld == false)
            {
                Debug.LogError("Not connected to world chat");
                return;
            }

            SendMessage(m_WorldSocket, message);
        }

        private static void SendMessage(Socket socket, ChatMessage message)
        {
            m_WorldSocket.Emit("send.message", message);
        }

        private static void LogSerialized(object obj)
        {
            Debug.Log(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }

        private static void OnApplicationQuitting()
        {
            m_SocketManager.Socket.Disconnect();
        }
    }
}
