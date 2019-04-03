using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CountryCodeJsonHelper : MonoBehaviour {

    public string json;
    private struct CountryCodeEntry
    {
        public string _id;
        public string name;
        public string code;
    }

    private struct SpiritTypeEntry
    {
        public string _id;
        public string tags;
    }

    [ContextMenu("Create countrycode string")]
    private void CreateCountrySheetString()
    {
        string result = "";
        CountryCodeEntry[] allCountries = Newtonsoft.Json.JsonConvert.DeserializeObject<CountryCodeEntry[]>(json);
        Dictionary<string, CountryCodeEntry> dict = new Dictionary<string, CountryCodeEntry>();


        for (int i = 0; i < allCountries.Length; i++)
        {
            if (dict.ContainsKey(allCountries[i].code) == false)
            {
                result += allCountries[i].code + "\t" + allCountries[i].name + "\n";
                dict.Add(allCountries[i].code, allCountries[i]);
            }
        }

        System.IO.File.WriteAllText(Application.dataPath + $"/../../Tools/countryCodes.txt", result);
    }

    [ContextMenu("Create spirittype string")]
    private void CreateSpiritSheetString()
    {
        string result = "";
        SpiritTypeEntry[] allSpirits = Newtonsoft.Json.JsonConvert.DeserializeObject<SpiritTypeEntry[]>(json);
        allSpirits = allSpirits.OrderBy(spirit => spirit._id).ToArray();

        for (int i = 0; i < allSpirits.Length; i++)
        {
            result += allSpirits[i]._id + "\t" + allSpirits[i].tags + "\n";
        }

        System.IO.File.WriteAllText(Application.dataPath + $"/../../Tools/spiritTypes.txt", result);
    }
}
