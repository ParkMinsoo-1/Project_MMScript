using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerManager
{
    private static TowerManager instance;
    public static TowerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TowerManager();
            }
            return instance;
        }
    }

    public readonly int maxEntryCounts = 3;
    private PlayerTowerData data => PlayerDataManager.Instance.player.towerData;

    private int currentStageID;

    public int GetClearedFloor(int raceID)
    {
        return PlayerDataManager.Instance.player.towerData.lastClearFloor.TryGetValue(raceID, out var floor) ? floor : 0;
    }

    public StageData GetCurrentFloorStage(int raceID)
    {
        int floor = GetClearedFloor(raceID);
        int nextFloor = floor + 1;

        // 해당 종족의 모든 타워 스테이지 중에서
        var stage = StageDataManager.Instance.GetAllTowerStageData()
            .Values
            .Where(s => s.RaceID == raceID)
            .OrderBy(s => s.Chapter)
            .FirstOrDefault(s => s.Chapter == nextFloor); // 클리어 층, 없으면 1층

        return stage;
    }
    public bool CanEnterTower(int raceID)
    {
        //ResetEntryCounts();

        if (!data.entryCounts.ContainsKey(raceID))
        {
            data.entryCounts[raceID] = maxEntryCounts;
        }
        return data.entryCounts[raceID] > 0;
    }

    public void DecreaseEntryCount(int raceID)
    {
        //ResetEntryCounts();

        if (!data.entryCounts.ContainsKey(raceID))
        {
            data.entryCounts[raceID] = maxEntryCounts;
        }
        if (data.entryCounts[raceID] > 0)
        {
            data.entryCounts[raceID]--;
        }
    }

    //public bool EnterTower(int raceID)
    //{
    //    if (!CanEnterTower(raceID))
    //    {
    //        return false;
    //    }

    //    data.entryCounts[raceID]--;
    //    return true;
    //}

    public int GetEnterCount(int raceID)
    {
        //ResetEntryCounts();

        if (!data.entryCounts.ContainsKey(raceID))
        {
            data.entryCounts[raceID] = maxEntryCounts;
        }
        return data.entryCounts[raceID];
    }

    private bool CheckRaceInDeck(int raceID)
    {
        //var deck = PlayerDataManager.Instance.player.currentDeck[PlayerDataManager.Instance.player.currentPresetIndex];

        var normalUnit = PlayerDataManager.Instance.player.currentDeck.GetAllNormalUnit();
        var leaderUnit = PlayerDataManager.Instance.player.currentDeck.GetLeaderUnitInDeck();

        bool hasUnit = (normalUnit != null && normalUnit.Count > 0) || leaderUnit != null;
        if (!hasUnit) return false;

        if (normalUnit != null && normalUnit.Any(unit => unit.RaceID != raceID))
            return false;

        if (leaderUnit != null && leaderUnit.RaceID != raceID)
            return false;

        return true;
    }

    public void EnterBattle(int stageID, Action raceMatch = null)
    {
        var stage = StageDataManager.Instance.GetStageData(stageID);
        int raceID = stage.RaceID;

        if (!CanEnterTower(raceID))
        {
            Debug.Log("입장 횟수가 부족합니다.");
            // 입장 불가 팝업으로
            return;
        }

        if (!PlayerCheckCurrentDeck.HasUnitsInCurrentDeck())
        {
            //Debug.Log("덱에 유닛이 없습니다.");
            StageManager.instance.PopUp("덱에 유닛이 없습니다.\n유닛을 편성해주세요.");
            SFXManager.Instance.PlaySFX(6);
            return;
        }
        //bool entered = EnterTower(raceID);
        //if (!entered)
        //{
        //    Debug.Log("입장 실패");
        //    return;
        //}

        if (!CheckRaceInDeck(raceID))
        {
            Debug.Log($"{RaceManager.GetNameByID(raceID)}와 동일한 유닛만 출전할 수 있습니다.");
            raceMatch?.Invoke();
            return;
        }

        currentStageID = stageID;  // 멤버 변수로 저장

        SceneManager.sceneLoaded += OnBattleSceneLoaded;
        SceneManager.LoadScene("BattleScene");
        Debug.Log($"{stageID} 입장");
    }

    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            //var deck = PlayerDataManager.Instance.player.currentDeck[PlayerDataManager.Instance.player.currentPresetIndex];

            var normalDeck = PlayerDataManager.Instance.player.currentDeck.GetAllNormalUnit();
            var leaderDeck = PlayerDataManager.Instance.player.currentDeck.GetLeaderUnitInDeck();
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;

            BattleManager.Instance.StartBattle(currentStageID, normalDeck, leaderDeck, 1);
        }
    }

    //private void ResetEntryCounts()
    //{
    //    if (!ResetTowerEntry()) return;

    //    foreach (var key in data.entryCounts.Keys.ToList())
    //    {
    //        data.entryCounts[key] = maxEntryCounts;
    //    }
    //    DateTime now = DateTime.UtcNow.AddHours(9);
    //    DateTime todayReset = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
    //    data.lastResetTime = todayReset;
    //    Debug.Log("타워 입장 횟수 초기화 완료");
    //}

    public async Task ResetTowerEntry(DateTime todayMidnightKst)
    {
        var data = PlayerDataManager.Instance.player.towerData;
        DateTime lastResetKst = DateTimeOffset.FromUnixTimeSeconds(data.lastResetTime).UtcDateTime.AddHours(9);

        if (lastResetKst < todayMidnightKst)
        {
            foreach (var key in data.entryCounts.Keys.ToList())
            {
                data.entryCounts[key] = maxEntryCounts;
            }
            data.lastResetTime = new DateTimeOffset(todayMidnightKst).ToUnixTimeSeconds();
            PlayerDataManager.Instance.Save();
            Debug.Log("서버 시간 기준 타워 입장 횟수 초기화 완료");
        }
    }
}
