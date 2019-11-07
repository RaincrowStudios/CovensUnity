using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;

public class OnLocationClose : IGameEventHandler
{
    public string EventName => "close.pop";
    public static event System.Action OnLocationBattleEnd;

    public struct CloseData
    {
        public string id;
        public string name;
    }

    public void HandleResponse(string eventData)
    {
        var data = JsonConvert.DeserializeObject<CloseData>(eventData);

        // if (MarkerSpawner.Markers.ContainsKey(data.id) && BackButtonListener.ActionCount == 0)
        // {
        //     LoadPOPManager.EnterPOP(data.id);
        // }
    }
}