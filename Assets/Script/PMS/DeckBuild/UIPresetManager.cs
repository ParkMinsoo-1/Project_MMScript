using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPresetManager : MonoBehaviour
{
    [SerializeField] private List<Button> presetBtn; 
    [SerializeField] private List<Image> presetBtnImg;
    [SerializeField] private List<PresetMover> presetMover;
    [SerializeField] private List<Image> presetNum;

    public static UIPresetManager instance;
    private int prevIndex = -1;
    private bool isSwitching = false;
    private void Awake()
    {
        instance = this;
    }

    public void OnClickPresetBtn(int index)
    {
        if (isSwitching || index == PlayerDataManager.Instance.player.currentPresetIndex) return;
        isSwitching = true;

        bool success = DeckManager.Instance.SwitchPreset(index);

        if (success)
        {
            UIDeckBuildManager.instance.SetDeckSlots();
            UIDeckBuildManager.instance.SetMyUnitIcons();

            if(prevIndex != -1 && prevIndex != index)
            {
                presetMover[prevIndex].MoveBackDelay();
            }

            presetMover[index].MoveRight();
            StartCoroutine(SwitchAfterDelay(index, 0.5f));
        }

        else
        {
            isSwitching = false;
        }
    }

    private IEnumerator SwitchAfterDelay(int newIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        prevIndex = newIndex;
        UpdatePresetBtn();
        isSwitching = false;
    }

    public void UpdatePresetBtn()
    {
        int currentIndex = PlayerDataManager.Instance.player.currentPresetIndex;

        for (int i = 0; i < presetBtnImg.Count; i++)
        {
            float alpha = (i == currentIndex) ? 1f : 0.5f;
            SetButtonAlpha(presetBtnImg[i], alpha);
            SetButtonAlpha(presetNum[i], alpha);
        }
    }

    private void SetButtonAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
