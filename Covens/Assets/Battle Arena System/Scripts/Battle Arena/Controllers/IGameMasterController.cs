using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public interface IGameMasterController
    {
        /// <summary>
        ///  Send to server an action to move the player on the grid
        /// </summary>
        /// <param name="battleId"></param>
        /// <returns></returns>
        IEnumerator<bool?> SendMove();

        /// <summary>
        ///  Send to server an action to flee of battle
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool?> SendFlee();

        /// <summary>
        ///  Send to server that the player is ready to the battle
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool?> SendReadyBattle(string battleId);
    }
}