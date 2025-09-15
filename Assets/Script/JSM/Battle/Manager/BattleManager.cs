using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
    public static BattleManager Instance;
    public GameObject gameoverUI;
    public GameObject allyPool;
    public GameObject enemyPool;
    public GameObject allyHeroPool;
    public GameObject enemyHeroPool;
    public GameObject touchBlock;
    public Timer timer;
    public bool isWin;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        gameoverUI.GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void StartBattle(int selectedStageID, List<UnitStats> normalDeck, UnitStats leaderDeck, int stageType = 0)
    {
        //Debug.Log(selectedStageID + "스타트 배틀");
        WaveManager.Instance.stageID = selectedStageID;
        WaveManager.Instance.stageType = stageType;
        UnitSpawner.Instance.Init(normalDeck, leaderDeck);
        Debug.Log(StageDataManager.Instance.GetStageData(selectedStageID).ID);
        SoundManager.Instance.PlayBGM(StageDataManager.Instance.GetStageData(selectedStageID).BGMName);
    }

    public void OnBaseDestroyed(bool isEnemyBase)
    {
        Time.timeScale = 1f;
        isWin =isEnemyBase;
        gameoverUI.SetActive(true);
        touchBlock.SetActive(true);
        if (isEnemyBase)
        {//승리
            gameoverUI.GetComponent<GameOverPanel>().Win();
            enemyPool.SetActive(false);
            enemyHeroPool.SetActive(false);
            SFXManager.Instance.PlaySFX(10);
        }
        else
        {//패배
            gameoverUI.GetComponent<GameOverPanel>().Lose();
            allyPool.SetActive(false);
            allyHeroPool.SetActive(false);
            SFXManager.Instance.PlaySFX(11);
        }
        timer.m_Running = false;
    }
    public void OnClicked()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("MainScene");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isWin)
        {
            if (WaveManager.Instance.stageType == 2)
            {
                PlayerDataManager.Instance.player.goldDungeonData.lastClearStage = Mathf.Max(PlayerDataManager.Instance.player.goldDungeonData.lastClearStage, WaveManager.Instance.stageID);
            }
            if (WaveManager.Instance.stageType != 2 || PlayerDataManager.Instance.player.goldDungeonData.entryCounts > 0)
            {
                StageManager.instance.AddReward(WaveManager.Instance.stageID);
            }
            if (WaveManager.Instance.stageType == 2 && PlayerDataManager.Instance.player.goldDungeonData.entryCounts > 0)
            {
                PlayerDataManager.Instance.player.goldDungeonData.entryCounts -= 1;
            }
            StageManager.instance.ClearStage(WaveManager.Instance.stageID);
        }
        if(!TutorialManager.Instance.isTutoring)
            UIController.Instance.OpenStage();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

