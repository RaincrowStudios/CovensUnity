using System.Collections;
using System.Collections.Generic;
using DeltaDNA;
using Oktagon.Utils;
using UnityEngine;

namespace Oktagon.Analytics.Services
{
    public class DeltaDNAAnalyticsService : MonoBehaviour, IOktAnalyticsService
    {
        /// <summary>
        /// Returns true if DeltaDNA has been initialized
        /// </summary>
        public bool Initialized => DDNA.Instance.HasStarted;        


        public void Initialize(IOktConfigFileReader configFileReader)
        {
            DDNA.Instance.StartSDK();
        }

        public void PushEvent(string eventName, Dictionary<string, object> eventParams = null)
        {            
            if (eventParams != null)
            {
                DDNA.Instance.RecordEvent(eventName, eventParams);
            }
            else
            {
                DDNA.Instance.RecordEvent(eventName);
            }
        }

        public string GetName()
        {
            return "DeltaDNA";
        }
    }
}