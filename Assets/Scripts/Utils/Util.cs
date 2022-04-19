using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    // �ش� ������Ʈ�� ������Ʈ�� ã�� ��ȯ�ϰ� ������ �߰��Ͽ� ��ȯ�Ѵ�.
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    // �ش� ������Ʈ���� name�� ���� �ڽ� ������Ʈ�� ã�´�.
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        var transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    // ���׸� ���� Ÿ������ �ڽ� ������Ʈ�߿� name�� ���� ������Ʈ�� ã�� ��ȯ�Ѵ�.
    // recursive�� �ڽ� ������Ʈ�� �ڽ�...�ڽ� ���� Ž���Ѵ�.
    // ���� name�� ����� ������ ������Ʈ �̸��� ���� ������Ʈ�� ã�´�.
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

    // ����Ƽ �����ͻ󿡼��� ȣ���ϱ����� ����� �α� �޼���.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DebugLog<T>(T logMessage)
    {
        Debug.Log(logMessage);
    }
}
