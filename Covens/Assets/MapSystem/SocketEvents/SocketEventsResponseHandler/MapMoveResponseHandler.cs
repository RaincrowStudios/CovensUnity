using Raincrow.GameEventResponses;
using UnityEngine;

namespace Raincrow.GameEventResponses
{
    public class MapMoveResponseHandler : IGameEventResponseHandler
    {
        public void HandleResponse(string eventData)
        {
            MapMoveResponse data = JsonUtility.FromJson<MapMoveResponse>(eventData);
            MarkerManagerAPI.HandleMarkersCallbackOnSuccess(data);
        }       
    }

    [System.Serializable]
    public class MapMoveResponse
    {
        [SerializeField] private CharacterMarker[] characters;
        [SerializeField] private SpiritMarker[] spirits;
        [SerializeField] private ItemMarker[] items;
        [SerializeField] private Location location;

        public CharacterMarker[] Characters { get => characters; }
        public SpiritMarker[] Spirits { get => spirits; }
        public ItemMarker[] Items { get => items; }
        public Location Location { get => location; set => location = value; }
    }
}