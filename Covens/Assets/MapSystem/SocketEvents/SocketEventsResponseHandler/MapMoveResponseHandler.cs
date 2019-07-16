using Raincrow.GameEventResponses;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MapMoveResponseHandler : IGameEventResponseHandler
    {
        public void HandleResponse(string eventData)
        {
            //MapMoveResponse data = JsonUtility.FromJson<MapMoveResponse>(eventData);
            //MarkerManagerAPI.HandleMarkersCallbackOnSuccess(data);
        }       
    }
}