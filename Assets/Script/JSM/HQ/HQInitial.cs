using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQInitial : MonoBehaviour
{
    [SerializeField] private ScrollRect targetScroll;
    [SerializeField] private GameObject[] UIsToDisable;

    private void OnDisable()
    {
        if (targetScroll != null)
        {
            targetScroll.horizontalNormalizedPosition = 0f;
            targetScroll.verticalNormalizedPosition = 1f;
        }

        foreach (var ui in UIsToDisable)
        {
            if (ui != null && ui.activeSelf)
                ui.SetActive(false);
        }
    }
}
