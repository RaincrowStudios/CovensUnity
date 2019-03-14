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

    public bool show;

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
        show = true;
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
        UIConditionItem instance = m_ItemPool.Spawn(m_Container.transform);
        instance.transform.localScale = Vector3.one;
        m_ActiveConditions.Add(instance);
        string spellId = condition.baseSpell;
        instance.Setup(condition, () =>
        {
            UIConditionInfo.Instance.Show(spellId, instance.GetComponent<RectTransform>());
        });
        show = true;
    }

    public void RemoveCondition(Conditions condition)
    {
        for (int i = 0; i < m_ActiveConditions.Count; i++)
        {
            Debug.Log(m_ActiveConditions[i].condition.instance + " : " + condition.instance);
            if (m_ActiveConditions[i].condition.instance == condition.instance)
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
