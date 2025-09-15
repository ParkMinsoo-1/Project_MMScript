using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITowerManager : MonoBehaviour
{
    [SerializeField] private GameObject towerParent;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private GameObject towerInfoPanel;

    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;

    private List<int> raceIDs;
    private int currentPage = 0;
    private int towerPerPage = 3;

    public static UITowerManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        this.gameObject.SetActive(true);
        raceIDs = StageDataManager.Instance.GetAllTowerStageData()
            .Values
            .Select(s => s.RaceID)
            .Distinct()
            .ToList();

        currentPage = 0;

        SetTower();

        //nextBtn.onClick.RemoveAllListeners();
        //prevBtn.onClick.RemoveAllListeners();
        //nextBtn.onClick.AddListener(NextPage);
        //prevBtn.onClick.AddListener(PrevPage);
    }
    public void SetTower()
    {
        foreach (Transform child in towerParent.transform)
            Destroy(child.gameObject);

        int startIndex = currentPage * towerPerPage;
        int endIndex = Mathf.Min(startIndex + towerPerPage, raceIDs.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject go = Instantiate(towerPrefab, towerParent.transform);
            UITowerSlot slot = go.GetComponent<UITowerSlot>();
            slot.Setup(raceIDs[i]);
        }

        //prevBtn.interactable = currentPage > 0;
        //nextBtn.interactable = endIndex < raceIDs.Count;
    }

    private void NextPage()
    {
        currentPage++;
        SetTower();
    }

    private void PrevPage()
    {
        currentPage--;
        SetTower();
    }

    public void SelectTower(int stageID)
    {
        towerInfoPanel.SetActive(true);
        towerInfoPanel.GetComponent<UITowerInfo>().SetTowerInfo(stageID);
    }
    
}
