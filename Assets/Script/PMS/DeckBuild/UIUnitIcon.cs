using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUnitIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

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
        //SetTagIcon(stats);
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
    //void SetTagIcon(UnitStats stats)
    //{
    //    if (stats.tagId == null || stats.tagId.Count == 0)
    //    {
    //        tagIcon1.gameObject.SetActive(false);
    //        tagIcon2.gameObject.SetActive(false);
    //        return;
    //    }

    //    if (stats.tagId.Count >= 1)
    //    {
    //        var tagSprite1 = Resources.Load<Sprite>($"Tags/Tag_{stats.tagId[0]}");
    //        if (tagSprite1 != null)
    //        {
    //            tagIcon1.sprite = tagSprite1;
    //            tagIcon1.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            tagIcon1.gameObject.SetActive(false);
    //        }
    //    }

    //    if (stats.tagId.Count >= 2)
    //    {
    //        var tagSprite2 = Resources.Load<Sprite>($"Tags/Tag_{stats.tagId[1]}");
    //        if (tagSprite2 != null)
    //        {
    //            tagIcon2.sprite = tagSprite2;
    //            tagIcon2.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            tagIcon2.gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        tagIcon2.gameObject.SetActive(false);
    //    }
    //}


    public UnitStats GetStats()
    {
        return myStats;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); // 캔버스 루트로 옮겨서 가장 위에 보이게
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.3f; // 드래그 중 반투명
        isDropped = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDropped)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (UIDeckBuildManager.instance != null && UIDeckBuildManager.instance.deckPanel.activeSelf)
        {
            UIDeckBuildManager.instance.SetMyUnitIcons();
            UIDeckBuildManager.instance.SetDeckSlots();
        }
        isDropped = false;
    }

    public void SetDropped()
    {
        isDropped = true;
    }


    public void SetDisabled()
    {
        canvasGroup.alpha = 0.5f;
        this.enabled = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.unscaledTime - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            AddUnitToDeck();
        }
        else
        {
            UIDeckBuildManager.instance.SelectedUnitIcon(this);
        }

        lastClickTime = Time.unscaledTime;
    }

    private void AddUnitToDeck()
    {
        if (myStats == null)
        {
            return;
        }

        bool success = DeckManager.Instance.TryAddUnitToDeck(myStats.ID);

        if (success)
        {
            UIDeckBuildManager.instance.SetMyUnitIcons();
            UIDeckBuildManager.instance.SetDeckSlots();
            DeckManager.Instance.SaveCurrentDeckToPreset();
            PlayerDataManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning($"{myStats.Name} 유닛을 덱에 추가하지 못했습니다.");
        }
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
