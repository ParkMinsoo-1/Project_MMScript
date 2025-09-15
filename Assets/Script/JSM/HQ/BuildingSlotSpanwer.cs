using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BuildingSlotSpanwer : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform parentObject;
    public ScrollRect scrollRect;

    public GameObject confirmPanel;
    public Button confirmButton;
    public GameObject buildListUI;
    public Image buildImg;
    public Button buildingGospelBtn;
    public TMP_Text buildName;
    public TMP_Text buildDesc;
    public TMP_Text goldText;
    public TMP_Text blueprintText;
    public GameObject buildGospelUI;
    public GameObject GospelConfirmUI;

    private readonly List<BuildSelectButton> allButtons = new();

    private void Start()
    {
        BackHandlerEntry entry;
        entry = new BackHandlerEntry(
           priority: 30,
           isActive: () => confirmPanel.activeInHierarchy,
           onBack: () => confirmPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 40,
           isActive: () => buildGospelUI.activeInHierarchy && confirmPanel.activeInHierarchy,
           onBack: () => buildGospelUI.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 50,
           isActive: () => buildGospelUI.activeInHierarchy && confirmPanel.activeInHierarchy && GospelConfirmUI.activeInHierarchy,
           onBack: () => GospelConfirmUI.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }
    private void OnEnable()
    {
        SetBuildingSlots();
    }

    private void OnDisable()
    {
        foreach (Transform child in parentObject)
        {
            Destroy(child.gameObject);
        }

        allButtons.Clear();
    }

    public void SetBuildingSlots()
    {
        int index = 0;

        foreach (var pair in BuildManager.Instance.buildingDict.OrderBy(p => p.Value.id))
        {
            int buildID = pair.Key;
            var building = pair.Value;

            // 이미 설치된 건물은 제외
            if (PlayerDataManager.Instance.player.buildingsList
                .Any(b => b.buildingData != null && b.buildingData.id == buildID))
            {
                index++;
                continue;
            }

            GameObject newObj = Instantiate(prefabToSpawn, parentObject);
            newObj.name = $"{prefabToSpawn.name}_{index}";

            var button = newObj.GetComponent<BuildSelectButton>();
            button.buildingIndex = buildID;
            button.buildConfirmPanel = confirmPanel;
            button.confirmButton = confirmButton;
            button.buildingGospelBtn = buildingGospelBtn;
            button.buildListUI = buildListUI;
            button.buildImg = buildImg;
            button.desc = buildDesc;
            button.buildingName = buildName;
            button.goldText = goldText;
            button.blueprintText = blueprintText;
            button.buildGospelUI = buildGospelUI;

            // raceID + raceIDList 통합
            button.allIDs = new List<int> { building.raceId };
            if (building.raceIDList != null)
                button.allIDs.AddRange(building.raceIDList);

            allButtons.Add(button);
            index++;
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// 필터된 ID 목록에 따라 버튼을 보이거나 숨김
    /// </summary>
    public void FilterByIDs(List<int> selectedIDs)
    {
        foreach (var btn in allButtons)
        {
            var buildingIDs = btn.allIDs;

            if (selectedIDs.Count == 0)
            {
                btn.gameObject.SetActive(true);
            }
            else if (selectedIDs.Count == 1)
            {
                btn.gameObject.SetActive(buildingIDs.Contains(selectedIDs[0]));
            }
            else
            {
                // 모든 선택 ID를 포함하는 빌딩만 표시
                bool containsAll = selectedIDs.All(id => buildingIDs.Contains(id));
                btn.gameObject.SetActive(containsAll);
            }
        }
    }
}
