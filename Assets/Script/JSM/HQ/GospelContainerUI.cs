using System.Collections.Generic;
using UnityEngine;

public class GospelContainerUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public int Layer;

    public List<GospelSlotUI> slotUIs = new();

    public void SetInteractable(bool interactable)
    {
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    public void AddSlot(GospelSlotUI slot)
    {
        if (!slotUIs.Contains(slot))
        {
            slotUIs.Add(slot);
            slot.containerUI = this;
        }
    }
}
