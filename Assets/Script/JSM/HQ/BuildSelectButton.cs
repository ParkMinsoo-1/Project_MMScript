using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildSelectButton : MonoBehaviour
{
    public int buildingIndex;
    public List<int> allIDs = new(); // 이 빌딩이 갖는 모든 ID들

    public GameObject buildConfirmPanel; // 확인용 패널
    public Button confirmButton;         // 패널 안의 확인 버튼
    public GameObject buildListUI;
    public Image buildImg;
    public Button buildingGospelBtn;
    public TMP_Text desc;
    public TMP_Text buildingName;
    public TMP_Text goldText;
    public TMP_Text blueprintText;
    private BuildingData building;
    public GameObject buildGospelUI;

    public Image img;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void Start()
    {
        building = BuildManager.Instance.GetBuildingData(buildingIndex);
        img.sprite = BuildManager.Instance.GetBuildingSprite(building.imageName);
        buildConfirmPanel.SetActive(false); // 처음엔 꺼두기

        allIDs.Clear();
        allIDs.Add(building.raceId);
        if (building.raceIDList != null)
            allIDs.AddRange(building.raceIDList);
    }

    public void OnClick()
    {
        buildConfirmPanel.SetActive(true);

        buildImg.sprite = BuildManager.Instance.GetBuildingSprite(building.imageName);
        buildingGospelBtn.onClick.AddListener(ShowGospel);
        desc.text = building.description;
        buildingName.text = building.displayName;
        goldText.text = $"{NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold)} / {NumberFormatter.FormatNumber(building.gold)}";
        goldText.color = PlayerDataManager.Instance.player.gold < building.gold ? Color.red : Color.black;
        blueprintText.text = $"{NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.bluePrint)} / {NumberFormatter.FormatNumber(building.blueprint)}";
        blueprintText.color = PlayerDataManager.Instance.player.bluePrint < building.blueprint ? Color.red : Color.black;

        SFXManager.Instance.PlaySFX(15);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (PlayerDataManager.Instance.player.gold < building.gold
            || PlayerDataManager.Instance.player.bluePrint < building.blueprint)
            {
                HQResourceUI.Instance.ShowLackPanel();
                buildConfirmPanel.SetActive(false);
                return;
            }
            PlayerDataManager.Instance.UseGold(building.gold);
            PlayerDataManager.Instance.UseBluePrint(building.blueprint);
            HQResourceUI.Instance.UpdateUI();
            BuildManager.Instance.BuildSelected(buildingIndex);
            SFXManager.Instance.PlaySFX(15);
            buildConfirmPanel.SetActive(false);
            buildListUI.SetActive(false);
        });
    }
    public void ShowGospel()
    {
        var goseplspawner = buildGospelUI.GetComponentInChildren<GospelSpawner>();
        goseplspawner.buildID = building.id;
        goseplspawner.toShow = true;
        buildGospelUI.SetActive(true);
    }
}
