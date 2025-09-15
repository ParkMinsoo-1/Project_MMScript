using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 스테이지 데이터 로드용
/// </summary>
public class StageDataManager
{
    private static StageDataManager instance;
    public static StageDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StageDataManager();
            }
            
            return instance;
        }
    }
    private Dictionary<int, StageData> stageDic = new();
    private Dictionary<int, StageData> towerStageDic = new();
    private Dictionary<int, StageData> goldDic = new();
    private Dictionary<int, StageData> tutoDic = new();
    public void LoadStageData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/StageData");
        if (csvFile == null)
        {
            Debug.LogError("StageData.csv 파일을 Resources/Data 폴더에 넣었는지 확인하세요.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] tokens = lines[i].Split(',');

            StageData stage = new StageData
            {
                ID = int.Parse(tokens[0]),
                Chapter = int.Parse(tokens[1]),
                BaseDistance = float.Parse(tokens[2]),
                EnemyBaseHP = int.Parse(tokens[3]),
                EnemyUnit1 = int.Parse(tokens[4]),
                EnemyUnit2 = int.Parse(tokens[5]),
                EnemyUnit3 = int.TryParse(tokens[6], out int enemyUnit3) ? enemyUnit3 : 0,
                StageName = tokens[11],
                TeaTime = float.Parse(tokens[12]),
                ResetTime = float.Parse(tokens[13]),
                EnemyHeroID = int.TryParse(tokens[14], out int enemyHeroID) ? enemyHeroID : 0,
                CastleSprite = tokens[15],
                ActionPoint = int.Parse(tokens[16]),

                Type = tokens.Length > 23 && int.TryParse(tokens[23], out var floor) ? floor : -1,
                GimicID = string.IsNullOrWhiteSpace(tokens[24]) ? new() : tokens[24].Split(';').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList(),
                RaceID = tokens.Length > 25 && int.TryParse(tokens[25], out var raceID) ? raceID : -1,
                BGMName = int.Parse(tokens[26]),

            };

            if (!string.IsNullOrWhiteSpace(tokens[7]))
                stage.firstRewardItemIDs = tokens[7].Split(';').Select(int.Parse).ToList();

            if (!string.IsNullOrWhiteSpace(tokens[9]))
                stage.firstRewardAmounts = tokens[9].Split(';').Select(int.Parse).ToList();

            if (!string.IsNullOrWhiteSpace(tokens[8]))
                stage.repeatRewardItemIDs = tokens[8].Split(';').Select(int.Parse).ToList();

            if (!string.IsNullOrWhiteSpace(tokens[10]))
                stage.repeatRewardAmounts = tokens[10].Split(';').Select(int.Parse).ToList();

            switch (stage.Type)
            {
                case 0:
                    stageDic[stage.ID] = stage;
                    break;
                case 1:
                    towerStageDic[stage.ID] = stage;
                    break;
                case 2:
                    goldDic[stage.ID] = stage;
                    break;
                case 99:
                    tutoDic[stage.ID] = stage;
                    break;
            }
        }

        Debug.Log($"스테이지 데이터 로딩 완료: 스테이지 {stageDic.Count}개, 타워 {towerStageDic.Count}개, 골드 {goldDic.Count}개");
    }


    public StageData GetStageData(int id)
    {
        if (stageDic.TryGetValue(id, out var data))
            return data;
        if (towerStageDic.TryGetValue(id, out data))
            return data;
        if (goldDic.TryGetValue(id, out data))
            return data;
        if (tutoDic.TryGetValue(id, out data))
            return data;
        Debug.Log($"스테이지ID {id}에 해당하는 정보를 찾을 올 수 없습니다.");
        return null;
    }

    
    public Dictionary<int, StageData> GetAllTowerStageData() => towerStageDic;

    public Dictionary<int, StageData> GetAllStageData() => stageDic;

    public Dictionary<int, StageData> GetAllGoldStageData() => goldDic;
   
}
