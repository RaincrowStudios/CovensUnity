using Oktagon.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Analytics
{
    public class CovensSessionTracker : MonoBehaviour
    {
        /// <summary>
        /// ID of our session
        /// </summary>
        private string _sessionId;

        protected virtual IEnumerator Start()
        {
            if (!OktAnalyticsManager.IsInitialized())
            {
                OktAnalyticsManager.Initialize();

                // Wait for Analytics Initialization
                yield return new WaitUntil(() => OktAnalyticsManager.IsInitialized());

                StartSession();                
            }
        }

        private void StartSession()
        {
            // Generates a unique session id
            _sessionId = System.Guid.NewGuid().ToString();

            Dictionary<string, object> eventParams = new Dictionary<string, object>
            {
                { "sessionID", _sessionId },
                { "appVersion", DownloadedAssets.AppVersion }
            };

            // How many times do people start the game? How often in a day to people play my game? Do they come back each week? Is my game improving week to week, version to version.  What are my critical drop out points?
            OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.GameStarted, eventParams);

            Application.quitting += EndSession;
        }

        private void EndSession()
        {
            if (!string.IsNullOrWhiteSpace(_sessionId))
            {
                int sessionLength = Mathf.RoundToInt(Time.unscaledTime);
                Dictionary<string, object> eventParams = new Dictionary<string, object>
                {
                    { "sessionID", _sessionId },
                    { "appVersion",  sessionLength } // seconds
                };

                // Are player's sessions long or short on average? Do they last enough for multiple matches, or usually only one or less? Do players open the game just to check other elements and then close without playing?
                OktAnalyticsManager.PushEvent(CovensAnalyticsEvents.GameEnded, eventParams);
                Application.quitting -= EndSession;

                _sessionId = string.Empty;
            }
        }
    }
}