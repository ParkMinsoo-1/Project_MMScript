using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillInfo : MonoBehaviour
{
    [SerializeField] private GameObject skillInfoPanel;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private GameObject blocker;

    public void ShowSkillInfo(SkillData data)
    {
        skillName.text = data.Name;
        skillDescription.text = data.Description;
        skillInfoPanel.SetActive(true);
        blocker.SetActive(true);
    }

    public void HideSkillInfo()
    {
        skillInfoPanel.SetActive(false);
        blocker.SetActive(false);
    }

    
}
