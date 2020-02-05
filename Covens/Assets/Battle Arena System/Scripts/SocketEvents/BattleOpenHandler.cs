using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattleOpenHandler : IGameEventHandler
    {
        public static event System.Action<BattleArenaModel> OnBattleOpen;

        public string EventName => "battle.open";

        public void HandleResponse(string eventData)
        {
            BattleArenaModel battle = JsonConvert.DeserializeObject<BattleArenaModel>(eventData);
            OnBattleOpen?.Invoke(battle);
        }
    }
}