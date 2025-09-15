using System;
using UnityEngine;
using DG.Tweening;

public class Prod0 : MonoBehaviour, IProdStep
{
    [Header("올라올 UI")]
    public RectTransform mover;     // 올라올 UI (Screen Space - Overlay)
    public float targetY = 0f;      // 목표 Y(anchoredPosition.y)
    public float moveDuration = 1f;
    public float preDelay = 0.6f;   // 올라오기 전 유예 (줌엔 적용 안 함)
    public bool useLossyScale = true; // 시작 Y 계산에 부모 스케일까지 고려

    [Header("줌 느낌(다른 오브젝트)")]
    public RectTransform zoomTarget;
    public float zoomScale = 1.15f;
    public float zoomDuration = 0.9f;
    public bool zoomReturn = true;    // 줌 후 원래 크기로 복귀

    [Header("단독 테스트용")]
    public bool autoPlay = false;       // 매니저 쓰면 false 권장

    // 내부 상태
    private Sequence seq;
    private Vector3 zoomOriginalScale;
    private Action onDone;

    // ===== IProdStep =====
    public void Prepare()
    {
        KillAll();

        if (!mover || !zoomTarget) return;

        // 시작 Y: 실제 보이는 높이(스케일 반영)만큼 아래
        float scaleY = useLossyScale ? mover.lossyScale.y : mover.localScale.y;
        float startY = -mover.sizeDelta.y * scaleY;

        var pos = mover.anchoredPosition;
        pos.y = startY;
        mover.anchoredPosition = pos;

        zoomOriginalScale = zoomTarget.localScale;
    }

    public void Play(Action onComplete)
    {
        onDone = onComplete;
        if (!mover || !zoomTarget)
        {
            onDone?.Invoke();
            return;
        }

        seq?.Kill();
        mover.DOKill();
        zoomTarget.DOKill();

        seq = DOTween.Sequence();

        // 줌: 즉시 시작(t=0)
        seq.Insert(0f,
            zoomTarget
                .DOScale(zoomOriginalScale * zoomScale, zoomDuration)
                .SetEase(Ease.InOutQuad)
        );

        // 줌 되돌리기
        if (zoomReturn)
        {
            float backDur = Mathf.Max(0.3f, zoomDuration * 0.6f);
            seq.Insert(zoomDuration,
                zoomTarget
                    .DOScale(zoomOriginalScale, backDur)
                    .SetEase(Ease.OutQuad)
            );
        }

        // mover: preDelay 후 y=targetY로
        seq.Insert(preDelay,
            mover
                .DOAnchorPosY(targetY, moveDuration)
                .SetEase(Ease.OutQuad)
        );

        seq.OnComplete(() => onDone?.Invoke());
    }

    public void Skip()
    {
        // 즉시 최종 상태로
        KillTweensOnly();

        if (mover)
        {
            var pos = mover.anchoredPosition;
            pos.y = targetY;
            mover.anchoredPosition = pos;
        }

        if (zoomTarget)
        {
            // zoomReturn에 따라 최종 스케일 결정
            zoomTarget.localScale = zoomReturn
                ? zoomOriginalScale
                : zoomOriginalScale * zoomScale;
        }

        onDone?.Invoke();
    }

    public void ResetState()
    {
        KillAll();

        if (!mover || !zoomTarget) return;

        // 다시 시작 전 상태로(아래 대기 + 줌 원래 크기)
        float scaleY = useLossyScale ? mover.lossyScale.y : mover.localScale.y;
        float startY = -mover.sizeDelta.y * scaleY;

        var pos = mover.anchoredPosition;
        pos.y = startY;
        mover.anchoredPosition = pos;

        zoomTarget.localScale = zoomOriginalScale == Vector3.zero
            ? zoomTarget.localScale
            : zoomOriginalScale;
    }
    // ===== /IProdStep =====

    // 단독 테스트용: 매니저 없이도 확인 가능
    //void OnEnable()
    //{
    //    if (autoPlay)
    //    {
    //        Prepare();
    //        Play(null);
    //    }
    //}

    void OnDisable() => KillAll();

    private void KillTweensOnly()
    {
        seq?.Kill();
        mover?.DOKill();
        zoomTarget?.DOKill();
    }

    private void KillAll() => KillTweensOnly();
}
