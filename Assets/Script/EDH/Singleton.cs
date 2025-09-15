using UnityEngine;
using System.Reflection;

using System;

[AttributeUsage(AttributeTargets.Method)]
public class MyButtonAttribute : Attribute { }

//[CustomEditor(typeof(MonoBehaviour), true)]
//public class MyButtonEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        var methods = target.GetType()
//            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

//        foreach (var method in methods)
//        {
//            if (method.GetCustomAttribute(typeof(MyButtonAttribute)) != null)
//            {
//                if (GUILayout.Button(method.Name))
//                {
//                    method.Invoke(target, null);
//                }
//            }
//        }
//    }
//}
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 씬에 이미 존재하는 인스턴스가 있는지 먼저 확인
                    _instance = FindObjectOfType<T>();

                    // 없으면 새로 생성
                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name + " (Singleton)");
                        _instance = singletonObj.AddComponent<T>();

                        // 씬 전환에도 살아남게
                        DontDestroyOnLoad(singletonObj);
                    }
                }
            }

            return _instance;
        }
    }

    // 중복 방지 및 초기화 Hook
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 중복 제거
        }
    }
}
