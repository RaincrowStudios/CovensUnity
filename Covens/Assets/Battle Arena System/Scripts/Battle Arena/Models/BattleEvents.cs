using Raincrow.GameEventResponses;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Events
{
    #region Events

    public class TurnStartEventHandler : IGameEventHandler
    {
        // TurnStartEvent
        private class TurnStartEvent : UnityEvent<TurnStartEventArgs> { }
        private static TurnStartEvent Response = new TurnStartEvent();

        // Properties        
        public string EventName => "battle.turn.start";

        public void HandleResponse(string eventData)
        {
            TurnStartEventArgs data = Newtonsoft.Json.JsonConvert.DeserializeObject<TurnStartEventArgs>(eventData);
            Response?.Invoke(data);
        }

        public static void AddListener(UnityAction<TurnStartEventArgs> turnStartAction)
        {
            if (Response == null)
            {
                Response = new TurnStartEvent();
            }
            Response.AddListener(turnStartAction);
        }

        public static void RemoveListener(UnityAction<TurnStartEventArgs> turnStartAction)
        {            
            Response?.RemoveListener(turnStartAction);
        }
    }

    public class TurnResolutionEventHandler : IGameEventHandler
    {
        private class TurnResolutionEvent : UnityEvent<TurnResolutionEventArgs> { }
        private static TurnResolutionEvent Response = new TurnResolutionEvent();

        // Properties        
        public string EventName => "battle.turn.resolution";     

        public void HandleResponse(string eventData)
        {
            TurnResolutionEventArgs data = Newtonsoft.Json.JsonConvert.DeserializeObject<TurnResolutionEventArgs>(eventData);
            Response?.Invoke(data);
        }

        public static void AddListener(UnityAction<TurnResolutionEventArgs> turnResolutionAction)
        {
            if (Response == null)
            {
                Response = new TurnResolutionEvent();
            }
            Response.AddListener(turnResolutionAction);
        }

        public static void RemoveListener(UnityAction<TurnResolutionEventArgs> turnResolutionAction)
        {
            Response?.RemoveListener(turnResolutionAction);
        }
    }

    public class BattleEndEventHandler : IGameEventHandler
    {
        private class BattleEndEvent : UnityEvent<BattleEndEventArgs> { }
        private static BattleEndEvent Response = new BattleEndEvent();

        // Properties        
        public string EventName => "battle.end";

        public void HandleResponse(string eventData)
        {
            BattleEndEventArgs data = Newtonsoft.Json.JsonConvert.DeserializeObject<BattleEndEventArgs>(eventData);
            Response?.Invoke(data);
        }

        public static void AddListener(UnityAction<BattleEndEventArgs> battleEndAction)
        {
            if (Response == null)
            {
                Response = new BattleEndEvent();
            }
            Response.AddListener(battleEndAction);
        }

        public static void RemoveListener(UnityAction<BattleEndEventArgs> battleEndAction)
        {
            Response?.RemoveListener(battleEndAction);
        }
    }

    #endregion

    #region Responses

    public struct TurnStartEventArgs { }

    public struct TurnResolutionEventArgs { }

    public struct BattleEndEventArgs { }

    #endregion
}