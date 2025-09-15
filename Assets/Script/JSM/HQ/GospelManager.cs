using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class GospelManager : MonoBehaviour
{
    public Dictionary<int, List<List<GospelData>>> gospelMap = new(); // buildID별 레이어 구조
    private Dictionary<int, GospelData> gospelByID = new(); // gospelID 기준 접근용

    public static GospelManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadGospels();
        //DebugGospelMap();
    }

    public void LoadGospels()
    {
        gospelMap.Clear();
        gospelByID.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>("Data/GospelData");
        if (csvFile == null)
        {
            Debug.LogError("Resources/Data/GospelData.csv 파일이 존재하지 않습니다.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            List<string> parts = ParseCSVLine(lines[i]);
            if (parts.Count < 8)
            {
                Debug.LogWarning($"CSV 파싱 실패: 줄 {i + 1}");
                continue;
            }

            int id = int.Parse(parts[0].Trim());
            int buildID = int.Parse(parts[1].Trim());
            int order = int.Parse(parts[2].Trim());
            int cost = int.Parse(parts[3].Trim());
            string desc = parts[4].Trim();
            string name = parts[5].Trim();
            List<int> statIndex = parts[6]
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s.Trim()))
                .ToList();

            float effectValue = float.Parse(parts[7].Trim());

            var gospel = new GospelData(id, buildID, order, cost, desc, name, statIndex, effectValue);

            if (!gospelMap.ContainsKey(buildID))
                gospelMap[buildID] = new List<List<GospelData>>();

            var layers = gospelMap[buildID];
            if (layers.Count == 0 || layers[^1][0].order != order)
                layers.Add(new List<GospelData>());

            layers[^1].Add(gospel);
            gospelByID[id] = gospel;
        }

        Debug.Log($"복음 데이터 로딩 완료: {gospelMap.Count}개 빌딩에 대해 로딩됨");
    }
    private static List<string> ParseCSVLine(string line)
    {
        var matches = Regex.Matches(line, @"(?<field>[^,""]+|""([^""]|"""")*"")(?=,|$)");
        List<string> result = new();

        foreach (Match match in matches)
        {
            string field = match.Groups["field"].Value;
            if (field.StartsWith("\"") && field.EndsWith("\""))
            {
                field = field[1..^1].Replace("\"\"", "\"");
            }
            result.Add(field);
        }

        return result;
    }

    public List<List<GospelData>> GetGospelsByBuildID(int buildID)
    {
        return gospelMap.ContainsKey(buildID) ? gospelMap[buildID] : new List<List<GospelData>>();
    }

    public bool IsSelected(int buildID, int gospelID)
    {
        return PlayerDataManager.Instance.player.selectedGospelIDsByBuildID.ContainsKey(buildID) &&
               PlayerDataManager.Instance.player.selectedGospelIDsByBuildID[buildID].Contains(gospelID);
    }

    public int GetCurrentSelectableOrder(int buildID)
    {
        if (!gospelMap.ContainsKey(buildID))
        {
            return 0;
        }

        var layers = gospelMap[buildID];
        for (int order = 1; order <= layers.Count; order++)
        {
            var layer = layers[order-1];
            bool anySelected = layer.Exists(g => IsSelected(buildID, g.id));
            if (!anySelected)
                return order;
        }
        return layers.Count+1;
    }

    public GospelData GetGospelByID(int gospelID)
    {
        if (gospelByID.TryGetValue(gospelID, out var data))
            return data;

        Debug.LogWarning($"Gospel ID {gospelID}에 해당하는 데이터를 찾을 수 없습니다.");
        return null;
    }

    public void SelectGospel(int buildID, int gospelID)
    {
        if (!PlayerDataManager.Instance.player.selectedGospelIDsByBuildID.ContainsKey(buildID))
            PlayerDataManager.Instance.player.selectedGospelIDsByBuildID[buildID] = new HashSet<int>();

        PlayerDataManager.Instance.player.selectedGospelIDsByBuildID[buildID].Add(gospelID);

        GospelData data = GetGospelByID(gospelID);
        if (data != null)
        {
            BuffManager.UpdateBuffStat(buildID, data.statIndex, data.effectValue);
        }
    }
    public void DebugGospelMap()
    {
        foreach (var kvp in gospelMap)
        {
            int buildID = kvp.Key;
            List<List<GospelData>> layers = kvp.Value;

            Debug.Log($"📦 BuildID: {buildID}, 레이어 수: {layers.Count}");

            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                Debug.Log($"  └─ Layer {i} (복음 수: {layer.Count})");

                foreach (var gospel in layer)
                {
                    Debug.Log($"      • ID: {gospel.id}, Order: {gospel.order}, Name: {gospel.name}, StatIndex: {gospel.statIndex}, Value: {gospel.effectValue}");
                }
            }
        }
    }

}
