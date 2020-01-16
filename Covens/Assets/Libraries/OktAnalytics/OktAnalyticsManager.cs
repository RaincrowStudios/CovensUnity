using Oktagon.Utils;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Oktagon.Analytics
{
    public class OktAnalyticsManager : MonoBehaviour
    {
        /// <summary>
        /// Private Instance of OktAnalyticsManager for easy initialization.
        /// </summary>
        private static OktAnalyticsManager _instance;

        private static readonly string DefaultConfigPath = "DefaultConfigFile";

        /// <summary>
        /// A list with all the analytics services of our game.
        /// </summary>
        private List<IOktAnalyticsService> _analyticsServices = new List<IOktAnalyticsService>();


        /// <summary>
        /// Public access to the OktAnalyticsManager.
        /// </summary>
        public static OktAnalyticsManager GetInstance()
        {
            return _instance;
        }                

        protected virtual void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            _instance = this;

            TextAsset defaultConfigTextAsset = Resources.Load<TextAsset>(DefaultConfigPath);
            IOktConfigFileReader configFileReader = GetComponent<IOktConfigFileReader>();
            configFileReader.SetConfig(defaultConfigTextAsset.text);

            _analyticsServices.Clear();
            _analyticsServices.AddRange(GetComponentsInChildren<IOktAnalyticsService>());

            foreach (IOktAnalyticsService analytics in _analyticsServices)
            {
                analytics.Initialize(configFileReader);
            }
        }

        public void PushEvent(string eventName, Dictionary<string, object> eventParams)
        {
            foreach (var analyticsService in _analyticsServices)
            {
                if (analyticsService.Initialized)
                {
                    analyticsService.PushEvent(eventName, eventParams);
                }                
                else
                {
                    Debug.LogWarningFormat("[OktAnalyticsManager]: Could not push event [{0}] on service [{1}]!", eventName, analyticsService.GetName());
                }
            }
        }
    }
}
