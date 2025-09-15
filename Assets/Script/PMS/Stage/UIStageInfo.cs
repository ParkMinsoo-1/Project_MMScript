using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour
{
    [SerializeField] private Image enemy1;
    [SerializeField] private Image enemy2;
    [SerializeField] private Image enemyLeader;

    [SerializeField] private GameObject firstRewardParent;
    [SerializeField] private GameObject frSlot;
    [SerializeField] private GameObject repeatRewardParent;
    [SerializeField] private GameObject rrSlot;

    [SerializeField] private Button deckBtn;
    [SerializeField] private Button enterBtn;

    [SerializeField] private TextMeshProUGUI stageNumText;

    private List<int> firstRewardIDs;
    private List<int> firstRewardAmounts;

    public void SetStageInfo(int stageID)
    {
        var stage = StageDataManager.Instance.GetStageData(stageID);
        bool isCleared = PlayerDataManager.Instance.HasClearedStage(stageID);

        foreach (Transform child in firstRewardParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in repeatRewardParent.transform)
        {
            Destroy(child.gameObject);
        }

        var en1 = UnitDataManager.Instance.GetStats(stage.EnemyUnit1);
        var en2 = UnitDataManager.Instance.GetStats(stage.EnemyUnit2);
        var enLeader = UnitDataManager.Instance.GetStats(stage.EnemyUnit3);

        enemy1.sprite = Resources.Load<Sprite>($"SPUMImg/{en1.ModelName}");
        enemy2.sprite = Resources.Load<Sprite>($"SPUMImg/{en2.ModelName}");
        enemyLeader.sprite = Resources.Load<Sprite>($"SPUMImg/{enLeader.ModelName}");


        firstRewardIDs = stage.firstRewardItemIDs;
        firstRewardAmounts = stage.firstRewardAmounts;

        for (int i = 0; i < firstRewardIDs.Count; i++)
        {
            CreateRewardSlot(firstRewardIDs[i], firstRewardAmounts[i], firstRewardParent.transform, frSlot, isCleared, isFirstReward: true);
        }

        if (stage.repeatRewardItemIDs != null && stage.repeatRewardItemIDs.Count > 0)
        {
            int itemID = stage.repeatRewardItemIDs[0];
            int amount = stage.repeatRewardAmounts[0];
            CreateRewardSlot(itemID, amount, repeatRewardParent.transform, rrSlot, false, false);
        }

        stageNumText.text = stage.StageName.Replace("stage", "");
    }
    private void CreateRewardSlot(int itemID, int amount, Transform parent, GameObject slotPrefab, bool isCleared, bool isFirstReward)
    {
        GameObject slot = Instantiate(slotPrefab, parent);
        Sprite icon = Resources.Load<Sprite>($"Rewards/{itemID}");
        if (icon == null)
        {
            Debug.LogWarning($"스프라이트를 찾을 수 없습니다: Rewards/{itemID}");
            return;
        }

        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }

        TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var t in texts)
        {
            if (t.name == "Amount")
            {
                t.text = amount.ToString();
                break;
            }
        }

        if (isFirstReward && isCleared)
        {
            Transform rewardMark = slot.transform.Find("ObtainMark");
            if (rewardMark != null)
            {
                rewardMark.gameObject.SetActive(true);
            }
        }
    }
    public void OnClickToDeck()
    {
        UIDeckBuildManager.instance.deckPanel.SetActive(true);
        UIDeckBuildManager.instance.Init();
        SFXManager.Instance.PlaySFX(0);
    }

    public void OnClickEnter()
    {
        StageManager.instance.OnClickEnterBattle();
        SFXManager.Instance.PlaySFX(0);
    }
}
