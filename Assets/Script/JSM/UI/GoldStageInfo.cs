using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldStageInfo : MonoBehaviour
{
    [SerializeField] private GameObject enemyParent;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private GameObject rewardParent;
    [SerializeField] private GameObject rewardPrefab;

    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button enterBtn;
    [SerializeField] private Button deckBtn;

    private List<int> firstRewardIDs;
    private List<int> firstRewardAmounts;

    private Coroutine popCoroutine;

    private int currentStageID;

    private void Awake()
    {
        enterBtn.onClick.AddListener(OnClickEnter);
        deckBtn.onClick.AddListener(OnclickOpenDeck);
    }


    public void SetGoldInfo(int stageID)
    {
        currentStageID = stageID;

        var stage = StageDataManager.Instance.GetStageData(stageID);

        foreach (Transform child in enemyParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in rewardParent.transform)
        {
            Destroy(child.gameObject);
        }
            
        GameObject enemySlot = Instantiate(enemyPrefab, enemyParent.transform);
        Image image = enemySlot.GetComponentInChildren<Image>();
        if (image != null)
        {
            Sprite sprite = Resources.Load<Sprite>($"Sprites/{stage.CastleSprite}");
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"적 스프라이트 없음: Sprites/{stage.CastleSprite}");
            }
        }

        if (PlayerDataManager.Instance.player.goldDungeonData.entryCounts <= 0)
            enterBtn.enabled = false;

        firstRewardIDs = stage.firstRewardItemIDs;
        firstRewardAmounts = stage.firstRewardAmounts;

        firstRewardIDs = stage.firstRewardItemIDs;
        firstRewardAmounts = stage.firstRewardAmounts;

        for (int i = 0; i < firstRewardIDs.Count; i++)
        {
            CreateRewardSlot(firstRewardIDs[i], firstRewardAmounts[i], rewardParent.transform, rewardPrefab);
        }

        int raceID = stage.RaceID;
        countText.text = PlayerDataManager.Instance.player.goldDungeonData.entryCounts.ToString() + " / 3";

        if (UIGimmickInfo.Instance != null && UIGimmickInfo.Instance.IsOpen())
        {
            UIGimmickInfo.Instance.Close();
        }
    }
    private void CreateRewardSlot(int itemID, int amount, Transform parent, GameObject slotPrefab)
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
    }

    private void OnclickOpenDeck()
    {
        UIDeckBuildManager.instance.deckPanel.SetActive(true);
        UIDeckBuildManager.instance.Init();
        SFXManager.Instance.PlaySFX(0);
    }

    private void OnClickEnter()
    {
        
    }
}
