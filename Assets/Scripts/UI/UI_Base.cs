using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Base : MonoBehaviour
{
    // 해당 타입으로 들고있는 오브젝트를 찾아 저장하는 데이터.
    protected Dictionary<Type, UnityEngine.Object[]> _UIObjects = new Dictionary<Type, UnityEngine.Object[]>();

    // AddUIEvent메서드는 해당 오브젝트에 지정한 이벤트타입으로 메서드를 추가한다.
    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type)
    {
        var evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler += action;
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

    // Bind 메서드는 인자 typeof(enum type)에 있는 enum 리스트들의 이름들을 통해
    // 해당 이름과 같은 하위 오브젝트를 찾아 제네릭 인자값 컴포넌트를
    // Dictionary<Type, UnityEngine.Object[]> 형태로 저장한다.
    // enum에 있는 텍스트값과 하위 오브젝트 이름이 동일해야한다.
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

    // Get 메서드는 Bind하여 저장하고 있는 데이터에서 매개변수값을 찾아 반환한다.
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
