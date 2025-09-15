using UnityEngine;

[DisallowMultipleComponent]
public class SelfDestroy : MonoBehaviour
{
    [SerializeField, Min(0f)] private float delay = 1f; // Time.timeScale에 영향 없이 대기
    private Coroutine co;

    void OnEnable()
    {
        if (co != null) StopCoroutine(co);

        if (delay <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        co = StartCoroutine(DisableAfterDelayUnscaled());
    }

    void OnDisable()
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }
    }

    private System.Collections.IEnumerator DisableAfterDelayUnscaled()
    {
        yield return new WaitForSecondsRealtime(delay); // ← unscaled 시간
        if (gameObject.activeSelf) gameObject.SetActive(false);
        co = null;
    }
}
