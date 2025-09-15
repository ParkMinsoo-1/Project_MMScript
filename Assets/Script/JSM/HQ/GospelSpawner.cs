using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GospelSpawner : MonoBehaviour
{
    public GameObject GospelContainerPrefab;
    public GameObject GospelSlotPrefab;
    public GameObject LackLevelPanel;
    public Transform parent;
    public int buildID;
    public GameObject gospelConfirmUI;
    public int level;
    public bool toShow = false;

    private readonly List<GameObject> spawnedContainers = new();

    private void OnEnable()
    {
        SpawnGospels();
    }

    private void OnDisable()
    {
        ClearGospels();
    }

    private void ClearGospels()
    {
        foreach (var go in spawnedContainers)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedContainers.Clear();
    }

    private void SpawnGospels()
    {
        if (parent == null) parent = this.transform;

        var layeredGospels = GospelManager.Instance.GetGospelsByBuildID(buildID);
        //Debug.Log(layeredGospels.Count+"+"+buildID);
        int currentSelectableOrder = GospelManager.Instance.GetCurrentSelectableOrder(buildID);
        //Debug.Log(currentSelectableOrder);
        for (int order = layeredGospels.Count; order > 0; order--)
        {
            var layerData = layeredGospels[order-1];
            bool islocked = false;
            GameObject container = Instantiate(GospelContainerPrefab, parent);
            container.name = $"GospelContainer_Order{order}";
            container.layer = order+1;
            spawnedContainers.Add(container);

            var containerUI = container.GetComponent<GospelContainerUI>();
            //if (!toShow && containerUI != null)
            //    containerUI.SetInteractable(order == currentSelectableOrder);

            if (!toShow && order > BuildManager.Instance.GetOrderLevel(buildID, level))
            {
                int requiredLevel = BuildManager.Instance.GetRequiredLevelForOrder(buildID, order);
                GameObject lack = Instantiate(LackLevelPanel, container.transform);
                lack.GetComponentInChildren<TextMeshProUGUI>().text = $"{requiredLevel}레벨에 해금됩니다.";
                islocked = true;
            }
            foreach (var gospel in layerData)
            {
                GameObject slot = Instantiate(GospelSlotPrefab, container.transform);
                slot.name = $"GospelSlot_{gospel.id}";
                var slotUI = slot.GetComponent<GospelSlotUI>();
                slotUI.gospelSpawner = this;
                slotUI.containerUI = containerUI;
                slotUI.containerUI.AddSlot(slotUI);

                if (slotUI != null)
                {
                    slotUI.confirmUI = gospelConfirmUI.GetComponent<GospelConfirmUI>();
                    GospelState state;
                    if (toShow)
                        state = GospelState.ToShow;
                    else if (GospelManager.Instance.IsSelected(buildID, gospel.id))
                        state = GospelState.Selected;
                    else if (order != currentSelectableOrder||islocked)
                        state = GospelState.Locked;
                    else
                        state = GospelState.Available;
                    slotUI.SetData(gospel, state);
                }
            }


        }
    }
    public void OnSlotSelected(GospelSlotUI selectedSlot)
    {
        ClearGospels();
        SpawnGospels();
    }
}
