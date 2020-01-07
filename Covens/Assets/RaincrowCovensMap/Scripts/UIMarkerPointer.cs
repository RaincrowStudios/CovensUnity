using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMarkerPointer : MonoBehaviour
{
    [SerializeField] private RectTransform m_CanvasRect;
    [SerializeField] private Vector2 m_HorizontalBorders;
    [SerializeField] private Vector2 m_VerticalBorders;
    [SerializeField] private Vector2 m_VerticalFeather;

    [Space()]
    [SerializeField] private CanvasGroup m_PointerCavnasGroup;
    [SerializeField] private RectTransform m_PointerTransform;
    [SerializeField] private RectTransform m_PointerArrow;
    [SerializeField] private Image m_Portrait;

    [Space()]
    [SerializeField] private CanvasGroup m_PhysicalCenter;

    private int m_TweenId;
    private int m_PhysCenterTweenId;

    private bool m_Enabled = false;
    private bool m_ShowingPointer = false;
    private bool m_ShowingPhysical = false;

    private CharacterMarker m_Target;

    private void Awake()
    {
        m_PointerTransform.gameObject.SetActive(false);
        m_PointerCavnasGroup.alpha = 0;
        m_PhysicalCenter.alpha = 0;
        m_Portrait.gameObject.SetActive(true);
    }

    private IEnumerator Start()
    {
        while (MapsAPI.Instance.IsInitialized == false)
            yield return null;
        
        while (m_Target == null)
            yield return null;

        m_Enabled = true;
        
        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;
    }

    private void OnDestroy()
    {
        MapsAPI.Instance.OnCameraUpdate -= OnMapUpdate;
    }

    private void OnMapUpdate(bool position, bool zoom, bool rotation)
    {
        if (m_Enabled == false)
        {
            HidePointer();
            return;
        }

        Vector2 screenPos = MapsAPI.Instance.camera.WorldToScreenPoint(MapsAPI.Instance.GetWorldPosition(m_Target.Token.longitude, m_Target.Token.latitude));
        Vector2 canvasPos = new Vector2(screenPos.x * (m_CanvasRect.sizeDelta.x / Screen.width), screenPos.y * (m_CanvasRect.sizeDelta.y / Screen.height));

        if (
            canvasPos.x > m_HorizontalBorders.x &&
            canvasPos.x < m_CanvasRect.sizeDelta.x - m_HorizontalBorders.y &&
            canvasPos.y > m_VerticalBorders.x + m_VerticalFeather.x &&
            canvasPos.y < m_CanvasRect.sizeDelta.y - m_VerticalBorders.y - m_VerticalFeather.y)
        {
            HidePointer();
            ShowPhysicalMarker(!MapsAPI.Instance.streetLevel);
        }
        else
        {
            ShowPointer();
            ShowPhysicalMarker(false);
        }

        canvasPos.x = Mathf.Clamp(
            canvasPos.x,
            m_HorizontalBorders.x,
            m_CanvasRect.sizeDelta.x - m_HorizontalBorders.y
        );

        canvasPos.y = Mathf.Clamp(
            canvasPos.y,
            m_VerticalBorders.x,
            m_CanvasRect.sizeDelta.y - m_VerticalBorders.y
        );

        m_PhysicalCenter.GetComponent<RectTransform>().anchoredPosition = canvasPos;
        m_PointerTransform.anchoredPosition = canvasPos;
        m_PointerArrow.localRotation = Quaternion.LookRotation(Vector3.forward, m_PointerTransform.localPosition);
    }

    private void ShowPointer()
    {
        m_PointerTransform.gameObject.SetActive(true);

        if (m_ShowingPointer)
        {
            return;
        }

        if (m_Target)
        {
            m_Target.GetPortrait(spr => m_Portrait.overrideSprite = spr);
        }

        m_ShowingPointer = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_PointerCavnasGroup.alpha = t;
                m_PointerTransform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1, t);
            })
            .uniqueId;
    }

    private void HidePointer()
    {
        m_PointerTransform.gameObject.SetActive(false);

        if (!m_ShowingPointer)
        {
            return;
        }

        m_ShowingPointer = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_PointerCavnasGroup.alpha = t;
                m_PointerTransform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1, t);
            })
            .setOnComplete(() => m_PointerTransform.gameObject.SetActive(false))
            .uniqueId;
    }
    
    public void ShowPhysicalMarker(bool show)
    {
        if (m_ShowingPhysical == show)
            return;
        m_ShowingPhysical = show;

        LeanTween.cancel(m_PhysCenterTweenId);
        m_PhysCenterTweenId = LeanTween.alphaCanvas(m_PhysicalCenter, show ? 1 : 0, 0.5f).setEaseOutCubic().uniqueId;
    }

    public void SetTarget(CharacterMarker marker)
    {
        m_Target = marker;
    }
}
