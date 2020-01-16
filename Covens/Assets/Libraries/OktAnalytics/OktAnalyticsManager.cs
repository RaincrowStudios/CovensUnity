using Oktagon.Utils;
using System.Collections.Generic;
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
               

        public static void Initialize()
        {
            if (_instance == null)
            {
                Debug.Log("[OktAnalyticsManager]: Finding an Analytics Manager instance!");
                _instance = FindObjectOfType<OktAnalyticsManager>();                
            }

            if (_instance != null)
            {
                DontDestroyOnLoad(_instance);

                TextAsset defaultConfigTextAsset = Resources.Load<TextAsset>(DefaultConfigPath);
                IOktConfigFileReader configFileReader = _instance.GetComponent<IOktConfigFileReader>();
                configFileReader.SetConfig(defaultConfigTextAsset.text);

                _instance._analyticsServices.Clear();
                _instance._analyticsServices.AddRange(_instance.GetComponentsInChildren<IOktAnalyticsService>());

                foreach (IOktAnalyticsService analytics in _instance._analyticsServices)
                {
                    analytics.Initialize(configFileReader);
                }
            } 
            else
            {
                Debug.LogWarning("[OktAnalyticsManager]: Analytics Manager does not have an instance!");
            }
        }

        public static bool IsInitialized()
        {
            if (_instance != null)
            {
                foreach (var analyticsService in _instance._analyticsServices)
                {
                    if (analyticsService.Initialized)
                    {
                        return true;
                    }
                }                
            }

            return false;
        }

        public static void PushEvent(string eventName, Dictionary<string, object> eventParams = null)
        {
            if (_instance != null)
            {
                foreach (var analyticsService in _instance._analyticsServices)
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
            else
            {
                Debug.LogWarning("[OktAnalyticsManager]: Analytics Manager does not have an instance!");
            }
        }        
    }
}
