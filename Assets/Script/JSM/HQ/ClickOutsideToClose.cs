using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class ClickOutsideToClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetPanel == null || !targetPanel.activeSelf) return;

        // 클릭한 위치가 패널의 RectTransform 안인지 확인
        RectTransform panelRect = targetPanel.GetComponent<RectTransform>();
        Vector2 localMousePosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect, eventData.position, eventData.pressEventCamera, out localMousePosition))
        {
            if (panelRect.rect.Contains(localMousePosition))
            {
                // 패널 안쪽을 눌렀다면 무시
                return;
            }
        }

        // 바깥쪽을 눌렀을 때만 닫기
        gameObject.SetActive(false); // Background도 함께 꺼짐
    }
}
