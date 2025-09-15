using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Prod2 : MonoBehaviour, IProdStep
{
    [Header("그룹 이동 (Prod1과 동일 동작)")]
    public List<RectTransform> fromRight = new();
    public List<RectTransform> fromLeft = new();
    public float groupOffsetX = 80f;
    public float groupDuration = 0.5f;
    public Ease groupEase = Ease.OutQuad;

    [Header("솔로 이동 (그룹 끝난 뒤 실행)")]
    public RectTransform soloTarget;      // 하나만
    public float soloDelay = 0.4f;        // 그룹 완료 후 대기
    public float soloDuration = 0.5f;
    public Ease soloEase = Ease.OutQuad;
    public bool useLossyScale = true;    // true: 부모 스케일까지 고려

    [Header("마지막 블랙아웃(페이드 인)")]
    public CanvasGroup blackout;          // 전체 화면을 덮는 오버레이(검은 패널 + CanvasGroup)
    public float blackoutDelay = 0f;      // 솔로 이동 끝난 뒤 추가 대기
    public float blackoutDuration = 0.6f; // 페이드인 시간

    [Header("실행")]
    public bool autoPlay = false;         // 매니저 쓸 땐 false 권장

    Sequence seq;
    readonly Dictionary<RectTransform, Vector2> rightOriginals = new();
    readonly Dictionary<RectTransform, Vector2> leftOriginals = new();
    Vector2 soloOriginal;
    float soloOffsetX;
    System.Action onDone;

    void OnDisable() => KillAll();

    // ===== IProdStep =====
    public void Prepare()
    {
        KillAll();
        rightOriginals.Clear();
        leftOriginals.Clear();

        // 그룹: 원위치 저장 + 시작 오프셋(X만 변경)
        foreach (var rt in fromRight)
        {
            if (!rt) continue;
            rightOriginals[rt] = rt.anchoredPosition;
            var p = rt.anchoredPosition; p.x = rightOriginals[rt].x + groupOffsetX; rt.anchoredPosition = p;
        }
        foreach (var rt in fromLeft)
        {
            if (!rt) continue;
            leftOriginals[rt] = rt.anchoredPosition;
            var p = rt.anchoredPosition; p.x = leftOriginals[rt].x - groupOffsetX; rt.anchoredPosition = p;
        }

        // 솔로: 원위치 저장 + “보이는 너비(스케일 반영)”만큼 왼쪽 대기(X만 변경)
        soloOriginal = Vector2.zero;
        soloOffsetX = 0f;
        if (soloTarget)
        {
            soloOriginal = soloTarget.anchoredPosition;
            float w = soloTarget.rect.width;
            float sx = useLossyScale ? soloTarget.lossyScale.x : soloTarget.localScale.x;
            soloOffsetX = w * sx;

            var p = soloTarget.anchoredPosition; p.x = soloOriginal.x - soloOffsetX; soloTarget.anchoredPosition = p;
        }

        // 블랙아웃 초기화(보이지 않게)
        if (blackout)
        {
            if (!blackout.gameObject.activeSelf) blackout.gameObject.SetActive(true);
            blackout.alpha = 0f;
            blackout.interactable = false;
            blackout.blocksRaycasts = false;
        }
    }

    public void Play(System.Action onComplete)
    {
        onDone = onComplete;
        seq?.Kill();
        seq = DOTween.Sequence();

        // 1) 그룹 동시 복귀 (X만 트윈)
        foreach (var rt in fromRight)
            if (rt) seq.Join(rt.DOAnchorPosX(rightOriginals[rt].x, groupDuration).SetEase(groupEase));
        foreach (var rt in fromLeft)
            if (rt) seq.Join(rt.DOAnchorPosX(leftOriginals[rt].x, groupDuration).SetEase(groupEase));

        // 2) 그룹 끝 → 딜레이 → 솔로가 왼쪽에서 제자리로 (X만 트윈)
        if (soloTarget)
        {
            seq.AppendInterval(soloDelay);
            seq.Append(soloTarget.DOAnchorPosX(soloOriginal.x, soloDuration).SetEase(soloEase));
        }

        // 3) 마지막 블랙아웃 페이드 인
        if (blackout)
        {
            if (blackoutDelay > 0f) seq.AppendInterval(blackoutDelay);
            seq.Append(blackout.DOFade(1f, blackoutDuration).SetEase(Ease.Linear));
            // 필요하면 입력 막기
            seq.AppendCallback(() =>
            {
                blackout.interactable = true;
                blackout.blocksRaycasts = true;
            });
        }

        seq.OnComplete(() => onDone?.Invoke());
    }

    public void Skip()
    {
        // 즉시 최종 상태로
        KillTweensOnly();

        foreach (var rt in fromRight)
            if (rt) { var p = rt.anchoredPosition; p.x = rightOriginals[rt].x; rt.anchoredPosition = p; }
        foreach (var rt in fromLeft)
            if (rt) { var p = rt.anchoredPosition; p.x = leftOriginals[rt].x; rt.anchoredPosition = p; }
        if (soloTarget)
        {
            var p = soloTarget.anchoredPosition; p.x = soloOriginal.x; soloTarget.anchoredPosition = p;
        }

        // 블랙아웃도 최종 상태(완전 검정)로
        if (blackout)
        {
            if (!blackout.gameObject.activeSelf) blackout.gameObject.SetActive(true);
            blackout.DOKill();
            blackout.alpha = 1f;
            blackout.interactable = true;
            blackout.blocksRaycasts = true;
        }

        onDone?.Invoke();
    }

    public void ResetState()
    {
        KillAll();

        foreach (var kv in rightOriginals)
            if (kv.Key) { var p = kv.Key.anchoredPosition; p.x = kv.Value.x; kv.Key.anchoredPosition = p; }
        foreach (var kv in leftOriginals)
            if (kv.Key) { var p = kv.Key.anchoredPosition; p.x = kv.Value.x; kv.Key.anchoredPosition = p; }
        if (soloTarget)
        {
            var p = soloTarget.anchoredPosition; p.x = soloOriginal.x; soloTarget.anchoredPosition = p;
        }

        // 블랙아웃 초기화(보이지 않게)
        if (blackout)
        {
            blackout.DOKill();
            if (!blackout.gameObject.activeSelf) blackout.gameObject.SetActive(true);
            blackout.alpha = 0f;
            blackout.interactable = false;
            blackout.blocksRaycasts = false;
        }
    }
    // ===== /IProdStep =====

    void KillTweensOnly()
    {
        seq?.Kill();
        foreach (var rt in fromRight) if (rt) rt.DOKill();
        foreach (var rt in fromLeft) if (rt) rt.DOKill();
        if (blackout) blackout.DOKill();
        soloTarget?.DOKill();
    }

    void KillAll() => KillTweensOnly();
}
