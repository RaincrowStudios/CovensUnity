using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationManagerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_popTier;
    [SerializeField] private TextMeshProUGUI m_popTitle;
    [SerializeField] private TextMeshProUGUI m_reward;
    [SerializeField] private TextMeshProUGUI m_spiritName;
    [SerializeField] private TextMeshProUGUI m_spiritEnergy;

    /*      TEMPORARILY REMOVED
    [SerializeField]private TextMeshProUGUI m_activePlayers;
    */

    [SerializeField] private TextMeshProUGUI m_enhanceDesc;
    [SerializeField] private Button m_flyTo;
    [SerializeField] private GameObject m_rewardButton;

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

            if (Utilities.GetTimeRemainingPOPUI(data.rewardOn) != string.Empty)
                m_reward.text = LocalizeLookUp.GetText("pop_reward")
                    .Replace("{{value}}", string.Concat("<color=white>", data.silver.ToString(), " ", LocalizeLookUp.GetText("store_silver_drachs_upper"), "</color>"))
                    .Replace("{{timestamp}}", Utilities.GetTimeRemainingPOPUI(data.rewardOn));
            else
            {
                //Debug.Log(Utilities.GetTimeRemainingPOPUI(data.rewardOn));
                m_reward.text = LocalizeLookUp.GetText("pop_click_reward");
                var image = transform.GetChild(0).GetChild(1);
                Instantiate(m_rewardButton, image);
                image.GetComponent<Image>().enabled = false;
                
                image.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                    Debug.LogError("TO DO: Add functionality for requesting pop reward here.");
                });
            }

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
    }

}
