using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoSLandingScollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private Vector2 m_vDragInitMousePosition = Vector2.zero;
    public BoSLandingScreenUI m_pUIManager;
    private Vector2 m_vInitDragPosition;
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
        Debug.Log(string.Format("Drag Horizontal Value {0:0.000}", fValue));
    }

    
}
