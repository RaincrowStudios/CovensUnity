using Raincrow.BattleArena.Events;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Controller
{
    public class GameMasterController : AbstractGameMasterController
    {        
        public override IEnumerator<bool?> SendReadyBattle(string battleId)
        {
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/ready/" + battleId, "{}",
               (response, result) =>
               {
                   resultCode = result;
                   responded = true;
               });

            while (!responded)
            {
                yield return null;
            }

            // request came back as 200
            yield return resultCode == 200;
        }

        public override IEnumerator<bool?> SendFlee()
        {
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/flee/", "{}",
               (response, result) =>
               {
                   resultCode = result;
                   responded = true;
               });

            while (!responded)
            {
                yield return null;
            }

            // request came back as 200
            yield return resultCode == 200;
        }

        public override IEnumerator<bool?> SendMove()
        {
            bool responded = false;
            int resultCode = 0;

            APIManager.Instance.Post(
               "battle/move/", "{}",
               (response, result) =>
               {
                   resultCode = result;
                   responded = true;
               });

            while (!responded)
            {
                yield return null;
            }

            // request came back as 200
            yield return resultCode == 200;
        }

        protected override void OnBattleEnd(BattleEndEventArgs response)
        {
            
        }

        protected override void OnTurnStart(TurnStartEventArgs response)
        {
            
        }

        protected override void OnTurnResolution(TurnResolutionEventArgs response)
        {
            
        }
    }
}