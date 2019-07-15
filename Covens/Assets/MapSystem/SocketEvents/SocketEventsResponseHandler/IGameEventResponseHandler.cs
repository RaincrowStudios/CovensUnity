namespace Raincrow.GameEventResponses
{
    public interface IGameEventResponseHandler
    {
        void HandleResponse(string eventData);
    }
}