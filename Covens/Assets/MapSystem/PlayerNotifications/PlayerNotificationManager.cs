using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PlayerNotificationManager : MonoBehaviour
{
    private static PlayerNotificationManager m_Instance;
    public static PlayerNotificationManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<PlayerNotificationManager>("UINotifications"));
            return m_Instance;
        }
    }

    private enum Anchoring
    {
        NONE = 0,
        DEFAULT = 1,
        LEFT = 2,
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private PlayerNotificationItem m_NotificationItemPrefab;

    [SerializeField] public Sprite spellBookIcon;
    [SerializeField] public Sprite covenIcon;
    [SerializeField] public Sprite popIcon;

    private RectTransform m_Container;
    private List<string> m_MessageQueue = new List<string>();
    private List<Sprite> m_IconQueue = new List<Sprite>();

    private int m_ScaleTweenId;
    private int m_AnchorsTweenId;
    private Anchoring m_Anchor;

    void Awake()
    {
        m_Container = m_NotificationItemPrefab.transform.parent.GetComponent<RectTransform>();

        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_NotificationItemPrefab.gameObject.SetActive(false);

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        Destroy(this.gameObject);
    }

    public void ShowNotificationPOP(string message)
    {
        ShowNotification(message, popIcon);
    }
    public void ShowNotification(string message, Sprite icon = null)
    {
        if (string.IsNullOrEmpty(message))
            return;

        Anchoring targetAnchor = Anchoring.DEFAULT;
        if (UIPlayerInfo.isShowing || UISpiritInfo.isOpen || UIPortalInfo.isOpen)
            targetAnchor = Anchoring.LEFT;

        if (targetAnchor == Anchoring.LEFT)
            SetLeftAnchors(m_Canvas.enabled ? 1f : 0f);
        else
            SetDefaultAnchors(m_Canvas.enabled ? 1f : 0f);

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        m_NotificationItemPrefab.Show(message, icon,
            () =>
            {
                m_Canvas.enabled = false;
                m_InputRaycaster.enabled = false;
            },
            () =>
            {
                targetAnchor = Anchoring.DEFAULT;
                if (UIPlayerInfo.isShowing || UISpiritInfo.isOpen || UIPortalInfo.isOpen)
                    targetAnchor = Anchoring.LEFT;

                if (targetAnchor == Anchoring.LEFT)
                    SetLeftAnchors(1f);
                else
                    SetDefaultAnchors(1f);

            });
    }

    public void Pop()
    {
        LeanTween.cancel(m_ScaleTweenId);
        transform.localScale = Vector3.one * 1.2f;
        m_ScaleTweenId = LeanTween.scale(this.gameObject, Vector3.one, 1f).setEaseOutCubic().uniqueId;
    }

    public void SetDefaultAnchors(float time)
    {
        if (m_Anchor == Anchoring.DEFAULT)
            return;

        m_Anchor = Anchoring.DEFAULT;
        TweenAnchors(new Vector2(0.175f, 0.81f), time);
    }

    public void SetLeftAnchors(float time)
    {
        if (m_Anchor == Anchoring.LEFT)
            return;

        m_Anchor = Anchoring.LEFT;
        TweenAnchors(new Vector2(0.01f, 0.55f), time);
    }

    private void TweenAnchors(Vector2 targetAnchors, float time)
    {
        //if (m_Instance == null || m_Instance.m_Canvas.enabled == false)
        //{
        //    m_HorizontalAnchors = targetAnchors;
        //    return;
        //}

        if (m_Container.anchorMin.x == targetAnchors.x && m_Container.anchorMax.x == targetAnchors.y)
            return;

        LeanTween.cancel(m_AnchorsTweenId);

        float minStart = m_Container.anchorMin.x;
        float maxStart = m_Container.anchorMax.x;
        float aux;

        m_Instance.m_AnchorsTweenId = LeanTween.value(0, 1, time)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                aux = Mathf.Lerp(minStart, targetAnchors.x, t);
                m_Container.anchorMin = new Vector2(aux, m_Container.anchorMin.y);

                aux = Mathf.Lerp(maxStart, targetAnchors.y, t);
                m_Container.anchorMax = new Vector2(aux, m_Container.anchorMax.y);

                m_Container.sizeDelta = new Vector2(0, m_Container.sizeDelta.y);
                m_Container.anchoredPosition = new Vector2(0, m_Container.anchoredPosition.y);
            })
            .uniqueId;
    }
}

