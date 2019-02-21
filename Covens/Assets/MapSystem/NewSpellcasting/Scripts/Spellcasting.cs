using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;

public class Spellcasting : MonoBehaviour
{
    public static Spellcasting Instance { get; private set; }

    private UISpellcasting m_SpellcastingUI;

    private void Awake()
    {
        Instance = this;
        this.enabled = false;
    }
}
