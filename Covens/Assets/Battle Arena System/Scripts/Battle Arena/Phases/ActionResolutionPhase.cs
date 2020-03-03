using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Model;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class ActionResolutionPhase : IState
    {
        // Variables
        private float _startTime = 0f;
        private ICoroutineHandler _coroutineStarter;
        private IBattleModel _battleModel;
        private ITurnModel _turnModel;
        private IList<IWitchUIModel> _uiWitches = new List<IWitchUIModel>();
        private IList<ISpiritUIModel> _uiSpirits = new List<ISpiritUIModel>();

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter, IBattleModel battleModel, ITurnModel turnModel)
        {
            _coroutineStarter = coroutineStarter;
            _battleModel = battleModel;
            _turnModel = turnModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            // Show Character Turn Order View
            _uiWitches.Clear();
            foreach (var view in _battleModel.WitchesViews)
            {
                _uiWitches.Add(view.UIModel);
                yield return null;
            }

            _uiSpirits.Clear();
            foreach (var view in _battleModel.SpiritsViews)
            {
                _uiSpirits.Add(view.UIModel);
                yield return null;
            }

            _startTime = Time.time;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            foreach (var key in _turnModel.ResponseActions)
            {
                foreach (var responseAction in key.Value)
                {
                    Debug.Log(responseAction.Type);
                    yield return new WaitForSeconds(1f);
                }
            }

            yield return stateMachine.ChangeState<BanishmentPhase>();
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            yield return null;
        }
    }
}