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

        // clientVersion, killedBy (spirit or witch), place (map, pop)
        public static readonly string Death = "death";

        // clientVersion, itemID, quantity
        public static readonly string ItemCollected = "itemCollected";

        // clientVersion, currentScreen, navigateTo
        public static readonly string UiInteraction = "uiInteraction";

        // clientVersion, spiritID, spiritEnergy (when the player found him), knowSpirit (true or false), spiritOwner (player, game)
        public static readonly string SpiritBanished = "spiritBanished";

        // clientVersion, alignment, degree
        public static readonly string DegreeChange = "degreeChange";

        // clientVersion, level, upgradedStats (power or resilience)
        public static readonly string LevelUp = "levelUp";

        // clientVersion, type (explore, gather or spellcraft)
        public static readonly string DailyQuest = "dailyQuest";

        // clientVersion, spiritID, spiritTier, spiritType, energyCost,
        public static readonly string SummonSpirit = "summonSpirit";

        // clientVersion, result (yes, no, never)
        public static readonly string EnjoyTheGamePopUp = "enjoyTheGamePopUp";

        // clientVersion, result (yes, no, never)
        public static readonly string ReviewPopUp = "reviewPopUp";

        public static readonly string HelpcrowPopUp = "helpcrowPopUp";

        public static readonly string PurchaseCurrency = "purchaseCurrency";

        public static readonly string PurchaseIAP = "purchaseIAP";
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
