
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Raincrow.GameEventResponses.RewardHandlerPOP;

public class LocationRewardInfo : UIInfoPanel
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI xp;
    [SerializeField] private Button continueBtn;

    private static LocationRewardInfo m_Instance;

    public static LocationRewardInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<LocationRewardInfo>("LocationRewardInfo"));
            return m_Instance;

        }
    }


    public static bool isShowing
    {
        get
        {
            if (m_Instance == null) return false;
            else return m_Instance.m_IsShowing;
        }
    }

    public void Setup(string title, RewardPOPData data, System.Action onContinue)
    {
        this.title.text = title;
        gold.text = data.gold.ToString() + " " + LocalizeLookUp.GetText("store_gold");
        xp.text = LocalizeLookUp.GetText("spell_xp").Replace("{{Number}}", data.xp.ToString());// + " " + LocalizeLookUp.GetText("");
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.AddListener(() => { onContinue(); base.Close(); });
        base.Show();
    }
}
