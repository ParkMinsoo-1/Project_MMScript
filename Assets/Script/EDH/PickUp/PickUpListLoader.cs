using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PickUpListLoader : Singleton<PickUpListLoader>
{
    private void Awake()
    {
        //PickUps();
    }

    private Dictionary<int, PickInfo> PickListsDict = new();
    public void PickUps()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/UnitData"); // 확장자 생략
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
            //int j=0;
            //foreach(var token in tokens)
            //{
            //    Debug.Log(i+"-"+j+" : "+token);
            //    j++;
            //}
            PickInfo pickinfo = new PickInfo
            {
                ID = int.Parse(tokens[0]),    // 유닛 ID
                Name = tokens[1],               // 유닛 이름
                Description = tokens[2],               // 설명
                raceId = int.Parse(tokens[3]),
                IsHero = bool.Parse(tokens[4]),   // 영웅 여부
                Uniticon = tokens[5],               // 유닛 아이콘
                IsEnemy = bool.Parse(tokens[19]),  // 적 여부
                warrant = int.Parse(tokens[20]),   //증명서 가격
                duplication = int.Parse(tokens[21]), //중복재화
                projectile = tokens[23].Trim(),
            };

            PickListsDict[pickinfo.ID] = pickinfo;
        }

        Debug.Log($"픽업 데이터 로딩 완료: {PickListsDict.Count}개");
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

    public Dictionary<int, PickInfo> GetAllPickList()
    { return PickListsDict; }
}



