using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildSlotUI : MonoBehaviour
{
    public int slotID;
    public int buildingID;
    public Image slotImage;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Button levelUpBtn;
    public GameObject plusText;
    public GameObject buildListUI;
    public GameObject buildGospelUI;
    public GameObject levelUpPanel;

    public float maxHeight = 300f;
    public float maxWidth = 300f;
    public float minHeight = 300f;
    public float minWidth = 300f;

    public int Level = 0;

    public GameObject EffectPrefab;
    public void Select()
    {
        BuildManager.Instance.SelectSlot(this);
        if (Level > 0)
        {
            var goseplspawner = buildGospelUI.GetComponentInChildren<GospelSpawner>();
            goseplspawner.buildID = buildingID;
            goseplspawner.level = Level;
            goseplspawner.toShow = false;
            buildGospelUI.SetActive(true);
        }
        else
        {
            buildListUI.SetActive(true);
        }
        SFXManager.Instance.PlaySFX(15);
    }

    public void Build(BuildingData building, int level)
    {
        buildingID = building.id;
        Level=level;

        if (plusText != null) Destroy(plusText);

        if (slotImage != null)
        {
            Sprite newSprite = BuildManager.Instance.GetBuildingSprite(building.imageName);
            slotImage.sprite = newSprite;

            AspectRatioFitter fitter = slotImage.GetComponent<AspectRatioFitter>();
            RectTransform rt = slotImage.GetComponent<RectTransform>();

            if (newSprite != null && rt != null)
            {
                float w = newSprite.rect.width;
                float h = newSprite.rect.height;
                float aspect = w / h;

                float targetWidth = rt.rect.width;
                float targetHeight = targetWidth / aspect;

                // 최대 높이 제한
                if (targetHeight > maxHeight && maxHeight!=0)
                {
                    targetHeight = maxHeight;
                    targetWidth = targetHeight * aspect;
                }

                // 최대 너비 제한
                if (targetWidth > maxWidth && maxWidth != 0)
                {
                    targetWidth = maxWidth;
                    targetHeight = targetWidth / aspect;
                }

                // 최소 높이 제한
                if (targetHeight < minHeight && minHeight != 0)
                {
                    targetHeight = minHeight;
                    targetWidth = targetHeight * aspect;
                }

                // 최소 너비 제한
                if (targetWidth < minWidth && minWidth != 0)
                {
                    targetWidth = minWidth;
                    targetHeight = targetWidth / aspect;
                }

                if (fitter != null) fitter.aspectMode = AspectRatioFitter.AspectMode.None;

                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            }

        }

        if (nameText != null)
        {
            if (!nameText.gameObject.activeSelf)
                nameText.gameObject.SetActive(true);
            //Debug.Log(buildingID);
            nameText.text = BuildManager.Instance.GetBuildingData(buildingID).displayName;
        }

        if (levelText != null)
        {
            if (!levelText.gameObject.activeSelf)
                levelText.gameObject.SetActive(true);

            levelText.text = Level==5?"Max":$"Lv.{Level}";
        }
        if(levelUpBtn != null)
        {
            if (!levelUpBtn.gameObject.activeSelf)
                levelUpBtn.gameObject.SetActive(true);
            levelUpBtn.GetComponent<BuildingLevelUpUI>().levelUpPanel = levelUpPanel;
            levelUpBtn.GetComponent<BuildingLevelUpUI>().buildSlotUI = this;
            if (Level == 5) 
            { 
                levelUpBtn.onClick.RemoveAllListeners();

                levelUpBtn.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            }
        }
    }
    public void LevelUp()
    {
        Level++;
        if (levelText != null)
        {
            if (!levelText.gameObject.activeSelf)
                levelText.gameObject.SetActive(true);

            levelText.text = Level == 5 ? "Max" : $"Lv.{Level}";
            OnEffect();
        }
        PlayerDataManager.Instance.player.buildingsList[slotID].level = Level;
    }
    void OnEffect()
    {
        GameObject obj = Instantiate(EffectPrefab, transform.position, Quaternion.identity);
        Destroy(obj, 1f);
    }
}