using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatManager : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] private TMP_InputField goldInput;
    [SerializeField] private TMP_InputField tributeInput;
    [SerializeField] private TMP_InputField ticketInput;
    [SerializeField] private TMP_InputField apInput;
    [SerializeField] private TMP_InputField bluePrintInput;
    [SerializeField] private TMP_InputField certiInput;

    [Header("Buttons")]
    [SerializeField] private Button goldBtn;
    [SerializeField] private Button tributeBtn;
    [SerializeField] private Button ticketBtn;
    [SerializeField] private Button apBtn;
    [SerializeField] private Button bluePrintBtn;
    [SerializeField] private Button certiBtn;
    [SerializeField] private Button closeBtn;

    [Header("Panel")]
    [SerializeField] private GameObject panel;

    private void Start()
    {
        goldBtn.onClick.AddListener(() => ApplyCheat(goldInput, "gold"));
        tributeBtn.onClick.AddListener(() => ApplyCheat(tributeInput, "tribute"));
        ticketBtn.onClick.AddListener(() => ApplyCheat(ticketInput, "ticket"));
        apBtn.onClick.AddListener(() => ApplyCheat(apInput, "actionPoint"));
        bluePrintBtn.onClick.AddListener(() => ApplyCheat(bluePrintInput, "bluePrint"));
        certiBtn.onClick.AddListener(() => ApplyCheat(certiInput, "certi"));

        closeBtn.onClick.AddListener(() => panel.SetActive(false));
    }

    void ApplyCheat(TMP_InputField input, string type) 
    {
        if(!int.TryParse(input.text, out int value))
        {
            Debug.Log("숫자를 입력해 주세요.");
            return;
        }

        switch (type)
        {
            case "gold":
                PlayerDataManager.Instance.AddGold(value);
                break;
            case "tribute":
                PlayerDataManager.Instance.AddTribute(value);
                break;
            case "ticket":
                PlayerDataManager.Instance.AddTicket(value);
                break;
            case "actionPoint":
                PlayerDataManager.Instance.AddActionPoint(value);
                break;
            case "bluePrint":
                PlayerDataManager.Instance.AddBluePrint(value);
                break;
            case "certi":
                PlayerDataManager.Instance.AddCerti(value);
                break;
        }
    }
}
