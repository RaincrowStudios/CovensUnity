using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ExecuteInEditMode]
public class UICustomScroller : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private RectTransform m_Container;

    [Header("Settings")]
    [SerializeField] private float m_Sensivity = 1;
    [SerializeField] private float m_Inertia = 0;

    [SerializeField, HideInInspector] private LayoutGroup m_LayoutGroup;
    [SerializeField, HideInInspector] private ContentSizeFitter m_ContentFitter;

    private int m_IntertiaTweenId;
    private int m_ChildCount;
    private int m_Transform;
    private RectTransform m_TopChild;
    private RectTransform m_BotChild;

    public event System.Action<RectTransform> OnTopChildExitView;
    public event System.Action<RectTransform> OnBotChildExitView;

    public int childCount { get { return m_ChildCount; } }

    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.cancel(m_IntertiaTweenId);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        m_Container.anchoredPosition = ClampPosition(m_Container.anchoredPosition + new Vector2(0, eventData.delta.y * m_Sensivity));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LeanTween.cancel(m_IntertiaTweenId);

        Vector3 start = m_Container.anchoredPosition;
        Vector3 target = m_Container.anchoredPosition + new Vector2(0, eventData.delta.y * m_Inertia);

        m_IntertiaTweenId = LeanTween.value(0, 1, 1f)
            .setOnUpdate((float t) => m_Container.anchoredPosition = ClampPosition(Vector3.Lerp(start, target, t)))
            .setEaseOutCubic()
            .uniqueId;
    }

    private Vector2 ClampPosition(Vector2 position)
    {
        position.y = Mathf.Clamp(position.y, -m_Container.rect.height + m_RectTransform.rect.height * 0.9f, 0);
        return position;
    }

    private void OnValidate()
    {
        if (m_Container == null)
            return;

        m_LayoutGroup = m_Container.GetComponent<LayoutGroup>();

        if (m_LayoutGroup == null)
            m_LayoutGroup = m_Container.gameObject.AddComponent<VerticalLayoutGroup>();

        m_ContentFitter = m_Container.GetComponent<ContentSizeFitter>();
        if (m_ContentFitter == null)
            m_ContentFitter = m_Container.gameObject.AddComponent<ContentSizeFitter>();

        m_ContentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        m_ContentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void OnChange()
    {
        m_ChildCount = m_Container.childCount;
        if (m_ChildCount > 0)
        {
            m_TopChild = m_Container.GetChild(0).GetComponent<RectTransform>();
            m_BotChild = m_Container.GetChild(m_ChildCount - 1).GetComponent<RectTransform>();
        }
        else
        {
            m_TopChild = m_BotChild = null;
        }
    }

    private void Start()
    {
        m_ChildCount = 0;
    }

    public void AddItem(RectTransform obj)
    {
        obj.SetParent(m_Container);
        OnChange();
    }

    public void RemoveItem(RectTransform obj)
    {
        obj.SetParent(null);
        OnChange();
    }

    private void Update()
    {

#if UNITY_EDITOR //debug
        if (Application.isPlaying == false)
        {
            if (transform.hasChanged || m_ChildCount != transform.childCount)
                OnChange();
        }
#endif

        //check if the items are not inside the main rect
        if (m_TopChild != null && m_Container.anchoredPosition.y + m_TopChild.localPosition.y > m_RectTransform.rect.height)
        {
            OnTopChildExitView?.Invoke(m_TopChild);
        }

        if (m_BotChild != null && m_Container.anchoredPosition.y + m_BotChild.localPosition.y + m_BotChild.rect.height < 0)
        {
            OnBotChildExitView?.Invoke(m_BotChild);
        }
    }
}
