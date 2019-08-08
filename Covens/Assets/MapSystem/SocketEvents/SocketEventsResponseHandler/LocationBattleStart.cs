using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;

public class LocationBattleStart : IGameEventHandler
{
    public string EventName => "pop.start";
    public static event System.Action OnLocationBattleStart;

    public void HandleResponse(string eventData)
    {
        Debug.Log("Location Battle has Started");
        OnLocationBattleStart?.Invoke();
    }
}