using BestHTTP.SocketIO.JsonEncoders;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Chat
{
    public enum MessageType
    {
        TEXT = 0,
        LOCATION = 1,
        IMAGE = 2
    }

    public class ChatPlayer
    {
        public string name;
        public int avatar;
        public int degree;
        public int level;
        public string id;
    }

    public class ChatMessageData
    {
        public string message;
        public double latitude;
        public double longitude;
        public string language;
        public byte[] image;
    }

    public class ChatMessage
    {
        public MessageType type;
        public ChatMessageData data;
        public ChatPlayer player;
        public long timestamp;
        public bool read;
    }

    public sealed class JsonDotNetEncoder : IJsonEncoder
    {
        public List<object> Decode(string json)
        {
            return JsonConvert.DeserializeObject<List<object>>(json);
        }

        public string Encode(List<object> obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}