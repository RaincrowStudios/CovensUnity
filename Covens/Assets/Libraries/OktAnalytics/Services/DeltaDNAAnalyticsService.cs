using System.Collections;
using System.Collections.Generic;
using DeltaDNA;
using Oktagon.Utils;
using UnityEngine;

namespace Oktagon.Analytics.Services
{
    public class DeltaDNAAnalyticsService : MonoBehaviour, IOktAnalyticsService
    {
        private static readonly string DevEnvKey = "deltaDNAEnvDevKey";
        private static readonly string LiveEnvKey = "deltaDNAEnvLiveKey";
        private static readonly string SelectedEnvKey = "deltaDNASelectedEnvKey";
        private static readonly string CollectUrl = "deltaDNACollectUrl";
        private static readonly string EngageUrl = "deltaDNAEngageUrl";
        private static readonly string HashSecret = "deltaDNAHashSecret";
        private static readonly string UseApplicationVersion = "deltaDNAUseApplicationVersion";

        /// <summary>
        /// Returns true if DeltaDNA has been initialized
        /// </summary>
        public bool Initialized => DDNA.Instance.HasStarted;        


        public void Initialize(IOktConfigFileReader configFileReader)
        {
            Configuration config = new Configuration()
            {
                environmentKeyDev = configFileReader.GetStringValue(DevEnvKey),
                environmentKeyLive = configFileReader.GetStringValue(LiveEnvKey),
                environmentKey = configFileReader.GetIntValue(SelectedEnvKey),
                collectUrl = configFileReader.GetStringValue(CollectUrl),
                engageUrl = configFileReader.GetStringValue(EngageUrl),
                hashSecret = configFileReader.GetStringValue(HashSecret),
                useApplicationVersion = configFileReader.GetBoolValue(UseApplicationVersion)
            };
            Debug.Log(config.environmentKey);
            Debug.Log(config.environmentKeyDev);
            Debug.Log(config.environmentKeyLive);
            Debug.Log(config.engageUrl);
            Debug.Log(config.collectUrl);
            DDNA.Instance.StartSDK(config);
            
            Debug.Log(DDNA.Instance.UserID);
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