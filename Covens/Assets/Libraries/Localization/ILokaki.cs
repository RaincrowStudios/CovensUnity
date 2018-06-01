using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Oktagon.Localization
{

    public interface ILokakiSource
    {
        Dictionary<string, string> Data { get; }
        string GetText(string sID);
        bool Contains(string sID);
        
        void Setup(Lokaki.LANGUAGES eLang, string sContext);
        bool Load(Lokaki.LANGUAGES eLang);
    }
}