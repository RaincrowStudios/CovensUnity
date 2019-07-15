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
        [SerializeField] private CharacterLocation[] characters;
        [SerializeField] private SpiritLocation[] spirits;
        [SerializeField] private ItemLocation[] items;
        [SerializeField] private Location location;

        public CharacterLocation[] Characters { get => characters; }
        public SpiritLocation[] Spirits { get => spirits; }
        public ItemLocation[] Items { get => items; }
        public Location Location { get => location; }
    }
}