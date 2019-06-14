using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocationManagerItem : MonoBehaviour
{
    private Sprite m_spiritTier;
    private TextMeshProUGUI m_popTier;
    private TextMeshProUGUI m_popTitle;
    private TextMeshProUGUI m_claimed;
    private TextMeshProUGUI m_reward;
    private TextMeshProUGUI m_guardianTitle;
    private TextMeshProUGUI m_spiritName;
    private TextMeshProUGUI m_spiritEnergy;
    private TextMeshProUGUI m_enhanceTitle;
    private TextMeshProUGUI m_enhanceDesc;
    private TextMeshProUGUI m_flyToText;
    private TextMeshProUGUI m_activePlayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(LocationManagerItemData data, Sprite sprite)
    {
        m_spiritTier = sprite;
        //add localization for all of these strings

    }

}
