using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GospelConfirmUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text descText;
    public Button confirmBtn;
    public Button cancleBtn;
    public Button showCancleBtn;

    public void Start()
    {
        BackHandlerEntry entry;
        entry = new BackHandlerEntry(
           priority: 30,
           isActive: () => gameObject.activeInHierarchy,
           onBack: () => gameObject.SetActive(false)
       );
        BackHandlerManager.Instance.Register(entry);
    }
    public void OnOpen(GospelData data, bool show=false)
    {
        nameText.text = data.name;
        costText.text = $"{NumberFormatter.FormatNumber(PlayerDataManager.Instance.player.tribute)}/{NumberFormatter.FormatNumber(data.cost)}";
        costText.color = PlayerDataManager.Instance.player.tribute < data.cost ? Color.red : Color.black;
        descText.text = data.description;
        confirmBtn.gameObject.SetActive(!show);
        cancleBtn.gameObject.SetActive(!show);
        showCancleBtn.gameObject.SetActive(show);
    }
}
