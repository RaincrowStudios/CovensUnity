using System.Collections.Generic;
using UnityEngine;

namespace Oktagon.Utils
{
    public class OktConfigJsonReader : MonoBehaviour, IOktConfigFileReader
    {
        /// <summary>
        /// Dictionary that stores configuration values
        /// </summary>
        private Dictionary<string, string> _configDict = new Dictionary<string, string>();



        public void SetConfig(string filename)
        {
            _configDict.Clear();

            OktConfigFile config = JsonUtility.FromJson<OktConfigFile>(filename);            
            foreach(var entry in config.Entries)
            {
                _configDict.Add(entry.Key, entry.Value);
            }
        }

        public bool GetBoolValue(string key)
        {
            if (_configDict.TryGetValue(key, out string configValue))
            {
                if (bool.TryParse(configValue, out bool result))
                {
                    return result;
                }
                else
                {
                    Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find parse [{0}] from key [{1}] to float", configValue, key);
                }
            }
            else
            {
                Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find key [{0}]", key);
            }

            return false;
        }

        public float GetFloatValue(string key)
        {
            if (_configDict.TryGetValue(key, out string configValue))
            {
                if (float.TryParse(configValue, out float result))
                {
                    return result;
                }
                else
                {
                    Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find parse [{0}] from key [{1}] to float", configValue, key);
                }
            }
            else
            {
                Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find key [{0}]", key);
            }
            
            return 0f;
        }

        public int GetIntValue(string key)
        {
            if (_configDict.TryGetValue(key, out string configValue))
            {
                if (int.TryParse(configValue, out int result))
                {
                    return result;
                }
                else
                {
                    Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find parse [{0}] from key [{1}] to int", configValue, key);
                }
            }
            else
            {
                Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find key [{0}]", key);
            }
            return 0;
        }

        public string GetStringValue(string key)
        {
            if (_configDict.TryGetValue(key, out string configValue))
            {
                return configValue;
            }
            else
            {
                Debug.LogWarningFormat("[OktConfigJsonReader]: Could not find key [{0}]", key);
            }
            return string.Empty;
        }        
    }    
}