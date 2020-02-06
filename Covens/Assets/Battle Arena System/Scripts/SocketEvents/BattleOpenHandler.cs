using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattleOpenHandler : IGameEventHandler
    {
        public static event System.Action<IBattleModel> OnBattleOpen;

        public string EventName => "battle.open";

        public void HandleResponse(string eventData)
        {
            IBattleModel battle = JsonConvert.DeserializeObject<BattleModel>(eventData);
            OnBattleOpen?.Invoke(battle);
        }
    }
}