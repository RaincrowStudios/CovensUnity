using System.Collections;
using System.Collections.Generic;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Model;
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
        private IDictionary<string, (IWitchModel, IWitchUIModel)> _witches = new Dictionary<string, (IWitchModel, IWitchUIModel)>(); // holy shit, it works
        private IDictionary<string, (ISpiritModel, ISpiritUIModel)> _spirits = new Dictionary<string, (ISpiritModel, ISpiritUIModel)>(); // holy shit, it works
        // Properties
        public string Name => "Banishment Phase";

        public BanishmentPhase(ICoroutineHandler coroutineStarter,
                               IBattleModel battleModel,
                               ITurnModel turnModel
                               )
        {
            _coroutineStarter = coroutineStarter;
            _battleModel = battleModel;
            _turnModel = turnModel;
        }

        public IEnumerator Enter(IStateMachine stateMachine)
        {
            _witches.Clear();
            foreach (var view in _battleModel.GridUI.WitchesViews)
            {
                _witches.Add(view.Model.Id, (view.Model, view.UIModel));
                yield return null;
            }

            _spirits.Clear();
            foreach (var view in _battleModel.GridUI.SpiritsViews)
            {
                _spirits.Add(view.Model.Id, (view.Model, view.UIModel));
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
                                break;
                            case ActionResponseType.Banish:
                                BanishActionResponseModel banishAction = responseAction as BanishActionResponseModel;
                                actionRoutine = Banish(characterId, banishAction);
                                break;
                        }

                        if(actionRoutine != default)
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
            ICharacterUIModel characterUI = default;
            ICharacterModel character = default;

            if (_witches.TryGetValue(characterId, out (IWitchModel, IWitchUIModel) witchTuple))
            {
                character = witchTuple.Item1;
                characterUI = witchTuple.Item2;
            }
            else if (_spirits.TryGetValue(characterId, out (ISpiritModel, ISpiritUIModel) spiritTuple))
            {
                character = spiritTuple.Item1;
                characterUI = spiritTuple.Item2;
            }

            // Animation

            // Remove it
            _battleModel.GridUI.RemoveObjectFromGrid(characterUI, character);

            _battleModel.GridUI.RecycleCharacter(characterUI.Transform.gameObject);

            Debug.LogFormat("Execute Action Flee Character ID: {0}", character.Id);

            yield return new WaitForSeconds(1f);
        }

        private IEnumerator Banish(string characterId, BanishActionResponseModel banishAction)
        {
            ICharacterUIModel characterUI = default;
            ICharacterModel character = default;

            if (_witches.TryGetValue(banishAction.TargetId, out (IWitchModel, IWitchUIModel) witchTuple))
            {
                character = witchTuple.Item1;
                characterUI = witchTuple.Item2;
            }
            else if (_spirits.TryGetValue(banishAction.TargetId, out (ISpiritModel, ISpiritUIModel) spiritTuple))
            {
                character = spiritTuple.Item1;
                characterUI = spiritTuple.Item2;
            }

            // Animation

            // Remove it
            _battleModel.GridUI.RemoveObjectFromGrid(characterUI, character);

            _battleModel.GridUI.RecycleCharacter(characterUI.Transform.gameObject);

            Debug.LogFormat("Execute Action Flee Character ID: {0}", character.Id);

            yield return new WaitForSeconds(1f);
        }

        #endregion
    }
}
