using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseBtn : MonoBehaviour
{
    public Button pauseBtn;
    public Button resumeBtn;
    public Button retreatBtn;

    private float gameSpeed=1;
    public void Awake()
    {
        pauseBtn.onClick.AddListener(Pause);
        resumeBtn.onClick.AddListener(Resume);
        retreatBtn.onClick.AddListener(Retreat);
    }
    private void Pause()
    {
        gameSpeed = Time.timeScale;
        Time.timeScale = 0;
        SFXManager.Instance.PlaySFX(0);
    }
    private void Resume()
    {
        Time.timeScale = gameSpeed;
        SFXManager.Instance.PlaySFX(0);
    }
    private void Retreat()
    {
        SFXManager.Instance.PlaySFX(0);
        Time.timeScale = 1;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainScene");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!TutorialManager.Instance.isTutoring)
            UIController.Instance.OpenStage();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
