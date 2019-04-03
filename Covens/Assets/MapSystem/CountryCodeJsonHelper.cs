using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryCodeJsonHelper : MonoBehaviour {

    public string json;
    private struct CountryCodeEntry
    {
        public string _id;
        public string name;
        public string code;
    }

    [ContextMenu("Create spreedsheet string")]
    private void CreateSheetString()
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
}
