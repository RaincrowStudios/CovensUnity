using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Phases
{
    public class ActionResolutionPhase : IState
    {
        // Variables        
        private ICoroutineHandler _coroutineStarter;
        private IBattleModel _battleModel;
        private ITurnModel _turnModel;
        private IBarEventLogView _barEventLogView;
        private IDictionary<string, ICharacterController<IWitchModel, IWitchUIModel>> _witches =
            new Dictionary<string, ICharacterController<IWitchModel, IWitchUIModel>>(); // holy shit, it works
        private IDictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>> _spirits =
            new Dictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>>(); // holy shit, it works

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter,
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
                            case ActionResponseType.Move:
                                MoveActionResponseModel moveAction = responseAction as MoveActionResponseModel;
                                actionRoutine = Move(characterId, moveAction);

                                string logMove = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                                _barEventLogView.AddLog(logMove);
                                break;
                            case ActionResponseType.Summon:
                                SummonActionResponseModel summonAction = responseAction as SummonActionResponseModel;
                                actionRoutine = Summon(summonAction);

                                string logSummom = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                                _barEventLogView.AddLog(logSummom);
                                break;
                            case ActionResponseType.Cast:
                                CastActionResponseModel castAction = responseAction as CastActionResponseModel;
                                actionRoutine = Cast(characterId, castAction);

                                string logCast = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                                _barEventLogView.AddLog(logCast);
                                break;
                        }

                        if (actionRoutine != default)
                        {
                            yield return _coroutineStarter.Invoke(actionRoutine);
                        }
                    }
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
            ICharacterController characterView = default;
            ICharacterModel character = default;
            if (_witches.TryGetValue(characterId, out ICharacterController<IWitchModel, IWitchUIModel> witchView))
            {
                character = witchView.Model;
                characterView = witchView;
            }
            else if (_spirits.TryGetValue(characterId, out ICharacterController<ISpiritModel, ISpiritUIModel> spiritView))
            {
                character = spiritView.Model;
                characterView = spiritView;
            }

            // Get transform of our target Cell
            BattleSlot targetBattleSlot = moveAction.Position;
            ICellUIModel targetCellView = _battleModel.GridUI.Cells[targetBattleSlot.Row, targetBattleSlot.Col];
            Vector3 targetPosition = targetCellView.Transform.position;

            // Animation
            yield return characterView.Move(1f, targetPosition, Easings.Functions.Linear);

            // Set it
            _battleModel.GridUI.SetObjectToGrid(characterView, character, targetBattleSlot.Row, targetBattleSlot.Col);

            Debug.LogFormat("Execute Action Move to Slot X:{0} Y:{1}", targetBattleSlot.Row, targetBattleSlot.Col);
        }

        private IEnumerator Summon(SummonActionResponseModel summonAction)
        {
            // Get cell transform
            BattleSlot targetBattleSlot = summonAction.Position;
            ISpiritModel spiritModel = summonAction.Spirit;

            IEnumerator<ICharacterController> enumerator = _battleModel.GridUI.SpawnObjectOnGrid(spiritModel, targetBattleSlot.Row, targetBattleSlot.Col);
            Coroutine<ICharacterController> spawnObjectOnGrid = _coroutineStarter.Invoke(enumerator);
            yield return spawnObjectOnGrid;

            // Animation
            ICharacterController characterView = spawnObjectOnGrid.ReturnValue;
            yield return characterView.Summon(1f, Easings.Functions.Linear);

            Debug.LogFormat("Execute Summon {0} to Slot X:{1} Y:{2}", spiritModel.Id, targetBattleSlot.Row, targetBattleSlot.Col);
        }

        private IEnumerator Cast(string characterId, CastActionResponseModel castAction)
        {
            // Get caster
            var casterView = GetCharacterView(castAction.Caster.Id);
            var targetView = GetCharacterView(castAction.Target.Id);

            if (castAction.Damage > 0)
            {
                yield return targetView.AddDamage(castAction.Damage);
            }

            Debug.LogFormat("Execute Cast to {0} and apply {1} damage", castAction.Target.Id, castAction.Damage);
            yield return new WaitForSeconds(1f);
        }

        private ICharacterController GetCharacterView(string characterId)
        {
            if (_witches.TryGetValue(characterId, out ICharacterController<IWitchModel, IWitchUIModel> witchView))
            {
                return witchView;
            }
            if (_spirits.TryGetValue(characterId, out ICharacterController<ISpiritModel, ISpiritUIModel> spiritView))
            {
                return spiritView;
            }
            return default;
        }

        #endregion
    }
}