using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Oktagon.Localization
{

    [Serializable]
    public struct LokakiJsonData
    {
        public Lokaki.LANGUAGES language;
        public LokakiJsonItem[] items;
    }

    [Serializable]
    public struct LokakiJsonItem
    {
        public string key;
        public string value;
    }

}
