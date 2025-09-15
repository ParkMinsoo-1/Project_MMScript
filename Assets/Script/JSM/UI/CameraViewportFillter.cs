using UnityEngine;

public class CameraViewportFitter : MonoBehaviour
{
    public RectTransform targetUIRect; // ObjectAreaPanel
    public Camera renderCamera;        // 대상 카메라
    public float unitsTall = 2f;       // 화면 세로에 보여줄 Unity 단위 높이
    private ObjectAreaController objectAreaController;
    private Vector2 lastSize;


    private void Awake()
    {
        objectAreaController = GetComponent<ObjectAreaController>();
    }
    private void OnEnable()
    {
        UpdateCameraView();
        objectAreaController.OnSpawn();
    }

    private void Update()
    {
        if (targetUIRect == null || renderCamera == null) return;

        Vector2 currentSize = GetPixelSize(targetUIRect);
        if (currentSize != lastSize)
        {
            UpdateCameraView();
        }
    }

    private void UpdateCameraView()
    {
        Vector2 size = GetPixelSize(targetUIRect);
        lastSize = size;

        float pixelWidth = size.x;
        float pixelHeight = size.y;

        float aspectRatio = pixelWidth / pixelHeight;

        renderCamera.orthographic = true;
        renderCamera.orthographicSize = unitsTall * 0.5f;
        renderCamera.aspect = aspectRatio;
    }

    private Vector2 GetPixelSize(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float width = Vector3.Distance(corners[1], corners[2]);  // 하단 → 우측
        float height = Vector3.Distance(corners[0], corners[1]); // 하단 → 상단

        Canvas canvas = rt.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            width *= canvas.scaleFactor;
            height *= canvas.scaleFactor;
        }

        return new Vector2(Mathf.Round(width), Mathf.Round(height));
    }
}
