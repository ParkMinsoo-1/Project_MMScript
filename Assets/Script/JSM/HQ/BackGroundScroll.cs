using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class InfiniteParallaxLayer
{
    public RectTransform container;     // 레이어 컨테이너 (이 안에 타일이 있음)
    public float parallaxFactor = 1f;
    public float tileWidth = 1920f;     // 한 타일의 실제 너비
}

public class BackGroundScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewport;
    public float contentWidthOffset = 0f;

    public List<InfiniteParallaxLayer> layers = new();

    private float contentWidth;
    private float viewportWidth;

    void Start()
    {
        contentWidth = scrollRect.content.rect.width + contentWidthOffset;
        viewportWidth = viewport.rect.width;
        
    }

    void Update()
    {
        float normalizedX = scrollRect.horizontalNormalizedPosition;

        foreach (var layer in layers)
        {
            if (layer.container == null) continue;

            // 전체 이동 거리 = (총 스크롤 가능한 거리) * 패럴랙스
            float totalScroll = contentWidth - viewportWidth;
            float moveX = normalizedX * totalScroll * layer.parallaxFactor;

            // 무한 반복 효과: offset 보정
            float loopX = moveX % layer.tileWidth;
            if (loopX > 0) loopX -= layer.tileWidth;

            layer.container.anchoredPosition = new Vector2(loopX, layer.container.anchoredPosition.y);
        }
    }
}
