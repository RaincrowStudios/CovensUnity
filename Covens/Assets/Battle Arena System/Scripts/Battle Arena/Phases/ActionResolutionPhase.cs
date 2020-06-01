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
        private IAnimationController _animController;
        private IPlayerBadgeView _playerBadgeView;
        private string _playerId;
        private IDictionary<string, ICharacterController<IWitchModel, IWitchUIModel>> _witches =
            new Dictionary<string, ICharacterController<IWitchModel, IWitchUIModel>>(); // holy shit, it works
        private IDictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>> _spirits =
            new Dictionary<string, ICharacterController<ISpiritModel, ISpiritUIModel>>(); // holy shit, it works

        // Properties
        public string Name => "Action Resolution Phase";

        public ActionResolutionPhase(ICoroutineHandler coroutineStarter,
                                     IBattleModel battleModel,
                                     string playerModel,
                                     ITurnModel turnModel,
                                     IBarEventLogView barEventLogView,
                                     IAnimationController animController,
                                     IPlayerBadgeView playerBadgeView)
        {
            _coroutineStarter = coroutineStarter;
            _battleModel = battleModel;
            _playerId = playerModel;
            _turnModel = turnModel;
            _barEventLogView = barEventLogView;
            _animController = animController;
            _playerBadgeView = playerBadgeView;
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
                    IEnumerator actionRoutine = default;
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
                            actionRoutine = Summon(characterId, summonAction);

                            string logSummom = "The <witch>Evil Wind</witch> cast <spell>Arcane Damage</spell> on you. <damage>1624 energy</damage><time>[01:12]</time>";
                            _barEventLogView.AddLog(logSummom);
                            break;

                        case ActionResponseType.Cast:
                            CastActionResponseModel castAction = responseAction as CastActionResponseModel;
                            actionRoutine = Cast(castAction);

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

            if (moveAction.IsSuccess)
            {
                // Get transform of our target Cell
                BattleSlot targetBattleSlot = moveAction.Position;

                // Animation
                yield return _animController.Move(characterView, targetBattleSlot);

                // Set it
                _battleModel.GridUI.SetObjectToGrid(characterView, character, targetBattleSlot.Row, targetBattleSlot.Col);

                Debug.LogFormat("Execute Action Move to Slot X:{0} Y:{1}", targetBattleSlot.Row, targetBattleSlot.Col);
            }
            else
            {
                // show move failed message
                string moveFailed = LocalizeLookUp.GetText("MOVE_FAILED");
                yield return _animController.ShowMessage(characterView, moveFailed);
            }
        }

        private IEnumerator Summon(string characterId, SummonActionResponseModel summonAction)
        {
            if (summonAction.IsSuccess)
            {
                // Get cell transform
                BattleSlot targetBattleSlot = summonAction.Position;
                ISpiritModel spiritModel = summonAction.Spirit;
                ICharacterController targetView = GetCharacterView(characterId);

                // Animation
                yield return _animController.ApplyDamage(targetView, -summonAction.SummoningEnergy, false);
                targetView.AddDamage(-summonAction.SummoningEnergy);

                IEnumerator<ICharacterController> enumerator = _battleModel.GridUI.SpawnObjectOnGrid(spiritModel, targetBattleSlot.Row, targetBattleSlot.Col);
                Coroutine<ICharacterController> spawnObjectOnGrid = _coroutineStarter.Invoke(enumerator);
                yield return spawnObjectOnGrid;

                // Animation
                yield return _animController.Summon(spawnObjectOnGrid.ReturnValue);



                Debug.LogFormat("Execute Summon {0} to Slot X:{1} Y:{2}", spiritModel.Id, targetBattleSlot.Row, targetBattleSlot.Col);
            }
            else
            {
                // show move failed message
                ICharacterController summonerView = GetCharacterView(characterId);
                string summonFailed = LocalizeLookUp.GetText("SUMMON_FAILED");
                yield return _animController.ShowMessage(summonerView, summonFailed);
            }
        }

        private IEnumerator Cast(CastActionResponseModel castAction)
        {
            // Get caster
            ICharacterController casterView = GetCharacterView(castAction.Caster.Id);
            ICharacterController targetView = GetCharacterView(castAction.Target.Id);

            string spellName = LocalizeLookUp.GetSpellName(castAction.Spell);


            yield return _animController.CastSpell(spellName, castAction.School, casterView, targetView);


            if (castAction.IsSuccess)
            {
                if (castAction.Result.EnergyChange != 0)
                {
                    casterView.AddDamage(-castAction.SpellCost);

                    // Animation
                    yield return _animController.ApplyDamage(targetView, castAction.Result.EnergyChange, castAction.Result.IsCritical, castAction.CastBlocked);

                    targetView.AddDamage(castAction.Result.EnergyChange);
                }

                if (castAction.Cooldown > 0)
                {
                    if (casterView.Model.Id == _playerId)
                    {
                        _battleModel.AddCooldown(castAction.Spell, castAction.Cooldown);
                    }
                }

                if (castAction.Result.AppliedEffect != null && castAction.Result.AppliedEffect.ExpiresOnTurn > 0)
                {
                    targetView.Model.AddStatusEffect(castAction.Spell, castAction.Result.AppliedEffect.ExpiresOnTurn);

                }

                if (casterView.Model.Id == _playerId)
                {
                    ICharacterController character = _battleModel.GridUI.GetCharacter(_playerId);
                    if (character != default && character.Model != default)
                    {
                        IList<IStatusEffect> statusEffects = character.Model.StatusEffects;

                        _playerBadgeView.UpdateConditions(statusEffects.Count);
                    }
                }

                if (string.Equals("spell_astral", castAction.Spell))
                {
                    IEnumerator animationEffectSpellAstral;
                    yield return animationEffectSpellAstral = _animController.ActiveEffectSpellAstral(((IWitchModel)casterView.Model).Degree, targetView);

                    IParticleEffectView particleEffectView = (IParticleEffectView)animationEffectSpellAstral.Current;

                    targetView.Model.AddParticleEffect(castAction.Spell, castAction.Result.AppliedEffect.ExpiresOnTurn, particleEffectView);

                }

                //if (castAction.CastBlocked)
                //{
                //    yield return _animController.ShowMessage(targetView, "BLOCKED!");
                //}

                //Debug.LogFormat("Execute Cast to {0} and apply {1} damage", castAction.Target.Id, castAction.Result.EnergyChange);
            }
            else
            {
                // show cast failed message
                string castFailed = LocalizeLookUp.GetText("CAST_FAILED");
                yield return _animController.ShowMessage(targetView, castFailed);
            }

            casterView.UpdateEnergy(castAction.Caster.Energy);
            targetView.UpdateEnergy(castAction.Target.Energy);
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