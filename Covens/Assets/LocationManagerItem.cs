using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationManagerItem : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI m_popTier;
    [SerializeField]private TextMeshProUGUI m_popTitle;
    [SerializeField]private TextMeshProUGUI m_reward;
    [SerializeField]private TextMeshProUGUI m_spiritName;
    [SerializeField]private TextMeshProUGUI m_spiritEnergy;

    /*      TEMPORARILY REMOVED
    [SerializeField]private TextMeshProUGUI m_activePlayers;
    */

    [SerializeField] private TextMeshProUGUI m_enhanceDesc;
    [SerializeField]private Button m_flyTo;

    // Start is called before the first frame update
    void Awake()
    {
        m_popTier.text = "";
        m_popTitle.text = "";
        m_reward.text = "";
        m_spiritName.text = "";
        m_spiritEnergy.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(LocationManagerItemData data, bool isItem = true)
    {
        //temporary spot
        m_enhanceDesc.text = "";
        Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        if (isItem)
        {
            m_flyTo.onClick.AddListener(() => {
                //PlayerManager.Instance.FlyTo(data.lng, data.lat);
                PlayerManager.Instance.FlyTo(data.longitude, data.latitude);
                LocationManagerUI.Instance.Close();
            });
            m_popTier.text = string.Concat(LocalizeLookUp.GetText("summoning_tier"), " ", data.tier);
            m_popTitle.text = data.name;

            m_reward.text = LocalizeLookUp.GetText("pop_reward")
                .Replace("{{value}}", string.Concat("<color=white>", data.silver.ToString(), " ", LocalizeLookUp.GetText("store_silver_drachs_upper"), "</color>"))
                .Replace("{{timestamp}}", Utilities.GetTimeRemainingPOPUI(data.rewardOn));

            if (string.IsNullOrEmpty(data.spirit))
            {
                m_spiritName.text = "    ";
                m_spiritEnergy.text = "    ";
            }
            else
            {
                m_spiritName.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
                m_spiritEnergy.text = string.Concat(LocalizeLookUp.GetText("lt_energy"), " ", data.spiritEnergy);
            }
        }
        else
        {
            //1st line turns off the image
            transform.GetChild(0).gameObject.SetActive(false);
            m_popTitle.text = "";
            m_flyTo.gameObject.SetActive(false);
            m_popTier.text = "";
            m_reward.text = "";
            m_spiritName.transform.parent.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            m_spiritName.text = "   " + LocalizeLookUp.GetText("coven_screen_nothing");
            m_spiritEnergy.text = "";
            m_enhanceDesc.text = "";
            transform.GetChild(4).gameObject.SetActive(false);
        }

        /*
        if (data.playersShown)
        {
            m_activePlayers.gameObject.SetActive(true);
            m_activePlayers.text = string.Concat(data.activePlayers, " ", LocalizeLookUp.GetText("pop_active_players"));
        }
        */

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
