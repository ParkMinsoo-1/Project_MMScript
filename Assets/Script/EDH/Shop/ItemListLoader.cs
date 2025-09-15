using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;


public class ItemListLoader : MonoBehaviour
{
    private static ItemListLoader _instance;

    public static ItemListLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ItemListLoader();
            }
            return _instance;
        }

    }
    void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            LoadItemData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private ItemListLoader GetInstance()
    {
        return Instance;
    }

    private Dictionary<int, Item> itemListsDict = new();
    private void LoadItemData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/MarketItemData");
        if (csvFile == null)
        {
            Debug.LogError("MarketItemData.csv 파일이 Resources/Data 폴더에 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        Debug.Log(lines.Length + " 가짓수");
        for (int i = 1; i < lines.Length; i++)
        {
            List<string> parts = ParseCSVLine(lines[i]);
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            Item item = new Item
            {
                ID = int.Parse(parts[0]),
                Name = parts[1],
                Cost = int.Parse(parts[2]),
                Description = parts[3].Trim(),
                DailyBuy = int.Parse(parts[4]),
                ItemIcon = parts[5]
            };

            itemListsDict[item.ID] = item;
            Debug.Log("생성함");
        }

        Debug.Log($"아이템 데이터 로딩 완료: {itemListsDict.Count}개");
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

    public Dictionary<int, Item> GetAllList()
    { return itemListsDict; }
}