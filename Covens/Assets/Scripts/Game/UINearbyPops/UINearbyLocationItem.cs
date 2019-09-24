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
    public LocationData m_Data;

    private void Awake()
    {
        m_FlyTo.onClick.AddListener(OnClickFlyTo);
    }

    private void OnClickFlyTo()
    {
        m_OnFlyTo?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Setup(LocationData data, System.Action onFlyTo)
    {
        StopAllCoroutines();
        m_Data = data;
        m_OnFlyTo = onFlyTo;
        m_NameText.text = data.name;
        m_ClaimedBy.text = LocalizeLookUp.GetText("location_unclaimed");// "Unclaimed";

        //setup cost
        int goldCost = DownloadedAssets.PlaceOfPowerSettings.entryCosts[data.tier - 1].gold;
        if (goldCost != 0)
        {
            m_Cost.text = goldCost.ToString();
        }
        else
        {
            int silverCost = DownloadedAssets.PlaceOfPowerSettings.entryCosts[data.tier - 1].silver;
            m_Cost.text = silverCost.ToString();
        }

        //setup status
        if (data.isActive)
        {
            m_Status.text = LocalizeLookUp.GetText("pop_ongoing");// "<In Battle>";
        }
        else if (data.isOpen)
        {
            StartCoroutine(TimerCoroutine(
                true,
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
            text = LocalizeLookUp.GetText("pop_open_for");// "Open for: {0}";
        else
            text = LocalizeLookUp.GetText("pop_cooldown").Replace("{{time}}", "{0}");// "Cooldown: {0}";

        while (seconds > 0)
        {
            m_Status.text = string.Format(text, (int)seconds);
            yield return new WaitForSeconds(1);
            seconds -= 1;
        }
        m_Status.text = string.Format(text, Mathf.Max(seconds, 0));

        //Setup(m_Data, m_OnFlyTo);
        UINearbyLocations.Refresh();
    }
}
