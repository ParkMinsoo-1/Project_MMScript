using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class CertiPurchaseBoxSet : MonoBehaviour
{
    public TMP_Text UnitDescriptionText; // 아이템 설명 텍스트

    public GameObject PurchaseCertiUnitBox;   // 구매UI 상자
    public GameObject CertiDescriptionBox;  // 아이템 설명 창

    public Image UnitIcon;

    public Button PurchaseItemIcon; // 상품 아이콘
    public Button CancelButton;     // 구매 취소 버튼

    public PickInfo _PickInfo;

    [Header("이미지/정보")]
    //[SerializeField] private GameObject infoImage;
    [SerializeField] private GameObject infoStats;
    //[SerializeField] private GameObject infoName;
    //[SerializeField] private TextMeshProUGUI nameValueText;
    [SerializeField] private TextMeshProUGUI hpValueText;
    [SerializeField] private TextMeshProUGUI damageValueText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private Image hpIcon;
    [SerializeField] private Image damageIcon;
    [SerializeField] private Image typeIcon;
    [SerializeField] private Sprite hpSprite;
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite typeSprite;
    [SerializeField] private TextMeshProUGUI price;

    [Header("스킬")]
    [SerializeField] private GameObject skillBox;

    private BackHandlerEntry entry;

    private void Start()
    {
        PurchaseItemIcon.onClick.AddListener(DescriptionSet);
        SFXManager.Instance.PlaySFX(0);
        CancelButton.onClick.AddListener(TabClose);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => gameObject.activeInHierarchy,
           onBack: TabClose
       );
        BackHandlerManager.Instance.Register(entry);
    }

    // Update is called once per frame
    public void TabClose()
    {
        PurchaseCertiUnitBox.SetActive(false);
        CertiDescriptionBox.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }

    public void DescriptionSet()
    {
        if (CertiDescriptionBox.activeSelf)
        {
            CertiDescriptionBox.SetActive(false);
            SFXManager.Instance.PlaySFX(0);
            return;
        }

        CertiDescriptionBox.SetActive(true);
        CertiDescriptionBox.GetComponentInChildren<TMP_Text>().text = _PickInfo.Description;
        SFXManager.Instance.PlaySFX(0);
    }

    public void SetUnitIcon(Sprite sprite)
    {
        UnitIcon.sprite = sprite;
    }

    public void ShowInfo(PickInfo info)
    {
        UnitStats stats = UnitDataManager.Instance.GetStats(info.ID);
        if (stats == null)
        {
            ClearInfo();
            return;
        }

        //infoImage.gameObject.SetActive(true);
        infoStats.gameObject.SetActive(true);
        //infoName.gameObject.SetActive(true);

        //var sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");
        //var image = infoImage.GetComponentInChildren<Image>();
        //if (image != null)
        //{
        //    image.sprite = sprite;
        //}

        //nameValueText.text = stats.Name;
        hpValueText.text = stats.MaxHP.ToString();
        damageValueText.text = stats.Damage.ToString();
        typeText.text = RaceManager.GetNameByID(stats.RaceID);
        price.text = stats.warrant.ToString();

        hpIcon.sprite = hpSprite;
        damageIcon.sprite = damageSprite;
        typeIcon.sprite = typeSprite;

        if (skillBox != null)
        {
            skillBox.SetActive(stats.IsHero);
            var skillUI = skillBox.GetComponent<UISkillBox>();
            if (stats.IsHero)
            {
                skillUI?.SetSkill(stats);
            }
            else
            {
                skillUI?.Hide();
            }
        }
    }
    public void ClearInfo()
    {
        //infoImage.gameObject.SetActive(false);
        infoStats.gameObject.SetActive(false);
        //infoName.gameObject.SetActive(false);
    }
}
