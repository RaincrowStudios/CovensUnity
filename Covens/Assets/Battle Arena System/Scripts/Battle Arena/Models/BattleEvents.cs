using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Events
{
    #region Event Handlers

    public class PlanningPhaseStartEventHandler : IGameEventHandler
    {
        // PlanningPhaseStartEvent        
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

    #region Events

    public class BattleEndEvent : UnityEvent<BattleEndEventArgs> { }

    public class PlanningPhaseStartEvent : UnityEvent<PlanningPhaseReadyEventArgs> { }

    public class TurnResolutionEvent : UnityEvent<TurnResolutionEventArgs> { }

    #endregion

    #region Responses

    public struct PlanningPhaseReadyEventArgs
    {
        [JsonProperty("maxActionsAllowed")] private int _maxActionsAllowed;
        [JsonProperty("planningMaxTime")] private float _planningMaxTime;
        [JsonProperty("planningOrder")] private string[] _planningOrder;

        public int MaxActionsAllowed { get => _maxActionsAllowed; set => _maxActionsAllowed = value; }
        public float PlanningMaxTime { get => _planningMaxTime; set => _planningMaxTime = value; }
        public string[] PlanningOrder { get => _planningOrder; set => _planningOrder = value; }
    }

    public struct PlanningPhaseFinishedEventArgs
    {

    }

    public struct TurnResolutionEventArgs { }

    public struct BattleEndEventArgs { }

    #endregion
}