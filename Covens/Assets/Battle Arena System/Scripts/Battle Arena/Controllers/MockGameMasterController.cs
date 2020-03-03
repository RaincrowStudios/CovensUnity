using Newtonsoft.Json;
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

        public override IEnumerator<bool?> SendPlanningPhaseReady(string battleId, UnityAction<PlanningPhaseReadyEventArgs> onPlanningPhaseReady)
        {
            _onPlanningPhaseReady = onPlanningPhaseReady;

            for (float f = 0; f < _sendPlanningPhaseReadyMaxTime; f += Time.deltaTime)
            {
                yield return null;
            }

            // request came back as 200
            yield return true;

            StartCoroutine(WaitForPlanningPhaseReadyEvent());
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

        private IEnumerator WaitForPlanningPhaseReadyEvent()
        {
            yield return new WaitForSeconds(_waitForPlanningPhaseReadyMaxTime);
            PlanningPhaseReadyEventArgs planningPhaseReadyEvent = new PlanningPhaseReadyEventArgs()
            {
                MaxActionsAllowed = 3,
                PlanningMaxTime = 30f,
                PlanningOrder = new string[2] { "witch1", "spirit1" }
            };

            _onPlanningPhaseReady.Invoke(planningPhaseReadyEvent);
        }

        private IEnumerator WaitForActionResolutionReadyEvent(IActionRequestModel[] actionRequests)
        {
            yield return new WaitForSeconds(_waitForActionResolutionReadyMaxTime);

            string json = @"{
                  participants: [
                    {
                      _id: '5dcf04c1a473310013cd6321',
                      actionResolution: [
                        {
                          action: { type: 'move', position: { row: 1, col: 1 } },
                          result: [
                            {
                              event: 'battle.move',
                              isSuccess: true,
                              position: { row: 1, col: 1, id: '5dcf04c1a473310013cd6321' }
                            }
                          ]
                        },
                        {
                          action: {
                            type: 'cast',
                            spellId: 'spell_hex',
                            targetId: '5dbe1d2ef3acb30013327d86',
                            ingredients: []
                          },
                          result: [
                            {
                              event: 'battle.cast',
                              isSuccess: true,
                                target: {
                                  name: 'spirit_whiteLady',
                                  _id: '5dbe1d2ef3acb30013327d86',
                                  energy: 874,
                                  type: 'spirit'
                                },
                                caster: {
                                  name: 'Pablo Vittar',
                                  _id: '5dcf04c1a473310013cd6321',
                                  energy: 60000,
                                  type: 'character'
                                },
                                spell: 'spell_hex',
                                cooldown: 1583236676382,
                                result: {
                                  damage: -76,
                                  isCritical: false,
                                  appliedEffect: {
                                    buff: false,
                                    modifiers: { beCrit: '#1*caster:power' },
                                    stackable: 3,
                                    duration: '60*caster:level/1',
                                    expiresOn: 0
                                  }
                                }
                            }
                          ]
                        }
                      ]
                    },
                    {
                      _id: '5dbe1d2ef3acb30013327d86',
                      actionResolution: [
                        {
                          action: {
                            type: 'cast',
                            spellId: 'attack',
                            targetId: '5dcf04c1a473310013cd6321'
                          },
                          result: [
                            {
                              event: 'battle.cast',
                              isSuccess: true,
                                target: {
                                  name: 'Pablo Vittar',
                                  _id: '5dcf04c1a473310013cd6321',
                                  energy: 59873,
                                  type: 'character'
                                },
                                caster: {
                                  name: 'spirit_whiteLady',
                                  _id: '5dbe1d2ef3acb30013327d86',
                                  energy: 874,
                                  type: 'spirit'
                                },
                                spell: 'attack',
                                cooldown: 1583236674883,
                                result: {
                                  damage: -127
                                }
                            }
                          ]
                        }
                      ]
                    }
                  ],
                  timestamp: 1583236674922
                }";

            PlanningPhaseFinishedEventArgs planningPhaseFinishedEvent = JsonConvert.DeserializeObject<PlanningPhaseFinishedEventArgs>(json);
            _onPlanningPhaseFinished.Invoke(planningPhaseFinishedEvent);
        }
    }
}