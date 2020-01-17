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
        private static readonly string FtueGameSteps = "ftueGameSteps";

        public static readonly string AccountCreated = "accountCreated";
        public static readonly string CharacterCreated = "characterCreated";
        public static readonly string SchoolOfWitchcraftAccepted = "schoolOfWitchcraftAccepted";
        public static readonly string SchoolOfWitchcraftRefused = "schoolOfWitchcraftRefused";

        public static void Record(string gameStep)
        {
            EventParameters.Clear();
            EventParameters.Add("step", gameStep);
            EventParameters.Add("clientVersion", Application.version);
            OktAnalyticsManager.PushEvent(FtueGameSteps, EventParameters);
        }
    }

    public static class CovensLoadingSteps
    {
        private static Dictionary<string, object> EventParameters = new Dictionary<string, object>();
        private static readonly string LoadingSteps = "loadingSteps";

        public static readonly string BeginLoading = "beginLoading";
        public static readonly string BeginGameDictionaryDownload = "beginGameDictionaryDownload";
        public static readonly string EndGameDictionaryDownload = "endGameDictionaryDownload";
        public static readonly string BeginStoreDictionaryDownload = "beginStoreDictionaryDownload";
        public static readonly string EndStoreDictionaryDownload = "endStoreDictionaryDownload";
        public static readonly string BeginLocalizationDownload = "beginLocalizationDownload";
        public static readonly string EndLocalizationDownload = "endLocalizationDownload";
        public static readonly string BeginAssetBundlesDownload = "beginAssetBundlesDownload";
        public static readonly string EndAssetBundlesDownload = "endAssetBundlesDownload";     

        public static void Record(string loadingSteps)
        {
            EventParameters.Clear();
            EventParameters.Add("step", loadingSteps);
            EventParameters.Add("clientVersion", Application.version);
            OktAnalyticsManager.PushEvent(LoadingSteps, EventParameters);
        }
    }
}
