using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler { get; set; } = null;
    public Action<PointerEventData> OnBeginDragHandler { get; set; } = null;
    public Action<PointerEventData> OnDragHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerEnterHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerExitHandler { get; set; } = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragHandler != null)
        {
            OnBeginDragHandler.Invoke(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
        {
            OnDragHandler.Invoke(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterHandler != null)
        {
            OnPointerEnterHandler.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitHandler != null)
        {
            OnPointerExitHandler.Invoke(eventData);
        }
    }
}
