using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestManager : MonoBehaviour
{
    public static UIQuestManager instance;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Init();
        QuestDataManager.Instance.Init();
        QuestManager.Instance.Init();

    }

    public GameObject questPanel;
    public Button dailyTabBtn;
    public Button weeklyTabBtn;
    public Button closeBtn;
    public Button getAllBtn;

    public Transform contentParent;
    public UIQuestSlot questSlotPrefab;

    private List<UIQuestSlot> spawnedSlots = new();

    public void Init()
    {
        dailyTabBtn.onClick.AddListener(() =>  ShowQuests(QuestType.Daily));
        weeklyTabBtn.onClick.AddListener(() => ShowQuests(QuestType.Weekly));
        closeBtn.onClick.AddListener(() => ClosePanel());
        getAllBtn.onClick.AddListener(GetAllRewards);
        
    }

    public void OpenPanel()
    {
        QuestEvent.OnLogin?.Invoke();
        questPanel.SetActive(true);
        ShowQuests(QuestType.Daily);
        SFXManager.Instance.PlaySFX(0);
    }
    public void ClosePanel()
    {
        questPanel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }

    public void ShowQuests(QuestType type)
    {
        ClearSlots();

        List<QuestData> quests = type == QuestType.Daily
            ? QuestDataManager.Instance.GetDailyQuests()
            : QuestDataManager.Instance.GetWeeklyQuests();

        foreach (var quest in quests)
        {
            var progress = PlayerDataManager.Instance.player.playerQuest.Find(q => q.QuestID == quest.ID);
            if (progress == null) continue;

            UIQuestSlot slot = Instantiate(questSlotPrefab, contentParent);
            slot.SetData(quest, progress);
            spawnedSlots.Add(slot);
            SFXManager.Instance.PlaySFX(0);
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
    }

    private void GetAllRewards()
    {
        foreach (var slot in spawnedSlots)
        {
            slot.TryGetReward();
        }
        SFXManager.Instance.PlaySFX(0);
    }
}

