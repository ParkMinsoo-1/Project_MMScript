using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private Button acceptButton;
    [SerializeField] private GameObject completeCheck; // 보상 완료 표시

    private QuestData questData;
    private PlayerQuestData progress;

    public void SetData(QuestData quest, PlayerQuestData playerProgress)
    {
        questData = quest;
        progress = playerProgress;

        titleText.text = quest.Title;
        rewardAmountText.text = $"x {quest.RewardValue}";
        rewardIcon.sprite = GetRewardIcon(quest.RewardType);

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(TryGetReward);

        RefreshUI();
    }

    public void TryGetReward()
    {
        if (!PlayerDataManager.Instance.IsQuestCompleted(questData.ID) || PlayerDataManager.Instance.HasReceivedQuestReward(questData.ID))
        {
            return;
        }

        bool success = PlayerDataManager.Instance.TryGetQuestReward(questData.ID);
        if (success)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        bool isCompleted = PlayerDataManager.Instance.IsQuestCompleted(questData.ID);
        bool isRewarded = PlayerDataManager.Instance.HasReceivedQuestReward(questData.ID);

        acceptButton.interactable = isCompleted && !isRewarded;
        completeCheck.SetActive(isRewarded);

        if (acceptButton.interactable)
        {
            var colors = acceptButton.colors;
            colors.normalColor = new Color32(255, 235, 59, 255);
            colors.highlightedColor = new Color32(255, 245, 100, 255);
            colors.pressedColor = new Color32(255, 215, 0, 255);
            acceptButton.colors = colors;
        }
    }

    private static readonly Dictionary<RewardType, int> rewardTypeToItemID = new()
    {
        { RewardType.Gold, 101},
        { RewardType.Ticket, 102},
        { RewardType.BluePrint, 103}
    };

    private Sprite GetRewardIcon(RewardType type)
    {
        if(rewardTypeToItemID.TryGetValue(type, out int itemID))
        {
            string path = $"Rewards/{itemID}";
            Sprite icon = Resources.Load<Sprite>(path);

            if(icon == null)
            {
                Debug.Log($"{path}에 이미지가 없습니다.");
            }
            return icon;
        }

        Debug.Log($"{type}에 해당하는 아이템ID가 없습니다.");
        return null;
    }

}
