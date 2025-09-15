using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingLevelUpUI : MonoBehaviour
{
    private TextMeshProUGUI moneyTxt;
    private TextMeshProUGUI tributeTxt;
    private TextMeshProUGUI currentLevelTxt;
    private TextMeshProUGUI nextLevelTxt;
    public GameObject levelUpPanel;
    private BuildLevelUpConfirmUI buildLevelUpConfirmUI;
    private Button confirmButton;
    private Image buildImg;
    public BuildSlotUI buildSlotUI;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        buildLevelUpConfirmUI = levelUpPanel.GetComponent<BuildLevelUpConfirmUI>();
        moneyTxt = buildLevelUpConfirmUI.gold;
        tributeTxt = buildLevelUpConfirmUI.cost;
        currentLevelTxt = buildLevelUpConfirmUI.currentLevel;
        nextLevelTxt = buildLevelUpConfirmUI.nextLevel;
        confirmButton = buildLevelUpConfirmUI.confirmBtn;
        buildImg = buildLevelUpConfirmUI.buildingImg;
    }
    public void OnClick()
    {
        levelUpPanel.SetActive(true);

        buildImg.sprite = buildSlotUI.slotImage.sprite;
        moneyTxt.text = $"{NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold)} / {NumberFormatter.FormatNumber(BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).goldList[buildSlotUI.Level-1])}";
        moneyTxt.color = PlayerDataManager.Instance.player.gold < BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).goldList[buildSlotUI.Level - 1] ? Color.red : Color.black;
        tributeTxt.text = $"{NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.tribute)} / {NumberFormatter.FormatNumber(BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).costList[buildSlotUI.Level-1])}";
        tributeTxt.color = PlayerDataManager.Instance.player.tribute < BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).costList[buildSlotUI.Level - 1] ? Color.red : Color.black;
        currentLevelTxt.text = buildSlotUI.Level.ToString();
        nextLevelTxt.text = (buildSlotUI.Level+1).ToString();
        

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (PlayerDataManager.Instance.player.gold < BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).goldList[buildSlotUI.Level - 1]
             || PlayerDataManager.Instance.player.tribute < BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).costList[buildSlotUI.Level - 1])
            {
                HQResourceUI.Instance.ShowLackPanel();
                SFXManager.Instance.PlaySFX(6);
                levelUpPanel.SetActive(false);
                return;
            }

            PlayerDataManager.Instance.UseGold(BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).goldList[buildSlotUI.Level - 1]);
            PlayerDataManager.Instance.UseTribute(BuildManager.Instance.GetBuildingData(buildSlotUI.buildingID).costList[buildSlotUI.Level - 1]);
            HQResourceUI.Instance.UpdateUI();
            buildSlotUI.LevelUp();
            SFXManager.Instance.PlaySFX(4);
            if (buildSlotUI.Level == 5)
            {
                this.GetComponent<Button>().onClick.RemoveAllListeners();
                this.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            }
                SFXManager.Instance.PlaySFX(14);
            levelUpPanel.SetActive(false);
        });
    }
}
