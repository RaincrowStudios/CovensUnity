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

    private System.Action m_OnClick;
    public StatusEffect condition { get; private set; }

    public System.Action OnTimerFinish;

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateTimerCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Setup(StatusEffect condition, System.Action onclick)
    {
        Setup(condition);
        m_OnClick = onclick;
    }

    public void Setup(StatusEffect condition)
    {
        m_OnClick = null;
        this.condition = condition;

        m_ConditionIcon.gameObject.SetActive(false);
        DownloadedAssets.GetSprite(condition.spell,
            (spr) =>
            {
                m_ConditionIcon.overrideSprite = spr;
                m_ConditionIcon.gameObject.SetActive(true);
            });

        m_CountObject.SetActive(condition.stack > 1);
        m_Count.text = condition.stack.ToString();
        // added this to try to enable the gameobject - wasnt here before
        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        m_OnClick?.Invoke();
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            while (condition == null)
            {
                yield return 0;
            }

            if (this.condition.expiresOn == 0)
            {
                m_TimerText.text = "-";
            }
            else
            {
                System.TimeSpan timespan = Utilities.TimespanFromJavaTime(this.condition.expiresOn);

                if (timespan.TotalSeconds <= 0)
                {
                    m_TimerText.text = "00:00";
                    OnTimerFinish?.Invoke();
                    OnTimerFinish = null;
                }
                else
                {

                    if (timespan.TotalHours >= 1)
                        m_TimerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timespan.Hours, timespan.Minutes, timespan.Seconds);
                    else if (timespan.TotalMinutes >= 1)
                        m_TimerText.text = string.Format("{0:D2}:{1:D2}", timespan.Minutes, timespan.Seconds);
                    else
                        m_TimerText.text = string.Format("{0:D2}:{1:D2}", 0, timespan.Seconds);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
