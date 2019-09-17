using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISummonSuccess : UISpiritDiscovered
{
    private static UISummonSuccess m_Instance;
    public static new UISummonSuccess Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISummonSuccess>("UISummonSuccess"));
            return m_Instance;
        }
    }

    protected override void Setup(string id, Sprite sprite)
    {
        string spiritName = LocalizeLookUp.GetSpiritName(id);

        m_Title.text = spiritName;
        m_Description.text = LocalizeLookUp.GetText("summoning_success");
        m_SpiritArt.overrideSprite = sprite;
    }
}
