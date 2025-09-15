using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Prod1 : MonoBehaviour, IProdStep
{
    [Header("대상 그룹")]
    public List<RectTransform> fromRight = new(); // 오른쪽에서 들어올 UI들
    public List<RectTransform> fromLeft = new(); // 왼쪽에서 들어올 UI들

    [Header("이동 설정")]
    public float offsetX = 80f;          // 출발 오프셋 (px)
    public float moveDuration = 0.5f;    // 복귀 시간
    public Ease moveEase = Ease.OutQuad;

    [Header("끝나고 깜빡일 대상")]
    public GameObject blinkTarget;       // 처음엔 비활성화돼 있어야 함
    public int blinkCount = 3;      // 깜빡임 횟수
    public float blinkInterval = 0.15f;  // 반주기 시간(한 번 페이드 시간)

    [Header("단독 테스트용")]
    public bool autoPlay = false;        // 매니저 쓸 땐 false 권장

    // 내부 상태
    private Sequence seq;
    private readonly Dictionary<RectTransform, Vector2> rightOriginals = new();
    private readonly Dictionary<RectTransform, Vector2> leftOriginals = new();
    private Action onDone;

    // ===== IProdStep =====
    public void Prepare()
    {
        KillAll();

        rightOriginals.Clear();
        leftOriginals.Clear();

        // 원위치 저장 + 시작 위치(좌/우 오프셋)로 이동
        foreach (var rt in fromRight)
        {
            if (!rt) continue;
            rightOriginals[rt] = rt.anchoredPosition;
            rt.anchoredPosition = rightOriginals[rt] + new Vector2(+offsetX, 0f);
        }
        foreach (var rt in fromLeft)
        {
            if (!rt) continue;
            leftOriginals[rt] = rt.anchoredPosition;
            rt.anchoredPosition = leftOriginals[rt] + new Vector2(-offsetX, 0f);
        }

        if (blinkTarget) blinkTarget.SetActive(false);
    }

    public void Play(Action onComplete)
    {
        onDone = onComplete;
        seq?.Kill();

        seq = DOTween.Sequence();

        // 그룹 동시 복귀
        foreach (var rt in fromRight)
            if (rt) seq.Join(rt.DOAnchorPos(rightOriginals[rt], moveDuration).SetEase(moveEase));
        foreach (var rt in fromLeft)
            if (rt) seq.Join(rt.DOAnchorPos(leftOriginals[rt], moveDuration).SetEase(moveEase));

        // 모두 복귀하면 깜빡임 → 완료 콜백
        seq.OnComplete(() =>
        {
            if (blinkTarget == null)
            {
                onDone?.Invoke();
                return;
            }

            // 깜빡이고 꺼진 뒤 onDone
            BlinkAndHide(blinkTarget, blinkCount, blinkInterval, onDone);
        });
    }

    public void Skip()
    {
        // 트윈 즉시 종료, 최종 상태로 세팅
        KillTweensOnly();

        foreach (var rt in fromRight)
            if (rt) rt.anchoredPosition = rightOriginals.TryGetValue(rt, out var p) ? p : rt.anchoredPosition;
        foreach (var rt in fromLeft)
            if (rt) rt.anchoredPosition = leftOriginals.TryGetValue(rt, out var p) ? p : rt.anchoredPosition;

        // 스킵은 깜빡임 없이 바로 다음으로
        if (blinkTarget) { var cg = blinkTarget.GetComponent<CanvasGroup>(); cg?.DOKill(); blinkTarget.SetActive(false); }

        onDone?.Invoke();
    }

    public void ResetState()
    {
        KillAll();

        // 원위치로 복구(안전)
        foreach (var kv in rightOriginals)
            if (kv.Key) kv.Key.anchoredPosition = kv.Value;
        foreach (var kv in leftOriginals)
            if (kv.Key) kv.Key.anchoredPosition = kv.Value;

        if (blinkTarget) blinkTarget.SetActive(false);
    }
    // ===== /IProdStep =====

    // 단독 테스트용
    //void OnEnable()
    //{
    //    if (autoPlay)
    //    {
    //        Prepare();
    //        Play(null);
    //    }
    //}

    void OnDisable() => KillAll();

    // --- helpers ---
    private void BlinkAndHide(GameObject target, int count, float halfPeriod, Action onComplete)
    {
        target.SetActive(true);
        var cg = target.GetComponent<CanvasGroup>();
        if (!cg) cg = target.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // Yoyo로 (1->0) 깜빡임 반복 후 비활성화, 알파 복구
        cg.DOFade(0f, halfPeriod)
          .From(1f)
          .SetLoops(count * 2, LoopType.Yoyo)
          .OnComplete(() =>
          {
              cg.alpha = 1f;
              target.SetActive(false);
              onComplete?.Invoke();
          });
    }

    private void KillTweensOnly()
    {
        seq?.Kill();
        foreach (var rt in fromRight) if (rt) rt.DOKill();
        foreach (var rt in fromLeft) if (rt) rt.DOKill();
        if (blinkTarget)
        {
            var cg = blinkTarget.GetComponent<CanvasGroup>();
            cg?.DOKill();
        }
    }

    private void KillAll() => KillTweensOnly();
}
