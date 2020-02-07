using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattlePlanningHandler : IGameEventHandler
    {
        public static event System.Action OnBattlePlanning;

        public string EventName => "battle.planning";

        public void HandleResponse(string eventData)
        {
            //TurnModel turnModel = JsonConvert.DeserializeObject<TurnModel>(eventData);

            OnBattlePlanning?.Invoke();
        }
    }
}