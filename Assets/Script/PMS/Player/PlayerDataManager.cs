using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;

public class PlayerDataManager
{
    private static PlayerDataManager instance;

    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerDataManager();
            }
            return instance;
        }
    }

    private Player Player = new Player();
    public Player player => Player;
    public bool IsLoaded { get; private set; } = false;
    public bool LoadFailed { get; private set; } = false;

    public async void Save() // 플레이어 데이터 저장
    {
        Debug.Log("저장 직전 tutorialDone Count: " + player.tutorialDone.Count);
        bool save = await SaveLoadManager.Save("playerJson", player);
        if (save)
        {
            Debug.Log("저장 성공");
        }
    }
    public async Task Load() // 플레이어 데이터 불러오기
    {
        Player load = await SaveLoadManager.Load<Player>("playerJson", null, ignoreLocal: true);
        if (load != null)
        {
            Player = load;
            IsLoaded = true;
            Debug.Log("불러오기 성공");
        }
        else
        {
            Player = Player.CreateDefaultPlayer();
            IsLoaded = true;
            LoadFailed = (load == null);
        }

        var allItems = new List<Item>(ItemListLoader.Instance.GetAllList().Values);
        await DailyReset(allItems);

    }

    public void AddGold(int amount)
    {
        player.gold += amount;
        PlayerCurrencyEvent.OnGoldChange?.Invoke(player.gold);
        Save();
    }

    public void AddTicket(int amount)
    {
        player.ticket += amount;
        PlayerCurrencyEvent.OnTicketChange?.Invoke(player.ticket);
        Save();
    }

    public void AddBluePrint(int amount)
    {
        player.bluePrint += amount;
        PlayerCurrencyEvent.OnBluePrintChange?.Invoke(player.bluePrint);
        Save();
    }

    public void AddActionPoint(int amount)
    {
        player.actionPoint += amount;
        PlayerCurrencyEvent.OnActionPointChange?.Invoke(player.actionPoint);
        Save();
    }

    public void AddTribute(int amount)
    {
        player.tribute += amount;
        PlayerCurrencyEvent.OnTributeChange?.Invoke(player.tribute);
        Save();
    }

    public void AddCerti(int amount)
    {
        player.certi += amount;
        PlayerCurrencyEvent.OnCertiChange?.Invoke(player.certi);
        Save();
    }
    public void AddSpecT(int amount)
    {
        player.specTicket += amount;
        PlayerCurrencyEvent.OnSpecTicketChange?.Invoke(player.specTicket);
        Save();
    }

    public bool AddUnit(int id)
    {
        if (player.myUnitIDs.Contains(id))
        {
            //Debug.Log("이미 동일한 유닛이 존재합니다."+id);
            return false;
        }
        Save();
        player.myUnitIDs.Add(id);
        return true;
    }

    public bool HasUnit(int id)
    {
        return player.myUnitIDs.Contains(id);
    }

    public List<UnitStats> GetAllNormalUnit()
    {
        return player.myUnitIDs
            .Select(id => UnitDataManager.Instance.GetStats(id))
            .Where(stat => stat != null && !stat.IsHero)
            .ToList();
    }

    public List<UnitStats> GetAllLeaderUnit()
    {
        return player.myUnitIDs
            .Select(id => UnitDataManager.Instance.GetStats(id))
            .Where(stat => stat != null && stat.IsHero)
            .ToList();
    }

    public List<UnitStats> GetAllUnit()
    {
        return player.myUnitIDs
            .Select(id => UnitDataManager.Instance.GetStats(id))
            .Where(stat => stat != null)
            .ToList();
    }

    public void ClearStage(int stageID)
    {
        if (!player.clearedStageIDs.Contains(stageID))
        {
            player.clearedStageIDs.Add(stageID);
        }

        var stage = StageDataManager.Instance.GetStageData(stageID);
        if (stage.Type == 1)
        {
            int raceID = stage.RaceID;
            int floor = stage.Chapter;

            if(!player.towerData.lastClearFloor.ContainsKey(raceID) || player.towerData.lastClearFloor[raceID] < floor)
            {
                player.towerData.lastClearFloor[raceID] = floor;
            }
        }

        player.lastClearedStageID = stageID;
    }

    public int RefreshActionPoint()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long last = player.lastActionPointTime;

        if (last == 0)
        {
            //player.lastActionPointTime = now;
            return 60;
        }

        long elapsedSeconds = now - last;
        int recovered = (int)(elapsedSeconds / 60); // 1분에 행동력 1씩 회복

        if (recovered > 0)
        {
            player.actionPoint = Mathf.Min(player.actionPoint + recovered, player.maxActionPoint); // 최대치 100과 비교해서 낮은쪽 사용.
            player.lastActionPointTime += recovered * 60;

            PlayerCurrencyEvent.OnActionPointChange?.Invoke(player.actionPoint);
        }
        return NextRecoverTime();
    }
    public int NextRecoverTime()
    {
        if(player.actionPoint >= player.maxActionPoint)
        {
            return 0;
        }

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long last = player.lastActionPointTime;
        long elapsed = now - last;

        return (int)(60 - (elapsed % 60));
    }

    public bool UseActionPoint(int amount)
    {
        RefreshActionPoint();

        if (player.actionPoint >= amount)
        {
            player.actionPoint -= amount;

            if (player.actionPoint < player.maxActionPoint && player.lastActionPointTime == 0)
            {
                player.lastActionPointTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            PlayerCurrencyEvent.OnActionPointChange?.Invoke(player.actionPoint);
            QuestEvent.UseActionPoint?.Invoke(amount); // 퀘스트 이벤트
            return true;
        }

        else
        {
            Debug.Log("행동력이 부족합니다.");
            return false;
        }
    }

    public bool UseGold(int amount)
    {
        if (player.gold >= amount)
        {
            player.gold -= amount;
            PlayerCurrencyEvent.OnGoldChange?.Invoke(player.gold);
            return true;
        }

        else
        {
            Debug.Log("골드가 부족합니다.");
            return false;
        }
    }

    public bool UseTicket(int amount)
    {
        if (player.ticket >= amount)
        {
            player.ticket -= amount;
            PlayerCurrencyEvent.OnTicketChange?.Invoke(player.ticket);
            return true;
        }
        else
        {
            Debug.Log("티켓이 부족합니다.");
            return false;
        }
    }
    public bool UseSpecTicket(int amount)
    {
        if (player.specTicket >= amount)
        {
            player.specTicket -= amount;
            PlayerCurrencyEvent.OnSpecTicketChange?.Invoke(player.specTicket);
            return true;
        }
        else
        {
            Debug.Log("특수모집티켓이 부족합니다.");
            return false;
        }
    }
    public bool UseTribute(int amount)
    {
        if (player.tribute >= amount)
        {
            player.tribute -= amount;
            PlayerCurrencyEvent.OnTributeChange?.Invoke(player.tribute);
            return true;
        }
        else
        {
            Debug.Log("공물이 부족합니다.");
            return false;
        }
    }

    public bool UseBluePrint(int amount)
    {
        if(player.bluePrint >= amount)
        {
            player.bluePrint -= amount;
            PlayerCurrencyEvent.OnBluePrintChange?.Invoke(player.bluePrint);
            return true;
        }

        else
        {
            Debug.Log("설계도가 부족합니다.");
            return false;
        }
    }

    public bool UseCerti(int amount)
    {
        if (player.certi >= amount)
        {
            player.certi -= amount;
            PlayerCurrencyEvent.OnCertiChange?.Invoke(player.certi);
            return true;
        }

        else
        {
            Debug.Log("증명서가 부족합니다.");
            return false;
        }
    }


    public bool HasClearedStage(int stageID)
    {
        return player.clearedStageIDs.Contains(stageID);
    }

    public PlayerQuestData GetQuestProgress(int questID)
    {
        return player.playerQuest.Find(quest => quest.QuestID == questID);
    }
    public bool IsQuestCompleted(int questID)
    {
        var progress = GetQuestProgress(questID);
        return progress != null && progress.IsCompleted;
    }
    public bool HasReceivedQuestReward(int questID)
    {
        var progress = GetQuestProgress(questID);
        return progress != null && progress.IsReward;
    }

    public void AddQuestProgress(ConditionType conditionType, int value) // 플레이어 퀘스트 진행도 추적
    {
        Debug.Log($"[퀘스트 진행도 추가] 호출됨: {conditionType} +{value}");
        foreach (var progress in player.playerQuest)
        {
            QuestData quest = QuestDataManager.Instance.GetQuestID(progress.QuestID);
            if(quest == null || progress.IsCompleted)
            {
                continue;
            }

            if(quest.ConditionType == conditionType)
            {
                progress.CurrentValue += value;
                Debug.Log($"{quest.Title} 진행도 {progress.CurrentValue} / {quest.ConditionValue}");

                if(progress.CurrentValue >= quest.ConditionValue)
                {
                    progress.CurrentValue = quest.ConditionValue;
                    progress.IsCompleted = true;
                    Debug.Log($"{quest.Title} 완료");
                }
            }
        }
    }
    
    public bool TryGetQuestReward(int questID)
    {
        var progress = GetQuestProgress(questID);
        var quest = QuestDataManager.Instance.GetQuestID(questID);

        if(progress == null || !progress.IsCompleted || progress.IsReward)
        {
            return false;
        }

        switch (quest.RewardType)
        {
            case RewardType.Gold:
                AddGold(quest.RewardValue);
                break;
            case RewardType.Ticket:
                AddTicket(quest.RewardValue); 
                break;
            case RewardType.BluePrint:
                AddBluePrint(quest.RewardValue);
                break;
        }

        progress.IsReward = true;
        Debug.Log($"{quest.RewardType} {quest.RewardValue} 획득");
        return true;
    }
    public async Task DailyReset(List<Item> allItems)
    {
        try
        {
            DateTime serverUtc = await ServerTime.GetServerTime();
            DateTime serverKst = ServerTime.ConvertUtcToKst(serverUtc);
            DateTime todayMidnightKst = ServerTime.GetLastMidnightKst(serverKst);

            ResetDailyPurchase(allItems, todayMidnightKst);
            await ResetGoldDungeonEntry(todayMidnightKst);
            await QuestManager.Instance.CheckReset(serverUtc);
            await TowerManager.Instance.ResetTowerEntry(todayMidnightKst);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"서버 시간 기반 초기화 실패: {e.Message}");
        }
    }

    private void ResetDailyPurchase(List<Item> allItems, DateTime todayMidnightKst)
    {
        DateTime lastResetKst = DateTimeOffset.FromUnixTimeSeconds(player.lastPurchaseResetTime).UtcDateTime.AddHours(9);
        if (lastResetKst < todayMidnightKst)
        {
            player.itemPurchaseLeft.Clear();
            foreach (var item in allItems)
            {
                player.itemPurchaseLeft[item.ID] = item.DailyBuy;
            }
            player.lastPurchaseResetTime = new DateTimeOffset(todayMidnightKst).ToUnixTimeSeconds();
            Save();
            Debug.Log("서버 시간 기준 상점 구매 초기화 완료");
        }
    }

    private async Task ResetGoldDungeonEntry(DateTime todayMidnightKst)
    {
        var goldDungeon = player.goldDungeonData;
        DateTime lastResetKst = DateTimeOffset.FromUnixTimeSeconds(goldDungeon.lastResetTime).UtcDateTime.AddHours(9);

        if (lastResetKst < todayMidnightKst)
        {
            goldDungeon.entryCounts = 3;
            goldDungeon.lastResetTime = new DateTimeOffset(todayMidnightKst).ToUnixTimeSeconds();
            Save();
            Debug.Log("서버 시간 기준 골드던전 입장횟수 초기화 완료");
        }
    }

}
