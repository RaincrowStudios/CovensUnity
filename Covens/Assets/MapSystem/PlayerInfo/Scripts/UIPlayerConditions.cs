using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private List<UIConditionItem> m_ConditionItems = new List<UIConditionItem>();

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
    }

    private void OnEnable()
    {
        SetupCounter();
        OnMapConditionAdd.OnPlayerConditionAdded += OnPlayerConditionUpdate;
        OnMapConditionRemove.OnPlayerConditionRemoved += OnPlayerConditionUpdate;
        LoginAPIManager.OnCharacterInitialized += OnPlayerInitialized;
    }

    private void OnDisable()
    {
        OnMapConditionAdd.OnPlayerConditionAdded -= OnPlayerConditionUpdate;
        OnMapConditionRemove.OnPlayerConditionRemoved -= OnPlayerConditionUpdate;
        LoginAPIManager.OnCharacterInitialized -= OnPlayerInitialized;
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
        Conditions[] conditions = ConditionsManager.conditions;
        for (int i = 0; i < conditions.Length; i++)
        {
            UIConditionItem item = m_ItemPool.Spawn();
            m_ConditionItems.Add(item);
            item.transform.SetParent(m_Container.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;

            int aux = i;
            item.Setup(conditions[aux], () =>
            {
                UIConditionInfo.Instance.Show(conditions[aux].baseSpell, item.GetComponent<RectTransform>(), new Vector2(0, 1));
            });
        }
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
                for (int i = 0; i < m_ConditionItems.Count; i++)
                    m_ItemPool.Despawn(m_ConditionItems[i]);

                m_Canvas.enabled = false;
            })
            .uniqueId;

        //animate the main UI
        m_MainUIAnimator.Play("out");
    }

    private void SetupCounter()
    {
        if (PlayerDataManager.playerData == null)
            return;

        Conditions[] conditions = ConditionsManager.conditions;
        if (conditions.Length == 0)
            HideConditionCounter();
        else
        {
            m_CounterText.text = conditions.Length.ToString();
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

    private void OnPlayerConditionUpdate(Conditions condition)
    {
        SetupCounter();

        if (m_IsOpen && ConditionsManager.conditions.Length == 0)
        {
            if (UIConditionInfo.IsOpen)
                UIConditionInfo.Instance.Close();
            Close();
        }
    }

    private void OnPlayerInitialized()
    {
        SetupCounter();
    }
}
