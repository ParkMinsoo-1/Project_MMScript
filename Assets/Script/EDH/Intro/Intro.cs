using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading;

public class Intro : MonoBehaviour
{
    public Button EnterGameButton;
    private bool isLoading = false;

    [SerializeField] private GameObject errorPopup;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button retryBtn;

    private const float timeOut = 30f;

    void Start()
    {
        EnterGameButton.onClick.AddListener(OnClickEnter);
        retryBtn.onClick.AddListener(OnClickRetry);
        errorPopup.SetActive(false);
    }

    private void OnClickEnter()
    {
        if (isLoading) return;
        StartCoroutine(WaitForPlayerDataAndEnter());
    }

    private IEnumerator WaitForPlayerDataAndEnter()
    {
        isLoading = true;
        EnterGameButton.interactable = false; // 클릭 방지
        
        float timer = 0f;

        // 1. Firebase 초기화 대기
        while (!FirebaseInitializer.IsInitialized)
        {
            if (timer > timeOut)
            {
                ShowError("네트워크가 불안정하여 데이터 로딩에 실패했습니다.");
                ResetLoadingState();
                yield break;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        timer = 0f;
        while (!PlayerDataManager.Instance.IsLoaded)
        {
            if (timer > timeOut)
            {
                ShowError("플레이어 데이터를 불러오지 못했습니다.");
                ResetLoadingState();
                yield break;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (PlayerDataManager.Instance.LoadFailed)
        {
            ShowError("저장된 데이터를 불러오지 못했습니다.\n기본 데이터로 시작합니다.");
            yield return new WaitForSeconds(2f);
        }

        EnterGame();
    }

    public void EnterGame()
    {
        ResetLoadingState();

        var player = PlayerDataManager.Instance.player;
        if (player != null && player.tutorialDone.ContainsKey(0) && player.tutorialDone[0] == false)
        {
            TutorialManager.Instance.StartTuto(0);
            return;
        }

        SceneManager.LoadScene("MainScene");
    }
    private void ResetLoadingState()
    {
        isLoading = false;
        EnterGameButton.interactable = true;
       
    }

    private void ShowError(string msg)
    {
        errorText.text = msg;
        errorPopup.SetActive(true);
    }

    private void OnClickRetry()
    {
        errorPopup.SetActive(false);
        if (!isLoading)
        {
            StartCoroutine(WaitForPlayerDataAndEnter());
        }
    }
}
