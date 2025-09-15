using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SceneIntroEffect : MonoBehaviour
{
    public RectTransform blackPanel; // Inspector 연결
    public float moveDuration = 1f;

    private void Start()
    {
        // 1. 검은 패널 중앙 정렬 (화면 덮기)
        blackPanel.anchoredPosition = Vector2.zero;

        // 2. 화면 왼쪽 밖으로 이동 애니메이션
        float endX = -Screen.width - blackPanel.rect.width;
        blackPanel.DOAnchorPosX(endX, moveDuration)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true);
    }
}
