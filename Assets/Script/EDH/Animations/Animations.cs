using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ButtonScaler_Coroutine : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float pressScale = 0.9f;
    public float returnTime = 0.005f;

    private Vector3 originalScale;
    private Image buttonImage;
    private Image image;
    private Color originalColor;
    private Color darkenedColor;

    private Coroutine resetCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
        image = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            darkenedColor = new Color(
                originalColor.r * 0.4f,
                originalColor.g * 0.4f,
                originalColor.b * 0.4f,
                originalColor.a
            );
        }
        if (image != null)
        {
            originalColor = image.color;
            darkenedColor = new Color(
                originalColor.r * 0.4f,
                originalColor.g * 0.4f,
                originalColor.b * 0.4f,
                originalColor.a
            );
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonImage != null) buttonImage.color = darkenedColor;
        transform.localScale = originalScale * pressScale;

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (image != null) image.color = darkenedColor;
        transform.localScale = originalScale * pressScale;

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        resetCoroutine = StartCoroutine(ResetButton());
    }

    private IEnumerator ResetButton()
    {
        float elapsed = 0f;
        Vector3 currentScale = transform.localScale;

        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(currentScale, originalScale, elapsed / returnTime);
            yield return null;
        }

        transform.localScale = originalScale;
        if (buttonImage != null) buttonImage.color = originalColor;
        resetCoroutine = null;
        if (image != null) image.color = originalColor;
        resetCoroutine = null;
    }

    private void OnDisable()
    {
        // 비활성화 시 상태 복구
        transform.localScale = originalScale;
        if (buttonImage != null) buttonImage.color = originalColor;
        resetCoroutine = null;
        if (image != null) image.color = originalColor;
        resetCoroutine = null;
    }

    private void OnEnable()
    {
        // 재활성화 시 상태 보정
        transform.localScale = originalScale;
        if (buttonImage != null) buttonImage.color = originalColor;
        transform.localScale = originalScale;
        if (image != null) image.color = originalColor;
    }
}

