using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConditionList : MonoBehaviour
{
    [SerializeField] private UIConditionItem m_ItemPrefab;
    [SerializeField] private LayoutGroup m_Container;
    [SerializeField] private RectTransform m_RectTransform;

    private SimplePool<UIConditionItem> m_ItemPool;
    private int m_TweenId;
    private Token m_Token;
    private MarkerDataDetail m_MarkerData;
    private List<UIConditionItem> m_ActiveConditions = new List<UIConditionItem>();

    private bool m_Show;
    public bool show
    {
        get { return m_Show; }
        set { m_Show = value; }
    }

    private void Awake()
    {
        m_ItemPool = new SimplePool<UIConditionItem>(m_ItemPrefab, 2);
        show = false;
    }

    public void Setup(Token token, MarkerDataDetail data)
    {
        m_Token = token;
        m_MarkerData = data;

        for (int i = 0; i < m_ActiveConditions.Count; i++)
            m_ItemPool.Despawn(m_ActiveConditions[i]);
        m_ActiveConditions.Clear();
        
        if (data.conditions.Count == 0)
            return;

        StartCoroutine(SetupCoroutine());
    }

    private IEnumerator SetupCoroutine()
    {
        for (int i = 0; i < m_MarkerData.conditions.Count; i++)
        {
            AddCondition(m_MarkerData.conditions[i]);
            yield return 1;
        }

        yield return 1;
        show = m_ActiveConditions.Count > 0;
    }


    private void Update()
    {
        if (show)
        {
            m_RectTransform.anchoredPosition = Vector3.Lerp(m_RectTransform.anchoredPosition, Vector3.zero, Time.deltaTime * 6f);
        }
        else
        {
            m_RectTransform.anchoredPosition = Vector3.Lerp(m_RectTransform.anchoredPosition, new Vector2(0, m_RectTransform.sizeDelta.y), Time.deltaTime * 8f);
        }
    }

    public void AddCondition(Conditions condition)
    {
        //ignore conditions with expireOn:0
        if (condition.constant == false)
        {
            System.TimeSpan timespan = Utilities.TimespanFromJavaTime(condition.expiresOn);
            if (timespan.TotalSeconds <= 0)
                return;
        }

        //check if already on list
        foreach (UIConditionItem _conditionItem in m_ActiveConditions)
        {
            if (_conditionItem.condition.baseSpell == condition.baseSpell)
            {
                _conditionItem.OnTimerFinish = () => RemoveCondition(condition);
                _conditionItem.Setup(condition, () =>
                {
                    UIConditionInfo.Instance.Show(condition.baseSpell, _conditionItem.GetComponent<RectTransform>(), new Vector2(1, 1));
                });
                return;
            }
        }

        //spawn new condition item
        string spellId = condition.baseSpell;

        UIConditionItem instance = m_ItemPool.Spawn(m_Container.transform);
        instance.transform.localScale = Vector3.one;

        instance.OnTimerFinish = () => RemoveCondition(condition);
        instance.Setup(condition, () =>
        {
            UIConditionInfo.Instance.Show(spellId, instance.GetComponent<RectTransform>(), new Vector2(1, 1));
        });

        m_ActiveConditions.Add(instance);
        show = true;
    }

    public void RemoveCondition(Conditions condition)
    {
        for (int i = 0; i < m_ActiveConditions.Count; i++)
        {
            if (m_ActiveConditions[i].condition.baseSpell == condition.baseSpell || m_ActiveConditions[i].condition.instance == condition.instance)
            {
                m_ItemPool.Despawn(m_ActiveConditions[i]);
                m_ActiveConditions.RemoveAt(i);

                if (m_ActiveConditions.Count == 0)
                    show = false;
                return;
            }
        }
    }
}
