using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [Header("챕터")]
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private TextMeshProUGUI chapterText;

    [Header("스테이지")]
    [SerializeField] private Transform nodeParent;
    [SerializeField] private StageNode stageNodePrefab;
    [SerializeField] private GameObject stageInfo;
    [SerializeField] private GameObject uiStage;
    [SerializeField] private GameObject pannel;

    public bool stageCheat = false;

    public Action<int> SetStageInfo;

    private int currentChapter = 1;
    private int selectedStageID = -1;

    public int lastSessionClearedStageID = -1;
    public int lastSessionClearedStageType = 0;

    public static StageManager instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        StageDataManager.Instance.LoadStageData();

    }

    public void Start()
    {
    }

    public void Init()
    {
        

        var clearedStageIDs = PlayerDataManager.Instance.player.clearedStageIDs;
        var allStageData = new Dictionary<int, StageData>();
        foreach (var kv in StageDataManager.Instance.GetAllStageData()) allStageData[kv.Key] = kv.Value;
        foreach (var kv in StageDataManager.Instance.GetAllTowerStageData()) allStageData[kv.Key] = kv.Value;
        foreach (var kv in StageDataManager.Instance.GetAllGoldStageData()) allStageData[kv.Key] = kv.Value;

        if (lastSessionClearedStageID != -1 && allStageData.TryGetValue(lastSessionClearedStageID, out var lastStageData))
        {

            switch (lastSessionClearedStageType)
            {
                case 0:
                    currentChapter = lastStageData.Chapter;
                    StageInit.instance.OnMainBtn();
                    break;
                case 1:
                    currentChapter = lastStageData.Chapter;
                    StageInit.instance.OnTowerBtn();
                    break;
                case 2:
                    StageInit.instance.OnGoldBtn();
                    break;
            }
        }
        else
        {
            StageInit.instance.OnMainBtn();
        }
        UpdateStageUI();

        prevBtn.onClick.AddListener(() => ChangeChapter(-1));
        nextBtn.onClick.AddListener(() => ChangeChapter(1));
    }

    public void ChangeChapter(int delta)
    {
        currentChapter += delta;
        SFXManager.Instance.PlaySFX(5);
        var stageDataDic = StageDataManager.Instance.GetAllStageData();
        int minChapter = stageDataDic.Values.Min(x => x.Chapter);
        if (minChapter <= 0)
        {
            minChapter = 1;
        }
        int maxChapter = stageDataDic.Values.Max(x => x.Chapter);

        currentChapter = Mathf.Clamp(currentChapter, minChapter, maxChapter);
        //Debug.Log("챕터변경");
        UpdateStageUI();
    }

    private void UpdateStageUI()
    {
        foreach (Transform child in nodeParent)
        {
            Destroy(child.gameObject);
        }

        chapterText.text = $"Chpater {currentChapter}";
        var stageDataDic = StageDataManager.Instance.GetAllStageData(); // 딕셔너리로 만들어진 모든 스테이지 데이터. 딕셔너리 키 값은 스테이지 아이디

        var chapterStages = stageDataDic.Values
            .Where(x => x.Chapter == currentChapter)
            .OrderBy(x => x.ID)
            .ToList();

        int nodeCount = chapterStages.Count;

        // 부모 영역의 크기
        RectTransform parentRect = nodeParent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float marginX = 50f; // 왼쪽/오른쪽 여유
        float availableWidth = parentWidth - marginX * 2;

        float spacingX = availableWidth / nodeCount;

        float topY = parentHeight * 0.6f;
        float bottomY = parentHeight * 0.1f;

        for (int i = 0; i < nodeCount; i++)
        {
            var stage = chapterStages[i];
            var node = Instantiate(stageNodePrefab, nodeParent);
            node.Init(stage);

            bool isClear = PlayerDataManager.Instance.HasClearedStage(stage.ID);
            node.SetClear(isClear);

            bool canEnter = CanEnterStage(stage.ID);
            node.SetInteractable(canEnter);

            RectTransform nodePosition = node.GetComponent<RectTransform>();
            if (nodePosition != null)
            {
                float x = marginX + spacingX * i;
                float y = (i % 2 == 0) ? topY : bottomY;

                if (i == nodeCount - 1)
                {
                    y = parentHeight * 0.28f;
                    x += 60;

                    nodePosition.sizeDelta *= 1.5f;

                    var img = node.GetComponent<Image>();
                    if (img != null)
                    {
                        var originalColor = img.color;
                        img.color = new Color(1f, 0f, 0f, originalColor.a);
                    }
                }

                nodePosition.anchoredPosition = new Vector2(x, y);
            }
        }

        var allStageData = StageDataManager.Instance.GetAllStageData();
        int minChapter = allStageData.Values.Min(x => x.Chapter);
        int maxChapter = allStageData.Values.Max(x => x.Chapter);

        prevBtn.gameObject.SetActive(currentChapter > minChapter);
        nextBtn.gameObject.SetActive(currentChapter < maxChapter);
    }


    public void SelectStage(int stageID)
    {
        if (!CanEnterStage(stageID))
        {
            PopUp("이전 스테이지를 클리어해야 합니다.");
            SFXManager.Instance.PlaySFX(6);
            return;
        }
        selectedStageID = stageID;
        Debug.Log($"스테이지 {stageID} 선택됨");
        stageInfo.SetActive(true);
        uiStage.GetComponent<UIStageInfo>().SetStageInfo(stageID);

    }

    public void OnClickEnterBattle()
    {
        if (selectedStageID == -1) return;

        if (!CanEnterStage(selectedStageID))
        {
            //Debug.Log("이전 스테이지를 클리어해야 입장이 가능합니다.");
            return;
        }

        if (!PlayerCheckCurrentDeck.HasUnitsInCurrentDeck())
        {
            //Debug.Log("덱에 유닛이 없습니다.");
            PopUp("덱에 유닛이 없습니다.\n유닛을 편성해주세요.");
            SFXManager.Instance.PlaySFX(6);
            return;
        }

        int stageAP = StageDataManager.Instance.GetStageData(selectedStageID).ActionPoint;
        if (PlayerDataManager.Instance.player.actionPoint < stageAP)
        {
            PopUp("행동력이 부족합니다.");
            SFXManager.Instance.PlaySFX(6);
            return;
        }

        SceneManager.sceneLoaded += OnBattleSceneLoaded;//씬 로드 후에 실행되게 설정
        SceneManager.LoadScene("BattleScene");
        Debug.Log($"{selectedStageID} 입장");
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            //var deck = PlayerDataManager.Instance.player.currentDeck[PlayerDataManager.Instance.player.currentPresetIndex];

            var normalDeck = PlayerDataManager.Instance.player.currentDeck.GetAllNormalUnit();
            var leaderDeck = PlayerDataManager.Instance.player.currentDeck.GetLeaderUnitInDeck();
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            BattleManager.Instance.StartBattle(selectedStageID, normalDeck, leaderDeck);
        }
    }

    public void ClearStage(int id) // 클리어 스테이지 플레이어에 추가. 배틀 끝나고 불러오기.
    {
        int ap = StageDataManager.Instance.GetStageData(id).ActionPoint;
        PlayerDataManager.Instance.UseActionPoint(ap);

        var stageData = StageDataManager.Instance.GetStageData(id);

        switch (stageData.Type)
        {
            case 0:
                QuestEvent.OnMainChapterClear?.Invoke();
                break;
            case 1:
                int raceID = stageData.RaceID;
                if (TowerManager.Instance.CanEnterTower(raceID))
                {
                    TowerManager.Instance.DecreaseEntryCount(raceID);
                }
                QuestEvent.OnTowerClear?.Invoke();
                break;
            case 2:
                QuestEvent.OnLooting?.Invoke();
                break;
        }

        lastSessionClearedStageID = id;
        lastSessionClearedStageType = stageData.Type;
        Debug.Log($"{lastSessionClearedStageID}");
        Debug.Log($"{lastSessionClearedStageType}");

        PlayerDataManager.Instance.ClearStage(id);
        PlayerDataManager.Instance.Save();
    }

    public void AddReward(int id) // 스테이지 클리어 보상
    {
        var stageData = StageDataManager.Instance.GetStageData(id);

        bool firstClear = !PlayerDataManager.Instance.HasClearedStage(id);

        if (firstClear)
        {
            for (int i = 0; i < stageData.firstRewardItemIDs.Count; i++)
            {
                int itemID = stageData.firstRewardItemIDs[i];
                int amount = stageData.firstRewardAmounts[i];
                GiveReward(itemID, amount);
            }
        }

        else
        {
            for (int i = 0; i < stageData.repeatRewardItemIDs.Count; i++)
            {
                int itemID = stageData.repeatRewardItemIDs[i];
                int amount = stageData.repeatRewardAmounts[i];
                GiveReward(itemID, amount);
            }
        }

    }

    private void GiveReward(int itemID, int amount)
    {
        switch (itemID)
        {
            case 101:
                PlayerDataManager.Instance.AddGold(amount);
                break;

            case 102:
                PlayerDataManager.Instance.AddTicket(amount);
                break;

            case 103:
                PlayerDataManager.Instance.AddBluePrint(amount);
                break;
            case 104:
                PlayerDataManager.Instance.AddTribute(amount);
                break;
        }


    }

    private bool CanEnterStage(int stageID)
    {
        if (stageCheat) return true;
        var stageDic = StageDataManager.Instance.GetAllStageData();

        if (!stageDic.TryGetValue(stageID, out var stageData))
            return false;

        var chapterStage = stageDic.Values
            .Where(x => x.Chapter == stageData.Chapter)
            .OrderBy(x => x.ID)
            .ToList();

        if (chapterStage.First().ID == stageID)
        {
            int currentChapter = stageData.Chapter;
            int prevChapter = currentChapter - 1;

            if (!stageDic.Values.Any(x => x.Chapter == prevChapter))
                return true;

            var prevChapterStages = stageDic.Values
                .Where(x => x.Chapter == prevChapter)
                .Select(x => x.ID)
                .ToList();

            return prevChapterStages.All(id => PlayerDataManager.Instance.HasClearedStage(id));
        }

        for (int i = 1; i < chapterStage.Count; i++)
        {
            if (chapterStage[i].ID == stageID)
            {
                int prevStageID = chapterStage[i - 1].ID;
                return PlayerDataManager.Instance.HasClearedStage(prevStageID);
            }

        }
        return false;

    }

    public void PopUp(string txt)
    {
        TextMeshProUGUI text = pannel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = txt;
        pannel.SetActive(true);
    }

}
