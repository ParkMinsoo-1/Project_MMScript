using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ItemID
{
    //101 =  일반 모집, 102 = 건설도구, 103 =  설계도, 104 = 특수모집

    NormalRecruit = 101,
    BuildTool = 102,
    Blueprint = 103,
    SpecialRecruit = 104
}
public class PurchaseSync : MonoBehaviour
{
    public TMP_InputField InputAmount;   // 수량 입력칸
    public TextMeshProUGUI totalCost;
    public TextMeshProUGUI NowHave;

    public Button AddButton;        // 수량 추가 버튼
    public Button SubtractButton;   // 수량 빼기버튼

    public Button PurchaseButton;   // 아이템 구매 버튼

    public Item _Item;
    public ItemSlot iSlot;

    public TMP_Text NotEnoughBoxText;  // 재화 부족 경고 텍스트
    public GameObject NotEnoughBox;    // 재화 부족 경고
    public PurchaseBoxSet purchaseBoxSet;

    private int AttemptLeft => GetAttemptLeft();            //남은 구매 수량

    public Action<int> PurchAct { get; set; }

    public void Start()
    {
        InputAmount.text = "1"; // 기본 세팅.

        //버튼 누적 호출 스택 초기화
        AddButton.onClick.RemoveAllListeners();
        SubtractButton.onClick.RemoveAllListeners();
        PurchaseButton.onClick.RemoveAllListeners();

        AddButton.onClick.AddListener(Add);            // 더하기 
        SubtractButton.onClick.AddListener(Subttract); // 빼기

        PurchaseButton.onClick.AddListener(PurchaseItem); // 구매
    }
    public void Add()
    {
        int amount = int.Parse(InputAmount.text);
        int maxAmount = _Item.DailyBuy;

        if (amount < maxAmount)
        {
            amount++;
            InputAmount.text = amount.ToString();

            if (amount > maxAmount)// 최대치일때
                AddButton.interactable = false;
        }
        if (amount > 1)
            SubtractButton.interactable = true;
        UpdateTotal();
        SFXManager.Instance.PlaySFX(7);
    }
    public void Subttract()
    {
        int amount = int.Parse(InputAmount.text);
        Debug.Log(amount);
        if (amount > 1)
        {
            amount -= 1;
            InputAmount.text = amount.ToString();
        }
        else { Debug.Log("s"); }
        UpdateTotal();
        SFXManager.Instance.PlaySFX(7);
    }
    public void PurchaseItem()
    {
        //AttemptLeft = _Item.DailyBuy;
        //int Attempt = AttemptLeft;

        if (!int.TryParse(InputAmount.text, out int amount))
        {
            amount = 1;
        }

        // 최대 구매 횟수 제한 적용
        if (amount > AttemptLeft)
        {
            amount = AttemptLeft;
            InputAmount.text = amount.ToString();
        }


        int Cost = _Item.Cost;
        //int Amount = int.Parse(InputAmount.text);
       

        if ((ItemID)_Item.ID == ItemID.NormalRecruit)
            PurchaseLogic(amount, Cost, PlayerDataManager.Instance.AddTicket);
        else if ((ItemID)_Item.ID == ItemID.BuildTool)
            PurchaseLogic(amount, Cost, PlayerDataManager.Instance.AddTribute);
        else if ((ItemID)_Item.ID == ItemID.Blueprint)
            PurchaseLogic(amount, Cost, PlayerDataManager.Instance.AddBluePrint);
        else if ((ItemID)_Item.ID == ItemID.SpecialRecruit)
            PurchaseLogic(amount, Cost, PlayerDataManager.Instance.AddSpecT);
        ShoppingManager.Instance.ShowNowGold();
        purchaseBoxSet.TabClose();
        SFXManager.Instance.PlaySFX(0);
    }
    public void PurchaseLogic(int amount, int cost, Action<int> ItemsP)
    {
        //AttemptLeft = _Item.DailyBuy;
        int Attempt = AttemptLeft;

        if (Attempt <= 0)
        {
            UIController.Instance.AtemptNotEnoungh();
            return;
        }

        if (amount > Attempt)
        {
            amount = Attempt;
            InputAmount.text = amount.ToString();
        }
        int totalCost = cost * amount;

        if (PlayerDataManager.Instance.player.gold >= totalCost)
        {
            PlayerDataManager.Instance.UseGold(totalCost);
            ItemsP.Invoke(amount);

            int remaining = Attempt - amount;
            if (remaining < 0) remaining = 0;

            PlayerDataManager.Instance.player.itemPurchaseLeft[_Item.ID] = remaining;
            PlayerDataManager.Instance.Save();

            InputAmount.text = "1";
            PurchAct?.Invoke(amount);
        }
        else
        {
            UIController.Instance.GoldNotEnoungh();
        }

        //int Cost = _Item.Cost;
        //int Amount = int.Parse(InputAmount.text);
        //if (AttemptLeft > 0)
        //{
        //    if (int.Parse(InputAmount.text) > _Item.DailyBuy)
        //    {
        //        Amount = AttemptLeft;
        //        InputAmount.text = Amount.ToString();
        //    }
        //    if (PlayerDataManager.Instance.player.gold >= int.Parse(InputAmount.text) * Cost)
        //    {
        //        PlayerDataManager.Instance.UseGold(Cost * Amount);
        //        ItemsP.Invoke(Amount);
        //        AttemptLeft -= Amount;
        //        InputAmount.text = "1";
        //        PurchAct.Invoke(Amount);
        //        _Item.DailyBuy = AttemptLeft;
        //    }
        //    else
        //    {
        //        UIController.Instance.GoldNotEnoungh();
        //    }
        //}
        //else
        //{
        //    UIController.Instance.AtemptNotEnoungh();
        //}
    }
    public void Init(Item item, ItemSlot slot)
    {
        //Debug.Log("c");
        _Item = item;
        iSlot = slot;

        if (PlayerDataManager.Instance.player.itemPurchaseLeft.TryGetValue(_Item.ID, out int left))
            _Item.DailyBuy = left;
        else
            _Item.DailyBuy = _Item.DailyBuy;
        
        InputAmount.text = "1";
        UpdateTotal();
    }
    public void UpdateTotal()
    {
        if (int.TryParse(InputAmount.text, out int amount))
            totalCost.text = NumberFormatter.FormatNumber(amount * _Item.Cost);
        else
            totalCost.text = NumberFormatter.FormatNumber(_Item.Cost);
    }

    private int GetAttemptLeft()
    {
        var player = PlayerDataManager.Instance.player;
        if (player.itemPurchaseLeft.TryGetValue(_Item.ID, out int left))
        {
            return left;
        }
        else
        {
            player.itemPurchaseLeft[_Item.ID] = _Item.DailyBuy;
            PlayerDataManager.Instance.Save();
            return _Item.DailyBuy;
        }
    }


}