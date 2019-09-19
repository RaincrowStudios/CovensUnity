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
    private Coroutine m_SetupCoroutine;

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

    public void Setup(List<StatusEffect> effects)
    {
        if (m_SetupCoroutine != null)
        {
            StopCoroutine(m_SetupCoroutine);
            m_SetupCoroutine = null;
        }

        m_ItemPool.DespawnAll();

        if (effects == null || effects.Count == 0)
            return;

        m_SetupCoroutine = StartCoroutine(SetupCoroutine(effects));
    }

    private IEnumerator SetupCoroutine(List<StatusEffect> statusEffects)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            AddCondition(statusEffects[i]);
            yield return 1;
        }

        yield return 1;
        show = m_ItemPool.GetInstances().Count > 0;
        m_SetupCoroutine = null;
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

    public void AddCondition(StatusEffect condition)
    {
        List<UIConditionItem> active = m_ItemPool.GetInstances();

        //check if already on list
        foreach (UIConditionItem _conditionItem in active)
        {
            if (_conditionItem.condition.spell == condition.spell)
            {
                _conditionItem.Setup(condition, () => UIConditionInfo.Instance.Show(condition.spell, _conditionItem.GetComponent<RectTransform>(), new Vector2(1, 1)));
                condition.ScheduleExpiration(() => RemoveCondition(condition));
                return;
            }
        }

        //spawn new condition item

        UIConditionItem instance = m_ItemPool.Spawn(m_Container.transform);
        instance.transform.localScale = Vector3.one;
        instance.Setup(condition, () => UIConditionInfo.Instance.Show(condition.spell, instance.GetComponent<RectTransform>(), new Vector2(1, 1)));
        condition.ScheduleExpiration(() => RemoveCondition(condition));

        show = true;
    }

    public void RemoveCondition(StatusEffect condition)
    {
        List<UIConditionItem> active = m_ItemPool.GetInstances();

        foreach (UIConditionItem item in active)
        {
            if (item.condition.spell == condition.spell)
            {
                m_ItemPool.Despawn(item);

                if (active.Count == 1)
                    show = false;

                break;
            }
        }
    }
}
