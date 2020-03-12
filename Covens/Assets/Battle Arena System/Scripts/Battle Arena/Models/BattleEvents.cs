using Newtonsoft.Json;
using Raincrow.BattleArena.Model;
using Raincrow.GameEventResponses;
using System.Collections.Generic;
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

    public class PlanningPhaseFinishedEventHandler : IGameEventHandler
    {
        // PlanningPhaseFinishedEvent
        private static PlanningPhaseFinishedEvent Response = new PlanningPhaseFinishedEvent();

        // Properties        
        public string EventName => "battle.turn.resolution";     

        public void HandleResponse(string eventData)
        {
            PlanningPhaseFinishedEventArgs data = JsonConvert.DeserializeObject<PlanningPhaseFinishedEventArgs>(eventData);
            Response?.Invoke(data);
        }

        public static void AddListener(UnityAction<PlanningPhaseFinishedEventArgs> finishPlanningPhaseAction)
        {
            if (Response == null)
            {
                Response = new PlanningPhaseFinishedEvent();
            }
            Response.AddListener(finishPlanningPhaseAction);
        }

        public static void RemoveListener(UnityAction<PlanningPhaseFinishedEventArgs> finishPlanningPhaseAction)
        {
            Response?.RemoveListener(finishPlanningPhaseAction);
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

    public class PlanningPhaseFinishedEvent : UnityEvent<PlanningPhaseFinishedEventArgs> { }

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
        [JsonProperty("participants")]
        public List<BattleActor> Actors { get; set; }
    }

    public struct BattleEndEventArgs : IBattleResultModel
    {
        [JsonProperty("reward")] private BattleRewardModel _reward;
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("ranking")] public BattleRankingModel[] Ranking { get; set; }

        [JsonIgnore] public IBattleRewardModel Reward { get => _reward; set => _reward = value as BattleRewardModel; }
    }
    #endregion
}