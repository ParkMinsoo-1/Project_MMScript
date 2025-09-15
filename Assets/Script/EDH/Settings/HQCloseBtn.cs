using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBtn : MonoBehaviour
{
    public Button Closebutton;
    public GameObject BuildMenu;
    public GameObject BuildConfirm;
        
    void Start()
    {
        Closebutton.onClick.AddListener(close);
    }
    void close()
    {
        BuildMenu.SetActive(false);
        BuildConfirm.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
  
}
