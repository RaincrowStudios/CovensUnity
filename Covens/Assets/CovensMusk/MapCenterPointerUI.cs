using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCenterPointerUI : MonoBehaviour
{
    [SerializeField] private CovensMuskMap m_Map;
    [SerializeField] private MapCameraController m_MapController;

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private RectTransform m_CanvasRect;
    [SerializeField] private CanvasGroup m_PointerCavnasGroup;
    [SerializeField] private RectTransform m_PointerTransform;
    [SerializeField] private RectTransform m_PointerArrow;
    [SerializeField] private UnityEngine.UI.Image m_Portrait;
    [SerializeField] private Vector2 m_HorizontalBorders;
    [SerializeField] private Vector2 m_VerticalBorders;

    private int m_TweenId;
    private bool m_Showing = false;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_PointerCavnasGroup.alpha = 0;
    }

    private void Start()
    {
        //m_MapController.onEnterStreetLevel += OnEnterStreetLevel;
        //m_MapController.onExitStreetLevel += OnExitStreetLevel;

        m_MapController.onUpdate += OnMapUpdate;
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
        if (!m_Map.streetLevel)
        {
            HidePointer();
            return;
        }

        Vector2 viewportPos = m_MapController.camera.WorldToViewportPoint(m_Map.transform.position);
        
        if(viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1)
        {
            HidePointer();
            return;
        }
        else
        {
            ShowPointer();
        }

        ShowPointer();

        viewportPos.x = Mathf.Clamp(viewportPos.x, 0, 1);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0, 1);

        //pointer position
        Vector2 canvasPos = new Vector2(
            viewportPos.x * m_CanvasRect.sizeDelta.x, 
            viewportPos.y * m_CanvasRect.sizeDelta.y
        );

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

        //arrow rotation
        m_PointerArrow.rotation = Quaternion.LookRotation(Vector3.forward, (viewportPos - new Vector2(0.5f, 0.5f)).normalized);
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
            });
        }

        m_Showing = true;
        m_Canvas.enabled = true;

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
            .setOnComplete(() => m_Canvas.enabled = false)
            .uniqueId;
    }
}
