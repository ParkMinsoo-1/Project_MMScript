using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderUnitPannel : MonoBehaviour
{
    [Header ("리더유닛 리스트")]
    [SerializeField] private GameObject leaderPrefab;
    [SerializeField] private Transform parent;

    [Header("리더유닛 인포")]
    [SerializeField] private GameObject infoImage;
    [SerializeField] private GameObject infoStats;
    [SerializeField] private GameObject infoName;
    [SerializeField] private TextMeshProUGUI nameValueText;
    [SerializeField] private TextMeshProUGUI hpValueText;
    [SerializeField] private TextMeshProUGUI damageValueText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Image hpIcon;
    [SerializeField] private Image damageIcon;
    [SerializeField] private Image typeIcon;
    [SerializeField] private Sprite hpSprite;
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite typeSprite;

    [Header("스킬")]
    [SerializeField] private GameObject skillBox;

    [Header("버튼")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button selectBtn;

    [SerializeField] private GameObject blocker;

    private List<int> cachedUnitOrder = new();
    private LeaderUnit selectedIcon;

    public static UILeaderUnitPannel instance;
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
        ClearInfo();
        CacheUnitListOrder();
        SetMyUnitIcons();

        closeBtn.onClick.RemoveListener(CloseBtn);
        closeBtn.onClick.AddListener(CloseBtn);

        selectBtn.onClick.RemoveListener(SelectBtn);
        selectBtn.onClick.AddListener(SelectBtn);
    }

    public void SetMyUnitIcons()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        List<int> sortedUnitID = cachedUnitOrder;
        int leaderUnitID = DeckManager.Instance.GetLeaderUnit();

        foreach (int unitID in sortedUnitID)
        {
            var stats = UnitDataManager.Instance.GetStats(unitID);
            if (stats == null) continue;

            GameObject iconGO = Instantiate(leaderPrefab, parent);
            var unitIcon = iconGO.GetComponent<LeaderUnit>();
            unitIcon.Setup(stats);

            bool isInDeck = DeckManager.Instance.CheckInDeck(stats.ID);

            if (isInDeck && stats.ID != leaderUnitID)
            {
                unitIcon.SetDisabled();
            }
            //else
            //{
            //    unitIcon.SetEnabled(); // 비활성화 상태 해제(없으면 추가)
            //}
        }
    }
    public void ShowInfo(UnitStats stats)
    {
        if (stats != null)
        {
            infoImage.gameObject.SetActive(true);
            infoStats.gameObject.SetActive(true);
            infoName.gameObject.SetActive(true);

            infoImage.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");
            if (stats == null)
            {
                nameValueText.text = "";
                hpValueText.text = "";
                damageValueText.text = "";
                return;
            }

            nameValueText.text = $"{stats.Name}";
            hpValueText.text = $"{stats.MaxHP.ToString()}";
            damageValueText.text = $"{stats.Damage.ToString()}";
            typeText.text = $"{RaceManager.GetNameByID(stats.RaceID)}";
            hpIcon.sprite = hpSprite;
            damageIcon.sprite = damageSprite;
            typeIcon.sprite = typeSprite;
            //SetTagIcon(stats);
            skillBox?.SetActive(stats.IsHero);

            if (skillBox != null)
            {
                var skillUI = skillBox.GetComponent<UISkillBox>();
                if (stats.IsHero && skillUI != null)
                {
                    skillUI.SetSkill(stats);
                }
                else if (!stats.IsHero)
                {
                    skillUI.Hide();
                }
                else
                {
                    //Debug.Log("UISkillBox 컴포넌트를 찾을 수 없습니다.");
                }
            }

        }

        else
        {
            ClearInfo();

        }
    }

    public void CacheUnitListOrder()
    {
        var allUnits = PlayerDataManager.Instance.GetAllLeaderUnit();
        cachedUnitOrder = allUnits
            .OrderByDescending(unit => unit.IsHero)
            .ThenBy(unit => unit.ID)
            .Select(u => u.ID)
            .ToList();
    }


    public void ClearInfo()
    {
        
        infoImage.gameObject.SetActive(false);
        infoStats.gameObject.SetActive(false);
        infoName.gameObject.SetActive(false);
        //tagParent.gameObject.SetActive(false);
    }

    public void SelectedUnitIcon(LeaderUnit icon)
    {
        if (selectedIcon != null)
        {
            selectedIcon.SetSelected(false);
        }

        selectedIcon = icon;
        selectedIcon.SetSelected(true);

        ShowInfo(icon.GetStats());
    }

    public void CloseBtn()
    {
        ClearInfo();
        this.gameObject.SetActive(false);
        blocker.gameObject.SetActive(false);
    }

    public void SelectBtn()
    {

        if (selectedIcon == null)
        {
            Debug.LogWarning("리더 유닛을 선택해주세요.");
            return;
        }

        var stats = selectedIcon.GetStats();
        if (stats == null)
        {
            Debug.LogWarning("선택한 리더 유닛 정보가 없습니다.");
            return;
        }

        bool success = DeckManager.Instance.TrySetLeaderUnit(stats.ID);
        if (!success)
        {
            Debug.LogWarning($"리더 유닛 {stats.ID}를 덱에 설정하지 못했습니다.");
            return;
        }

        DeckManager.Instance.SaveCurrentDeckToPreset();

        UIDeckBuildManager.instance.SetMyUnitIcons();
        UIDeckBuildManager.instance.SetDeckSlots();
        SetMyUnitIcons();
        PlayerDataManager.Instance.Save();
    }
}
