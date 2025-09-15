using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum UnitFilterType
{
    All,
    Undead,
    Crawler
}

//public enum DeckMode
//{
//    DeckBuild,
//    Preset
//}

public class UIDeckBuildManager : MonoBehaviour
{
    [Header("보유 유닛 슬롯")]
    [SerializeField] private GameObject unitIconPrefab;      // 유닛 아이콘 프리팹
    [SerializeField] private Transform myUnitContentParent;  // ScrollView의 Content

    [Header("일반 유닛 덱 슬롯")]
    [SerializeField] private GameObject deckSlotPrefab; // 덱 슬롯 프리팹
    [SerializeField] private Transform deckSlotParent; // 덱 슬롯 프리팹의 부모 (생성위치)

    [Header("리더 덱 슬롯")]
    [SerializeField] private UIDeckSlot leaderSlot; // 리더 슬롯

    [SerializeField] public GameObject deckPanel;

    public List<UIDeckSlot> deckSlotList = new();
    private List<int> cachedUnitOrder = new();

    public static UIDeckBuildManager instance;

    [Header("필터")]
    [SerializeField] private Button allBtn;
    [SerializeField] private Button undeadBtn;
    [SerializeField] private Button crawlerBtn;

    [SerializeField] private Image allBtnImg;
    [SerializeField] private Image undeadBtnImg;
    [SerializeField] private Image crawlerBtnImg;

    private UIUnitIcon selectedIcon;

    private UnitFilterType currentFilter = UnitFilterType.All; // 필터

    private int? raceFilter = null;

    //private DeckMode currentMode = DeckMode.DeckBuild;
    //private UIPresetDeck currentPresetDeck;
    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Init()
    {
        int currentIndex = PlayerDataManager.Instance.player.currentPresetIndex;
        DeckManager.Instance.SwitchPreset(currentIndex);

        //raceFilter = filterRaceID;
        currentFilter = UnitFilterType.All;

        //InitDeckSlots();
        CacheUnitListOrder();
        SetDeckSlots();
        SetMyUnitIcons();
        UIUnitInfo.instance.ClearInfo();

        allBtn.onClick.RemoveAllListeners();
        undeadBtn.onClick.RemoveAllListeners();
        crawlerBtn.onClick.RemoveAllListeners();

        allBtn.onClick.AddListener(() => OnClickFilter(UnitFilterType.All));
        undeadBtn.onClick.AddListener(() => OnClickFilter(UnitFilterType.Undead));
        crawlerBtn.onClick.AddListener(() => OnClickFilter(UnitFilterType.Crawler));

        UpdateFilterUI();
        UIPresetManager.instance.UpdatePresetBtn();
    }

    public void InitDeckSlots() // 시작할 때 덱 슬롯 만들기. 처음에만 쓰면됨.
    {
        foreach (Transform child in deckSlotParent)
        {
            Destroy(child.gameObject);
        }

        deckSlotList.Clear();

        for (int i = 0; i < 6; i++)
        {
            GameObject slot = Instantiate(deckSlotPrefab, deckSlotParent);
            UIDeckSlot uiSlot = slot.GetComponent<UIDeckSlot>();
            deckSlotList.Add(uiSlot);
        }
    }

    public void SetDeckSlots() // 덱 슬롯 데이터 리셋용
    {
        var normalUnitIDs = DeckManager.Instance.GetAllNormalUnit();
        //var leaderUnitID = DeckManager.Instance.GetLeaderUnit();
        int? leaderUnitID = DeckManager.Instance.GetLeaderUnit();
        Debug.Log("현재 덱 유닛 수: " + normalUnitIDs.Count);
        Debug.Log("덱 유닛 목록: " + string.Join(",", normalUnitIDs));

        for (int i = 0; i < deckSlotList.Count; i++)
        {
            UIDeckSlot slot = deckSlotList[i];

            if (i < normalUnitIDs.Count)
            {
                UnitStats stats = UnitDataManager.Instance.GetStats(normalUnitIDs[i]);
                slot.unitImage.sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");
                slot.unitImage.color = Color.white;
                slot.unitData = stats;
                slot.SetRaceIcon(stats);

                //UIUnitInfo.instance.ShowInfo(stats);
            }
            else
            {
                slot.unitImage.sprite = null;
                slot.unitImage.color = new Color(1, 1, 1, 0);
                slot.unitData = null;

                slot.raceIcon.sprite = null;
                slot.raceIcon.color = new Color(1, 1, 1, 0);

                //UIUnitInfo.instance.ShowInfo(null);
            }
        }

        if (leaderSlot != null)
        {
            if (leaderUnitID.HasValue)
            {
                var leaderStats = UnitDataManager.Instance.GetStats(leaderUnitID.Value);

                if (leaderStats != null)
                {
                    leaderSlot.unitImage.sprite = Resources.Load<Sprite>($"SPUMImg/{leaderStats.ModelName}");
                    leaderSlot.unitImage.color = Color.white;
                    leaderSlot.unitData = leaderStats;
                    leaderSlot.SetRaceIcon(leaderStats);
                    //UIUnitInfo.instance.ShowleaderInfo(leaderStats);
                }

                else
                {
                    ClearLeaderSlot();
                }
            }

            else
            {
                ClearLeaderSlot();
            }
        }
    }
    
