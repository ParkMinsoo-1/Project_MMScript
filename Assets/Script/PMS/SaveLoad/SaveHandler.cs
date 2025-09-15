using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveHandler : MonoBehaviour
{
    private static SaveHandler instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PlayerDataManager.Instance.Save();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerDataManager.Instance.Save();
    }
}
