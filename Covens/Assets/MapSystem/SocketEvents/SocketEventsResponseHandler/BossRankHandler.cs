using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using UnityEngine;

public class BossRankHandler : IGameEventHandler
{
    public string EventName => "boss.rank";

    //public static event System.Action<Token> OnSpiritAddPOP;


    public struct EventData
    {

    }

    public void HandleResponse(string eventData)
    {

    }
}