
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
        gold.text = gold.text.Replace("{amount}", data.gold.ToString());
        xp.text = xp.text.Replace("{amount}", data.xp.ToString());
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.AddListener(() => { onContinue(); base.Close(); });
        base.Show();
    }
}
