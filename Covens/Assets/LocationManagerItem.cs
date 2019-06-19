using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationManagerItem : MonoBehaviour
{
    [SerializeField]private Image m_spiritTier;
    [SerializeField]private TextMeshProUGUI m_popTier;
    [SerializeField]private TextMeshProUGUI m_popTitle;
    [SerializeField]private TextMeshProUGUI m_claimed;
    [SerializeField]private TextMeshProUGUI m_reward;
    [SerializeField]private TextMeshProUGUI m_guardianTitle;
    [SerializeField]private TextMeshProUGUI m_spiritName;
    [SerializeField]private TextMeshProUGUI m_spiritEnergy;

    /*      TEMPORARILY REMOVED
    [SerializeField]private TextMeshProUGUI m_enhanceTitle;
    [SerializeField]private TextMeshProUGUI m_enhanceDesc;
    */

    [SerializeField]private TextMeshProUGUI m_flyToText;
    [SerializeField]private TextMeshProUGUI m_activePlayers;

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
        m_flyToText.transform.GetComponent<Button>().onClick.AddListener(() => {
            //PlayerManager.Instance.FlyTo(data.lng, data.lat);
            PlayerManager.Instance.FlyTo(0, 0);
            StartCoroutine(LocationManagerUI.Instance.Close());
        });
        m_spiritTier.sprite = sprite;
        //add localization for all of these strings
        m_popTier.text = string.Concat(LocalizeLookUp.GetText("summoning_tier"), " ", DownloadedAssets.spiritDictData[data.spirit].spiritTier.ToString());
        m_popTitle.text = data.popName;
        m_reward.text = Utilities.EpocToDateTime(data.rewardOn);
        m_guardianTitle.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
        m_spiritName.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
        m_spiritEnergy.text = string.Concat(LocalizeLookUp.GetText("lt_energy"), " ", data.spiritEnergy);
        //m_enhanceTitle.text = LocalizeLookUp.GetText("pop_enhancement");
        m_flyToText.text = LocalizeLookUp.GetText("pop_fly_to");
        if (data.playersShown)
        {
            m_activePlayers.gameObject.SetActive(true);
            m_activePlayers.text = string.Concat(data.activePlayers, " ", LocalizeLookUp.GetText("pop_active_players"));
        }

        //buff description below

        /*
        var perk = data.enhancement;
        var enhanceText = "";
        if (perk.type == "collectible")
        {
            enhanceText = 
        }
        else if (perk.type == "spirit")
        {
            enhanceText = LocalizeLookUp.GetText("pop_enhancement_spirit_spell")
                .Replace("{{Name}}", DownloadedAssets.spiritDictData[perk.spiritID].spiritName)
                .Replace("{{Attribute}}");

        }
        else if (perk.type == "spells")
        {
            enhanceText = LocalizeLookUp.GetText("pop_enhancement_spirit_spell")
                .Replace("{{Name}}", DownloadedAssets.spellDictData[perk.spellId].spellName);
        }
        else if (perk.type == "character")
        {
            enhanceText =

        }
        else
        {
            Debug.LogError("unable to parse the type of perk");
        }
        */
    }

}
