using System.Collections;
using System.Collections.Generic;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class BanishmentPhase : IState
    {
        // Variables
        private ICoroutineHandler _coroutineStarter;
        private IBattleModel _battleModel;
        private ITurnModel _turnModel;
        private IBarEventLogView _barEventLogView;
        private IDictionary<string, ICharacterController<IWitchModel, IWitchUIModel>> _witches = new Dictionary<string, ICharacterController<IWitchModel, IWitchUIModel>>();
        private IDictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>> _spirits = new Dictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>>();
        // Properties
        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineHandler coroutineStarter,
                               IBattleModel battleModel,
                               ITurnModel turnModel,
                               IBarEventLogView barEventLogView)
        {
            _coroutineStarter = coroutineStarter;
            _battleModel = battleModel;
            _turnModel = turnModel;
            _barEventLogView = barEventLogView;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _witches.Clear();
            foreach (var view in _battleModel.GridUI.WitchesViews)
            {
                _witches.Add(view.Model.Id, view);
                yield return null;
            }

            _spirits.Clear();
            foreach (var view in _battleModel.GridUI.SpiritsViews)
            {
                _spirits.Add(view.Model.Id, view);
                yield return null;
            }

            yield return null;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {

            foreach (var key in _turnModel.ResponseActions)
            {
                string characterId = key.Key;
                foreach (IActionResponseModel responseAction in key.Value)
                {
                    if (responseAction.IsSuccess)
                    {
                        IEnumerator actionRoutine = default;
                        Debug.Log(responseAction.Type);
                        switch (responseAction.Type)
                        {
                            case ActionResponseType.Flee:
                                FleeActionResponseModel fleeAction = responseAction as FleeActionResponseModel;
                                actionRoutine = Flee(characterId, fleeAction);

                                string logFlee = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                                _barEventLogView.AddLog(logFlee);
                                break;
                            case ActionResponseType.Banish:
                                BanishActionResponseModel banishAction = responseAction as BanishActionResponseModel;
                                actionRoutine = Banish(banishAction.TargetId, banishAction);

                                string logBanish = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                                _barEventLogView.AddLog(logBanish);
                                break;
                        }

                        if (actionRoutine != default)
                        {
                            yield return _coroutineStarter.Invoke(actionRoutine);
                        }
                    }
                }
            }

            yield return stateMachine.ChangeState<InitiativePhase>();
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            _witches.Clear();
            _spirits.Clear();
            yield return null;
        }

        #region Actions

        private IEnumerator Flee(string characterId, FleeActionResponseModel fleeAction)
        {
            ICharacterController characterView = default;
            ICharacterModel character = default;

            if (_witches.TryGetValue(characterId, out var witchView))
            {
                character = witchView.Model;
                characterView = witchView;
            }
            else if (_spirits.TryGetValue(characterId, out var spiritView))
            {
                character = spiritView.Model;
                characterView = spiritView;
            }

            if (character.BattleSlot.HasValue)
            {
                // Animation

                // Remove it
                _battleModel.GridUI.RemoveObjectFromGrid(characterView, character);

                _battleModel.GridUI.RecycleCharacter(characterView.Transform.gameObject);

                Debug.LogFormat("Execute Action Flee Character ID: {0}", character.Id);

                yield return new WaitForSeconds(1f);
            }            
        }

        private IEnumerator Banish(string characterId, BanishActionResponseModel banishAction)
        {
            ICharacterController characterView = default;
            ICharacterModel character = default;
            if (_witches.TryGetValue(characterId, out var witchView))
            {
                character = witchView.Model;
                characterView = witchView;
            }
            else if (_spirits.TryGetValue(characterId, out var spiritView))
            {
                character = spiritView.Model;
                characterView = spiritView;
            }

            // Animation

            // Remove it
            if (character.BattleSlot.HasValue)
            {
                _battleModel.GridUI.RemoveObjectFromGrid(characterView, character);

                _battleModel.GridUI.RecycleCharacter(characterView.Transform.gameObject);

                Debug.LogFormat("Execute Action Flee Character ID: {0}", character.Id);

                yield return new WaitForSeconds(1f);
            }            
        }

        #endregion
    }
}
