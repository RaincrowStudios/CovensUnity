
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Raincrow.GameEventResponses.RewardHandlerPOP;

public class LocationRewardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI xp;
    [SerializeField] private Button continueBtn;

    public void Setup(string title, RewardPOPData data, System.Action onContinue)
    {
        this.title.text = title;
        gold.text = data.gold.ToString();
        xp.text = data.xp.ToString();
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.AddListener(() => { onContinue(); });
    }
}
