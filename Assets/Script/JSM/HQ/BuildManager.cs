using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingState
{
    public BuildingData buildingData;
    public int level;
}

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public Dictionary<int, BuildingData> buildingDict = new(); // ID 기반 딕셔너리
    public int count = 5;

    private BuildSlotUI selectedSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < count; i++)
        {
            PlayerDataManager.Instance.player.buildingsList.Add(new BuildingState { buildingData = null, level = 0 });
        }

        TagManager.LoadCSV();
        RaceManager.LoadCSV();
        //BuffManager.InitBuffs();//버프 초기화 함수, 플레이어 데이터 저장시 위치 바꿔야함
    }

    private void Start()
    {
        LoadBuildings();
    }

    public void SelectSlot(BuildSlotUI slot)
    {
        selectedSlot = slot;
    }

    public void BuildSelected(int buildingID)
    {
        if (selectedSlot == null) return;

        BuildingData building = GetBuildingData(buildingID);
        if (building == null)
        {
            Debug.LogError($"[BuildManager] ID {buildingID}에 해당하는 건물이 없습니다.");
            return;
        }

        selectedSlot.Build(building, 1);
        PlayerDataManager.Instance.player.buildingsList[selectedSlot.slotID].buildingData = building;
        PlayerDataManager.Instance.player.buildingsList[selectedSlot.slotID].level = 1;
        selectedSlot = null;
    }

    private void LoadBuildings()
    {
        buildingDict.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>("Data/BuildingData");
        if (csvFile == null)
        {
            Debug.LogError("[BuildManager] Resources/Data/BuildingData.csv 파일을 찾을 수 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var parts = lines[i].Split(',');
            int id = int.Parse(parts[0].Trim());
            string displayName = parts[1].Trim();
            string imageName = parts[2].Trim();
            int raceId = int.TryParse(parts[3].Trim(), out var result) ? result:-1;
            int gold = int.Parse(parts[4].Trim());
            int blueprint = int.Parse(parts[5].Trim());

            List<int> goldList = new();
            for (int j = 6; j <= 9; j++)
                goldList.Add(j < parts.Length && int.TryParse(parts[j].Trim(), out int val) ? val : 0);

            List<int> costList = new();
            for (int j = 10; j <= 13; j++)
                costList.Add(j < parts.Length && int.TryParse(parts[j].Trim(), out int val) ? val : 0);

            List<int> orderByLevel = new();
            if (parts.Length > 14 && !string.IsNullOrWhiteSpace(parts[14]))
            {
                string[] rawValues = parts[14].Split(';');
                foreach (var raw in rawValues)
                    orderByLevel.Add(int.TryParse(raw.Trim(), out int val) ? val : 0);
            }
            List<int> raceIDList = new();
            if (parts.Length > 15 && !string.IsNullOrWhiteSpace(parts[15]))
            {
                string[] rawValues = parts[15].Split(';');
                foreach (var raw in rawValues)
                    raceIDList.Add(int.TryParse(raw.Trim(), out int val) ? val : 0);
            }
            string desc = parts[16].Trim();

            buildingDict[id] = new BuildingData(id, displayName, imageName, raceId, gold, blueprint, goldList, costList, orderByLevel, raceIDList, desc);
        }

        Debug.Log($"[BuildManager] 건물 데이터 로딩 완료: {buildingDict.Count}개");
    }

    public Sprite GetBuildingSprite(string imageName)
    {
        return Resources.Load<Sprite>($"Sprites/{imageName}");
    }

    public int GetBuildingCount()
    {
        return buildingDict.Count;
    }

    public int GetOrderLevel(int id, int level)
    {
        if(level == 0)
        {
            return -1;
        }
        if (!buildingDict.TryGetValue(id, out var building))
        {
            Debug.LogError($"[BuildManager] ID {id}에 해당하는 건물이 없습니다.");
            return -1;
        }

        if (level < 1 || level > building.orderByLevel.Count)
        {
            Debug.LogError($"[BuildManager] 잘못된 레벨 접근: {level}, 건물 ID: {id}");
            return -1;
        }
        return building.orderByLevel[level-1];
    }

    public int GetNextLevel(int id, int layer)
    {
        if (!buildingDict.TryGetValue(id, out var building)) return -1;

        var orders = building.orderByLevel;
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i] > layer)
                return i + 1;
        }

        return -1;
    }

    public BuildingData GetBuildingData(int id)
    {
        return buildingDict.TryGetValue(id, out var building) ? building : null;
    }
    
    public int GetRequiredLevelForOrder(int buildID, int targetOrder)
    {
        if (!buildingDict.TryGetValue(buildID, out var building))
            return -1;

        for (int i = 0; i < building.orderByLevel.Count; i++)
        {
            if (building.orderByLevel[i] >= targetOrder)
                return i + 1;
        }

        return -1; // 해당 order를 해금할 수 없음
    }

}
