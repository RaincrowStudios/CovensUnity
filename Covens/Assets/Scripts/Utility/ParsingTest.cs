using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ParsingTest : MonoBehaviour
{
    public string k;
    List<WitchToken> stuff = new List<WitchToken>();
    void Start()
    {
        parse();
    }

    void parse()
    {
        JObject data = JObject.Parse(k);
        LocationData locationData = new LocationData();
        locationData._id = data["_id"].ToString();
        locationData.maxSlots = (int)data["maxSlots"];

        foreach (var slot in (IEnumerable)data["slots"])
        {
            foreach (var position in (IEnumerable)slot)
            {
                var token = position.ToString();
                if (!String.IsNullOrEmpty(token))
                {
                    object result;
                    if (token.TryParseJson<WitchToken>(out result))
                    {
                        WitchToken witchToken = result as WitchToken;
                    }
                    else if (token.TryParseJson<SpiritToken>(out result))
                    {
                        SpiritToken spiritToken = result as SpiritToken;
                    }
                }
            }
        }
    }


}

