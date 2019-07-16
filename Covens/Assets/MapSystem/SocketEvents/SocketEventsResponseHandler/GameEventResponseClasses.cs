using UnityEngine;

namespace Raincrow.GameEventResponses
{
    [System.Serializable]
    public class SpellCastResponse
    {
        [SerializeField] private string spell;
        [SerializeField] private CharacterLocation caster;
        [SerializeField] private CharacterLocation target;
        [SerializeField] private DamageResult result;
        [SerializeField] private double timestamp;

        public string Spell { get => spell; set => spell = value; }
        public CharacterLocation Caster { get => caster; set => caster = value; }
        public CharacterLocation Target { get => target; set => target = value; }
        public DamageResult Result { get => result; set => result = value; }
        public double Timestamp { get => timestamp; set => timestamp = value; }
    }

    [System.Serializable]
    public class DamageResult
    {
        [SerializeField] private int damage;
        [SerializeField] private bool isCritical;
        [SerializeField] private bool isSuccess;

        public int Damage { get => damage; set => damage = value; }
        public bool IsCritical { get => isCritical; set => isCritical = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
    }

    [System.Serializable]
    public class SpiritLocation
    {
        [SerializeField] private string id;
        [SerializeField] private string spirit;
        [SerializeField] private MarkerSpawner.MarkerType type;
        [SerializeField] private int baseEnergy;
        [SerializeField] private int energy;
        [SerializeField] private float longitude;
        [SerializeField] private float latitude;

        public string Id { get => id; set => id = value; }
        public string Spirit { get => spirit; set => spirit = value; }
        public MarkerSpawner.MarkerType Type { get => type; set => type = value; }
        public int BaseEnergy { get => baseEnergy; set => baseEnergy = value; }
        public int Energy { get => energy; set => energy = value; }
    }

    [System.Serializable]
    public class ItemLocation
    {
        [SerializeField] private string id;
        [SerializeField] private string collectible;
        [SerializeField] private int amount;
        [SerializeField] private float longitude;
        [SerializeField] private float latitude;
        [SerializeField] private IngredientType type;

        public string Id { get => id; }
        public string Collectible { get => collectible; }
        public int Amount { get => amount; }
        public IngredientType Type { get => type; }
        public float Longitude { get => longitude; }
        public float Latitude { get => latitude; }
    }

    [System.Serializable]
    public class CharacterLocation
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private MarkerSpawner.MarkerType type;
        [SerializeField] private int baseEnergy;        
        [SerializeField] private int energy;
        [SerializeField] private string state;
        [SerializeField] private int level;
        [SerializeField] private Equipment[] equipped;
        [SerializeField] private string[] immunities;
        [SerializeField] private int degree;
        [SerializeField] private float longitude;
        [SerializeField] private float latitude;

        public string Id { get => id; }
        public string Name { get => name; }
        public MarkerSpawner.MarkerType Type { get => type; }
        public int BaseEnergy { get => baseEnergy; }
        public int Energy { get => energy; }                
        public string State { get => state; }
        public int Level { get => level; }
        public Equipment[] Equipped { get => equipped; }
        public string[] Immunities { get => immunities; }
        public int Degree { get => degree; }
        public float Longitude { get => longitude; }
        public float Latitude { get => latitude; }
    }

    public class Equipment
    {
        [SerializeField] private string id;
        [SerializeField] private string[] assets;

        public string Id { get => id; }
        public string[] Assets { get => assets; }        
    }
    
    [System.Serializable]
    public class Location
    {
        [SerializeField] private string dominion;
        [SerializeField] private string garden;        
        [SerializeField] private int zone;
        [SerializeField] private int music;

        public string Dominion { get => dominion; }
        public string Garden { get => garden; }
        public int Zone { get => zone; }
        public int Music { get => music; }
    }
}