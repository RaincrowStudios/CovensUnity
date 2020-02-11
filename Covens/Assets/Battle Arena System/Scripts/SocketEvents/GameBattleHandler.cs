namespace Raincrow.GameEventResponses
{
    public class GameBattleHandler<T> : IGameEventHandler
    {
        private System.Action<T> m_Response;

        public string EventName { set; get; }

        public void HandleResponse(string eventData)
        {
            T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(eventData);
            m_Response?.Invoke(data);
        }

        /// <summary>
        /// Create listener for battle events
        /// </summary>
        /// <param name="EventName">Event name for to listen</param>
        /// <param name="response">Function with a string parameter, recive JSON from the server</param>
        public GameBattleHandler (string EventName, System.Action<T> response)
        {
            this.EventName = EventName;
            this.m_Response = response;

            SocketClient.Instance.AddHandle(this);
        }
    }
}
