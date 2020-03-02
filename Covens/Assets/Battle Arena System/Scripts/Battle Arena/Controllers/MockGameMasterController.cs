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
            PlanningPhaseFinishedEventArgs planningPhaseFinishedEvent = new PlanningPhaseFinishedEventArgs
            {
                BattleActions = new Dictionary<string, List<IActionResultModel>>()
            };

            //planningPhaseFinishedEvent.BattleActions.Add("", )
            List<IActionResultModel> actionResults = new List<IActionResultModel>();
            foreach (IActionRequestModel actionRequest in actionRequests)
            {
                actionResults.Add(ConvertActionRequestToResult(actionRequest));
            }

            planningPhaseFinishedEvent.BattleActions.Add("witch1", actionResults);

            _onPlanningPhaseFinished.Invoke(planningPhaseFinishedEvent);
        }

        private IActionResultModel ConvertActionRequestToResult(IActionRequestModel actionRequest)
        {
            if (actionRequest.Type == ActionRequestType.Cast)
            {
                CastSpellActionRequestModel castSpellActionRequest = actionRequest as CastSpellActionRequestModel;
                return new CastSpellActionResultModel()
                {
                    SpellId = castSpellActionRequest.SpellId,
                    TargetId = castSpellActionRequest.TargetId
                };
            }

            if (actionRequest.Type == ActionRequestType.Move)
            {
                MoveActionRequestModel moveactionRequest = actionRequest as MoveActionRequestModel;
                return new MoveActionResultModel()
                {
                    Position = moveactionRequest.Position
                };
            }

            if (actionRequest.Type == ActionRequestType.Summon)
            {
                SummonActionRequestModel summonActionRequest = actionRequest as SummonActionRequestModel;
                return new SummonResultActionModel()
                {
                    SpiritId = summonActionRequest.SpiritId,
                    Position = summonActionRequest.Position
                };
            }

            return new FleeActionResultModel();
        }
    }
}