using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaderUnit : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage; //유닛 이미지 표시
    public TextMeshProUGUI costText; //코스트 소모량 표시
    public CanvasGroup canvasGroup; //캔버스 그룹, 투명도 위함
    public Transform originalParent; //시작 위치
    public UnitStats myStats;
    public Image slotBG;
    public Image raceIcon;
    public GameObject leaderMark;

    [SerializeField] private Sprite undeadSprite;
    [SerializeField] private Sprite crawlerSprite;

    [SerializeField] private GameObject selectedMark;

    //[SerializeField] private Image tagIcon1;
    //[SerializeField] private Image tagIcon2;

    private bool isDropped = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(UnitStats stats)
    {
        myStats = stats;
        costText.text = stats.Cost.ToString();
        iconImage.sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");

        bool leader = stats.IsHero ? true : false;

        leaderMark.SetActive(leader);

        SetRaceIcon(stats);
        SetRaceColor(stats);
        
    }

    void SetRaceColor(UnitStats stats)
    {
        var raceID = stats.RaceID;
        switch (raceID)
        {
            case 0:
                //slotBG.color = Color.white;
                slotBG.color = new Color32(0x66, 0x87, 0xE9, 0xFF);
                //slotBG.color = new Color32(0x9E, 0xA5, 0xEE, 0xFF);
                break;
            case 1:
                slotBG.color = new Color32(0x78, 0xB6, 0xFF, 0xFF);
                break;
        }

    }

    void SetRaceIcon(UnitStats stats)
    {
        var raceID = stats.RaceID;
        switch (raceID)
        {
            case 0:
                raceIcon.sprite = undeadSprite;
                break;
            case 1:
                raceIcon.sprite = crawlerSprite;
                break;
        }
    }

    public UnitStats GetStats()
    {
        return myStats;
    }


    public void SetDisabled()
    {
        canvasGroup.alpha = 0.5f;
        this.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UILeaderUnitPannel.instance.SelectedUnitIcon(this);
    }

    public void UpdateInteractable(bool disabled)
    {
        canvasGroup.alpha = disabled ? 0.5f : 1f;
        this.enabled = !disabled;
    }

    public void SetSelected(bool selected)
    {
        selectedMark.SetActive(selected);
    }
}
