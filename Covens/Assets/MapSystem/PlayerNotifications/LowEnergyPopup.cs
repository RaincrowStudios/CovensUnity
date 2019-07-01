using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LowEnergyPopup : MonoBehaviour
{
    public static LowEnergyPopup Instance { get; set; }

    [SerializeField] private TextMeshProUGUI m_offerText;
    [SerializeField] private TextMeshProUGUI m_buttonText;
    [SerializeField] private Button m_close;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_offerText.text = LocalizeLookUp.GetText("energy_restore_message");
        m_close.onClick.AddListener(Close);
        //this will need localization
        if (PlayerDataManager.playerData.gold < 1)
        {
            m_buttonText.text = "<color=red>Missing 1 Gold Drach</color>";
            m_buttonText.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                ShopManager.Instance.Open();
                ShopManager.Instance.ShowSilver();
            });
        }
        else
        {
            m_buttonText.text = "Offer 1 Gold Drach to Restore Energy";
            m_buttonText.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
                SetupConfirmation();
            });
        }

    }

    void SetupConfirmation()
    {
        m_offerText.text = LocalizeLookUp.GetText("energy_restore_confirm");
        m_buttonText.text = LocalizeLookUp.GetText("store_accept");
        m_buttonText.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => {
            AddEnergy();
        });
    }

    // Update is called once per frame
    void AddEnergy()
    {

        var pData = PlayerDataManager.playerData;        
        pData.gold--;
        PlayerManagerUI.Instance.UpdateDrachs();
        pData.energy = pData.baseEnergy;
        PlayerManagerUI.Instance.UpdateEnergy();
        Close();
        //need to post new data for player energy here
        //not sure if below is how it will look, but here ya are.
        /*
        var postData = new PlayerDataDetail
        {
            energy = pData.baseEnergy,
            gold = pData.gold
        };
        */

    }

    void Close()
    {
        Destroy(gameObject);
    }
}