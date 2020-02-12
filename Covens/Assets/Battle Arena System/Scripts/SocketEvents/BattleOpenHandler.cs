using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattleOpenHandler : IGameEventHandler
    {
        public static event System.Action<BattleObjectServer> OnBattleOpen;

        public string EventName => "battle.open";

        public void HandleResponse(string eventData)
        {
            BattleObjectServer battle = JsonConvert.DeserializeObject<BattleObjectServer>(eventData);
            OnBattleOpen?.Invoke(battle);
        }
    }
}