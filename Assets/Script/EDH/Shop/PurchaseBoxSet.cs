using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PurchaseBoxSet : MonoBehaviour
{
    public TMP_Text ItemDescriptionText; // 아이템 설명 텍스트

    public GameObject purchaseUIBox;   // 구매UI 상자

    public Button PurchaseItemIcon; // 상품 아이콘
    public Button CancelButton;     // 구매 취소 버튼

    public Image itemIcon;
    public Item _Item;

    private BackHandlerEntry entry;

    private void Start()
    {
        CancelButton.onClick.AddListener(TabClose);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => gameObject.activeInHierarchy,
           onBack: TabClose
       );
        BackHandlerManager.Instance.Register(entry);
    }

    public void TabClose()
    {
        purchaseUIBox.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }

    public void DescriptionSet()
    {
        Debug.Log("아이템 설명" + _Item.Description);
        ItemDescriptionText.text = _Item.Description; // null일경우 넣어주면 안됨.
    }
    public void SetitemIcon(Sprite sprite)
    {
        itemIcon.sprite = sprite;
        DescriptionSet();
    }
}
