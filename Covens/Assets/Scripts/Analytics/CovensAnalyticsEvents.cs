namespace Raincrow.Analytics
{
    public static class CovensAnalyticsEvents
    {
        public static readonly string GameStarted = "gameStarted";
        public static readonly string GameEnded = "gameEnded";
        public static readonly string NewPlayer = "newPlayer";

        // GameSteps
        public static readonly string BeginLoading = "beginLoading";
        public static readonly string EndLoading = "endLoading";
        public static readonly string LoginSuccess = "loginSuccess";
    }
}
