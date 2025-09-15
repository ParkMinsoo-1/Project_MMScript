using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform parentObject;
    public ScrollRect scrollRect;
    public GameObject buildListUI;
    public GameObject buildGospelUI;
    public GameObject levelUpPanel;

    private void Start()
    {
        for (int i = 0; i < BuildManager.Instance.count; i++)
        {
            GameObject newObj = Instantiate(prefabToSpawn, parentObject);
            newObj.name = $"{prefabToSpawn.name}_{i}";

            Button button = newObj.GetComponentInChildren<Button>();
            BuildSlotUI buildSlotUI = newObj.GetComponentInChildren<BuildSlotUI>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    buildSlotUI.Select();
                });
            }
            buildSlotUI.Level = PlayerDataManager.Instance.player.buildingsList[i].level;
            newObj.GetComponentInChildren<BuildSlotUI>().buildListUI = buildListUI;
            newObj.GetComponentInChildren<BuildSlotUI>().buildGospelUI = buildGospelUI;
            newObj.GetComponentInChildren<BuildSlotUI>().levelUpPanel = levelUpPanel;
            newObj.GetComponentInChildren<BuildSlotUI>().slotID = i;
            if (PlayerDataManager.Instance.player.buildingsList[i].buildingData != null)
            {
                buildSlotUI.Build(PlayerDataManager.Instance.player.buildingsList[i].buildingData, PlayerDataManager.Instance.player.buildingsList[i].level);
            }
        }

        BackHandlerEntry entry;
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => buildListUI.activeInHierarchy,
           onBack: () => buildListUI.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => buildGospelUI.activeInHierarchy,
           onBack: () => buildGospelUI.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => levelUpPanel.activeInHierarchy,
           onBack: () => levelUpPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }
}
