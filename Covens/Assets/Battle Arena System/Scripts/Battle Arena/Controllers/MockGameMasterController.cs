using Raincrow.BattleArena.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class MockGameMasterController : AbstractGameMasterController
    {
        [SerializeField] private float _sendPlanningPhaseReadyMaxTime = 3f;
        [SerializeField] private float _waitForPlanningPhaseReadyMaxTime = 3f;

        public override IEnumerator<bool?> SendPlanningPhaseReady(string battleId)
        {
            for (float f = 0; f < _sendPlanningPhaseReadyMaxTime; f += Time.deltaTime)
            {
                yield return null;
            }

            // request came back as 200
            yield return true;

            StartCoroutine(WaitForPlanningPhaseReadyEvent());
        }

        public override IEnumerator<bool?> SendFlee()
        {
            yield return false;
        }

        public override IEnumerator<bool?> SendMove()
        {
            yield return false;
        }

        private IEnumerator WaitForPlanningPhaseReadyEvent()
        {
            yield return new WaitForSeconds(_waitForPlanningPhaseReadyMaxTime);
            PlanningPhaseReadyEventArgs eventArgs = new PlanningPhaseReadyEventArgs();
            OnPlanningPhaseReadyEvent.Invoke(eventArgs);
        }
    }
}