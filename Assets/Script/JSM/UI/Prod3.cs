using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Prod3 : MonoBehaviour, IProdStep
{
    [Header("등장 대상 (isTen=true)")]
    public List<RectTransform> targetsTen = new();

    [Header("등장 대상 (isTen=false)")]
    public List<RectTransform> targetsDefault = new();

    [Header("애니메이션 설정")]
    [Range(0f, 1f)] public float startScale = 0.7f;
    [Range(0f, 1f)] public float startAlpha = 0.5f;
    public float duration = 0.5f;
    public float stagger = 0.05f;
    public Ease ease = Ease.OutBack;

    public bool autoPlay = false;

    private Sequence seq;
    private readonly Dictionary<RectTransform, Vector3> originals = new();
    private System.Action onDone;

    // 내부에서 실제 사용할 리스트
    private List<RectTransform> activeTargets;

    public void Prepare()
    {
        KillAll();

        // isTen 값에 따라 사용할 리스트 결정
        activeTargets = ProdManager.Instance != null && ProdManager.Instance.isTen
            ? targetsTen
            : targetsDefault;

        // 사용하지 않는 리스트는 비활성화
        var inactiveTargets = activeTargets == targetsTen ? targetsDefault : targetsTen;
        foreach (var t in inactiveTargets)
            if (t) t.gameObject.SetActive(false);

        originals.Clear();
        foreach (var t in activeTargets)
        {
            if (!t) continue;

            originals[t] = t.localScale;

            t.localScale = originals[t] * Mathf.Max(0.0001f, startScale);

            var cg = t.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = t.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = startAlpha;

            if (!t.gameObject.activeSelf)
                t.gameObject.SetActive(true);
        }
    }

    public void Play(System.Action onComplete)
    {
        onDone = onComplete;
        KillTweenOnly();

        seq = DOTween.Sequence();

        for (int i = 0; i < activeTargets.Count; i++)
        {
            var t = activeTargets[i];
            if (!t) continue;

            float at = i * Mathf.Max(0f, stagger);
            var cg = t.GetComponent<CanvasGroup>();

            seq.Insert(at, t.DOScale(originals[t], duration).SetEase(ease));
            seq.Insert(at, cg.DOFade(1f, duration));
        }

        seq.AppendInterval(1f);

        seq.OnComplete(() => onDone?.Invoke());
    }

    public void Skip()
    {
        KillAll();
        foreach (var t in activeTargets)
        {
            if (!t) continue;
            t.localScale = originals.TryGetValue(t, out var s) ? s : Vector3.one;
            var cg = t.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 1f;
        }
        onDone?.Invoke();
    }

    public void ResetState()
    {
        KillAll();
        foreach (var t in activeTargets ?? new List<RectTransform>())
        {
            if (!t) continue;
            t.localScale = Vector3.one;
            var cg = t.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;
        }
    }

    void OnDisable() => KillAll();

    private void KillTweenOnly()
    {
        seq?.Kill();
        foreach (var t in activeTargets ?? new List<RectTransform>())
        {
            if (!t) continue;
            t.DOKill();
            var cg = t.GetComponent<CanvasGroup>();
            cg?.DOKill();
        }
    }

    private void KillAll()
    {
        KillTweenOnly();
    }
}
