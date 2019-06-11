using System.Collections.Generic;


namespace Prime31.MessageKitLite
{
    public interface IMessageReceiver
    {
        void onMessageReceived(int messageType);
    }

    public interface IMessageReceiver<T>
    {
        void onMessageReceived(int messageType, T message);
    }

    public interface IMessageReceiver<T, U>
    {
        void onMessageReceived(int messageType, T message, U other);
    }


    /// <summary>
    /// this is a slightly different take dealing with message receivers. Instead of Actions like MessageKit uses receivers must implement
    /// the appropriate MessageReceiver interface. The main advantage to this way of doing things is something that really only matters on a
    /// mobile/resource constrained game with a SHIT-TON of message handlers being added/removed at runtime. Getting rid of the Action means
    /// there is no allocation when adding a receiver.
    /// 
    /// Downsides are having to implement interfaces for each different message type. When working with Actions you get to pass in named methods
    /// which is very handy for code readability. With interfaces you are stuck with a generic name which isn't fabulous.
    /// </summary>
    public static class MessageKitLite
    {
        private static Dictionary<int, List<IMessageReceiver>> _messageTable = new Dictionary<int, List<IMessageReceiver>>();


        public static void AddObserver(int messageType, IMessageReceiver handler)
        {
            List<IMessageReceiver> list = null;
            if (!_messageTable.TryGetValue(messageType, out list))
            {
                list = new List<IMessageReceiver>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
            {
                _messageTable[messageType].Add(handler);
            }
        }


        public static void RemoveObserver(int messageType, IMessageReceiver handler)
        {
            List<IMessageReceiver> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                if (list.Contains(handler))
                {
                    list.Remove(handler);
                }
            }
        }


        public static void Post(int messageType)
        {
            List<IMessageReceiver> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    list[i].onMessageReceived(messageType);
                }
            }
        }


        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
            {
                _messageTable.Remove(messageType);
            }
        }


        public static void clearMessageTable()
        {
            _messageTable.Clear();
        }
    }


    public static class MessageKitLite<U>
    {
        private static Dictionary<int, List<IMessageReceiver<U>>> _messageTable = new Dictionary<int, List<IMessageReceiver<U>>>();


        public static void AddObserver(int messageType, IMessageReceiver<U> handler)
        {
            List<IMessageReceiver<U>> list = null;
            if (!_messageTable.TryGetValue(messageType, out list))
            {
                list = new List<IMessageReceiver<U>>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
            {
                _messageTable[messageType].Add(handler);
            }
        }


        public static void RemoveObserver(int messageType, IMessageReceiver<U> handler)
        {
            List<IMessageReceiver<U>> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                if (list.Contains(handler))
                {
                    list.Remove(handler);
                }
            }
        }


        public static void Post(int messageType, U param)
        {
            List<IMessageReceiver<U>> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    list[i].onMessageReceived(messageType, param);
                }
            }
        }


        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
                _messageTable.Remove(messageType);
        }


        public static void ClearMessageTable()
        {
            _messageTable.Clear();
        }
    }


    public static class MessageKitLite<U, V>
    {
        private static Dictionary<int, List<IMessageReceiver<U, V>>> _messageTable = new Dictionary<int, List<IMessageReceiver<U, V>>>();


        public static void AddObserver(int messageType, IMessageReceiver<U, V> handler)
        {
            List<IMessageReceiver<U, V>> list = null;
            if (!_messageTable.TryGetValue(messageType, out list))
            {
                list = new List<IMessageReceiver<U, V>>();
                _messageTable.Add(messageType, list);
            }

            if (!list.Contains(handler))
            {
                _messageTable[messageType].Add(handler);
            }
        }


        public static void RemoveObserver(int messageType, IMessageReceiver<U, V> handler)
        {
            List<IMessageReceiver<U, V>> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                if (list.Contains(handler))
                    list.Remove(handler);
            }
        }


        public static void Post(int messageType, U firstParam, V secondParam)
        {
            List<IMessageReceiver<U, V>> list = null;
            if (_messageTable.TryGetValue(messageType, out list))
            {
                for (var i = list.Count - 1; i >= 0; i--)
                    list[i].onMessageReceived(messageType, firstParam, secondParam);
            }
        }


        public static void ClearMessageTable(int messageType)
        {
            if (_messageTable.ContainsKey(messageType))
            {
                _messageTable.Remove(messageType);
            }
        }


        public static void ClearMessageTable()
        {
            _messageTable.Clear();
        }
    }

}