using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Openbtn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Pannel;
    public Button OpenButton;
    void Start()
    {
        OpenButton.onClick.AddListener(Open);
    }

    void Open()
    {
        Pannel.SetActive(true);
        SFXManager.Instance.PlaySFX(0);
    }
}
