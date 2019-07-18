namespace Raincrow.GameEventResponses
{
    public interface IGameEventHandler
    {
        void HandleResponse(string eventData);
    }
}