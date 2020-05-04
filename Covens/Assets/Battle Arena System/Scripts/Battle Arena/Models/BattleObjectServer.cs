using Newtonsoft.Json;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo = Raincrow.BattleArena.Model.CharacterInfo;

[System.Serializable]
public class BattleObjectServer
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("type")]
    public string type { get; set; }
    [JsonProperty("participants")]
    public GenericCharacterObjectServer[] Participants { get; set; }
    [JsonProperty("grid")]
    public GridObjectServer Grid { get; set; }
}

[System.Serializable]
public class GridObjectServer
{

    public int MaxCellsPerLine { set; get; }

    public int MaxCellsPerColumn { set; get; }

    public CellObjectServer[,] Cells { set; get; }
}

[System.Serializable]
public class CellObjectServer : ICellModel
{
    [JsonProperty("id")]
    public string ObjectId { get; set; }
    public int Height { get; private set; }
    [JsonProperty("row")]
    public int X { get; set; }
    [JsonProperty("col")]
    public int Y { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(ObjectId);
    }
}

[System.Serializable]
public class GenericCharacterObjectServer : ISpiritModel, IWitchModel
{
    // Properties
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("type")] public string ObjectType { get; set; }
    public string Name { get; set; }
    public int Degree { get; set; }
    public int Level { get; set; }
    public string OwnerId { get; set; }
    [JsonProperty("spirit")] public string Texture { get; set; }
    public int BaseEnergy { get; set; }
    public int Energy { get; set; }
    public int Power { get; set; }
    public int Resilience { get; set; }
    public InventoryModel Inventory { get; set; }
    public CharacterInfo Info { get; set; }
    public BattleSlot? BattleSlot { get; set; }

    public GenericCharacterObjectServer()
    {
        Inventory = new InventoryModel();
        Info = new CharacterInfo();
    }

    public Color GetAlignmentColor()
    {
        if (ObjectType == Raincrow.BattleArena.Model.ObjectType.Spirit)
        {
            return Color.white;
        }

        if (Degree > 0)
        {
            return Utilities.Orange;
        }
        else if (Degree < 0)
        {
            return Utilities.Purple;
        }
        return Utilities.Blue;
    }

    public IList<IStatusEffect> StatusEffects { get; private set; } = new List<IStatusEffect>();
    public IList<IParticleEffect> ParticlesEffects { get; private set; } = new List<IParticleEffect>();
    public void AddStatusEffect(string spellId, int maxDuration)
    {
        IStatusEffect newStatusEffect = new Raincrow.BattleArena.Model.StatusEffect(spellId, maxDuration);

        bool addedStatusEffect = false;
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            IStatusEffect statusEffect = StatusEffects[i];
            if (statusEffect.SpellId == newStatusEffect.SpellId)
            {
                StatusEffects[i] = newStatusEffect;
                addedStatusEffect = true;
                break;
            }
        }

        if (!addedStatusEffect)
        {
            StatusEffects.Add(newStatusEffect);
        }
    }

    public IStatusEffect GetStatusEffect(string spellId)
    {
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            IStatusEffect statusEffect = StatusEffects[i];
            if (statusEffect.SpellId == spellId)
            {
                return statusEffect;

            }
        }
        return default;
    }

    public void UpdateStatusEffects()
    {
        IList<IStatusEffect> statusEffectsCopy = new List<IStatusEffect>();
        for (int i = StatusEffects.Count - 1; i >= 0; i--)
        {
            IStatusEffect statusEffect = StatusEffects[i];
            int duration = statusEffect.Duration - 1;
            if (duration > 0)
            {
                statusEffect.Duration = duration;
                statusEffectsCopy.Add(statusEffect);
            }
        }

        StatusEffects = new List<IStatusEffect>(statusEffectsCopy);
    }

    public void AddParticleEffect(string spellId, int maxDuration, IParticleEffectView particleInstance)
    {
        IParticleEffect newParticleEffect = new Raincrow.BattleArena.Model.ParticleEffect(spellId, maxDuration, particleInstance);

        bool addedParticleEffect = false;
        for (int i = ParticlesEffects.Count - 1; i >= 0; i--)
        {
            IParticleEffect particleEffect = ParticlesEffects[i];
            if (particleEffect.SpellId == newParticleEffect.SpellId)
            {
                ParticlesEffects[i] = newParticleEffect;
                addedParticleEffect = true;
                break;
            }
        }

        if (!addedParticleEffect)
        {
            ParticlesEffects.Add(newParticleEffect);
        }
    }

    public IParticleEffect GetParticleEffect(string spellId)
    {
        for (int i = ParticlesEffects.Count - 1; i >= 0; i--)
        {
            IParticleEffect particleEffect = ParticlesEffects[i];
            if (particleEffect.SpellId == spellId)
            {
                return particleEffect;

            }
        }
        return default;
    }

    public void UpdateParticlesEffects()
    {
        IList<IParticleEffect> particleEffectsCopy = new List<IParticleEffect>();
        for (int i = ParticlesEffects.Count - 1; i >= 0; i--)
        {
            IParticleEffect particleEffect = ParticlesEffects[i];
            int duration = particleEffect.Duration - 1;
            if (duration > 0)
            {
                particleEffect.Duration = duration;
                particleEffectsCopy.Add(particleEffect);
            }
            else
            {
                particleEffect.Particle.DestroyEffect();
            }
        }

        ParticlesEffects = new List<IParticleEffect>(particleEffectsCopy);
    }
}