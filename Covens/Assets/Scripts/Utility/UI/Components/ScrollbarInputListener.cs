using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// just listens the input
/// </summary>
public class ScrollbarInputListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    bool m_bIsDragging = false;
    public event Action OnBeginDragEvent;
    public event Action OnDragEvent;
    public event Action OnEndDragEvent;


    public bool IsDragging
    {
        get { return m_bIsDragging; }
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        m_bIsDragging = true;
        if (OnBeginDragEvent != null)
            OnBeginDragEvent();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent!= null)
            OnDragEvent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_bIsDragging = false;
        if (OnEndDragEvent != null)
            OnEndDragEvent();
    }

}

