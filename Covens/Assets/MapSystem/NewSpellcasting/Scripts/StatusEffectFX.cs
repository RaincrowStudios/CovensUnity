using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusEffectFX
{
    private static SimplePool<Transform> m_HexedPool = new SimplePool<Transform>("StatusEffectFX/Fancy_DebuffHexed_VFX");
    private static SimplePool<Transform> m_SealedPool = new SimplePool<Transform>("StatusEffectFX/Fancy_DebuffSeal_VFX");

    public static Transform SpawnHexFX() => m_HexedPool.Spawn();
    public static void DespawnHexFX(Transform instance) => m_HexedPool.Despawn(instance);

    public static Transform SpawnSealFX() => m_SealedPool.Spawn();
    public static void DespawnSealFX(Transform instance) => m_SealedPool.Despawn(instance);
}
