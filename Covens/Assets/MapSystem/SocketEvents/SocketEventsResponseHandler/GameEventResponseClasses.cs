using UnityEngine;

namespace Raincrow.GameEventResponses
{
    [System.Serializable]
    public class SpellCastResponse
    {
        public string spell;
        public CharacterToken caster;
        public CharacterToken target;
        public DamageResult result;
        public double timestamp;

        public string Spell { get => spell; set => spell = value; }
        public CharacterToken Caster { get => caster; set => caster = value; }
        public CharacterToken Target { get => target; set => target = value; }
        public DamageResult Result { get => result; set => result = value; }
        public double Timestamp { get => timestamp; set => timestamp = value; }
    }

    [System.Serializable]
    public class DamageResult
    {
        public int damage;
        public bool isCritical;
        public bool isSuccess;

        public int Damage { get => damage; set => damage = value; }
        public bool IsCritical { get => isCritical; set => isCritical = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
    }
}