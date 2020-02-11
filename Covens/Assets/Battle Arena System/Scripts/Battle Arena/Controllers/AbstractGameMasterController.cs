using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public abstract class AbstractGameMasterController : MonoBehaviour
    {
        /// <summary>
        ///  Send to server an action to move the player on the grid
        /// </summary>
        /// <param name="battleId"></param>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendMove();

        /// <summary>
        ///  Send to server an action to flee of battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendFlee();

        /// <summary>
        ///  Send to server that the player is ready to the battle
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<bool?> SendReadyBattle(string battleId);
    }
}