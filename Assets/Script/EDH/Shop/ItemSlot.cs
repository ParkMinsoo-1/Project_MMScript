using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image ItemIcon;                 // 아이템 이미지

    [SerializeField] private TMP_Text ItemCost;             // 아이템 가격
    [SerializeField] private TMP_Text ItemNameText;         // 아이템 이름
    [SerializeField] public TMP_Text TotalAtempt;          // 아이템 구매 가능 횟수

    [SerializeField] private Button itemSlot;               // 아이템 슬롯

    private PurchaseSync _purchaseSync;

    private Item _Item;

    public void init(Item item, PurchaseSync purchaseSync)
    {
        _Item = item;
        _purchaseSync = purchaseSync;
        ItemIcon.sprite = Resources.Load<Sprite>($"Currency/{_Item.ItemIcon}");          // 아이콘 나오면 사용할 예정

        Debug.Log(_Item +"정보 들어옴");

       
        Debug.Log($"로드하려는 경로: Currency/{item.ItemIcon}");
        Debug.Log(GetItems(item).Count +"들어온 갯수");

        ItemNameText.text = item.Name;
        ItemCost.text = NumberFormatter.FormatNumber(_Item.Cost);
        //TotalAtempt.text = NumberFormatter.FormatNumber(_Item.DailyBuy);

        if (PlayerDataManager.Instance.player.itemPurchaseLeft.TryGetValue(_Item.ID, out int left))
        {
            TotalAtempt.text = NumberFormatter.FormatNumber(left);
            _Item.DailyBuy = left;
        }
        else
        {
            TotalAtempt.text = NumberFormatter.FormatNumber(_Item.DailyBuy);
        }

        itemSlot.onClick.RemoveAllListeners();

        itemSlot.onClick.AddListener(OnButtonClicked);
    }
    public void OnButtonClicked()
    {
        _purchaseSync.Init(_Item, this);
        //리펙터링 필요
        UIController.Instance.PurchaseUIBox.SetActive(true);
        ItemSlotSet();
        SFXManager.Instance.PlaySFX(15);
    }

    private static Dictionary<int, Item> GetItems(Item item)
    {
        return ItemListLoader.Instance.GetAllList();
    }

    public void ItemSlotSet()
    {
        UIController.Instance.PurchaseUIBox.GetComponent<PurchaseBoxSet>()._Item = _Item;
        UIController.Instance.PurchaseUIBox.GetComponent<PurchaseBoxSet>().SetitemIcon(ItemIcon.sprite);
        _purchaseSync.PurchAct = (count) => 
        {
            if (!PlayerDataManager.Instance.player.itemPurchaseLeft.ContainsKey(_Item.ID))
            {
                PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID] = _Item.DailyBuy;
            }

            PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID] -= count;
            if (PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID] < 0)
                PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID] = 0;

            TotalAtempt.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID]);

            PlayerDataManager.Instance.Save();
        }; 
    }
}
