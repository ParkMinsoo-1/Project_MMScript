using Firebase;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerDataLoader : MonoBehaviour
{
    private void OnEnable()
    {
        FirebaseInitializer.OnFirebaseInitialized += OnFirebaseInitializedHandler;
    }

    private void OnDisable()
    {
        FirebaseInitializer.OnFirebaseInitialized -= OnFirebaseInitializedHandler;
    }

    private async void OnFirebaseInitializedHandler()
    {
        await PlayerDataManager.Instance.Load();
    }

    private async void Start()
    {
        if (FirebaseInitializer.IsInitialized)
        {
            await PlayerDataManager.Instance.Load();
        }
    }
}
