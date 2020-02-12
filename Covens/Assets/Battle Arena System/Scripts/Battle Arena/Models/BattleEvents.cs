using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Events
{
    #region Events

    public class PlanningPhaseStartEventHandler : IGameEventHandler
    {
        // PlanningPhaseStartEvent
        private class PlanningPhaseStartEvent : UnityEvent<PlanningPhaseReadyEventArgs> { }
        private static PlanningPhaseStartEvent Response = new PlanningPhaseStartEvent();

        // Properties        
        public string EventName => "battle.turn.start";

        public void HandleResponse(string eventData)
        {
            PlanningPhaseReadyEventArgs data = JsonConvert.DeserializeObject<PlanningPhaseReadyEventArgs>(eventData);
            Response?.Invoke(data);
        }

        public static void AddListener(UnityAction<PlanningPhaseReadyEventArgs> planningPhaseStart)
        {
            if (Response == null)
            {
                Response = new PlanningPhaseStartEvent();
            }
            Response.AddListener(planningPhaseStart);
        }

        public static void RemoveListener(UnityAction<PlanningPhaseReadyEventArgs> planningPhaseStart)
        {            
            Response?.RemoveListener(planningPhaseStart);
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
            TurnResolutionEventArgs data = JsonConvert.DeserializeObject<TurnResolutionEventArgs>(eventData);
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
            BattleEndEventArgs data = JsonConvert.DeserializeObject<BattleEndEventArgs>(eventData);
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

    public struct PlanningPhaseReadyEventArgs
    {
        [JsonProperty("maxActionsAllowed")] private int _maxActionsAllowed;
        [JsonProperty("planningMaxTime")] private float _planningMaxTime;
        [JsonProperty("planningOrder")] private string[] _planningOrder;

        public int MaxActionsAllowed { get => _maxActionsAllowed; }
        public float PlanningMaxTime { get => _planningMaxTime; }
        public string[] PlanningOrder { get => _planningOrder; }
    }

    public struct TurnResolutionEventArgs { }

    public struct BattleEndEventArgs { }

    #endregion
}