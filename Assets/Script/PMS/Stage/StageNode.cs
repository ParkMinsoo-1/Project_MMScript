using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageNode : MonoBehaviour
{
    [SerializeField] private Button stageBtn;
    [SerializeField] private TextMeshProUGUI stageNameTxt;
    [SerializeField] private GameObject clearText;

    public int stageID;

    public void Init(StageData data)
    {
        stageID = data.ID;
        stageNameTxt.text = data.StageName.Replace("stage","");
        
        stageBtn.onClick.RemoveAllListeners();
        stageBtn.onClick.AddListener(OnClickNode);
    }

    public void OnClickNode()
    {
        StageManager.instance.SelectStage(stageID);
        SFXManager.Instance.PlaySFX(0);
    }

    public void SetClear(bool isClear)
    {
        if(clearText != null)
        {
            clearText.gameObject.SetActive(isClear);
        }
    }

    public void SetInteractable(bool interactable)
    {
        var image = GetComponent<Image>();
        
        if (image != null)
        {
            Color color = image.color;
            color.a = interactable ? 1f : 0.7f;
            image.color = color;
        }

        Transform childImageTransform = transform.Find("Image");
        if (childImageTransform != null)
        {
            var childImage = childImageTransform.GetComponent<Image>();
            if (childImage != null)
            {
                var color = childImage.color;
                color.a = interactable ? 1f : 0.7f;
                childImage.color = color;
            }

            var text = childImageTransform.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                var color = text.color;
                color.a = interactable ? 1f : 0.7f;
                text.color = color;
            }
        }

    }

}
