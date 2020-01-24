using Oktagon.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Analytics
{
    public class CovensSessionTracker : MonoBehaviour
    {
        private bool _sessionStarted = true;

        protected virtual IEnumerator Start()
        {
            if (!OktAnalyticsManager.IsInitialized())
            {
                OktAnalyticsManager.Initialize(CovenConstants.ConfigFilePath);

                // Wait for Analytics Initialization
                yield return new WaitUntil(() => OktAnalyticsManager.IsInitialized());
                StartSession();
                _sessionStarted = true;
            }
        }

        private void StartSession()
        {
            Dictionary<string, object> eventParams = new Dictionary<string, object>
            {
                { "clientVersion", Application.version }
            };

            // How many times do people start the game? How often in a day to people play my game? Do they come back each week? Is my game improving week to week, version to version.  What are my critical drop out points?
            OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.GameStarted, eventParams);

            //Application.quitting += EndSession;
        }

        protected virtual void OnApplicationQuit()
        {
            if (_sessionStarted)
            {
                EndSession();
            }
        }

        private void EndSession()
        {
            int sessionFramerate = Mathf.RoundToInt(Time.frameCount / Time.time);
            int sessionLength = Mathf.RoundToInt(Time.unscaledTime);
            Dictionary<string, object> eventParams = new Dictionary<string, object>
            {
                { "clientVersion",  Application.version }, // version
                { "sessionLength",  sessionLength }, // seconds
                { "sessionFramerate", sessionFramerate }
            };

            // Are player's sessions long or short on average? Do they last enough for multiple matches, or usually only one or less? Do players open the game just to check other elements and then close without playing?
            OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.GameEnded, eventParams);
            Application.quitting -= EndSession;
        }
    }
}