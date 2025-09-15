using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class QuestDataManager
{
    private static QuestDataManager instance;
    public static QuestDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new QuestDataManager();
            }
            return instance;
        }
    }

    public List<QuestData> allQuests = new();
    public Dictionary<int, QuestData> questDic = new();

    public void Init()
    {
        LoadQuestData();
    }

    private void LoadQuestData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/QuestData"); // 확장자 생략
        if (jsonFile == null)
        {
            Debug.LogWarning("Resources/Data/QuestData.json 파일이 존재하지 않습니다.");
            return;
        }

        try
        {
            string json = jsonFile.text;
            List<QuestData> data = JsonConvert.DeserializeObject<List<QuestData>>(json);

            allQuests.Clear();
            questDic.Clear();

            foreach (QuestData quest in data)
            {
                if (quest != null)
                {
                    allQuests.Add(quest);
                    questDic[quest.ID] = quest;
                }
            }

            allQuests.Sort((a, b) => a.Order.CompareTo(b.Order));
            Debug.Log($"{allQuests.Count}개 퀘스트 데이터 로드 완료");
        }
        catch (Exception ex)
        {
            Debug.LogError($"퀘스트 데이터 로드 실패: {ex.Message}");
        }
    }


    public QuestData GetQuestID(int id)
    {
        questDic.TryGetValue(id, out QuestData quest);
        return quest;
    }

    public List<QuestData> GetDailyQuests()
    {
        return allQuests.FindAll(quest => quest.Type == QuestType.Daily);
    }
    public List<QuestData> GetWeeklyQuests()
    {
        return allQuests.FindAll(quest => quest.Type == QuestType.Weekly);
    }

}
