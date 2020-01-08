using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;

public class OnLocationOpen : IGameEventHandler
{
    public string EventName => "open.pop";
    public static event System.Action OnLocationBattleEnd;

    public struct OpenData
    {
        public string id;
        public string name;
    }

    public void HandleResponse(string eventData)
    {
        Debug.LogError("todo: " + EventName);
        //var data = JsonConvert.DeserializeObject<OpenData>(eventData);

        //if (MarkerSpawner.Markers.ContainsKey(data.id) && BackButtonListener.ActionCount == 0 && !LocationIslandController.isInBattle)
        //{
        //    LoadPOPManager.EnterPOP(data.id);
        //}
    }
}