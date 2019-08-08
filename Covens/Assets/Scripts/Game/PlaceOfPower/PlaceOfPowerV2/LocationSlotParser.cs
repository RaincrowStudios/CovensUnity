using Microsoft.Win32;
using System;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raincrow.GameEventResponses;
using UnityEngine;
using System.Collections.Generic;

public static class LocationSlotParser
{

    public static LocationData HandleResponse(string eventData)
    {
        MultiKeyDictionary<int, string, object> tokens = new MultiKeyDictionary<int, string, object>();
        JObject data = JObject.Parse(eventData);

        LocationData locationData = new LocationData();
        locationData._id = data["_id"].ToString();
        locationData.maxSlots = (int)data["maxSlots"];
        locationData.currentOccupants = (int)data["currentOccupants"];

        int island = 0;

        foreach (var slot in (IEnumerable)data["slots"])
        {
            int pos = 0;
            foreach (var position in (IEnumerable)slot)
            {
                var token = position.ToString();
                if (!String.IsNullOrEmpty(token))
                {
                    object result;
                    if (token.TryParseJson<WitchToken>(out result))
                    {
                        WitchToken witchToken = result as WitchToken;
                        witchToken.position = pos;
                        witchToken.island = island;
                        tokens.Add(island * 3 + (pos - 1), witchToken.instance, result);
                    }
                    else if (token.TryParseJson<SpiritToken>(out result))
                    {
                        SpiritToken spiritToken = result as SpiritToken;
                        spiritToken.position = pos;
                        spiritToken.island = island;
                        AddSpiritHandlerPOP.RaiseEvent(spiritToken);
                        tokens.Add(island * 3 + (pos - 1), spiritToken.instance, result);
                    }
                    //tokenList.Add(result);
                }
                pos++;
            }
            island++;
        }
        locationData.tokens = tokens;
        return locationData;
    }

}

public class MultiKeyDictionary<TKey1, TKey2, TValue> : Dictionary<(TKey1, TKey2), TValue>, IDictionary<(TKey1, TKey2), TValue>
{
    private HashSet<TKey1> t1Keys = new HashSet<TKey1>();
    private HashSet<TKey2> t2Keys = new HashSet<TKey2>();

    public TValue this[TKey1 key1, TKey2 key2]
    {
        get { return base[(key1, key2)]; }
        set { base[(key1, key2)] = value; }
    }

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        t1Keys.Add(key1);
        t2Keys.Add(key2);
        base.Add((key1, key2), value);
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return base.ContainsKey((key1, key2));
    }

    public bool ContainsKey1(TKey1 key)
    {
        return t1Keys.Contains(key);
    }

    public bool ContainsKey2(TKey2 key)
    {
        return t2Keys.Contains(key);
    }
}