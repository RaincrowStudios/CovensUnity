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
    //dictionary to keep track of hex/bless stacking int : index of m_ActiveConditions
    private Dictionary<string, int> m_ActiveConditionsDict = new Dictionary<string, int>();
    public bool show;

    private void Awake()
    {
        m_ItemPool = new SimplePool<UIConditionItem>(m_ItemPrefab, 2);
        show = false;
    }

    public void Setup(Token token, MarkerDataDetail data)
    {
        m_ActiveConditionsDict.Clear();
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
        //if its not in dictionary do the regular logic and add to the dictionary
        if (!m_ActiveConditionsDict.ContainsKey(condition.baseSpell))
        {
            UIConditionItem instance = m_ItemPool.Spawn(m_Container.transform);
            instance.transform.localScale = Vector3.one;
            m_ActiveConditions.Add(instance);
            string spellId = condition.baseSpell;
            instance.Setup(condition, () =>
            {
                UIConditionInfo.Instance.Show(spellId, instance.GetComponent<RectTransform>(), new Vector2(1, 1));
            });
            show = true;
            m_ActiveConditionsDict[condition.baseSpell] = m_ActiveConditions.Count - 1;
        } // update condition item count
        else
        {
            m_ActiveConditions[m_ActiveConditionsDict[condition.baseSpell]].Setup(condition);
        }
    }

    public void RemoveCondition(Conditions condition)
    {
        for (int i = 0; i < m_ActiveConditions.Count; i++)
        {
            if (m_ActiveConditions[i].condition.baseSpell == condition.baseSpell || m_ActiveConditions[i].condition.instance == condition.instance)
            {
                // remove the condtion from dictionary
                if (m_ActiveConditionsDict.ContainsKey(m_ActiveConditions[i].condition.baseSpell))
                {
                    m_ActiveConditionsDict.Remove(m_ActiveConditions[i].condition.baseSpell);
                }
                m_ItemPool.Despawn(m_ActiveConditions[i]);
                m_ActiveConditions.RemoveAt(i);

                if (m_ActiveConditions.Count == 0)
                    show = false;
                return;
            }
        }
    }
}
