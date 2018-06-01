using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Oktagon.Localization
{

    public class LocalizedText : MonoBehaviour
    {
        public string m_Key;


        // Use this for initialization
        void Start()
        {
            UpdateText();
        }


        private void UpdateText()
        {
            Text text = GetComponent<Text>();
            text.text = GetText(m_Key);
        }

        public void SetKey(string sKey)
        {
            m_Key = sKey;
            UpdateText();
        }

        public string GetText(string sKey)
        {
            return Lokaki.GetText(sKey);
        }
    }
}