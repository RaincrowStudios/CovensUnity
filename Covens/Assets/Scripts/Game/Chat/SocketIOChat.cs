using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.SocketIO;
using UnityEngine;

public class SocketIOChat : MonoBehaviour
{

    private SocketManager Manager;
    // Use this for initialization
    void Start()
    {
        Manager = new SocketManager(new Uri("http://127.0.0.1:8083/socket.io/"));
        Manager.Socket.On(SocketIOEventTypes.Error, (socket, packet, args) => Debug.LogError(string.Format("Error: {0}", args[0].ToString())));
        Manager.Open();

        Socket worldChat = Manager["/world"];
        worldChat = Manager["/news"];
        //   worldChat.On(SocketIOEventTypes.Connect, (socket, packet, args) => { Debug.Log("connected"); });
        worldChat.On("event", (s, p, a) =>
        {
            Debug.Log("afd");
        });
    }

}
