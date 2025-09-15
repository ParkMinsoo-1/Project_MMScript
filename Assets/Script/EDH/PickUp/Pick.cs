using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using Button = UnityEngine.UI.Button;

public class Pick : MonoBehaviour
{
    public TextMeshProUGUI ShowTicketAmountText; // 소유하고 있는 티켓 수
    public TextMeshProUGUI PityCount;            // 모집 마일리지

    public Button PickOnce; //  1회 버튼
    public Button PickTen;  // 10회 버튼
    public Button Exitbutton;// 뽑기 그만두는 버튼

    public Button RePickOne; //다시  1회 뽑기 버튼
    public Button RePickTen; //다시 10회 뽑기 버튼

    public GameObject PickTenPage; // 10회 뽑기 화면
    public GameObject BtnList;

    [SerializeField]
    public GotchaInit gotchaInit;
    public PickSlotSpawner pickSlotSpawner;

    private BackHandlerEntry entry;


    private void Awake()
    {
        PickOnce.onClick.AddListener(PickOneTime);       //  1회 뽑기
        PickTen.onClick.AddListener(PickTenTimes);      // 10회 뽑기
        RePickOne.onClick.AddListener(() => PickOneTime());      //  1회 다시 뽑기
        RePickTen.onClick.AddListener(() => PickTenTimes());     // 10회 다시 뽑기
        Exitbutton.onClick.AddListener(ExitPickPage);

        Debug.Log(PlayerDataManager.Instance.player.ticket.ToString() + "티켓 보유수");
    }

    private void OnEnable()
    {
        ShowTicketAmountText.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.ticket);          // 현재 보유 티켓 수량
        PityCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.certi);                      // 현재 마일리지

        //외부에서 데이터 가져와야함. 플레이어에서 데이터 가져와야함.(완료)
    }
    public void ExitPickPage()
    {
        PickTenPage.SetActive(false);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(2);
    }


    public void PickOneTime()
    {
        SFXManager.Instance.PlaySFX(0);
        if (PickUp(1))
        {
            SoundManager.Instance.PlayBGM(10);
            RePickOne.gameObject.SetActive(true);
            RePickTen.gameObject.SetActive(false);
            BtnList.SetActive(false);
            PickTenPage.SetActive(true);
            pickSlotSpawner.SpawnCard(1);
            ShowTicketAmountText.text = NumberFormatter.FormatNumber(gotchaInit.state == -1 ? PlayerDataManager.Instance.player.ticket : PlayerDataManager.Instance.player.specTicket);
        }
    }

    public void PickTenTimes()
    {
        SFXManager.Instance.PlaySFX(0);
        if (PickUp(10))
        {
            SoundManager.Instance.PlayBGM(10);
            RePickOne.gameObject.SetActive(false);
            RePickTen.gameObject.SetActive(true);
            BtnList.SetActive(false);
            PickTenPage.SetActive(true);
            pickSlotSpawner.SpawnCard(10);
            ShowTicketAmountText.text = NumberFormatter.FormatNumber(gotchaInit.state == -1 ? PlayerDataManager.Instance.player.ticket : PlayerDataManager.Instance.player.specTicket);
        }
    }
    public bool PickUp(int num)
    {
        bool spec = gotchaInit.state == -1;
        var ticket = spec ? PlayerDataManager.Instance.player.ticket : PlayerDataManager.Instance.player.specTicket;
        var pickTable = PickUpListLoader.Instance.GetAllPickList();
        if (ticket >= num)
        {
            if (spec)
                PlayerDataManager.Instance.UseTicket(num);
            else
                PlayerDataManager.Instance.UseSpecTicket(num);
            ticket = Math.Max(ticket, 0); // 예외처리
            Debug.Log(NumberFormatter.FormatNumber(ticket) + "티켓 보유수");

            PlayerDataManager.Instance.player.certi += num;

            ShowTicketAmountText.text = NumberFormatter.FormatNumber(ticket);
            PityCount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.certi);
            BackHandlerManager.Instance.SetBackEnabled(false);
            entry = new BackHandlerEntry(
               priority: 100,
               isActive: () => PickTenPage.activeInHierarchy,
               onBack: () => PickTenPage.SetActive(false)
            );
            BackHandlerManager.Instance.Register(entry);
            return true;
        }
        else
        {
            UIController.Instance.TicketNotEnoungh();
            return false;
        }
    }
    
}
