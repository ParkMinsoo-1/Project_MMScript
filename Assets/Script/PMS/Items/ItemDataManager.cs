using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemDataManager
{
    private static ItemDataManager instance;
    public static ItemDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ItemDataManager();
            }
            return instance;
            
        }
    }
    private Dictionary<int, ItemData> itemDic = new();
    public void LoadItemData()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData.csv");

        if (!File.Exists(path))
        {
            Debug.Log($"ItemData.csv is null");
            return;
        }
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] tokens = lines[i].Split(',');

            ItemData data = new ItemData
            {
                ID = int.Parse(tokens[0]),
                Name = tokens[1],
            };

            itemDic[data.ID] = data;
        }
        Debug.Log($"아이템 데이터 로딩 완료: {itemDic.Count}개");
    }

    public ItemData GetItemData(int id)
    {
        if (itemDic.ContainsKey(id)) return itemDic[id];
        Debug.Log($"아이템 {id}에 해당하는 정보를 찾을 올 수 없습니다.");
        return null;
    }
}
