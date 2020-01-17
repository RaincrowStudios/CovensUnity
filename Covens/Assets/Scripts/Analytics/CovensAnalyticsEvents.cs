namespace Raincrow.Analytics
{
    public static class CovensAnalyticsEvents
    {
        public static readonly string GameStarted = "gameStarted";
        public static readonly string GameEnded = "gameEnded";

        // Triggers after an account is created
        public static readonly string NewPlayer = "newPlayer";

        // GameSteps
        public static readonly string FirstGameSteps = "firstgameSteps";
    }

    public static class CovensAnalyticsGameSteps
    {
        public static readonly string BeginLoading = "beginLoading"; // [done]
        public static readonly string EndLoading = "endLoading"; // [done]
        public static readonly string LoginSuccess = "loginSuccess"; // [done]
        public static readonly string TouchedBarghest = "touchedBarghest";
        public static readonly string OpenedSpellbook = "openedSpellbook";
        public static readonly string SelectedSkill = "selectedSkill";
        public static readonly string ClickedTowerOfPower = "enteredTowerOfPower";
        public static readonly string ClickedPlaceOfPower = "clickedPlaceOfPower";
        public static readonly string ClickedMadameFortuna = "clickedMadameFortuna";
        public static readonly string ClickedIngredients = "clickedIngredients";
        public static readonly string ClickedWhiteIngredients = "clickedWhiteIngredients";
        public static readonly string ClaimFirstGift = "claimFirstGift";
        public static readonly string AcceptFirstGift = "acceptFirstGift";
        public static readonly string EndFTUE = "endFTUE";
        public static readonly string SchoolOfWitchcraftAccepted = "schoolOfWitchcraftAccepted";
        public static readonly string SchoolOfWitchcraftRefused = "schoolOfWitchcraftRefused";        
    }
}
