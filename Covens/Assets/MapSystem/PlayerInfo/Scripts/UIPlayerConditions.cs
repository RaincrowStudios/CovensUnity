using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.GameEventResponses;

public class UIPlayerConditions : MonoBehaviour
{
    [Header("ui")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private RectTransform m_Panel;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Animator m_MainUIAnimator;

    [Header("conditions")]
    [SerializeField] private Transform m_Container;
    [SerializeField] private UIConditionItem m_ConditionPrefab;
    [SerializeField] private Button m_OpenButton;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private RectTransform m_ConditionCounter;
    [SerializeField] private TextMeshProUGUI m_CounterText;

    private bool m_IsOpen;
    private int m_CounterTweenId;
    private int m_TweenId;
    private SimplePool<UIConditionItem> m_ItemPool;

    private void Awake()
    {
        m_ItemPool = new SimplePool<UIConditionItem>(m_ConditionPrefab, 0);
        m_ConditionCounter.localScale = Vector3.zero;

        m_OpenButton.onClick.AddListener(OnClickOpen);
        m_CloseButton.onClick.AddListener(OnClickClose);

        m_Container.gameObject.SetActive(false);
        m_ConditionPrefab.gameObject.SetActive(false);

        m_Panel.anchoredPosition = new Vector2(-250, 0);
        m_CanvasGroup.alpha = 0;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        SpellCastHandler.OnPlayerApplyStatusEffect += AddCondition;
        OnMapEnergyChange.OnPlayerDead += SetupCounter;
    }

    private IEnumerator Start()
    {
        while (PlayerDataManager.playerData == null)
            yield return 0;

        var conditions = PlayerDataManager.playerData.effects;

        foreach(StatusEffect effect in conditions)
        {
            AddCondition(effect);
        }
    }

    public void Open()
    {
        if (m_IsOpen)
            return;

        m_IsOpen = true;

        //animate the panel
        LeanTween.cancel(m_TweenId, true);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_Panel.anchoredPosition = new Vector2((1 - t) * -250, 0);
            })
            .uniqueId;

        //enable the components and animate the MainUI
        m_Container.gameObject.SetActive(true);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_MainUIAnimator.Play("in");

        //setup the conditions
        foreach(StatusEffect condition in PlayerDataManager.playerData.effects)
            AddCondition(condition);
    }

    public void Close()
    {
        if (m_IsOpen == false)
            return;

        m_IsOpen = false;

        m_InputRaycaster.enabled = false;

        //animate the panel
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 0.8f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_Panel.anchoredPosition = new Vector2((1 - t) * -250, 0);
            })
            .setOnComplete(() =>
            {
                //disable to stop layouts
                m_Container.gameObject.SetActive(false);

                //despawn active condition items
                m_ItemPool.DespawnAll();

                m_Canvas.enabled = false;
            })
            .uniqueId;

        //animate the main UI
        m_MainUIAnimator.Play("out");
    }

    private void AddCondition(StatusEffect condition)
    {
        //chek if existing
        List<UIConditionItem> active = m_ItemPool.GetInstances();
        foreach (UIConditionItem _item in active)
        {
            if (_item.condition.spell == condition.spell)
            {
                _item.OnTimerFinish = () => RemoveCondition(condition.spell);
                _item.Setup(
                    condition,
                    () => UIConditionInfo.Instance.Show(condition.spell, _item.GetComponent<RectTransform>(), new Vector2(0, 1), true));
                return;
            }
        }

        UIConditionItem item = m_ItemPool.Spawn();
        item.transform.SetParent(m_Container.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;

        item.Setup(
            condition, 
            () => UIConditionInfo.Instance.Show(condition.spell, item.GetComponent<RectTransform>(), new Vector2(0, 1), true));

        SetupCounter();
    }

    private void RemoveCondition(string condition)
    {
        List<UIConditionItem> active = m_ItemPool.GetInstances();
        foreach (UIConditionItem _item in active)
        {
            if (_item.condition.spell == condition)
            {
                _item.condition.Expire();
                m_ItemPool.Despawn(_item);
                break;
            }
        }

        SetupCounter();
    }

    private void SetupCounter()
    {
        if (PlayerDataManager.playerData == null)
            return;

        List<UIConditionItem> conditions = m_ItemPool.GetInstances();

        if (conditions.Count == 0)
            HideConditionCounter();
        else
        {
            m_CounterText.text = conditions.Count.ToString();
            ShowConditionCounter();
        }
    }

    private void ShowConditionCounter()
    {
        LeanTween.cancel(m_CounterTweenId);
        m_CounterTweenId = LeanTween.value(m_ConditionCounter.localScale.x, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() => { m_ConditionCounter.gameObject.SetActive(true); })
            .setOnUpdate((float t) =>
            {
                m_ConditionCounter.localScale = new Vector3(t, t, t);
            })
            .uniqueId;
    }

    private void HideConditionCounter()
    {
        if (m_ConditionCounter.gameObject.activeSelf == false)
            return;

        LeanTween.cancel(m_CounterTweenId);
        m_CounterTweenId = LeanTween.value(m_ConditionCounter.localScale.x, 0f, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_ConditionCounter.localScale = new Vector3(t, t, t);
            })
            .setOnComplete(() => { m_ConditionCounter.gameObject.SetActive(false); })
            .uniqueId;
    }

    private void OnClickOpen()
    {
        if (m_IsOpen)
            Close();
        else
            Open();
    }

    private void OnClickClose()
    {
        Close();
    }
}
