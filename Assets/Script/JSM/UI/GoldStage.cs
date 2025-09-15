using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;


public class GoldStage : MonoBehaviour
{
    public TextMeshProUGUI goldStageLevel;
    public Button leftButton;
    public Button rightButton;
    public Button infoButton;
    public Button startButton;
    public Dictionary<int, StageData> goldStageData;
    public GameObject goldInfo;
    private int currentGoldStage = 1;

    private void Start()
    {
        startButton.onClick.AddListener(OnClickEnterBattle);
        infoButton.onClick.AddListener(OnClickInfo);
        leftButton.onClick.AddListener(OnClickLeft);
        rightButton.onClick.AddListener(OnClickRight);
        //PlayerDataManager.Instance.player.goldDungeonData.entryCounts.ToString()+" / 3";
        var goldStageDataByType = StageDataManager.Instance.GetAllGoldStageData();
        goldStageData = goldStageDataByType.Values
            .ToDictionary(data => data.Chapter, data => data);
        int targetID = PlayerDataManager.Instance.player.goldDungeonData.lastClearStage;

        // 1. 해당 ID를 가진 항목의 Key 찾기
        var match = goldStageData.FirstOrDefault(pair => pair.Value.ID == targetID);

        if (EqualityComparer<KeyValuePair<int, StageData>>.Default.Equals(match, default))
        {
            currentGoldStage = 1;
        }
        else
        {
            int key = match.Key;
            currentGoldStage = goldStageData.ContainsKey(key + 1) ? key + 1 : key;
        }
        UpdateStageText();
        OnChangeLevel();
    }

    private void OnClickLeft()
    {
        if (currentGoldStage > 1)
        {
            currentGoldStage--;
            UpdateStageText();
            OnChangeLevel();
            goldInfo.GetComponent<GoldStageInfo>().SetGoldInfo(goldStageData[currentGoldStage].ID);
        }
        SFXManager.Instance.PlaySFX(5);
    }

    private void OnClickRight()
    {
        if (currentGoldStage < goldStageData.Count)
        {
            currentGoldStage++;
            UpdateStageText();
            OnChangeLevel();
            goldInfo.GetComponent<GoldStageInfo>().SetGoldInfo(goldStageData[currentGoldStage].ID);
        }
        SFXManager.Instance.PlaySFX(5);

    }

    private void UpdateStageText()
    {
        goldStageLevel.text = $"약탈 레벨 {currentGoldStage}";
    }
    private void OnClickInfo()
    {
        goldInfo.SetActive(true);
        goldInfo.GetComponent<GoldStageInfo>().SetGoldInfo(goldStageData[currentGoldStage].ID);
        SFXManager.Instance.PlaySFX(0);
    }
    private void OnClickEnterBattle()
    {
        int selectedStageID = goldStageData[currentGoldStage].ID;
        if (selectedStageID == -1) return;

        if (PlayerDataManager.Instance.player.goldDungeonData.entryCounts <= 0) return;

        if (!PlayerCheckCurrentDeck.HasUnitsInCurrentDeck())
        {
            //Debug.Log("덱에 유닛이 없습니다.");
            StageManager.instance.PopUp("덱에 유닛이 없습니다.\n유닛을 편성해주세요.");
            return;
        }
        SceneManager.sceneLoaded += OnBattleSceneLoaded;//씬 로드 후에 실행되게 설정
        SceneManager.LoadScene("BattleScene");
        Debug.Log($"{selectedStageID} 입장");
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int selectedStageID = goldStageData[currentGoldStage].ID;
        if (scene.name == "BattleScene")
        {
            //var deck = PlayerDataManager.Instance.player.currentDeck[PlayerDataManager.Instance.player.currentPresetIndex];

            var normalDeck = PlayerDataManager.Instance.player.currentDeck.GetAllNormalUnit();
            var leaderDeck = PlayerDataManager.Instance.player.currentDeck.GetLeaderUnitInDeck();
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            BattleManager.Instance.StartBattle(selectedStageID, normalDeck, leaderDeck, 2);
        }
    }

    private void OnChangeLevel()
    {
        //startButton.interactable = (goldStageData[currentGoldStage].ID <= PlayerDataManager.Instance.player.goldDungeonData.lastClearStage + 1) || currentGoldStage == 1 ? true : false;

        int lastClearStage = PlayerDataManager.Instance.player.goldDungeonData.lastClearStage;

        bool canEnter = currentGoldStage == 1 ||
                        goldStageData[currentGoldStage].ID <= lastClearStage + 1;
        startButton.interactable = canEnter;
        
        leftButton.gameObject.SetActive(currentGoldStage > 1);

        bool canMoveRight = currentGoldStage < goldStageData.Count &&
                            goldStageData[currentGoldStage].ID <= lastClearStage;
        rightButton.gameObject.SetActive(canMoveRight);
    }
}
