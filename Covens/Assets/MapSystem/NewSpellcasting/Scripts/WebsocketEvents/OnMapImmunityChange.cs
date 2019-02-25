using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapImmunityChange
{
    private static SimplePool<GameObject> m_ImmunityShieldPool;
    private static SimplePool<GameObject> m_ImmunityAuraPool;

    public static SimplePool<GameObject> auraPool
    {
        get
        {
            if (m_ImmunityAuraPool == null)
                m_ImmunityAuraPool = new SimplePool<GameObject>(
                    "SpellFX/ImmunityAura",
                    aura => aura.SetActive(true),
                    aura => aura.SetActive(false)
                );
            return m_ImmunityAuraPool;
        }
    }
    public static SimplePool<GameObject> shieldPool
    {
        get
        {
            if (m_ImmunityShieldPool == null)
                m_ImmunityShieldPool = new SimplePool<GameObject>(
                    "SpellFX/ImmunityShield",
                    shield => shield.SetActive(true),
                    shield => shield.SetActive(false)
                );
            return m_ImmunityShieldPool;
        }
    }

    private static Dictionary<string, GameObject> m_shieldDictionary = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> m_AuraDictionary = new Dictionary<string, GameObject>();

    public static void AddImmunityFX(IMarker target)
    {
        Token token = target.customData as Token;

        if (m_shieldDictionary.ContainsKey(token.instance))
            return;

        GameObject shield = shieldPool.Spawn();
        GameObject aura = auraPool.Spawn();

        m_shieldDictionary.Add(token.instance, shield);
        m_AuraDictionary.Add(token.instance, aura);

        aura.transform.position = target.gameObject.transform.position;
        aura.transform.SetParent(target.gameObject.transform);
        shield.transform.rotation = target.characterTransform.rotation;
        shield.transform.position = target.characterTransform.position;
        shield.transform.SetParent(target.characterTransform);
    }

    public static void RemoveImmunityFX(IMarker target)
    {
        Token token = target.customData as Token;
        if (m_shieldDictionary.ContainsKey(token.instance))
        {
            GameObject shield = m_shieldDictionary[token.instance];
            m_shieldDictionary.Remove(token.instance);
            shieldPool.Despawn(shield);
        }
        if (m_AuraDictionary.ContainsKey(token.instance))
        {
            GameObject aura = m_AuraDictionary[token.instance];
            m_AuraDictionary.Remove(token.instance);
            auraPool.Despawn(aura);
        }
    }

    public static void OnAddImmunity(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        if (data.immunity == player.instance)
        {
            if (MarkerSpawner.ImmunityMap.ContainsKey(data.instance))
                MarkerSpawner.ImmunityMap[data.instance].Add(data.immunity);
            else
                MarkerSpawner.ImmunityMap[data.instance] = new HashSet<string>() { data.immunity };

            AddImmunityFX(MarkerManager.GetMarker(data.instance));

            return;
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                if (data.instance == MarkerSpawner.instanceID)
                {
                    //							HitFXManager.Instance.SetImmune (true);
                    //StartCoroutine(DelayWitchImmune());
                }
            }
            if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance && ShowSelectionCard.selectedType == MarkerSpawner.MarkerType.witch)
            {
                if (ShowSelectionCard.currCard != null)
                    ShowSelectionCard.currCard.GetComponent<PlayerSelectionCard>().SetCardImmunity(true);
            }
            //MarkerManager.SetImmunity(true, data.instance);
        }
    }

    public static void OnRemoveImmunity(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        if (data.immunity == player.instance)
        {
            if (MarkerSpawner.ImmunityMap.ContainsKey(data.instance))
            {
                if (MarkerSpawner.ImmunityMap[data.instance].Contains(data.immunity))
                    MarkerSpawner.ImmunityMap[data.instance].Remove(data.immunity);
            }

            RemoveImmunityFX(MarkerManager.GetMarker(data.instance));

            return;

            string logMessage = "<color=#008bff> Map_immunity_remove</color>";
            if (data.instance == MarkerSpawner.instanceID && data.instance == player.instance)
            {
                logMessage += "\n <b>" + MarkerSpawner.SelectedMarker.displayName + " <color=#008bff> is no longer Immune to </color> " + player.displayName + "</b>";
            }

            if (MapSelection.currentView == CurrentView.IsoView)
            {
                if (data.instance == MarkerSpawner.instanceID)
                {
                    HitFXManager.Instance.SetImmune(false);
                }
            }
            if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance && ShowSelectionCard.selectedType == MarkerSpawner.MarkerType.witch)
            {
                if (ShowSelectionCard.currCard != null)
                    ShowSelectionCard.currCard.GetComponent<PlayerSelectionCard>().SetCardImmunity(false);
            }
            //MarkerManager.SetImmunity(false, data.instance);
        }
    }
}
