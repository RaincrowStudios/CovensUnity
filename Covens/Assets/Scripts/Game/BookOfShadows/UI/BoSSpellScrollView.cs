using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoSSpellScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector2 m_vDragInitMousePosition = Vector2.zero;
    public BoSSpellScreenUI m_pUIManager;
    private Vector2 m_vInitDragPosition;
    //public float m_fMultiplier = 0.05f;
    private float m_fInitBarValue = 0.0f;

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_vDragInitMousePosition = eventData.position;
        m_vInitDragPosition = m_vDragInitMousePosition;
        m_fInitBarValue = m_pUIManager.GetHorizontalbarValue();
    }

    public void OnEndDrag(PointerEventData data)
    {
        m_pUIManager.OnHorizontalDrag(m_vDragInitMousePosition, data.position);
    }

    public void OnDrag(PointerEventData data)
    {
        float fValue = (data.position.x - m_vInitDragPosition.x) / Screen.width;
        m_pUIManager.ForceDrag(-fValue, m_fInitBarValue);
    }
}
