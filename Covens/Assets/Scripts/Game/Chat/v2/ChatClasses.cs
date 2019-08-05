namespace Raincrow.Chat
{
    public enum MessageType
    {
        TEXT = 0,
        LOCATION = 1,
        IMAGE = 2
    }

    public enum ChatCategory
    {
        NONE = 0,
        NEWS,
        WORLD,
        COVEN,
        DOMINION,
        SUPPORT,
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

    public class ChatCoven
    {
        public string instance { get; set; }
        public string name { get; set; }
        public string dominion { get; set; }
        public int members { get; set; }
        public int worldRank { get; set; }
        public int dominionRank { get; set; }
        public int xp { get; set; }
        public int alignment { get; set; }
        public int level { get; set; }
        public string founder { get; set; }
    }

    [System.Serializable]
    public class ChatMessage
    {
        public ChatMessage()
        {
            data = new ChatMessageData();
            player = new ChatPlayer();
        }

        public string _id;
        public MessageType type;
        public ChatMessageData data;
        public ChatPlayer player;
        public long timestamp;
        public bool read;

        public override bool Equals(object obj)
        {
            if (obj is ChatMessage chatMessage)
            {
                return _id == chatMessage._id;
            }
            return false;
        }
    }    
}