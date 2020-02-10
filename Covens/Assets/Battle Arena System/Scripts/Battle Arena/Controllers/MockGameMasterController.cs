using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class MockGameMasterController : AbstractGameMasterController
    {
        public override IEnumerator<bool> SendReadyBattle(string battleId)
        {
            for (float f = 0; f < 3f; f += Time.deltaTime)
            {
                yield return false;
            }

            // request came back as 200
            yield return true;
        }
    }
}