using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    public static bool IsInitialized { get; private set; } = false;
    public static event Action OnFirebaseInitialized;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                IsInitialized = true;
                Debug.Log("Firebase is initialized!");

                OnFirebaseInitialized?.Invoke();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
}
