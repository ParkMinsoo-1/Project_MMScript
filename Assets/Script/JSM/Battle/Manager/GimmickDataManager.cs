using System.Collections.Generic;
using UnityEngine;

public class GimmickData
{
    public int ID;
    public string Name;
    public float EffectValue;
    public string Desc;

    public GimmickData(int id, string name, float value, string desc)
    {
        ID = id;
        Name = name;
        EffectValue = value;
        Desc = desc;
    }
}

public class GimmickDataManager
{
    private static GimmickDataManager instance;
    public static GimmickDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GimmickDataManager();
                instance.LoadGimmickData();
            }
            return instance;
        }
    }
    public Dictionary<int, GimmickData> gimmickDict = new();
    private bool isLoaded = false;

    public void LoadGimmickData()
    {
        if (isLoaded) return;
        isLoaded = true;

        TextAsset csvFile = Resources.Load<TextAsset>("Data/GimmickData");
        if (csvFile == null)
        {
            Debug.LogError("GimmickData.csv 파일이 Resources/Data에 없습니다.");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] tokens = line.Split(',');

            int id = int.Parse(tokens[0]);
            string name = tokens[1];
            float effectValue = 0f;
            string desc = tokens[3];
            if (tokens.Length > 2 && !string.IsNullOrWhiteSpace(tokens[2]))
                float.TryParse(tokens[2], out effectValue);

            GimmickData data = new(id, name, effectValue, desc);
            gimmickDict[id] = data;
        }

        Debug.Log($"[GimmickManager] 기믹 데이터 {gimmickDict.Count}개 로드 완료");
    }
    public GimmickData GetGimmick(int id)
    {
        if (gimmickDict.TryGetValue(id, out var stats)) return stats;
        Debug.LogWarning($"ID {id}에 해당하는 유닛 데이터를 찾을 수 없습니다.");
        return null;
    }
}
