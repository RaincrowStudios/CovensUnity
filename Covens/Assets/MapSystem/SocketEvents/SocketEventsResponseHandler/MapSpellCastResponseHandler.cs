using UnityEngine;

namespace Raincrow.GameEvent
{
    public class MapSpellCastResponseHandler : IGameEventResponseHandler
    {
        public const string ResponseName = "cast.spell";

        public void HandleResponse(string eventData)
        {
            MapSpellCastResponse response = JsonUtility.FromJson<MapSpellCastResponse>(eventData);
            OnMapSpellcast.HandleEvent(response);
        }
    }

    [System.Serializable]
    public class MapSpellCastResponse
    {
        [SerializeField] private string spell;
        [SerializeField] private MapSpellCastCharacter caster;
        [SerializeField] private MapSpellCastCharacter target;
        [SerializeField] private MapSpellCastResult result;
        [SerializeField] private double timestamp;

        public string Spell { get => spell; set => spell = value; }
        public MapSpellCastCharacter Caster { get => caster; set => caster = value; }
        public MapSpellCastCharacter Target { get => target; set => target = value; }
        public MapSpellCastResult Result { get => result; set => result = value; }
        public double Timestamp { get => timestamp; set => timestamp = value; }
    }

    [System.Serializable]
    public class MapSpellCastResult
    {
        [SerializeField] private int damage;
        [SerializeField] private bool isCritical;
        [SerializeField] private bool isSuccess;

        public int Damage { get => damage; set => damage = value; }
        public bool IsCritical { get => isCritical; set => isCritical = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
    }

    [System.Serializable]
    public class MapSpellCastCharacter
    {
        [SerializeField] private string id;
        [SerializeField] private string type;
        [SerializeField] private int energy;

        public string Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public int Energy { get => energy; set => energy = value; }
    }
}
