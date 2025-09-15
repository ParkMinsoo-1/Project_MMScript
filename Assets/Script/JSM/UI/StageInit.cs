using UnityEngine;
using UnityEngine.UI;

public class StageInit : MonoBehaviour
{
    public Button goldBtn;
    public Button mainBtn;
    public Button towerBtn;

    public GameObject goldStage;
    public GameObject mainStage;
    public GameObject infoPanel;
    public GameObject goldinfoPanel;
    public GameObject towerinfoPanel;
    public GameObject towerPanel;

    private BackHandlerEntry entry;

    public static StageInit instance;
    private void Awake()
    {
        instance = this;
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => infoPanel.activeInHierarchy,
           onBack: () => infoPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => goldinfoPanel.activeInHierarchy,
           onBack: () => goldinfoPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
        entry = new BackHandlerEntry(
           priority: 20,
           isActive: () => towerinfoPanel.activeInHierarchy,
           onBack: () => towerinfoPanel.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }

    public void OnDisable()
    {
        OnMainBtn();
    }
    public void OnMainBtn()
    {
        goldStage.SetActive(false);
        mainStage.SetActive(true);
        infoPanel.SetActive(false);
        towerPanel.SetActive(false);
        goldinfoPanel.SetActive(false);
        towerinfoPanel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
    public void OnGoldBtn()
    {
        goldStage.SetActive(true);
        mainStage.SetActive(false);
        infoPanel.SetActive(false);
        towerPanel.SetActive(false);
        goldinfoPanel.SetActive(false);
        towerinfoPanel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }
    public void OnTowerBtn()
    {
        goldStage.SetActive(false);
        mainStage.SetActive(false);
        infoPanel.SetActive(false);
        towerPanel.SetActive(true);
        UITowerManager.instance.Init();
        goldinfoPanel.SetActive(false);
        towerinfoPanel.SetActive(false);
        SFXManager.Instance.PlaySFX(0);
    }

}
