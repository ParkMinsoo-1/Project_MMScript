using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FillterUI : MonoBehaviour
{
    public GameObject fillter;
    public GameObject fillterBtn;       // FillterBtn 프리팹
    public Transform parent;            // 버튼 부모
    public BuildingSlotSpanwer spawner; // 슬롯 스크립트 참조

    private Dictionary<int, Toggle> toggles = new();

    private void Awake()
    {
        var tagData = RaceManager.GetAll();

        foreach (var kvp in tagData)
        {
            int id = kvp.Key;
            string name = kvp.Value;

            GameObject go = Instantiate(fillterBtn, parent);
            FillterBtn btn = go.GetComponent<FillterBtn>();

            btn.id = id;
            btn.text.text = name;
            toggles[id] = btn.toggle;

            btn.toggle.onValueChanged.AddListener(_ => OnFilterChanged());
        }

        BackHandlerEntry entry;
        entry = new BackHandlerEntry(
           priority: 30,
           isActive: () => fillter.activeInHierarchy,
           onBack: () => fillter.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }

    private void OnFilterChanged()
    {
        List<int> selectedIDs = toggles
            .Where(p => p.Value.isOn)
            .Select(p => p.Key)
            .ToList();

        spawner.FilterByIDs(selectedIDs);
    }
    private void OnEnable()
    {
        SFXManager.Instance.PlaySFX(0);
    }
    private void OnDisable()
    {
        SFXManager.Instance.PlaySFX(0);
    }
}