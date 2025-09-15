using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CertiSlot : MonoBehaviour
{
    [SerializeField] public Image UnitIcon;                     // 유닛 아이콘
    [SerializeField] private Button certiSlot;                  // 증명서 유닛 슬롯
    [SerializeField] private TMP_Text CertiCost;
    [SerializeField] private CertiPurChaseSync certiPurChaseSync;
    [SerializeField] private TextMeshProUGUI certiName;
    [SerializeField] private TextMeshProUGUI certiDesc;
    [SerializeField] private GameObject hasMark;
    private PickInfo _Info;

    public void init(PickInfo pickInfo)
    {
        var stats = UnitDataManager.Instance.GetStats(pickInfo.ID);
        UnitIcon.sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");

        _Info = pickInfo;
        CertiCost.text = _Info.warrant.ToString();
        certiName.text = _Info.Name;
        certiSlot.onClick.RemoveAllListeners();

        bool isOwned = PlayerDataManager.Instance.HasUnit(_Info.ID);

        if (isOwned)
        {
            hasMark.SetActive(true);
            certiSlot.interactable = false;
            var color = UnitIcon.color;
            color.a = 0.4f;
            UnitIcon.color = color;
        }
        else
        {

            certiSlot.onClick.AddListener(() =>
            {
                Debug.Log("버튼 눌림");
                if (certiPurChaseSync == null)
                {
                    certiPurChaseSync = FindObjectOfType<CertiPurChaseSync>();
                }
                CertiPurChaseSync.Instance.Init(pickInfo, this);

                UIController.Instance.PurchaseCertiUnitBox.SetActive(true);
                CertiSlotSet();
                SFXManager.Instance.PlaySFX(15);
            });
        }
    }

    public void CertiSlotSet()
    {
        UIController.Instance.PurchaseCertiUnitBox.GetComponent<CertiPurchaseBoxSet>()._PickInfo = _Info;
        UIController.Instance.PurchaseCertiUnitBox.GetComponent<CertiPurchaseBoxSet>().SetUnitIcon(UnitIcon.sprite);
        UIController.Instance.PurchaseCertiUnitBox.GetComponent<CertiPurchaseBoxSet>().ShowInfo(_Info);
    }

    public void RefreshUI()
    {
        bool isOwned = PlayerDataManager.Instance.HasUnit(_Info.ID);

        if (isOwned)
        {
            hasMark.SetActive(true);
            certiSlot.interactable = false;
            var color = UnitIcon.color;
            color.a = 0.4f;
            UnitIcon.color = color;
        }
        else
        {
            hasMark.SetActive(false);
            certiSlot.interactable = true;
            var color = UnitIcon.color;
            color.a = 1f;
            UnitIcon.color = color;
        }
    }
}
