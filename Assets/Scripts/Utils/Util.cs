using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    // 해당 오브젝트에 컴포넌트를 찾아 반환하고 없으면 추가하여 반환한다.
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    // 해당 오브젝트에서 name이 같은 자식 오브젝트를 찾는다.
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        var transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    // 제네릭 인자 타입으로 자식 오브젝트중에 name이 같은 오브젝트를 찾아 반환한다.
    // recursive는 자식 오브젝트의 자식...자식 까지 탐색한다.
    // 만약 name이 비워져 있으면 컴포넌트 이름과 같은 오브젝트를 찾는다.
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
        {
            return null;
        }

        if (recursive)
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name.Equals(name))
                {
                    return component;
                }
            }
        }
        else
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name.Equals(name))
                {
                    var component = transform.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }

        return null;
    }

    // 유니티 에디터상에서만 호출하기위한 디버그 로그 메서드.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(T logMessage)
    {
        Debug.Log(logMessage);
    }
}
