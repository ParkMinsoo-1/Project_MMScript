using UnityEngine;
using UnityEngine.EventSystems;

public class UISkillInfoBlocker : MonoBehaviour, IPointerClickHandler
{
    public UISkillInfo skillInfo;

    public void OnPointerClick(PointerEventData eventData)
    {
        skillInfo.HideSkillInfo();
        gameObject.SetActive(false);
    }
}
