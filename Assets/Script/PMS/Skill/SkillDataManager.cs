using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    public static SkillDataManager Instance { get; private set; }

    private Dictionary<int, SkillData> skillDic = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Init()
    {
        LoadSkillData();
    }
    private void LoadSkillData()
    {
        try
        {
            TextAsset csvFile = Resources.Load<TextAsset>("Data/HeroSkillData");
            if (csvFile == null)
            {
                throw new FileNotFoundException("HeroSkillData를 찾을 수 없습니다.");
            }

            // Windows/Mac/Unix 개행 모두 \n 으로 통일
            string csvText = csvFile.text.Replace("\r\n", "\n").Replace("\r", "\n");

            // CSV 한 줄씩 읽되, 따옴표 안의 줄바꿈은 무시
            List<string> rows = SplitCsvIntoRows(csvText);

            for (int i = 1; i < rows.Count; i++) // 첫 줄은 헤더
            {
                if (string.IsNullOrWhiteSpace(rows[i])) continue;

                List<string> tokens = ParseCSVLine(rows[i]);

                SkillData skill = new SkillData()
                {
                    ID = int.Parse(tokens[0]),
                    Name = tokens[1],
                    SkillType = tokens[2],
                    Type = tokens[3],
                    Description = tokens[4],
                    EffectValue = tokens[5].Split(';').Select(int.Parse).ToList(),
                    TargetRaceID = int.Parse(tokens[6]),
                };
                skillDic[skill.ID] = skill;
            }
            Debug.Log($"스킬 데이터 {skillDic.Count}개 로드 성공");
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    /// <summary>
    /// CSV 텍스트를 줄 단위로 나누되, "" 안의 줄바꿈은 그대로 둠
    /// </summary>
    private static List<string> SplitCsvIntoRows(string csvText)
    {
        List<string> rows = new();
        bool inQuotes = false;
        string currentRow = "";

        foreach (char c in csvText)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                currentRow += c;
            }
            else if (c == '\n' && !inQuotes)
            {
                rows.Add(currentRow);
                currentRow = "";
            }
            else
            {
                currentRow += c;
            }
        }

        if (!string.IsNullOrEmpty(currentRow))
            rows.Add(currentRow);

        return rows;
    }

    /// <summary>
    /// CSV 한 줄을 필드 단위로 파싱 (따옴표 포함, 줄바꿈 포함 가능)
    /// </summary>
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

    public SkillData GetSkillData(int id)
    {
        if (skillDic.TryGetValue(id, out var data)) return data;
        return null;
    }
}
