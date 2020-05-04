using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    public interface ICharacterModel : IObjectModel
    {
        int BaseEnergy { get; set; }
        int Energy { get; set; }
        int Power { get; set; }
        int Resilience { get; set; }
        IList<IStatusEffect> StatusEffects { get; }
        IList<IParticleEffect> ParticlesEffects { get; }
        void UpdateStatusEffects();
        void UpdateParticlesEffects();
        void AddStatusEffect(string spellId, int maxDuration);
        void AddParticleEffect(string spellId, int maxDuration, IParticleEffectView particleInstance);
        IStatusEffect GetStatusEffect(string spellId);
        IParticleEffect GetParticleEffect(string spellId);
        Color GetAlignmentColor();
    }

    public interface IObjectModel
    {
        string Id { get; set; }
        string ObjectType { get; set; }    
        BattleSlot? BattleSlot { get; set; }
    }

    public interface IWitchModel : ICharacterModel
    {
        string Name { get; set; }
        int Degree { get; set; }
        int Level { get; set; }        
        CharacterInfo Info { get; set; }
        InventoryModel Inventory { get; set; }
    }

    public interface ISpiritModel : ICharacterModel
    {
        string OwnerId { get; set; }
        string Texture { get; set; }
    }

    public interface ICharacterUIModel
    {
        Texture Texture { get; set; }
        Material AlignmentMaterial { get; set; }        
    }

    public interface IWitchUIModel : ICharacterUIModel
    {
        
    }

    public interface ISpiritUIModel : ICharacterUIModel
    {
        
    }

    public interface ICloneable<T>
    {
        T Clone();
    }
}