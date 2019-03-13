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

    private void Awake()
    {
        m_ItemPool = new SimplePool<UIConditionItem>(m_ItemPrefab, 2);
        m_RectTransform.anchoredPosition = new Vector2(0, m_RectTransform.sizeDelta.y);
    }

    public void Setup(Token token, MarkerDataDetail data)
    {
        m_Token = token;
        m_MarkerData = data;
        
        if (data.conditions.Count == 0)
            return;

        for (int i = 0; i < data.conditions.Count; i++)
        {
            AddCondition(data.conditions[i]);
        }

        Show();
    }

    private void Show()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_RectTransform.anchoredPosition.y, 10, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_RectTransform.anchoredPosition = new Vector2(0, t);
            })
            .uniqueId;
    }

    public void Hide()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_RectTransform.anchoredPosition.y, m_RectTransform.sizeDelta.y, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_RectTransform.anchoredPosition = new Vector2(0, t);
            })
            .setOnComplete(() =>
            {
                for (int i = 0; i < m_ActiveConditions.Count; i++)
                    m_ItemPool.Despawn(m_ActiveConditions[i]);
            })
            .uniqueId;
    }

    public void AddCondition(Conditions condition)
    {
        UIConditionItem instance = m_ItemPool.Spawn(m_Container.transform);
        string spellId = condition.baseSpell;
        instance.Setup(condition, () =>
        {
            UIConditionInfo.Instance.Show(spellId, instance.GetComponent<RectTransform>());
        });
    }

    public void RemoveCondition(Conditions condition)
    {
        for (int i = 0; i < m_ActiveConditions.Count; i++)
        {
            if (m_ActiveConditions[i].condition.instance == condition.instance)
            {
                m_ItemPool.Despawn(m_ActiveConditions[i]);
                m_ActiveConditions.RemoveAt(i);
                return;
            }
        }
    }
}
