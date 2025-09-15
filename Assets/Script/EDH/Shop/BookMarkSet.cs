using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookMarkSet : MonoBehaviour
{
    public Button GoldStoreBtn;             //   골드 상점 버튼
    public Button CertificateStoreBtn;      // 증명서 상점 버튼

    public GameObject GoldStore;
    public GameObject CertificateStore;

    public CertiPurchaseBoxSet certiPurchaseBoxSet;

    public void Start()
    {
        GoldStoreBtn.onClick.AddListener(GoldStoreSet);
        CertificateStoreBtn.onClick.AddListener(CertificateStoreSet);
    }
    public void GoldStoreSet()
    {
        GoldStore.SetActive(true);
        CertificateStore.SetActive(false);
        SFXManager.Instance.PlaySFX(0);

        if (certiPurchaseBoxSet != null)
        {
            certiPurchaseBoxSet.TabClose();
        }
    }

    public void CertificateStoreSet()
    {
        CertificateStore.SetActive(true);
        GoldStore.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
}
