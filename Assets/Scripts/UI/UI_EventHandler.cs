using System;
using UnityEngine;
using UnityEngine.EventSystems;

// UI의 이벤트에 따라 메서드를 추가하기 위한 클래스
public class UI_EventHandler : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerEnterHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerExitHandler { get; set; } = null;
    
    // 마우스 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    // 마우스 포인터가 처음 들어올 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterHandler != null)
        {
            OnPointerEnterHandler.Invoke(eventData);
        }
    }

    // 마우스 포인터가 벗어날 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitHandler != null)
        {
            OnPointerExitHandler.Invoke(eventData);
        }
    }
}
