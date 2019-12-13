using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellChanneling
{
    private static SimplePool<Transform> m_BossFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingBoss");
    private static SimplePool<Transform> m_ShadowFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingShadow");
    private static SimplePool<Transform> m_GreyFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingGrey");
    private static SimplePool<Transform> m_WhiteFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingWhite");

    private static Dictionary<IMarker, Transform> m_SpawnedFX = new Dictionary<IMarker, Transform>();

    public static IMarker Target { get; private set; }

    public static bool IsChanneling => PlayerManager.witchMarker.witchToken.HasStatus(SpellData.CHANNELING_STATUS);
    public static bool IsChanneled => PlayerManager.witchMarker.witchToken.HasStatus(SpellData.CHANNELED_STATUS);

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

       m_StartActionId = "channel.start." + Time.unscaledTime.ToString("N2");

        APIManager.Instance.Post(
            "character/cast/" + target.Token.Id,
            $"{{\"spell\":\"spell_channeling\",\"actionId\":\"{m_StartActionId}\"}}",
            (response, result) =>
            {
                if (result == 200)
                {
                    UIChanneling.Instance.SetInteractable(true);
                    //cast.spell socket event will do the rest
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
            UIChanneling.Instance.Close();
            TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
            return;
        }

        m_StopActionId = "channel.sto." + Time.unscaledTime.ToString("N2");
        SpellCastHandler.OnPlayerCast += OnCastSpell;

        APIManager.Instance.Post(
            "character/cast/" + Target.Token.Id,
            $"{{\"spell\":\"spell_channeling\",\"actionId\":\"{m_StopActionId}\"}}",
            (response, result) =>
            {
                if (IsChanneling)
                {
                    if (result == 200)
                    {
                        //wait for cast.spell socket event
                    }
                    else
                    {
                        SpellCastHandler.OnPlayerCast -= OnCastSpell;
                        UIChanneling.Instance.ShowResults(new SpellCastHandler.Result(), APIManager.ParseError(response));
                    }
                }

                TickSpellHandler.OnPlayerSpellTick -= OnCastSpell;
            });
    }

    public static string m_StartActionId;
    public static string m_StopActionId;

    private static void OnCastSpell(SpellCastHandler.SpellCastEventData data)
    {
        if (data.actionId != m_StopActionId)
            return;

        SpellCastHandler.OnPlayerCast -= OnCastSpell;
        UIChanneling.Instance.ShowResults(data.result, null);
    }

    private static void OnSpellTick(SpellCastHandler.SpellCastEventData data)
    {
        if (data.caster.id == PlayerDataManager.playerData.instance && data.spell == "spell_channeling")
        {
            UIChanneling.Instance.OnTickChanneling(data);
            if (IsChanneled)
            {
                UIChanneling.Instance.ShowResults(data.result, null);
            }
        }
    }

    public static Transform SpawnFX(IMarker marker, CharacterToken token)
    {
        int degree = token.degree;

        if (token.Type == MarkerSpawner.MarkerType.BOSS)
            return m_BossFx.Spawn();
        if (degree > 0)
            return m_WhiteFx.Spawn();
        if (degree < 0)
            return m_ShadowFx.Spawn();
        
        return m_GreyFx.Spawn();
    }

    public static void DespawnFX(Transform fx)
    {
        m_WhiteFx.Despawn(fx);
        m_ShadowFx.Despawn(fx);
        m_GreyFx.Despawn(fx);
        m_BossFx.Despawn(fx);
    }
}
