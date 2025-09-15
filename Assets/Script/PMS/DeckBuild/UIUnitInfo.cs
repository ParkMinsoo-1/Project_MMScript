using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInfo : MonoBehaviour
{
    [Header("이미지/정보")]
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

    [Header("태그")]
    [SerializeField] private Transform tagParent;
    [SerializeField] private GameObject tagIconPrefab;

    //[Header("리더 유닛 이미지/정보")]
    //[SerializeField] private Image leaderIMG;
    //[SerializeField] private TextMeshProUGUI costText;
    //[SerializeField] private Image costIcon;

    [Header("스킬")]
    [SerializeField] private GameObject skillBox;

    public static UIUnitInfo instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
            SetTagIcon(stats);
            skillBox?.SetActive(stats.IsHero);
            
            if (skillBox != null)
            {
                var skillUI = skillBox.GetComponent<UISkillBox>();
                if(stats.IsHero && skillUI != null)
                {
                    skillUI.SetSkill(stats);
                }
                else if (!stats.IsHero)
                {
                    skillUI.Hide();
                }
                else
                {
                    Debug.LogWarning("UISkillBox 컴포넌트를 찾을 수 없습니다.");
                }
            }

        }

        else
        {
            ClearInfo();
            //infoImage.gameObject.SetActive(false);
            //infoStats.gameObject.SetActive(false);
            //infoName.gameObject.SetActive(false);
        }
    }

    //public void ShowleaderInfo(UnitStats stats)
    //{
    //    if (stats != null)
    //    {
    //        leaderIMG.gameObject.SetActive(true);
    //        costText.gameObject.SetActive(true);
    //        costIcon.gameObject.SetActive(true);

    //        leaderIMG.sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");
    //        costText.text = stats.Cost.ToString();
    //    }

    //    else
    //    {
    //        leaderIMG.gameObject.SetActive(false);
    //        costText.gameObject.SetActive(false);
    //        costIcon.gameObject.SetActive(false);
    //    }
    //}

    public void ClearInfo()
    {
        //infoImage.sprite = null;
        infoImage.gameObject.SetActive(false);
        infoStats.gameObject.SetActive(false);
        infoName.gameObject.SetActive(false);
        tagParent.gameObject.SetActive(false);
    }

    void SetTagIcon(UnitStats stats)
    {
        tagParent.gameObject.SetActive(true);

        foreach (Transform child in tagParent)
        {
            Destroy(child.gameObject);
        }

        if (stats.tagId == null || stats.tagId.Count == 0)
            return;

        foreach (int tagId in stats.tagId)
        {
            Sprite tagSprite = Resources.Load<Sprite>($"Tags/Tag_{tagId}");
            if (tagSprite != null)
            {
                GameObject tagIconObj = Instantiate(tagIconPrefab, tagParent);
                Image iconImage = tagIconObj.GetComponentInChildren<Image>();
                iconImage.sprite = tagSprite;
            }
        }
    }


}
