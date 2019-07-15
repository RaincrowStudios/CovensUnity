using UnityEngine;

namespace Raincrow.GameEventResponses
{
    [System.Serializable]
    public class SpellCastResponse
    {
        [SerializeField] private string spell;
        [SerializeField] private Character caster;
        [SerializeField] private Character target;
        [SerializeField] private SpellCastResult result;
        [SerializeField] private double timestamp;

        public string Spell { get => spell; set => spell = value; }
        public Character Caster { get => caster; set => caster = value; }
        public Character Target { get => target; set => target = value; }
        public SpellCastResult Result { get => result; set => result = value; }
        public double Timestamp { get => timestamp; set => timestamp = value; }
    }

    [System.Serializable]
    public class SpellCastResult
    {
        [SerializeField] private int damage;
        [SerializeField] private bool isCritical;
        [SerializeField] private bool isSuccess;

        public int Damage { get => damage; set => damage = value; }
        public bool IsCritical { get => isCritical; set => isCritical = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
    }

    [System.Serializable]
    public class Character
    {
        [SerializeField] private string id;
        [SerializeField] private string type;
        [SerializeField] private int energy;

        public string Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public int Energy { get => energy; set => energy = value; }
    }
}