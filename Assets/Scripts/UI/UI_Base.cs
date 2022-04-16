using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _UIObjects = new Dictionary<Type, UnityEngine.Object[]>();

    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type)
    {
        var evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.BiginDrag:
                evt.OnBeginDragHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                evt.OnPointerEnterHandler += action;
                break;
            case Define.UIEvent.PointerExit:
                evt.OnPointerExitHandler += action;
                break;
            default:
                Util.DebugLog($"[UI_Base] Non-existent {type}_Handler.");
                break;
        }
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        var objects = new UnityEngine.Object[names.Length];
        _UIObjects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild(gameObject, names[i], recursive: true);
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i], recursive: true);
            }

            if (objects[i] == null)
            {
                Util.DebugLog($"[UIManager] Failed to bind({names[i]})");
            }
        }
    }

    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects;
        if (!_UIObjects.TryGetValue(typeof(T), out objects))
        {
            return null;
        }

        return objects[index] as T;
    }
}
