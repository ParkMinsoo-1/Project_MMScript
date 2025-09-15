using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MainInit : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject settingPanel;

    private void Start()
    {
        BackHandlerEntry entry;
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => questPanel.activeInHierarchy,
           onBack: () => questPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => settingPanel.activeInHierarchy,
           onBack: () => settingPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }
    private void OnDisable()
    {
        questPanel.SetActive(false);
        settingPanel.SetActive(false);
    }
}
