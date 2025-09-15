using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WalletLevelUp : MonoBehaviour
{
    public Button levelUpButton;
    public TMP_Text levelText;
    public TextMeshProUGUI costText;
    public GameObject img;

    void Start()
    {
        levelUpButton.onClick.AddListener(TryLevelUp);
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void TryLevelUp()
    {
        if (BattleResourceManager.Instance.CanLevelUp())
        {
            BattleResourceManager.Instance.LevelUp();
            UpdateUI();
        }
        if(BattleResourceManager.Instance.walletLevel == 8)
        {
            img.SetActive(false);
            costText.alignment = TextAlignmentOptions.Center;
        }
        SFXManager.Instance.PlaySFX(17);
    }

    void UpdateUI()
    {
        var mgr = BattleResourceManager.Instance;

        levelText.text = $"Lv.{mgr.walletLevel}";
        costText.text = mgr.CanLevelUp() ? $"{mgr.GetLevelUpCost()}" : "MAX";
        levelUpButton.interactable = mgr.CanLevelUp() && mgr.currentResource >= mgr.GetLevelUpCost();
    }
}
