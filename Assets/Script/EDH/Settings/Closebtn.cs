using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Closebtn : MonoBehaviour
{
    public GameObject Pannel;
    public Button CloseButton;
    void Start()
    {
        CloseButton.onClick.AddListener(Close);
    }

    void Close()
    {
        Pannel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
}
