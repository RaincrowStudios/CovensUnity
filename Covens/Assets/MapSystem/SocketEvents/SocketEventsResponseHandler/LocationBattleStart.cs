using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;

public class LocationBattleStart : IGameEventHandler
{
    public string EventName => "start.pop";
    public static event System.Action<SpiritToken> OnLocationBattleStart;

    public void HandleResponse(string eventData)
    {
        Debug.Log("STARTING POP");
        Debug.Log(eventData);
        Debug.Log("Location Battle has Started");
        SoundManagerOneShot.Instance.SetBGTrack(1);
        var spiritData = JsonConvert.DeserializeObject<SpiritToken>(eventData);
        spiritData.island = -1;
        spiritData.position = -1;
        OnLocationBattleStart?.Invoke(spiritData);
    }
}