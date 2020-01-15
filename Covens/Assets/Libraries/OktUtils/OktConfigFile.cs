using System.Collections.Generic;

namespace Oktagon.Utils
{
    [System.Serializable]
    public class OktConfigFile
    {
        public List<OktConfigFileEntry> Entries;
    }

    [System.Serializable]
    public class OktConfigFileEntry
    {
        public string Key;
        public string Value;
    }
}