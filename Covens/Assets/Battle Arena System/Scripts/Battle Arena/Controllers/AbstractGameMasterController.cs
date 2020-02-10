using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public abstract class AbstractGameMasterController : MonoBehaviour
    {
        public abstract IEnumerator<bool> SendReadyBattle(string battleId);
    }
}