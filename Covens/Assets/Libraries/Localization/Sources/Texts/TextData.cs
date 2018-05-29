// Oktagon Games
// FileName: TextData.cs
// Author: Oktagon
// Created on: 2016/10/13
//
using System;

namespace Oktagon.Localization
{

    // Text data containing it's ID in database and it's content.
    // created for smart dynamic replace tags. (gives support to all texts without much performance impact)
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
    public class TextData
    {
        private int m_iId;
        private string m_sValue;
        private bool? m_bContainsDynamicTag;
        internal bool m_bLoaded;


        public TextData(int iId, string sValue)
        {
            this.m_iId = iId;
            this.m_sValue = sValue;
            this.m_bContainsDynamicTag = null;
        }

        public int GetId()
        {
            return m_iId;
        }

        public string GetValue()
        {
            return m_sValue;
        }

        /// <summary>
        /// Needed to know if we should re-scan all dynamic tags for this text.
        /// Used to init the m_bContainsDynamicTag only once, and cache it.
        /// </summary>
        public bool HasCheckedDynamicTag()
        {
            return m_bContainsDynamicTag.HasValue;
        }

        public bool HasDynamicTag()
        {
            if (m_bContainsDynamicTag.HasValue)
            {
                return m_bContainsDynamicTag.Value;
            }
            else
            {
                return false;
            }
        }

        public void SetContainsDynamicTag(bool? bContains)
        {
            m_bContainsDynamicTag = bContains;
        }

        /// <summary>
        /// Returns true if this is a valid localized text.
        /// invalid texts are if met any of these:
        /// - text not translated
        /// - text does not exists
        /// - text not loaded
        /// </summary>
        public bool HasText()
        {
            bool bHas = true;

            if (!m_bLoaded)
            {
                bHas = false;
            }
            else if (GetValue().EndsWith("translate please"))
            { // todo: cache this bool result
                bHas = false;
            }
            return bHas;
        }

        public string DebuggerDisplay
        {
            get
            {
                return string.Format("\"{0}\"", m_sValue);
            }
        }
    }
}
