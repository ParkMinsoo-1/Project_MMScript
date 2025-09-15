using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingSet : MonoBehaviour
{
    public Button Openbtn;
    public Button Closebtn;

    public GameObject SettingPanel;

    private void Start()
    {
        Openbtn.onClick.AddListener(OpenSetting);
        Closebtn.onClick.AddListener(CloseSetting);
    }
    void OpenSetting()
    {
        SettingPanel.SetActive(true);
        SFXManager.Instance.PlaySFX(0);
    }

    void CloseSetting()
    {
        SettingPanel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
}
