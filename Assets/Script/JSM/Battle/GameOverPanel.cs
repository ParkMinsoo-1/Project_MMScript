using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Image title;
    public TextMeshProUGUI description;
    public GameObject rewardPanel;
    public GameObject rewardSlot;
    private List<int> rewardItemId;
    private List<int> rewardItemCount;

    public void Win()
    {
        title.sprite = Resources.Load<Sprite>("Sprites/Image_Victory");
        Debug.Log(WaveManager.Instance.currentStage.StageName);
        description.text = "승리 보상";
        rewardItemId = PlayerDataManager.Instance.HasClearedStage(WaveManager.Instance.stageID) ? WaveManager.Instance.currentStage.repeatRewardItemIDs : WaveManager.Instance.currentStage.firstRewardItemIDs;
        rewardItemCount = PlayerDataManager.Instance.HasClearedStage(WaveManager.Instance.stageID) ? WaveManager.Instance.currentStage.repeatRewardAmounts : WaveManager.Instance.currentStage.firstRewardAmounts;
        if (PlayerDataManager.Instance.player.goldDungeonData.entryCounts <= 0 && WaveManager.Instance.currentStage.Type == 2)
            return;
        for (int i = 0; i < rewardItemId.Count; i++)
        {
            int itemId = rewardItemId[i];
            int itemCount = rewardItemCount[i];

            GameObject slot = Instantiate(rewardSlot, rewardPanel.transform);

            Sprite icon = Resources.Load<Sprite>($"Rewards/{itemId}");
            if (icon == null)
            {
                Debug.LogWarning($"스프라이트를 찾을 수 없습니다: Rewards/{itemId}");
                continue;
            }

            Image iconImage = slot.GetComponentInChildren<Image>();
            if (iconImage != null) iconImage.sprite = icon;

            TextMeshProUGUI countText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (countText != null) countText.text = itemCount.ToString();
        }
    }
    public void Lose()
    {
        title.sprite = Resources.Load<Sprite>("Sprites/Image_Fail");
        description.text = "전투 패배";
    }
}
