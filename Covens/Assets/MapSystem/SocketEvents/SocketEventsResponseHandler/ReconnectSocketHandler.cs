using UnityEngine;
using System.Collections;
using Raincrow.Maps;

namespace Raincrow.GameEventResponses
{
    public class ReconnectSocketHandler : IGameEventHandler
    {
        public string EventName => "reconnect.socket";

        public void HandleResponse(string eventData)
        {
            SocketClient.Instance.InitiateSocketConnection(true);
        }
    }
}