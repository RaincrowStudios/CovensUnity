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

        public static bool Initialized { get; private set; }

        //events
        public static event System.Action OnReceiveNews;
        public static event System.Action OnReceiveWorld;
        public static event System.Action OnReceiveCoven;
        public static event System.Action OnReceiveDominion;
        public static event System.Action OnReceiveHelp;
        public static event System.Action<string> OnSocketError;

        public static void InitChat()
        {
            if (Initialized)
            {
                Debug.LogError("Chat already initialized");
                return;
            }

            string chatAddress = CovenConstants.chatAddress;

            Debug.Log("Initializing chat\n" + chatAddress);

            m_SocketManager = new SocketManager(new System.Uri(chatAddress));
            m_SocketManager.Encoder = new JsonDotNetEncoder();
            m_SocketManager.Socket.On(SocketIOEventTypes.Error, OnError);
            m_SocketManager.Socket.On(SocketIOEventTypes.Connect, OnConnect);

            m_SocketManager.Open();
        }

        private static void OnError(Socket socket, Packet packet, object[] args)
        {
            string errorMessage = args[0].ToString();
            Debug.LogError("Chat socket error: " + errorMessage);
            OnSocketError?.Invoke(errorMessage);
        }

        private static void OnConnect(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("Chat connected");

            //set up the worldchat and connect to it
        }
    }
}
