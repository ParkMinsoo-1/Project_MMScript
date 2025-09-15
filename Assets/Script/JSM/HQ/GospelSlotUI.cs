using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GospelState
{
    Available,   // 선택 가능
    Locked,      // 선택 불가
    Selected,     // 이미 선택됨
    ToShow          //선택 가능, 건설 불가
}
public class GospelSlotUI : MonoBehaviour
{
    public GameObject SelectedImg;
    public CanvasGroup canvasGroup;

    public TMP_Text nameText;
    public TMP_Text costText;

    private GospelData gospelData;
    private GospelState state;

    public GospelSpawner gospelSpawner;
    public GospelContainerUI containerUI;
    public GospelConfirmUI confirmUI;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void SetData(GospelData data, GospelState newState)
    {
        gospelData = data;
        nameText.text = data.name;
        costText.text = NumberFormatter.FormatNumber(data.cost);
        state = newState;
        UpdateUIByState();
    }
    public void OnClick()
    {
        if (confirmUI.gameObject != null && gospelData != null)
        {
            confirmUI.gameObject.SetActive(true);
            confirmUI.OnOpen(gospelData, state != GospelState.Available);

            SFXManager.Instance.PlaySFX(15);

            confirmUI.confirmBtn.onClick.AddListener(ConfirmBuild);
            confirmUI.cancleBtn.onClick.AddListener(CancleBuild);
        }
    }
    private void UpdateUIByState()
    {
        switch (state)
        {
            case GospelState.Locked:
                SelectedImg.SetActive(false);
                canvasGroup.alpha = 0.3f;
                break;
            case GospelState.Available:
                SelectedImg.SetActive(false);
                canvasGroup.alpha = 1f;
                break;
            case GospelState.Selected:
                SelectedImg.SetActive(true);
                canvasGroup.alpha = 1f;
                break;
            case GospelState.ToShow:
                SelectedImg.SetActive(false);
                canvasGroup.alpha = 1f;
                break;
        }
    }
    public void ConfirmBuild()
    {
        if (state == GospelState.Locked) return;

        if (PlayerDataManager.Instance.player.tribute < gospelData.cost)
        {
            HQResourceUI.Instance.ShowLackPanel();
            CancleBuild();
            return;
        }
        PlayerDataManager.Instance.UseTribute(gospelData.cost);
        HQResourceUI.Instance.UpdateUI();

        GospelManager.Instance.SelectGospel(gospelData.buildID, gospelData.id);

        gospelSpawner.OnSlotSelected(this);

        //if (confirmUI.gameObject != null && gospelData != null)
        //{
        //    confirmUI.gameObject.SetActive(true);
        //    confirmUI.OnOpen(gospelData);
        //}
        CancleBuild();
    }
    public void CancleBuild()
    {
        confirmUI.confirmBtn.onClick.RemoveListener(ConfirmBuild);
        confirmUI.cancleBtn.onClick.RemoveListener(CancleBuild);
        confirmUI.gameObject.SetActive(false);

        SFXManager.Instance.PlaySFX(0);
    }
}
