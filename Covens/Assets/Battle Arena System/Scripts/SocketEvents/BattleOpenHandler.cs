using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattleOpenHandler : IGameEventHandler
    {
        public static event System.Action<BattleModel> OnBattleOpen;

        public string EventName => "battle.open";

        public void HandleResponse(string eventData)
        {
            BattleModel battle = JsonConvert.DeserializeObject<BattleModel>(eventData);
            OnBattleOpen?.Invoke(battle);
        }
    }
}