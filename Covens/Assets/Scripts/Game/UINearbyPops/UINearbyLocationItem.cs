using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class UINearbyLocationItem : MonoBehaviour
{
    public struct LocationData
    {
        [JsonProperty("_id")]
        public string id;
        public string name;

        public int slots;
        public float latitude;
        public double longitude;
        public int tier;

        public double battleBeginsOn;
        public double battleFinishedOn;
        public double closeOn;
        public double openOn;

        public bool isOpen;
        public bool isActive;
    }

    [SerializeField] private TextMeshProUGUI m_Cost;
    [SerializeField] private TextMeshProUGUI m_NameText;
    [SerializeField] private TextMeshProUGUI m_Status;
    [SerializeField] private TextMeshProUGUI m_ClaimedBy;

    [SerializeField] private Button m_FlyTo;

    private System.Action m_OnFlyTo;

    private void Awake()
    {
        m_FlyTo.onClick.AddListener(OnClickFlyTo);
    }

    private void OnClickFlyTo()
    {
        m_OnFlyTo?.Invoke();
    }

    public void Setup(LocationData data, System.Action onFlyTo)
    {
        m_OnFlyTo = onFlyTo;
        m_NameText.text = data.name;
        m_ClaimedBy.text = "Unclaimed";

        //setup cost
        int goldCost = DownloadedAssets.PlaceOfPowerSettings.goldCost[data.tier - 1];
        if (goldCost != 0)
        {
            m_Cost.text = goldCost.ToString();
        }
        else
        {
            int silverCost = DownloadedAssets.PlaceOfPowerSettings.silverCost[data.tier - 1];
            m_Cost.text = silverCost.ToString();
        }

        //setup status
        if (data.isActive)
        {
            m_Status.text = "<In Battle>";
        }
        else if (data.isOpen)
        {
            StartCoroutine(TimerCoroutine(
                false,
                (float)(Utilities.FromJavaTime(data.battleBeginsOn) - System.DateTime.UtcNow).TotalSeconds
            ));
        }
        else
        {
            StartCoroutine(TimerCoroutine(
                false,
                (float)(Utilities.FromJavaTime(data.openOn) - System.DateTime.UtcNow).TotalSeconds
            ));
        }
    }

    private IEnumerator TimerCoroutine(bool isOpen, float seconds)
    {
        string text;
        if (isOpen)
            text = "Open for: {0}";
        else
            text = "Cooldown: {0}";

        while (seconds > 0)
        {
            m_Status.text = string.Format(text, seconds);
            yield return new WaitForSeconds(1);
            seconds -= 1;
        }
        m_Status.text = string.Format(text, Mathf.Max(seconds, 0));
    }
}
