using Raincrow.BattleArena.Events;
using Raincrow.BattleArena.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Controller
{
    public class MockGameMasterController : AbstractGameMasterController
    {
        // Serialized Variables
        [SerializeField] private float _sendPlanningPhaseReadyMaxTime = 3f;
        [SerializeField] private float _waitForPlanningPhaseReadyMaxTime = 3f;
        [SerializeField] private float _sendFinishPlanningPhaseMaxTime = 3f;
        [SerializeField] private float _waitForActionResolutionReadyMaxTime = 3f;

        // Variables
        private UnityAction<PlanningPhaseReadyEventArgs> _onPlanningPhaseReady;
        private UnityAction<PlanningPhaseFinishedEventArgs> _onPlanningPhaseFinished;
        private UnityAction<BattleEndEventArgs> _onBattleEnd;
        private IGridUIModel _gridUIModel;

        protected virtual void OnEnable()
        {
            if (_gridUIModel == null)
            {
                _gridUIModel = GetComponent<IGridUIModel>();
            }
        }

        public override IEnumerator<bool?> SendPlanningPhaseReady(string battleId, string playerId, UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady, UnityAction<BattleEndEventArgs> onBattleEnd)
        {
            _onPlanningPhaseReady = onPlanningPhaseReady;
            _onBattleEnd = onBattleEnd;

            for (float f = 0; f < _sendPlanningPhaseReadyMaxTime; f += Time.deltaTime)
            {
                yield return null;
            }

            // request came back as 200
            yield return true;

            StartCoroutine(WaitForPlanningPhaseReadyEvent(playerId));
        }

        public override IEnumerator<bool?> SendFinishPlanningPhase(string battleId, IActionRequestModel[] actionRequests, UnityAction<PlanningPhaseFinishedEventArgs> onPlanningPhaseFinished)
        {
            _onPlanningPhaseFinished = onPlanningPhaseFinished;

            for (float f = 0; f < _sendFinishPlanningPhaseMaxTime; f += Time.deltaTime)
            {
                yield return null;
            }

            // request came back as 200
            yield return true;
            StartCoroutine(WaitForActionResolutionReadyEvent(actionRequests));
        }

        private IEnumerator WaitForPlanningPhaseReadyEvent(string playerId)
        {
            yield return new WaitForSeconds(_waitForPlanningPhaseReadyMaxTime);           

            bool isPlayerAlive = false;            
            if (_gridUIModel.WitchesViews.Count > 0)
            {
                foreach (var view in _gridUIModel.WitchesViews)
                {
                    if (view.Model.Id == playerId && view.Model.Energy > 0)
                    {
                        isPlayerAlive = true;
                        break;
                    }
                }
            }

            bool isWildSpiritAlive = false;
            if (_gridUIModel.SpiritsViews.Count > 0)
            {
                foreach (var view in _gridUIModel.SpiritsViews)
                {
                    bool isWildSpirit = string.IsNullOrWhiteSpace(view.Model.OwnerId);
                    if (isWildSpirit && view.Model.Energy > 0)
                    {
                        isWildSpiritAlive = true;
                        break;
                    }
                }
            }

            if (!isPlayerAlive || !isWildSpiritAlive)
            {
                BattleEndEventArgs battleEndEventArgs = new BattleEndEventArgs()
                {
                    Type = BattleResultType.PlayerWins,
                    Ranking = new string[2] { "witch1", "spirit2" }
                };
                _onBattleEnd.Invoke(battleEndEventArgs);
            }
            else
            {
                PlanningPhaseReadyEventArgs planningPhaseReadyEvent = new PlanningPhaseReadyEventArgs()
                {
                    MaxActionsAllowed = 3,
                    PlanningMaxTime = 30f,
                    PlanningOrder = new string[3] { "witch1", "spirit1", "spirit2" }
                };
                _onPlanningPhaseReady.Invoke(planningPhaseReadyEvent);
            }
        }

        private IEnumerator WaitForActionResolutionReadyEvent(IActionRequestModel[] actionRequests)
        {
            yield return new WaitForSeconds(_waitForActionResolutionReadyMaxTime);

            string characterId = "witch1";

            List<BattleAction> actionResults = new List<BattleAction>();
            foreach (ActionRequestModel actionRequest in actionRequests)
            {
                actionResults.Add(ConvertActionRequestToResult(characterId, actionRequest));
            }

            BattleActor actor = new BattleActor()
            {
                Id = characterId,
                Actions = new List<BattleAction>(actionResults)
            };

            PlanningPhaseFinishedEventArgs planningPhaseFinishedEvent = new PlanningPhaseFinishedEventArgs
            {
                Actors = new List<BattleActor> { actor }
            };

            _onPlanningPhaseFinished.Invoke(planningPhaseFinishedEvent);

            //string json = @"{
            //      participants: [
            //        {
            //          _id: 'witch1',
            //          actionResolution: [
            //            {
            //              action: { type: 'move', position: { row: 1, col: 1 } },
            //              result: [
            //                {
            //                  event: 'battle.move',
            //                  isSuccess: true,
            //                  position: { row: 1, col: 1, id: 'witch1' }
            //                }
            //              ]
            //            },
            //            {
            //              action: {
            //                type: 'cast',
            //                spellId: 'spell_hex',
            //                targetId: 'spirit1',
            //                ingredients: []
            //              },
            //              result: [
            //                {
            //                  event: 'battle.cast',
            //                  isSuccess: true,
            //                    target: {
            //                      name: 'spirit_moonSnake',
            //                      _id: 'spirit1',
            //                      energy: 874,
            //                      type: 'spirit'
            //                    },
            //                    caster: {
            //                      name: 'SHADOW THE HEDGEHOG',
            //                      _id: 'witch1',
            //                      energy: 60000,
            //                      type: 'character'
            //                    },
            //                    spell: 'spell_hex',
            //                    cooldown: 1583236676382,
            //                    result: {
            //                      damage: -76,
            //                      isCritical: false,
            //                      appliedEffect: {
            //                        buff: false,
            //                        modifiers: { beCrit: '#1*caster:power' },
            //                        stackable: 3,
            //                        duration: '60*caster:level/1',
            //                        expiresOn: 0
            //                      }
            //                    }
            //                }
            //              ]
            //            }
            //          ]
            //        },
            //        {
            //          _id: 'spirit1',
            //          actionResolution: [
            //            {
            //              action: {
            //                type: 'cast',
            //                spellId: 'attack',
            //                targetId: 'witch1'
            //              },
            //              result: [
            //                {
            //                  event: 'battle.cast',
            //                  isSuccess: true,
            //                    target: {
            //                      name: 'SHADOW THE HEDGEHOG',
            //                      _id: 'witch1',
            //                      energy: 59873,
            //                      type: 'character'
            //                    },
            //                    caster: {
            //                      name: 'spirit_moonSnake',
            //                      _id: 'spirit1',
            //                      energy: 874,
            //                      type: 'spirit'
            //                    },
            //                    spell: 'attack',
            //                    cooldown: 1583236674883,
            //                    result: {
            //                      damage: -127
            //                    }
            //                }
            //              ]
            //            }
            //          ]
            //        }
            //      ],
            //      timestamp: 1583236674922
            //    }";
            //PlanningPhaseFinishedEventArgs planningPhaseFinishedEvent = JsonConvert.DeserializeObject<PlanningPhaseFinishedEventArgs>(json);      
        }

        private BattleAction ConvertActionRequestToResult(string characterId, ActionRequestModel actionRequest)
        {
            BattleAction battleAction = new BattleAction()
            {
                Request = actionRequest,
                Results = new List<ActionResponseModel>()
            };

            if (actionRequest.Type == ActionRequestType.Cast)
            {
                CastActionRequestModel castSpellActionRequest = actionRequest as CastActionRequestModel;

                GenericCharacterObjectServer target = new GenericCharacterObjectServer()
                {
                    Id = castSpellActionRequest.TargetId
                };

                GenericCharacterObjectServer caster = new GenericCharacterObjectServer()
                {
                    Id = characterId
                };

                int damage = Random.Range(40, 80);

                battleAction.Results.Add(new CastActionResponseModel()
                {
                    IsSuccess = true,
                    Target = target,
                    Caster = caster,
                    Spell = castSpellActionRequest.SpellId,
                    Damage = damage
                }); ;

                if (target.Energy <= damage)
                {
                    battleAction.Results.Add(new BanishActionResponseModel()
                    {
                        IsSuccess = true,
                        TargetId = target.Id
                    });
                }

                return battleAction;
            }

            if (actionRequest.Type == ActionRequestType.Move)
            {
                MoveActionRequestModel moveactionRequest = actionRequest as MoveActionRequestModel;

                battleAction.Results.Add(new MoveActionResponseModel()
                {
                    IsSuccess = true,
                    Position = moveactionRequest.Position
                });
                return battleAction;
            }

            if (actionRequest.Type == ActionRequestType.Flee)
            {
                battleAction.Results.Add(new FleeActionResponseModel()
                {
                    IsSuccess = true
                });
                return battleAction;
            }

            if (actionRequest.Type == ActionRequestType.Summon)
            {
                SummonActionRequestModel summonActionRequest = actionRequest as SummonActionRequestModel;

                GenericCharacterObjectServer spiritModel = new GenericCharacterObjectServer()
                {
                    ObjectType = ObjectType.Spirit,
                    Id = System.Guid.NewGuid().ToString(),
                    Texture = "spirit_moonSnake",
                    BaseEnergy = 200,
                    Energy = 80
                };

                battleAction.Results.Add(new SummonActionResponseModel()
                {
                    IsSuccess = true,
                    Spirit = spiritModel,
                    Position = summonActionRequest.Position
                });
                return battleAction;
            }

            battleAction.Results.Add(new FleeActionResponseModel());
            return battleAction;
        }
    }
}