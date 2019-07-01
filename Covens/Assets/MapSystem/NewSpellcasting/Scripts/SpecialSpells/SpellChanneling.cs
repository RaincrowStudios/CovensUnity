using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellChanneling
{
    public static event System.Action<string, string> OnChannelingStart;
    public static event System.Action<string, string> OnChannelingFinish;

    public static bool IsChanneling { get; private set; }

    /*{
       command: 'map_channel_start',
       caster: caster.displayName,
       spell: data.spell,
       instance: data.instance
     }*/
    public static void OnMapChannelingStart(WSData data)
    {
        //OnChannelingStart?.Invoke(data.caster);
    }

    public static void OnMapChannelingFinish(WSData data)
    {
        //OnChannelingFinish?.Invoke(data.caster);
    }

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<Result> onFinishFlow, System.Action onCancelFlow)
    {
        //send begin channeling
        //if fail, send failed Result and stop listening for map_channel_start
        //if success, show the channeling screen

        UIChanneling.Instance.Show(onFinishFlow);

        string data = $"{{\"spell\":\"{spell.id}\"}}";
        APIManager.Instance.PostCoven("spell/begin-channel", data, (response, result) =>
        {
            /*{
                "instance":"local:069c2e16-81a5-4b7d-bcb9-c1de7aa4162d"
              }*/

            if (result == 200)
            {
                Dictionary<string, object> responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                string instance = responseData["instance"] as string;
                UIChanneling.Instance.SetChannelingInstance(instance);

                SpawnChannelingSFX(PlayerManager.marker, instance, 1f, 5f);
            }
            else
            {
                UIChanneling.Instance.ShowResults(null, response);
                onFinishFlow?.Invoke(null);
            }
        });
    }

    public static void StopChanneling(string instance, System.Action<Result, string> callback)
    {
        string data = $"{{\"spellInstance\":\"{instance}\"}}";

        APIManager.Instance.PostCoven("spell/end-channel", data, (response, result) =>
        {
            if (result == 200)
            {
                callback?.Invoke(
                    new Result
                    {

                    },
                    null
                );

                //simulate event to despawn the fx
                OnChannelingFinish?.Invoke(PlayerDataManager.playerData.instance, instance);
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

    private static SimplePool<Transform> m_ShadowFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingShadow");
    private static SimplePool<Transform> m_GreyFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingGrey");
    private static SimplePool<Transform> m_WhiteFx = new SimplePool<Transform>("SpellFX/Channeling/ChannelingWhite");

    private static void SpawnChannelingSFX(IMarker marker, string instance, float timePerTick, float maxTime)
    {
        WebSocketClient.Instance.StartCoroutine(ChannelingFxCoroutine(marker, instance, timePerTick, maxTime));
    }

    private static IEnumerator ChannelingFxCoroutine(IMarker marker, string instance, float timePerTick, float maxTime)
    {
        bool channeling = true;
        System.Action<string, string> onFinish = (_caster, _channel) =>
        {
            if (_channel == instance)
                channeling = false;
        };
        OnChannelingFinish += onFinish;

        //spawn fx
        Transform newFx = m_GreyFx.Spawn();
        marker.AddChild(newFx, marker.characterTransform, m_GreyFx);
        ParticleSystem[] particles = newFx.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem _ps in particles)
            _ps.Play(false);

        int tweenId = 0;
        float totalTime = 0;

        while (channeling && !marker.isNull)
        {
            yield return new WaitForSeconds(timePerTick);
            totalTime += timePerTick;
            var p = newFx.transform.GetChild(0).GetComponent<ParticleSystem>();
            var main = p.main;

            if (totalTime <= maxTime)
            {
                main.loop = true;
                //animate pulse fx
                //newFx.localScale = Vector3.one * 1.2f;
                //tweenId = LeanTween.scale(newFx.gameObject, Vector3.one, timePerTick / 2).setEaseOutCubic().uniqueId;
            }
            else
            {
                main.loop = false;
                Debug.Log("Max Channeling!");
                //animate max reached fx
                //tweenId = LeanTween.scale(newFx.gameObject, Vector3.one * 1.05f, timePerTick / 4f).setLoopPingPong().uniqueId;
                yield return new WaitUntil(() => !channeling || marker.isNull);
            }
        }

        LeanTween.cancel(tweenId);

        //stop particles
        foreach (ParticleSystem _ps in particles)
            _ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        //animate channelign complete
        tweenId = LeanTween.scale(newFx.gameObject, Vector3.one * 1.5f, timePerTick).uniqueId;

        //desapwn fx
        yield return new WaitForSeconds(5f);
        marker.RemoveChild(newFx);
        m_GreyFx.Despawn(newFx);
    }
}
