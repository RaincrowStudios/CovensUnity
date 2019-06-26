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
        //listen for map_channel_start
        //call onCOmplete with the result
        //stop listening map_channel_start

        //send begin channeling
        //if fail, send failed Result and stop listening for map_channel_start
        //if success start listening to map_channel_end
        
        string data = $"{{\"spell\":\"{spell.id}\"}}";
        APIManager.Instance.PostCoven("begin-channel", data, (response, result) =>
        {
            if (result == 200)
            {

            }
            else
            {

            }
        });
    }
}
