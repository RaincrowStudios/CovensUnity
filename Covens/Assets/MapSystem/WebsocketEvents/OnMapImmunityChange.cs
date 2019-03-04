using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapImmunityChange
{
    private static SimplePool<Transform> m_ImmunityShieldPool = new SimplePool<Transform>("SpellFX/ImmunityShield");
    private static SimplePool<Transform> m_ImmunityAuraPool = new SimplePool<Transform>("SpellFX/ImmunityAura");

    private static Dictionary<string, Transform> m_shieldDictionary = new Dictionary<string, Transform>();
    private static Dictionary<string, Transform> m_AuraDictionary = new Dictionary<string, Transform>();

    public static void AddImmunityFX(IMarker target)
    {
        Token token = target.customData as Token;

        if (token.Type != MarkerSpawner.MarkerType.witch)
            return;

        if (m_shieldDictionary.ContainsKey(token.instance))
            return;

        target.SetAlpha(0.38f);
    }

    public static void RemoveImmunityFX(IMarker target)
    {
        if (target == null)
            return;

        if (token.Type != MarkerSpawner.MarkerType.witch)
            return;

        Token token = target.customData as Token;
        target.SetAlpha(1f);
    }

    public static void OnAddImmunity(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        MarkerSpawner.AddImmunity(data.immunity, data.instance);

        if (data.immunity == player.instance)
        {
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
