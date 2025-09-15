using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
   // [SerializeField] private TextMeshProUGUI timerText;
    

    private Player player => PlayerDataManager.Instance.player;
    private Coroutine refreshAP;

    private void OnEnable()
    {
        PlayerCurrencyEvent.OnActionPointChange += UpdateActionPoint;
        UpdateActionPoint(player.actionPoint);
        refreshAP = StartCoroutine(RefreshActionPointRoutine());

    }

    private void OnDisable()
    {
        PlayerCurrencyEvent.OnActionPointChange -= UpdateActionPoint;
        if(refreshAP != null)
        {
            StopCoroutine(refreshAP);
        }
    }

    private void UpdateActionPoint(int value)
    {
        actionPointText.text = $"{value}/{player.maxActionPoint}";
    }

    private IEnumerator RefreshActionPointRoutine()
    {
        while (true)
        {
            int nextRecover = PlayerDataManager.Instance.RefreshActionPoint();

            if (player.actionPoint >= 100)
            {
                //timerText.text = "MAX";
            }
            else
            {
                TimeSpan span = TimeSpan.FromSeconds(nextRecover);
                //timerText.text = $"{span.Minutes:D2}:{span.Seconds:D2}";
            }
            actionPointText.text = $"{player.actionPoint}/{player.maxActionPoint}";

            yield return new WaitForSeconds(1f);
        }
    }
}
