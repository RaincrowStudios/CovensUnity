using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCenterPointerUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_CanvasRect;
    [SerializeField] private CanvasGroup m_PointerCavnasGroup;
    [SerializeField] private RectTransform m_PointerTransform;
    [SerializeField] private RectTransform m_PointerArrow;
    [SerializeField] private UnityEngine.UI.Image m_Portrait;
    [SerializeField] private Vector2 m_HorizontalBorders;
    [SerializeField] private Vector2 m_VerticalBorders;

    private int m_TweenId;
    private bool m_Showing = false;
    private bool m_Enabled = true;

    private void Awake()
    {
        m_PointerTransform.gameObject.SetActive(false);
        m_PointerCavnasGroup.alpha = 0;
    }

    private void Start()
    {
        //m_MapController.onEnterStreetLevel += OnEnterStreetLevel;
        //m_MapController.onExitStreetLevel += OnExitStreetLevel;

        MapsAPI.Instance.OnCameraUpdate += OnMapUpdate;
    }

    //private void OnEnterStreetLevel()
    //{
    //    m_MapController.onUpdate += OnMapUpdate;
        
    //    OnMapUpdate(true, false, false);
    //}

    //private void OnExitStreetLevel()
    //{
    //    m_MapController.onUpdate -= OnMapUpdate;
    //    HidePointer();
    //}

    private void OnMapUpdate(bool position, bool zoom, bool rotation)
    {
        if (m_Enabled == false)
            return;

        if (!MapsAPI.Instance.streetLevel)
        {
            HidePointer();
            return;
        }

        //if (MapsAPI.Instance.IsPointInsideView(Vector3.zero, -100))
        //{
        //    HidePointer();
        //    return;
        //}

        Vector2 screenPos = MapsAPI.Instance.camera.WorldToScreenPoint(Vector3.zero);
        Vector2 canvasPos = new Vector2(screenPos.x * (m_CanvasRect.sizeDelta.x/ Screen.width), screenPos.y * (m_CanvasRect.sizeDelta.y/ Screen.height));
        
        if (canvasPos.x > m_HorizontalBorders.x && canvasPos.x < m_CanvasRect.sizeDelta.x - m_HorizontalBorders.y && canvasPos.y > m_VerticalBorders.x && canvasPos.y < m_CanvasRect.sizeDelta.y - m_VerticalBorders.y)
        {
            HidePointer();
            return;
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

        m_PointerTransform.anchoredPosition = canvasPos;

        Vector3 mapCenter = MapsAPI.Instance.mapCenter.position.normalized;
        m_PointerArrow.localRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-mapCenter.x, -mapCenter.z));
        
        ShowPointer();
    }

    private void ShowPointer()
    {
        if (m_Showing)
            return;

        if (PlayerManager.witchMarker)
        {
            PlayerManager.witchMarker.GetPortrait(spr =>
            {
                m_Portrait.sprite = spr;
                m_Portrait.gameObject.SetActive(true);
            });
        }
        else
        {
            m_Portrait.gameObject.SetActive(false);
        }

        m_Showing = true;
        m_PointerTransform.gameObject.SetActive(true);

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
        if (!m_Showing)
            return;

        m_Showing = false;

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

    public void EnablePointer(bool enable)
    {
        if (m_Enabled == enable)
            return;

        m_Enabled = enable;

        if (m_Enabled)
            ShowPointer();
        else
            HidePointer();
    }
}
