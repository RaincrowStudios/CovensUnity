using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Oktagon.Localization
{

    public class LocalizedText : MonoBehaviour
    {
        public string m_Key;

        void Start()
        {
            UpdateText();
            this.enabled = false;
        }


        private void UpdateText()
        {
            Text text = GetComponent<Text>();
            if (text != null)
            {
                text.text = GetText(m_Key);
            }
            else
            {
                TMPro.TextMeshProUGUI textPro = GetComponent<TMPro.TextMeshProUGUI>();
                if (textPro != null)
                    textPro.text = GetText(m_Key);
            }
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