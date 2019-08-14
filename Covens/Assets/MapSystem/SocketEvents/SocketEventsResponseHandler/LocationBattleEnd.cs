using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;

public class LocationBattleEnd : IGameEventHandler
{
    public string EventName => "pop.end";
    public static event System.Action OnLocationBattleEnd;

    public void HandleResponse(string eventData)
    {
        Debug.Log("Location Battle has Ended");
        OnLocationBattleEnd?.Invoke();
    }
}