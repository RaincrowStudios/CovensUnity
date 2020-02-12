using Raincrow.BattleArena.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class MockGameMasterController : AbstractGameMasterController
    {
        public override IEnumerator<bool?> SendReadyBattle(string battleId)
        {
            for (float f = 0; f < 3f; f += Time.deltaTime)
            {
                yield return null;
            }

            // request came back as 200
            yield return true;
        }

        public override IEnumerator<bool?> SendFlee()
        {
            yield return false;
        }

        public override IEnumerator<bool?> SendMove()
        {
            yield return false;
        }        

        protected override void OnTurnStart(TurnStartEventArgs response)
        {
            
        }

        protected override void OnTurnResolution(TurnResolutionEventArgs response)
        {
            
        }

        protected override void OnBattleEnd(BattleEndEventArgs response)
        {

        }
    }
}