using System.Linq;
using System.Net.Http.Headers;
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
                        Debug.Log(witchToken.popIndex + " On Enter");
                        tokens.Add(witchToken.popIndex, witchToken.instance, result);
                    }
                    else if (token.TryParseJson<SpiritToken>(out result))
                    {
                        SpiritToken spiritToken = result as SpiritToken;
                        spiritToken.position = pos;
                        spiritToken.island = island;
                        AddSpiritHandlerPOP.RaiseEvent(spiritToken);
                        tokens.Add(spiritToken.popIndex, spiritToken.instance, result);
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

public class MultiKeyDictionary<TKey1, TKey2, TValue>
{

    private Dictionary<TKey1, TValue> Key1Dictionary = new Dictionary<TKey1, TValue>();
    private Dictionary<TKey2, TValue> Key2Dictionary = new Dictionary<TKey2, TValue>();

    public int Count
    {
        get
        {
            return Key1Dictionary.Count;
        }
    }

    public TValue this[TKey1 key1, TKey2 key2]
    {
        get
        {
            if (Key1Dictionary.ContainsKey(key1) && Key2Dictionary.ContainsKey(key2))
                return Key1Dictionary[key1];
            else
            {
                Debug.LogError("Key Not found " + key1 + " \n " + key2);
                return default(TValue);
            }
        }
        set
        {
            Key1Dictionary[key1] = value;
            Key2Dictionary[key2] = value;
        }
    }

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        Key1Dictionary[key1] = value;
        Key2Dictionary[key2] = value;
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return (Key1Dictionary.ContainsKey(key1) && Key2Dictionary.ContainsKey(key2));
    }

    public bool ContainsKey1(TKey1 key)
    {
        return Key1Dictionary.ContainsKey(key);
    }

    public bool ContainsKey2(TKey2 key)
    {
        return Key2Dictionary.ContainsKey(key);
    }

    public void Remove(TKey1 key1, TKey2 key2)
    {
        if (ContainsKey(key1, key2))
        {
            Key1Dictionary.Remove(key1);
            Key2Dictionary.Remove(key2);
        }
    }

    public string Keys1ToString()
    {
        string s = "";
        foreach (var item in Key1Dictionary.Keys.ToArray())
        {
            s += item + "\n";
        }
        return s;
    }

    public string Keys2ToString()
    {
        string s = "";
        foreach (var item in Key2Dictionary.Keys.ToArray())
        {
            s += item + "\n";
        }
        return s;
    }

    public TValue[] Values()
    {
        return Key1Dictionary.Values.ToArray();
    }
}

