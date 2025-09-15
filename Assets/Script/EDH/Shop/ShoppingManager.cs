using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShoppingManager : Singleton<ShoppingManager>
{
    public GameObject Merchandise;
    public ItemListLoader ItemListLoader;
    public Transform Content;
    
    public TMP_Text GoldAmount; //골드 상점용
    public TMP_Text PityCount; // 뽑기 횟수 =  증명서 갯수

    public TMP_Text TicketCount;
    public TMP_Text BluprintCount;
    public TMP_Text buildToolCount;


    void Start()
    {
        ItemListLoader.GetAllList();
        ItemSlot.Instantiate(Content);
        if (ItemListLoader.Instance == null)
        {
            Instantiate(ItemListLoader);
        }
        UIController.Instance.PurchaseUIBox.SetActive(false);
    }

    public void ShowNowGold()
    {
        GoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold); 
    }

    public void ShowNowCertificate()
    {
        PityCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.certi);
    }
    public void ShowNowTiket()
    {
        TicketCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.ticket);
    }
    public void ShowNowBlueprint()
    {
        BluprintCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.bluePrint);
    }

    public void ShowNowBuildTool()
    {
        buildToolCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.tribute);
    }
}