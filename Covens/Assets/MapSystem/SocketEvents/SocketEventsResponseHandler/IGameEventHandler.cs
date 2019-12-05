namespace Raincrow.GameEventResponses
{
    public interface IGameEventHandler
    {
        string EventName { get; }
        void HandleResponse(string json);
    }
}