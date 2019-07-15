using Raincrow.GameEventResponses;
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
        "command":"map_channel_start",
        "casterInstance":"local:db1a793d-a294-4a22-802d-85b94857d2a9",
        "spell":"spell_channeling",
        "channeling":{"instance":"local:23cc3a98-aa1e-4259-b4c6-851cc1fe2977","power":10,"resilience":10,"crit":1,"limit":20,"tick":1},
        "timestamp":1562003633345
     }*/
    public static void OnMapChannelingStart(WSData data)
    {
        if (string.IsNullOrEmpty(data.casterInstance))
            return;
        
        IMarker marker = MarkerSpawner.GetMarker(data.casterInstance);
        if (marker != null)
        {
            SpawnChannelingSFX(marker, data.channeling.instance, data.channeling.tick, data.channeling.limit);
        }

        OnChannelingStart?.Invoke(data.casterInstance, data.channeling.instance);
    }

    public static void OnMapChannelingFinish(WSData data)
    {
        OnChannelingFinish?.Invoke(data.casterInstance, data.instance);
    }

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<SpellCastResult> onFinishFlow, System.Action onCancelFlow)
    {
        //send begin channeling
        //if fail, send failed Result and stop listening for map_channel_start
        //if success, show the channeling screen

        UIChanneling.Instance.Show(onFinishFlow);

        string data = $"{{\"spell\":\"{spell.id}\"}}";
        APIManager.Instance.Post("spell/begin-channel", data, (response, result) =>
        {
            /*{
                "instance":"local:f4ff9966-7880-4b30-b897-52c455cd903d",
                "power":10,
                "resilience":10,
                "crit":1,
                "limit":20,
                "tick":1
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

        APIManager.Instance.Post("spell/end-channel", data, (response, result) =>
        {
            /*{
                "power":{
                    "oldPower":20,
                    "newPower":20
                },
                "resilience":{
                    "oldResilience":20,
                    "newResilience":20
                }
             }*/
            if (result == 200)
            {
                Dictionary<string, Dictionary<string, int>> resultDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(response);
                Dictionary<string, int> power = resultDict["power"] as Dictionary<string, int>;
                Dictionary<string, int> resilience = resultDict["resilience"] as Dictionary<string, int>;

                callback?.Invoke(
                    new Result
                    {
                        newPower = power["newPower"],
                        newResilience = resilience["newResilience"]
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
        SocketClient.Instance.StartCoroutine(ChannelingFxCoroutine(marker, instance, timePerTick, maxTime));
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

        Transform newFx;
        //spawn fx
        if (PlayerDataManager.playerData.degree > 0)
        {
            newFx = m_WhiteFx.Spawn();
        }
        else if (PlayerDataManager.playerData.degree < 0)
        {
            newFx = m_ShadowFx.Spawn();
        }
        else
        {
            newFx = m_GreyFx.Spawn();
        }
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
                //animate max reached fx
                //tweenId = LeanTween.scale(newFx.gameObject, Vector3.one * 1.05f, timePerTick / 4f).setLoopPingPong().uniqueId;
                yield return new WaitUntil(() => !channeling || marker.isNull);
            }
        }

        OnChannelingFinish -= onFinish;

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
