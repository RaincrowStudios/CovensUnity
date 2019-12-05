using Newtonsoft.Json;
using Raincrow.GameEventResponses;
using System.Collections.Generic;
using UnityEngine;

public class BossRankHandler : IGameEventHandler
{
    public string EventName => "boss.rank";

    //public static event System.Action<Token> OnSpiritAddPOP;


    public struct EventData
    {
        public struct RankItem
        {
            public bool coven;
            public long total;
        }
        public Dictionary<string, RankItem>[] rank;
        public ulong energy;
        public double timestamp;
    }

    public void HandleResponse(string data)
    {
        EventData eventData = JsonConvert.DeserializeObject<EventData>(data);

        string debug = eventData.energy.ToString();
        foreach(var dict in eventData.rank)
        {
            foreach(var pair in dict)
                debug += "\n\t" + pair.Key + ": " + pair.Value.total;
        }

        Debug.LogError(debug);
    }
}