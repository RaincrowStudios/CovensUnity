using Raincrow.BattleArena.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public abstract class AbstractGameMasterController : MonoBehaviour, IGameMasterController
    {
        protected virtual void OnEnable()
        {
            TurnStartEventHandler.AddListener(OnTurnStart);
            TurnResolutionEventHandler.AddListener(OnTurnResolution);
            BattleEndEventHandler.AddListener(OnBattleEnd);
        }

        protected virtual void OnDisable()
        {
            TurnStartEventHandler.RemoveListener(OnTurnStart);
            TurnResolutionEventHandler.RemoveListener(OnTurnResolution);
            BattleEndEventHandler.RemoveListener(OnBattleEnd);
        }

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

        public Coroutine<T> DispatchCoroutine<T>(IEnumerator<T> routine)
        {
            return this.StartCoroutine<T>(routine);
        }

        protected abstract void OnBattleEnd(BattleEndEventArgs response);

        protected abstract void OnTurnStart(TurnStartEventArgs response);

        protected abstract void OnTurnResolution(TurnResolutionEventArgs response);
    }
}