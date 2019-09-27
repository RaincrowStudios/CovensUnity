using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellChanneling
{
    //private static SimplePool<Transform> m_ShadowFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingShadow");
    //private static SimplePool<Transform> m_GreyFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingGrey");
    //private static SimplePool<Transform> m_WhiteFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingWhite");
    private const string m_ShadowFxPath = "SpellFX/Channeling/ChannelingShadow";
    private const string m_GreyFxPath = "SpellFX/Channeling/ChannelingGrey";
    private const string m_WhiteFxPath = "SpellFX/Channeling/ChannelingWhite";
    private static Dictionary<IMarker, Transform> m_SpawnedFX = new Dictionary<IMarker, Transform>();

    public static IMarker Target { get; private set; }

    public static bool IsChanneling => PlayerDataManager.playerData.HasStatus("channeling");
    public static bool IsChanneled => PlayerDataManager.playerData.HasStatus("channeled");

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result> onFinishFlow, System.Action onCancelFlow)
    {
        Target = target;
        TickSpellHandler.OnPlayerSpellTick += OnSpellTick;

        UIChanneling.Instance.Show(onFinishFlow);
        UIChanneling.Instance.SetInteractable(false);

        //if already channeling, just show the ui
        if (IsChanneling)
        {
            UIChanneling.Instance.SetInteractable(true);
            return;
        }
        
        APIManager.Instance.Post(
            "character/cast/" + target.Token.Id,
            $"{{\"spell\":\"spell_channeling\"}}",
            (response, result) =>
            {
                if (result == 200)
                {
                    SpellCastHandler.SpellCastEventData data = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(response);

                    OnMapEnergyChange.ForceEvent(target, data.target.energy, data.timestamp);

                    if (data.result.effect != null && string.IsNullOrEmpty(data.result.effect.spell) == false)
                        ConditionManager.AddCondition(data.result.effect, Target);

                    UIChanneling.Instance.SetInteractable(true);
                }
                else
                {
                    UIChanneling.Instance.Close();
                    TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
                }
            });
    }

    public static void StopChanneling(System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result, string> callback)
    {
        if (IsChanneling == false)
        {
            TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
            return;
        }

        APIManager.Instance.Post(
            "character/cast/" + Target.Token.Id,
            $"{{\"spell\":\"spell_channeling\"}}",
            (response, result) =>
            {
                if (IsChanneling)
                {
                    if (result == 200)
                    {
                        SpellCastHandler.SpellCastEventData data = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(response);

                        ////remove the channeling status
                        //for (int i = PlayerDataManager.playerData.effects.Count - 1; i >= 0 ; i--)
                        //{
                        //    if (PlayerDataManager.playerData.effects[i].spell == "spell_channeling")
                        //        PlayerDataManager.playerData.effects.RemoveAt(i);
                        //}

                        //add cooldown
                        CooldownManager.AddCooldown("spell_channeling", data.timestamp, data.cooldown);

                        //add the new status effect
                        ConditionManager.ExpireStatusEffect("spell_channeling");
                        ConditionManager.AddCondition(data.result.effect, Target);
                        
                        UIChanneling.Instance.ShowResults(data.result, null);
                    }
                    else
                    {
                        UIChanneling.Instance.ShowResults(new SpellCastHandler.Result(), APIManager.ParseError(response));
                    }
                }

                TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
            });
    }

    private static void OnSpellTick(SpellCastHandler.SpellCastEventData data)
    {
        if (data.caster.id == PlayerDataManager.playerData.instance && data.spell == "spell_channeling")
        {
            UIChanneling.Instance.OnTickChanneling(data);
            if (IsChanneled)
            {
                DespawnPlayerFX(data.result.effect);
                StopChanneling(null);
                UIChanneling.Instance.ShowResults(data.result, null);
            }
        }
    }

    public static void SpawnFX(IMarker marker, StatusEffect effect)
    {
        if (effect.modifiers.status == null)
            return;

        //if (m_SpawnedFX.ContainsKey(marker))
        //    return;

        bool channeling = false;
        bool channeled = false;

        foreach (var status in effect.modifiers.status)
        {
            if (status == "channeling")
                channeling = true;
            if (status == "channeled")
                channeled = true;
        }

        if (channeling && !m_SpawnedFX.ContainsKey(marker))
        {
            Transform instance;// = marker.SpawnItem(m_GreyFx);
            int degree = (marker.Token as WitchToken).degree;

            if (degree > 0)
                instance = marker.SpawnItem(m_WhiteFxPath);
            else if (degree < 0)
                instance = marker.SpawnItem(m_ShadowFxPath);
            else
                instance = marker.SpawnItem(m_GreyFxPath);

            instance.localScale = Vector3.one;
            instance.GetComponentInChildren<ParticleSystem>().Play();
            m_SpawnedFX.Add(marker, instance);
        }

        if (channeled)
        {
            DespawnFX(marker, effect);
        }
    }

    public static void DespawnFX(IMarker marker, StatusEffect effect)
    {
        if (m_SpawnedFX.ContainsKey(marker) == false)
            return;

        Transform instance = m_SpawnedFX[marker];

        if (instance == null)
            return;

        instance.GetComponentInChildren<ParticleSystem>().Stop();
        m_SpawnedFX.Remove(marker);

        LeanTween.value(0, 0, 5f).setOnComplete(() =>
        {
            if (instance != null)
                GameObject.Destroy(instance.gameObject);
        });
    }


    public static void SpawnPlayerFX(StatusEffect effect, IMarker caster)
    {
        if (effect.modifiers.status == null)
            return;
        
        SpawnFX(PlayerManager.marker, effect);       
    }

    public static void DespawnPlayerFX(StatusEffect effect)
    {
        DespawnFX(PlayerManager.marker, effect);
    }
}
