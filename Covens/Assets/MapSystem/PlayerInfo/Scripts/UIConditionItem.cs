using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIConditionItem : MonoBehaviour
{
    [SerializeField] private Image m_ConditionIcon;
    [SerializeField] private TextMeshProUGUI m_TimerText;
    [SerializeField] private Button m_Button;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private GameObject m_CountObject;

    private Conditions m_Condition;
    private System.Action m_OnClick;
    public Conditions condition { get; private set; }

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(Conditions condition, System.Action onclick)
    {
        m_OnClick = onclick;
        Setup(condition);
    }

    public void Setup(Conditions condition)
    {
        this.condition = condition;

        m_ConditionIcon.gameObject.SetActive(false);
        Debug.Log(condition.baseSpell);
        DownloadedAssets.GetSprite(condition.baseSpell,
            (spr) =>
            {
                m_ConditionIcon.sprite = spr;
                m_ConditionIcon.gameObject.SetActive(true);
                Debug.Log("set True");
            });

        m_CountObject.SetActive(condition.stacked > 1);
        m_Count.text = condition.stacked.ToString();
        // added this to try to enable the gameobject - wasnt here before
        gameObject.SetActive(true);
        StartCoroutine(UpdateTimerCoroutine());
    }

    private void OnClick()
    {
        m_OnClick?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            System.TimeSpan timespan = Utilities.TimespanFromJavaTime(this.condition.expiresOn);

            if (timespan.TotalSeconds <= 0)
            {
                m_TimerText.text = "00:00";
                break;
            }

            if (timespan.TotalHours >= 1)
                m_TimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);
            else if (timespan.TotalMinutes >= 1)
                m_TimerText.text = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
            else
                m_TimerText.text = string.Format("{0:D2}:{1:D2}", 0, timespan.Seconds);

            yield return new WaitForSeconds(1f);
        }
    }
}
