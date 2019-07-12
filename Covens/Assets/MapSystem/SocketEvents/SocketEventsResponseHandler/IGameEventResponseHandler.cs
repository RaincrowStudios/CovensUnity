namespace Raincrow.GameEvent
{
    public interface IGameEventResponseHandler
    {
        void HandleResponse(string eventData);
    }
}