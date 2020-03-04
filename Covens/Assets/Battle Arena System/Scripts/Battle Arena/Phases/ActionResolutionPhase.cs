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
        private ICellUIModel[,] _gridView = new ICellUIModel[0, 0];
        private IDictionary<string, (IWitchModel, IWitchUIModel)> _witches = new Dictionary<string, (IWitchModel, IWitchUIModel)>(); // holy shit, it works
        private IDictionary<string, (ISpiritModel, ISpiritUIModel)> _spirits = new Dictionary<string, (ISpiritModel, ISpiritUIModel)>(); // holy shit, it works

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter, ICellUIModel[,] gridView, IBattleModel battleModel, ITurnModel turnModel)
        {
            _coroutineStarter = coroutineStarter;
            _gridView = gridView;
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

            // TODO: Show Character Turn Order View
            _startTime = Time.time;
        }

        public IEnumerator Update(IStateMachine stateMachine)
        {
            foreach (var key in _turnModel.ResponseActions)
            {
                string characterId = key.Key;
                foreach (IActionResponseModel responseAction in key.Value)
                {
                    IEnumerator actionRoutine = default;
                    Debug.Log(responseAction.Type);
                    switch (responseAction.Type)
                    {
                        case ActionResponseType.Move:
                            MoveActionResponseModel moveAction = responseAction as MoveActionResponseModel;
                            actionRoutine = Move(characterId, moveAction);
                            break;
                    }

                    yield return _coroutineStarter.Invoke(actionRoutine);
                }
            }
            yield return stateMachine.ChangeState<BanishmentPhase>();
        }

        public IEnumerator Exit(IStateMachine stateMachine)
        {
            _witches.Clear();
            _spirits.Clear();
            yield return null;
        }

        #region Actions

        private IEnumerator Move(string characterId, MoveActionResponseModel moveAction)
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

            // Get transform of our character
            Transform characterTransform = characterUI.Transform;

            // Get transform of our target Cell
            BattleSlot targetPosition = moveAction.Position;
            ICellUIModel targetCellView = _gridView[targetPosition.Row, targetPosition.Col];
            Transform cellTransform = targetCellView.Transform;

            // Remove it
            _battleModel.GridUI.RemoveObjectFromGrid(characterUI, character);

            // Animation

            // Set it
            _battleModel.GridUI.SetObjectToGrid(characterUI, character, targetPosition.Row, targetPosition.Col);

            Debug.LogFormat("Execute Action Move to Slot X:{0} Y:{1}", targetPosition.Row, targetPosition.Col);

            yield return new WaitForSeconds(1f);
        }

        #endregion
    }
}