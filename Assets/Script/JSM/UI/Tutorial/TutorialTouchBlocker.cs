using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTouchBlocker : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0.0f;
        TutorialManager.Instance.isPlaying = true;
    }
    private void OnDisable()
    {
        Time.timeScale = 1.0f;
        TutorialManager.Instance.isPlaying = false;
    }
}
