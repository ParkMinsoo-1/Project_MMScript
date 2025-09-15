using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITowerSlot : MonoBehaviour
{
    [SerializeField] private Image towerImg;
    [SerializeField] private Image raceImg;
    [SerializeField] private Image floorImg;
    [SerializeField] private TextMeshProUGUI raceNameText;
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private Button btn;


    private int raceID;

    public void Setup(int raceID)
    {
        this.raceID = raceID;

        
        var stage = TowerManager.Instance.GetCurrentFloorStage(raceID);
        if (stage == null)
        {
            Debug.LogWarning($"해당 종족({raceID})의 스테이지 데이터를 찾을 수 없습니다.");
            return;
        }

        //towerImg.sprite = 
        raceNameText.text = RaceManager.GetNameByID(raceID);
        floorText.text = $"{stage.Chapter}층";
        GetColorByRaceID(raceID);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClickTower);
    }

    private void GetColorByRaceID(int raceID)
    {
        switch (raceID)
        {
            case 0:
                towerImg.color = Color.white;
                raceImg.color = new Color32(0x66, 0x87, 0xE9, 0xFF);
                floorImg.color = new Color32(0x9E, 0xA5, 0xEE, 0xFF);
                break;
            case 1:
                towerImg.color = new Color32(0x78, 0xB6, 0xFF, 0xFF);
                raceImg.color = new Color32(0xD0, 0x5C, 0xF1, 0xFF);
                floorImg.color = new Color32(0x9E, 0xA5, 0xEE, 0xFF);
                break;
        }
    }

    public void OnClickTower()
    {
        var stage = TowerManager.Instance.GetCurrentFloorStage(raceID);
        if(stage == null)
        {
            return;
        }

        UITowerManager.instance.SelectTower(stage.ID);
        SFXManager.Instance.PlaySFX(15);
    }
}
