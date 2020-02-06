using Newtonsoft.Json;
using Raincrow.BattleArena.Model;

namespace Raincrow.GameEventResponses
{
    public class BattleRefuseHandler : IGameEventHandler
    {
        public static event System.Action OnBattleRefuse;

        public string EventName => "battle.refuse";

        public void HandleResponse(string eventData)
        {
            UIWarning.GetInstance().Show("Battle Challenge!", "Your challange was been refuse", "Continue");
            OnBattleRefuse?.Invoke();
        }
    }
}