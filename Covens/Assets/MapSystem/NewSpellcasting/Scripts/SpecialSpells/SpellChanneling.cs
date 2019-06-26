using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellChanneling
{
    public static event System.Action<string> OnChannelingStart;
    public static event System.Action<string> OnChannelingFinish;

    public static bool IsChanneling { get; private set; }

    /*{
       command: 'map_channel_start',
       caster: caster.displayName,
       spell: data.spell,
       instance: data.instance
     }*/
    public static void OnMapChannelingStart(WSData data)
    {
        OnChannelingStart?.Invoke(data.caster);
    }

    public static void OnMapChannelingFinish(WSData data)
    {
        OnChannelingStart?.Invoke(data.caster);
    }

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<Result> onFinishFlow, System.Action onCancelFlow)
    {
        //send begin channeling
        //if fail, send failed Result and stop listening for map_channel_start
        //if success, show the channeling screen
        
        string data = $"{{\"spell\":\"{spell.id}\"}}";
        APIManager.Instance.PostCoven("spell/begin-channel", data, (response, result) =>
        {
            if (result == 200)
            {
                UIChanneling.Instance.Show(onFinishFlow);
            }
            else
            {
                UIGlobalErrorPopup.ShowError(null, "channeling fail: " + response);
                onFinishFlow?.Invoke(null);
            }
        });
    }

    public static void StopChanneling(System.Action<Result, string> callback)
    {
        APIManager.Instance.PostCoven("spell/end-channel", "", (response, result) =>
        {
            if (result == 200)
            {
                callback?.Invoke(
                    new Result
                    {

                    },
                    null
                );
            }
            else
            {
                if (result == 400)
                    callback?.Invoke(null, response);
                else
                    callback?.Invoke(null, result.ToString());
            }
        });
    }
}