    void ClearLeaderSlot()
    {
        leaderSlot.unitData = null;
        leaderSlot.unitImage.sprite = null;
        leaderSlot.unitImage.color = new Color(1, 1, 1, 0);

        leaderSlot.raceIcon.sprite = null;
        leaderSlot.raceIcon.color = new Color(1, 1, 1, 0);
    }

    public void SetMyUnitIcons()
    {
        // 기존 아이콘 정리
        foreach (Transform child in myUnitContentParent)
        {
            Destroy(child.gameObject);
        }



        //if (currentMode == DeckMode.DeckBuild)
        //{
        //    sortedUnitID = cachedUnitOrder.OrderBy(id => DeckManager.Instance.CheckInDeck(id)).ToList();
        //}
        //else if (currentMode == DeckMode.Preset && currentPresetDeck != null)
        //{
        //    sortedUnitID = cachedUnitOrder.OrderBy(id => currentPresetDeck.ContainsUnit(id)).ToList();
        //}
        //else
        //{
        //    sortedUnitID = cachedUnitOrder;
        //}
        List<int> sortedUnitID = cachedUnitOrder;

        foreach (int unitID in sortedUnitID)
        {
            var stats = UnitDataManager.Instance.GetStats(unitID);
            if (stats == null) continue;

            if (raceFilter.HasValue && stats.RaceID != raceFilter.Value) continue; // 종족 필터. 미궁의 탑을 위함.

            switch (currentFilter)
            {
                case UnitFilterType.Undead:
                    if (stats.RaceID != 0) continue;
                    break;
                case UnitFilterType.Crawler:
                    if (stats.RaceID != 1) continue;

                    break;
                case UnitFilterType.All:
                    break;
            }


            GameObject iconGO = Instantiate(unitIconPrefab, myUnitContentParent);
            var unitIcon = iconGO.GetComponent<UIUnitIcon>();
            unitIcon.Setup(stats);

            bool shouldDisable = DeckManager.Instance.CheckInDeck(stats.ID);
            //if (currentDeck != DeckMode.DeckBuild)
            //{
            //    shouldDisable = DeckManager.Instance.CheckInDeck(stats.ID);
            //}
            //else if (currentMode == DeckMode.Preset && currentPresetDeck != null)
            //{
            //    shouldDisable = currentPresetDeck.ContainsUnit(stats.ID);
            //}

            if (shouldDisable)
            {
                unitIcon.SetDisabled();
            }
        }
    }

    public void CacheUnitListOrder()
    {
        var allUnits = PlayerDataManager.Instance.GetAllNormalUnit();
        cachedUnitOrder = allUnits
            .OrderByDescending(unit => unit.IsHero)
            .ThenBy(unit => unit.ID)
            .Select(u => u.ID)
            .ToList();
    }

    //public void OnToggleUndead(bool isOn)
    //{
    //    if (isOn)
    //    {
    //        RefreshFilter();

    //    }

    //}

    //public void OnToggleCrawler(bool isOn)
    //{
    //    if (isOn)
    //    {
    //        RefreshFilter();

    //    }
    //}

    //public void RefreshFilter()
    //{
    //    bool undeadOn = undeadToggle.isOn;
    //    bool crawlerOn = crawlerToggle.isOn;

    //    if (undeadOn && !crawlerOn)
    //    {
    //        currentFilter = UnitFilterType.Undead;
    //    }
    //    else if (!undeadOn && crawlerOn)
    //    {
    //        currentFilter = UnitFilterType.Crawler;
    //    }

    //    else
    //    {
    //        currentFilter = UnitFilterType.All;
    //    }

    //    SetMyUnitIcons();
    //}

    //public void SetRaceFilter(int? stageID)
    //{
    //    raceFilter = stageID;
    //    SetMyUnitIcons();
    //}

    //public void SetMode(DeckMode mode, UIPresetDeck presetDeck = null)
    //{
    //    currentMode = mode;
    //    currentPresetDeck = presetDeck;
    //}

    private void OnClickFilter(UnitFilterType selected)
    {
        if (currentFilter == selected)
        {
            currentFilter = UnitFilterType.All;
        }
        else
        {
            currentFilter = selected;
        }
        SFXManager.Instance.PlaySFX(0);
        UpdateFilterUI();
        SetMyUnitIcons();
    }

    private void UpdateFilterUI()
    {
        SetButtonAlpha(allBtnImg, currentFilter == UnitFilterType.All ? 1f : 0.5f);
        SetButtonAlpha(undeadBtnImg, currentFilter == UnitFilterType.Undead ? 1f : 0.5f);
        SetButtonAlpha(crawlerBtnImg, currentFilter == UnitFilterType.Crawler ? 1f : 0.5f);
    }

    private void SetButtonAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }

    public void SelectedUnitIcon(UIUnitIcon icon)
    {
        if (selectedIcon != null)
        {
            selectedIcon.SetSelected(false);
        }

        selectedIcon = icon;
        selectedIcon.SetSelected(true);

        UIUnitInfo.instance.ShowInfo(icon.GetStats());
    }

}
