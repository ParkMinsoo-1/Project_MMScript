using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float timeLimit=60;
    public float m_Time;
    public bool m_Running=false;
    private bool tickling = false;

    public void Start()
    {
        m_Running = true;
    }
    public void FixedUpdate()
    {
        if (!m_Running) return;

        m_Time += Time.deltaTime;
        float remainingTime = Mathf.Max(0f, timeLimit - m_Time);

        if (remainingTime >= 60f)
        {
            if (!tickling) { tickling = true; SFXManager.Instance.PlaySFX(3); }
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            float seconds = remainingTime % 60f;
            timer.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            timer.text = $"{remainingTime:00.00}";
        }

        if (remainingTime <= 0f)
        {
            BattleManager.Instance.OnBaseDestroyed(false);
        }
    }

}
