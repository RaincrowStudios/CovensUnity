using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellChanneling
{
    public static IMarker Target { get; private set; }

    public static bool IsChanneling => PlayerDataManager.playerData.HasStatus("channeling");
    public static bool IsChanneled => PlayerDataManager.playerData.HasStatus("channeled");

    private static SimplePool<Transform> m_ShadowFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingShadow");
    private static SimplePool<Transform> m_GreyFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingGrey");
    private static SimplePool<Transform> m_WhiteFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingWhite");


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
                SpellCastHandler.SpellCastEventData data = JsonConvert.DeserializeObject<SpellCastHandler.SpellCastEventData>(response);

                OnMapEnergyChange.ForceEvent(target, data.target.energy, data.timestamp);

                if (data.result.statusEffect != null && string.IsNullOrEmpty(data.result.statusEffect.spell) == false)
                    ConditionManager.AddCondition(data.result.statusEffect, Target);

                UIChanneling.Instance.SetInteractable(true);
            });
    }

    public static void StopChanneling(System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result, string> callback)
    {
        if (IsChanneling == false)
        {
            TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
            Target = null;
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

                        //add the new status effect
                        if (data.result.statusEffect != null && string.IsNullOrEmpty(data.result.statusEffect.spell) == false)
                            ConditionManager.AddCondition(data.result.statusEffect, Target);

                        UIChanneling.Instance.ShowResults(data.result, null);
                    }
                    else
                    {
                        UIChanneling.Instance.ShowResults(new SpellCastHandler.Result(), APIManager.ParseError(response));
                    }
                }

                TickSpellHandler.OnPlayerSpellTick -= OnSpellTick;
                Target = null;
            });
    }

    private static void OnSpellTick(SpellCastHandler.SpellCastEventData data)
    {
        if (data.caster.id == PlayerDataManager.playerData.instance && data.spell == "spell_channeling")
        {

        }
    }
}
