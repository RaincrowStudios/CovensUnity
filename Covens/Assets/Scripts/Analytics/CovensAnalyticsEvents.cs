using Oktagon.Analytics;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Analytics
{
    public static class CovensAnalyticsEvents
    {
        public static readonly string GameStarted = "gameStarted";
        public static readonly string GameEnded = "gameEnded";

        // Triggers after an account is created
        public static readonly string NewPlayer = "newPlayer";        
        
    }

    public static class CovensFTFGameSteps
    {
        private static Dictionary<string, object> EventParameters = new Dictionary<string, object>();
        private static readonly string FirstGameSteps = "firstgameSteps";

        public static readonly string AccountCreated = "accountCreated";
        public static readonly string CharacterCreated = "characterCreated";
        public static readonly string SchoolOfWitchcraftAccepted = "schoolOfWitchcraftAccepted";
        public static readonly string SchoolOfWitchcraftRefused = "schoolOfWitchcraftRefused";

        public static void Record(string gameStep)
        {
            string retryCountKey = string.Concat(FirstGameSteps, " ", gameStep);
            int retryCount = 0;

            // if there's already a number of retries registered, add 1
            if (PlayerPrefs.HasKey(retryCountKey))
            {
                retryCount = PlayerPrefs.GetInt(retryCountKey);
                retryCount += 1;
            }
            PlayerPrefs.SetInt(retryCountKey, retryCount);

            EventParameters.Clear();
            EventParameters.Add("Step", gameStep);
            EventParameters.Add("Retries", retryCount);
            OktAnalyticsManager.PushEvent(FirstGameSteps, EventParameters);
        }
    }
}
