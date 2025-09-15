using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UnitDataManager
{
    private static UnitDataManager instance;
    public static UnitDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UnitDataManager();
                instance.LoadUnitData();
            }
            return instance;
        }
    }

    private Dictionary<int, UnitStats> unitStatsDict = new();

    private UnitDataManager() { }
    public void LoadUnitData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/UnitData"); // 확장자 없이
        if (csvFile == null)
        {
            Debug.LogError("Resources/Data/UnitData.csv 파일을 찾을 수 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            List<string> tokens = ParseCSVLine(lines[i]);
            if (tokens.Count < 23)
            {
                Debug.LogWarning($"라인 {i}의 필드 수가 부족합니다. ({tokens.Count}개): {lines[i]}");
                continue;
            }
            UnitStats stat = new UnitStats
            {
                ID = int.Parse(tokens[0].Trim()),
                Name = tokens[1].Trim(),
                Description = tokens[2].Trim(),
                RaceID = int.Parse(tokens[3].Trim()),
                IsHero = bool.Parse(tokens[4].Trim()),
                IsAOE = bool.Parse(tokens[5].Trim()),
                AttackRange = float.Parse(tokens[6].Trim()),
                Damage = float.Parse(tokens[7].Trim()),
                MaxHP = float.Parse(tokens[8].Trim()),
                MoveSpeed = float.Parse(tokens[9].Trim()),
                SpawnInterval = float.Parse(tokens[10].Trim()),
                Cost = int.Parse(tokens[11].Trim()),
                Hitback = int.Parse(tokens[12].Trim()),
                PreDelay = float.Parse(tokens[13].Trim()),
                PostDelay = float.Parse(tokens[14].Trim()),
                ModelName = tokens[15].Trim(),
                AttackType = int.Parse(tokens[16].Trim()),
                Size = float.Parse(tokens[17].Trim()),
                SkillID = ParseIntList(tokens[18].Trim()),
                isEnemy = bool.Parse(tokens[19].Trim()),
                warrant = int.Parse(tokens[20].Trim()),
                shopPrice = int.Parse(tokens[21].Trim()),
                tagId = new List<int>(),
                projectile = tokens[23].Trim(),
            };

            Debug.Log(stat.projectile);
            if (!string.IsNullOrWhiteSpace(tokens[18]))
            {
                string[] tagParts = tokens[18].Split(';');
                foreach (var part in tagParts)
                {
                    if (int.TryParse(part.Trim(), out int tag))
                        stat.SkillID.Add(tag);
                }
            }
            if (tokens.Count > 22 && !string.IsNullOrWhiteSpace(tokens[22]))
            {
                string[] tagParts = tokens[22].Split(';');
                foreach (var part in tagParts)
                {
                    string trimmed = part.Trim();
                    if (!string.IsNullOrEmpty(trimmed) && int.TryParse(trimmed, out int tag))
                        stat.tagId.Add(tag);
                }
            }

            unitStatsDict[stat.ID] = stat;
        }

        Debug.Log($"유닛 데이터 로딩 완료: {unitStatsDict.Count}개");
        PickUpListLoader.Instance.PickUps();
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
    private List<int> ParseIntList(string s)
    {
        List<int> list = new();
        if (string.IsNullOrWhiteSpace(s)) return list;

        foreach (var part in s.Split(';'))
        {
            if (int.TryParse(part.Trim(), out var val))
                list.Add(val);
        }
        return list;
    }

    public UnitStats GetStats(int id)
    {
        if (unitStatsDict.TryGetValue(id, out var stats)) return stats;
        Debug.LogWarning($"ID {id}에 해당하는 유닛 데이터를 찾을 수 없습니다.");
        return null;
    }
    public int GetRaceCount()
    {
        HashSet<int> raceSet = new HashSet<int>();
        foreach (var unit in unitStatsDict.Values)
        {
            raceSet.Add(unit.RaceID);
        }
        return raceSet.Count;
    }
}
