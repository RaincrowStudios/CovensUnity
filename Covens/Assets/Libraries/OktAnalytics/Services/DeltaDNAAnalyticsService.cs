using Oktagon.Utils;
using UnityEngine;

namespace Oktagon.Analytics.Services
{
    public class DeltaDNAAnalyticsService : MonoBehaviour, IOktAnalyticsService
    {
        /// <summary>
        /// Returns true if DeltaDNA has been initialized
        /// </summary>
        public bool Initialized => DeltaDNA.DDNA.Instance.HasStarted;
        


        public void Initialize(IOktConfigFileReader configFileReader)
        {
            DeltaDNA.DDNA.Instance.StartSDK();
        }
    }
}