using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SaveTime
{
    public int gold = 10000;
    public int ticket = 10;
    public int specTicket = 0;
    public int bluePrint = 5;
    public int tribute = 10;
    public int certi = 0;

    public List<int> myUnitIDs = new();
    public DeckData currentDeck = new();
    public List<DeckData> preset = new List<DeckData>();


    public int currentPresetIndex;
    public List<int> clearedStageIDs = new();
    public int lastClearedStageID;

    public int actionPoint = 100;
    public int maxActionPoint = 100;
    public long lastActionPointTime;

    public long lastDailyQuestTime = 0; // 일일 퀘스트 초기화 시간
    public long lastWeeklyQuestTime = 0; // 주간 퀘스트 초기화 시간

    public List<PlayerQuestData> playerQuest = new();
    public GoldDungeonData goldDungeonData = new();

    public List<BuildingState> buildingsList = new();
    public Dictionary<int, HashSet<int>> selectedGospelIDsByBuildID = new();
    public Dictionary<int, UnitStats> buildingBuffs = new();
    public Dictionary<int, bool> tutorialDone = new Dictionary<int, bool>() {
            { 0, false },
            { 1, false },
            { 2, false },
            { 3, false }
        };
    //public int pickPoint = 0;

    public PlayerTowerData towerData = new();
    public long lastSaveTime { get; set; }

    public Dictionary<int, int> itemPurchaseLeft = new Dictionary<int, int>();
    public long lastPurchaseResetTime;

    public static Player CreateDefaultPlayer()
    {
        var player = new Player();
        //for (int i = 0; i < 3; i++)
        //{
        //    player.preset.Add(new DeckData());
        //}

        player.preset = new List<DeckData>()
        {
            new DeckData(),
            new DeckData(),
            new DeckData()
        };

        if (!player.myUnitIDs.Contains(1001))
            player.myUnitIDs.Add(1001);

        if (!player.myUnitIDs.Contains(1002))
            player.myUnitIDs.Add(1002);

        if (!player.myUnitIDs.Contains(3001))
            player.myUnitIDs.Add(3001);

        player.preset[0].SetLeaderUnit(3001);

        player.preset[0].AddNormalUnit(1001);
        player.preset[0].AddNormalUnit(1002);

        player.currentPresetIndex = 0;
        player.currentDeck = DeckManager.Instance.CloneDeck(player.preset[0]);

        return player;
    }

    public void AddUnit(int unitID)
    {
        if (!myUnitIDs.Contains(unitID))
        {
            myUnitIDs.Add(unitID);
        }
    }

}

public class PlayerQuestData
{
    public int QuestID; // 퀘스트 아이디
    public int CurrentValue; // 퀘스트 데이터의 컨디션 벨류
    public bool IsCompleted; // 퀘스트 완료 여부
    public bool IsReward; // 퀘스트 보상 획득 여부
}

public class GoldDungeonData
{
    public int lastClearStage = 0;  //클리어한 가장 높은 스테이지
    public int entryCounts = 3;     //보상획득가능 횟수
    public long lastResetTime = 0;
}
