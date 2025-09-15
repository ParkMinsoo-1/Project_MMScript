using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CertiPurChaseSync : Singleton<CertiPurChaseSync>
{
    public Button PurchaseButton;   // 아이템 구매 버튼

    PlayerDataManager PlayerDataManager;

    public TMP_Text NotEnoughBoxText;  // 재화 부족 경고 텍스트
    public GameObject NotEnoughBox;    // 재화 부족 경고
    public GameObject PurchaseCertiUnitBox;   // 구매UI 상자

    public CertiSlot cSlot;
    public PickInfo Info;

    public void Start()
    {
        PurchaseButton.onClick.AddListener(PurchaseCertiUnit);
    }

    public void PurchaseCertiUnit()
    {

        int Amount = PlayerDataManager.Instance.player.certi;
        int Cost = Info.warrant;

        if (Amount >= Cost)
        {
            PlayerDataManager.Instance.UseCerti(Cost);
            PlayerDataManager.Instance.AddUnit(Info.ID);
            Debug.Log(PlayerDataManager.Instance.AddUnit(Info.ID) + "이 유닛을 구매");

            if (cSlot != null)
            {
                cSlot.RefreshUI();
            }
            ShoppingManager.Instance.ShowNowCertificate();
                        PurchaseCertiUnitBox.SetActive(false);
            SFXManager.Instance.PlaySFX(0);
            SFXManager.Instance.PlaySFX(7);
        }
        else 
        {
            UIController.Instance.CertiNotEnoungh();
        }
    }
    public void Init(PickInfo pickInfo, CertiSlot certiSlot)
    {
        Debug.Log("c");
        Info = pickInfo;
        cSlot = certiSlot;
    }
}
