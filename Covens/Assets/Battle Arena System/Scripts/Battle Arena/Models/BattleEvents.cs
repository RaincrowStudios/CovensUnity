using Raincrow.GameEventResponses;

namespace Raincrow.BattleArena.Events
{
    public class TurnStartEvent : IGameEventHandler
    {
        // Variables
        private System.Action<TurnStartResponse> _response;

        // Properties
        public string EventName => "battle.turn.start";

        public struct TurnStartResponse { }

        public TurnStartEvent(System.Action<TurnStartResponse> response)
        {
            _response = response;
        }

        public void HandleResponse(string eventData)
        {
            TurnStartResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<TurnStartResponse>(eventData);
            _response?.Invoke(data);
        }
    }

    public class TurnResolutionEvent : IGameEventHandler
    {
        // Variables
        private System.Action<TurnResolutionResponse> _response;

        // Properties
        public string EventName => "battle.turn.resolution";

        public struct TurnResolutionResponse { }

        public TurnResolutionEvent(System.Action<TurnResolutionResponse> response)
        {
            _response = response;
        }

        public void HandleResponse(string eventData)
        {
            TurnResolutionResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<TurnResolutionResponse>(eventData);
            _response?.Invoke(data);
        }
    }

    public class BattleEndEvent : IGameEventHandler
    {
        // Variables
        private System.Action<BattleEndResponse> _response;

        // Properties
        public string EventName => "battle.end";

        public struct BattleEndResponse { }

        public BattleEndEvent(System.Action<BattleEndResponse> response)
        {
            _response = response;
        }

        public void HandleResponse(string eventData)
        {
            BattleEndResponse data = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleEndResponse>(eventData);
            _response?.Invoke(data);
        }
    }
}