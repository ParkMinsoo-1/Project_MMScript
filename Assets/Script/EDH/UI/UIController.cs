using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;


public class UIController : MonoBehaviour
{
    private static UIController instance;

    public static UIController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIController();
            }
            return instance;
        }
    }
    void Awake()
    {
        frameSetting.LoadFrame();
        if (Instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //기본 Ui
    public GameObject Main;         //메인
    public GameObject Stage;        //스테이지
    public GameObject DeckBuild;    //덱빌드
    public GameObject Shop;         //상점
    public GameObject HQ;           //내실
    public GameObject Gotta;        //뽑기

    //Pannal
    public GameObject PurchaseUIBox;         // 구매시 상자창
    public GameObject PurchaseCertiUnitBox;  // 유닛 교환시 상자창
    public GameObject CertiDescriptionBox;   // 증명서 유닛 설명창
    public GameObject NotEnoughBox;          // 재화 부족 경고 창
    public TMP_Text MainGoldAmount;          // 골드 표기
    public TMP_Text StoreGoldAmount;         // 상점에서 골드 표기
    public TMP_Text NotEnoughBoxText;        // 재화 부족 경고 텍스트                                             
    public TMP_Text mainStageGoldAmount;
    public TMP_Text gdStageGoldAmount;
    public TMP_Text towerStageGoldAmount;
    public GameObject QuitGamePanel;

    //열기버튼
    public Button UnitManagementButton;  // 덱빌딩
    public Button SelectStageButton;     // 스테이지 선택
    public Button UnitGotchaBotton;      // 유닛 뽑기
    public Button SacredPlaceButton;     // 내실
    public Button StoreButton;           // 상점

    // 닫기 버튼
    public Button CloseGottcha;         // 뽑기 닫기
    public Button CloseUnitManage;      // 유닛관리 닫기
    public Button CloseShop;            // 상점닫기 
    public Button CloseHQ;              // 전초기지 닫기
    public Button Closestage;           // 침략 닫기

    //기본 설정
    public BookMarkSet bookMarkSet;
    public FrameSetting frameSetting;

    private BackHandlerEntry entry;

    private void Start()
    {
        Main.SetActive(true);
        SetButton();
        ShowNowGold();
        PlayerCurrencyEvent.OnGoldChange += value => ShowNowGold();
        PlayerCurrencyEvent.OnTributeChange += value => ShowNowGold();
        SoundManager.Instance.PlayBGM(0);
        TutorialManager.Instance.OnEventTriggered("battleOver");
        UnitDataManager.Instance.LoadUnitData();
        entry = new BackHandlerEntry(
           priority: 30,
           isActive: () => Stage.activeInHierarchy && DeckBuild.activeInHierarchy,
           onBack: () =>
           {
               OnExitBtn();
           }
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => Stage.activeInHierarchy,
           onBack: OnExitBtn
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => DeckBuild.activeInHierarchy,
           onBack: OnExitBtn
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => Shop.activeInHierarchy,
           onBack: OnExitBtn
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => Gotta.activeInHierarchy,
           onBack: OnExitBtn
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 10,
           isActive: () => HQ.activeInHierarchy,
           onBack: OnExitBtn
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 5,
           isActive: () => Main.activeInHierarchy && !QuitGamePanel.activeInHierarchy,
           onBack: () => QuitGamePanel.SetActive(true)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 5,
           isActive: () => QuitGamePanel.activeInHierarchy,
           onBack: () => QuitGamePanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }

    public void SetButton()
    {
        UnitManagementButton.onClick.AddListener(UnitManageActive);
        SelectStageButton.onClick.AddListener(OpenStage);
        UnitGotchaBotton.onClick.AddListener(OpenGottcha);
        SacredPlaceButton.onClick.AddListener(OpenSacredPlace);
        StoreButton.onClick.AddListener(OpenShop);

        CloseGottcha.onClick.AddListener(OnExitBtn);
        CloseUnitManage.onClick.AddListener(OnExitBtn);
        CloseShop.onClick.AddListener(OnExitBtn);
        CloseHQ.onClick.AddListener(OnExitBtn);
        Closestage.onClick.AddListener(OnExitBtn);
    }

    public void UnitManageActive()
    {
        DeckBuild.SetActive(true);
        Main.SetActive(false);
        UIDeckBuildManager.instance.Init();
        SoundManager.Instance.PlayBGM(3);
        SFXManager.Instance.PlaySFX(0); // 버튼
    }
    public void OpenStage()
    {
        Stage.SetActive(true);
        Main.SetActive(false);
        
        SFXManager.Instance.PlaySFX(0); // 버튼
        TutorialManager.Instance.StartTuto(3);

        switch (StageManager.instance.lastSessionClearedStageType)
        {
            case 0:
                StageInit.instance.OnMainBtn();
                break;
            case 1:
                StageInit.instance.OnTowerBtn();
                break;
            case 2:
                StageInit.instance.OnGoldBtn();
                break;
            default:
                StageInit.instance.OnMainBtn();
                break;
        }

        StageManager.instance.Init();
    }
    public void OpenGottcha()
    {
        Gotta.SetActive(true);
        Main.SetActive(false);
        SoundManager.Instance.PlayBGM(2);
        SFXManager.Instance.PlaySFX(0); // 버튼
    }
    public void OpenSacredPlace()
    {
        HQ.SetActive(true);
        Main.SetActive(false);
        SoundManager.Instance.PlayBGM(4);
        SFXManager.Instance.PlaySFX(0); // 버튼
        TutorialManager.Instance.StartTuto(2);
    }
    public void OpenShop()
    {
        bookMarkSet.GoldStoreSet();
        Shop.SetActive(true);
        Main.SetActive(false);
        ShoppingManager.Instance.ShowNowCertificate();
        ShoppingManager.Instance.ShowNowTiket();
        ShoppingManager.Instance.ShowNowBlueprint();
        ShoppingManager.Instance.ShowNowBuildTool();
        SoundManager.Instance.PlayBGM(5);
        SFXManager.Instance.PlaySFX(0); // 버튼
        TutorialManager.Instance.StartTuto(1);
    }
    public void OnExitBtn()
    {
        //Debug.Log(Stage.activeSelf + " " + DeckBuild.activeSelf);
        
        if (Stage != null && DeckBuild != null && Stage.activeSelf && DeckBuild.activeSelf)
        {
            SFXManager.Instance.PlaySFX(0);
            DeckBuild.SetActive(false);
            return;
        }

        StageManager.instance.lastSessionClearedStageID = -1;
        StageManager.instance.lastSessionClearedStageType = 0;

        Debug.Log($"{StageManager.instance.lastSessionClearedStageID}");
        Debug.Log($"{StageManager.instance.lastSessionClearedStageType}");

        Main.SetActive(true);
        Stage.SetActive(false);
        DeckBuild.SetActive(false);
        Shop.SetActive(false);
        HQ.SetActive(false);
        Gotta.SetActive(false);
        SoundManager.Instance.PlayBGM(0);
        SFXManager.Instance.PlaySFX(0); // 버튼
    }
    public void ShowNowGold()
    {
        MainGoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold);
        StoreGoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold);
        mainStageGoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold);
        gdStageGoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.gold);
        towerStageGoldAmount.text = NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.tribute);
    }
 
    public void AtemptNotEnoungh() //구매 가능 횟수 부족 알림
    {
        UIController.Instance.NotEnoughBox.SetActive(true);
        NotEnoughBoxText.text = "모든 구매 횟수를 모두 사용하셨습니다.";
        StartCoroutine(HideNotEnoughBox());

        IEnumerator HideNotEnoughBox()
        {
            yield return new WaitForSeconds(1f); // 3초 대기
            UIController.Instance.NotEnoughBox.SetActive(false);       // 경고창 비활성화
        }
    }
    public void GoldNotEnoungh() // 골드 부족 알림
    {
        UIController.Instance.NotEnoughBox.SetActive(true);
        SFXManager.Instance.PlaySFX (6);
        NotEnoughBoxText.text = "골드가 부족합니다.";
        StartCoroutine(HideNotEnoughBox());

        IEnumerator HideNotEnoughBox()
        {
            yield return new WaitForSeconds(1f); // 3초 대기
            UIController.Instance.NotEnoughBox.SetActive(false);       // 경고창 비활성화
        }
    }
    public void CertiNotEnoungh() // 증명서 부족 알림
    {
        UIController.Instance.NotEnoughBox.SetActive(true);
        SFXManager.Instance.PlaySFX(6);
        NotEnoughBoxText.text = "증명서가 부족합니다.";
        StartCoroutine(HideNotEnoughBox());

        IEnumerator HideNotEnoughBox()
        {
            yield return new WaitForSeconds(1f); // 3초 대기
            UIController.Instance.NotEnoughBox.SetActive(false);       // 경고창 비활성화
        }
    }
    public void TicketNotEnoungh() // 티켓 부족 알림
    {
        UIController.Instance.NotEnoughBox.SetActive(true);
        SFXManager.Instance.PlaySFX(6);
        NotEnoughBoxText.text = "티켓이 부족합니다.";
        StartCoroutine(HideNotEnoughBox());

        IEnumerator HideNotEnoughBox()
        {
            yield return new WaitForSeconds(1f); // 1초 대기
            UIController.Instance.NotEnoughBox.SetActive(false);       // 경고창 비활성화
        }
    }
    //public void SpecTicketNotEnoungh() // 티켓 부족 알림
    //{
    //    UIController.Instance.NotEnoughBox.SetActive(true);
    //    SFXManager.Instance.PlaySFX(6);
    //    NotEnoughBoxText.text = "특수모집티켓이 부족합니다.";
    //    StartCoroutine(HideNotEnoughBox());

    //    IEnumerator HideNotEnoughBox()
    //    {
    //        yield return new WaitForSeconds(1f); // 3초 대기
    //        UIController.Instance.NotEnoughBox.SetActive(false);       // 경고창 비활성화
    //    }
    //}
}