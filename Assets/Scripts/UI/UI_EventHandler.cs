using System;
using UnityEngine;
using UnityEngine.EventSystems;

// UI�� �̺�Ʈ�� ���� �޼��带 �߰��ϱ� ���� Ŭ����
public class UI_EventHandler : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerEnterHandler { get; set; } = null;
    public Action<PointerEventData> OnPointerExitHandler { get; set; } = null;
    
    // ���콺 Ŭ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    // ���콺 �����Ͱ� ó�� ���� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterHandler != null)
        {
            OnPointerEnterHandler.Invoke(eventData);
        }
    }

    // ���콺 �����Ͱ� ��� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitHandler != null)
        {
            OnPointerExitHandler.Invoke(eventData);
        }
    }
}
