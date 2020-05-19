using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectFX
{
    private static SimplePool<Transform> m_HexedPool = new SimplePool<Transform>("StatusEffectFX/Fancy_DebuffHexed_VFX");
    public static Transform SpawnHexFX() => m_HexedPool.Spawn();
    public static void DespawnHexFX(Transform instance) => m_HexedPool.Despawn(instance);


    private static SimplePool<Transform> m_SealedPool = new SimplePool<Transform>("StatusEffectFX/Fancy_DebuffSeal_VFX");
    public static Transform SpawnSealFX() => m_SealedPool.Spawn();
    public static void DespawnSealFX(Transform instance) => m_SealedPool.Despawn(instance);


    private static SimplePool<Transform> m_CovenBuffShadow = new SimplePool<Transform>("StatusEffectFX/Fancy_CovenBuff_Grey_VFX");
    private static SimplePool<Transform> m_CovenBuffGrey = new SimplePool<Transform>("StatusEffectFX/Fancy_CovenBuff_Shadow_VFX");
    private static SimplePool<Transform> m_CovenBuffWhite = new SimplePool<Transform>("StatusEffectFX/Fancy_CovenBuff_White_VFX");

    //Elixirs effects

    private static Dictionary<string, SimplePool<Transform>> m_BuffElixirs = new Dictionary<string, SimplePool<Transform>>() {
        {"elixir_degree",new SimplePool<Transform>("StatusEffectFX/Fancy_BuffElixir_Alignment_VFX2")},
        {"elixir_xp",new SimplePool<Transform>("StatusEffectFX/Fancy_BuffElixir_Experience_VFX2")},
        {"elixir_gathering",new SimplePool<Transform>("StatusEffectFX/Fancy_BuffElixir_Gathering_VFX2")},
        {"elixir_power",new SimplePool<Transform>("StatusEffectFX/Fancy_BuffElixir_Power_VFX2")},
        {"elixir_resilience",new SimplePool<Transform>("StatusEffectFX/Fancy_BuffElixir_Resilience_VFX2")}
    };

    public static Transform SpawnCovenBuff(StatusEffect effect)
    {
        if (effect.modifiers.covenSchool < 0)
            return m_CovenBuffShadow.Spawn();

        if (effect.modifiers.covenSchool > 0)
            return m_CovenBuffWhite.Spawn();

        return m_CovenBuffGrey.Spawn();
    }

    public static void DespawnCovenBuff(Transform instance)
    {
        m_CovenBuffShadow.Despawn(instance);
        m_CovenBuffGrey.Despawn(instance);
        m_CovenBuffWhite.Despawn(instance);
    }

    public static Transform Spawn(SimplePool<Transform> effect)
    {
        return effect.Spawn();
    }

    public static SimplePool<Transform> GetElixirEffect(string id)
    {
        if (m_BuffElixirs.ContainsKey(id))
            return m_BuffElixirs[id];

        return null;
    }
}
