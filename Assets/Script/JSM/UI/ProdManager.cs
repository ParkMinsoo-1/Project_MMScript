using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public interface IProdStep
{
    void Prepare();
    void Play(System.Action onComplete);
    void Skip();
    void ResetState();
}

public class ProdManager : MonoBehaviour
{
    public static ProdManager Instance { get; private set; }

    [Header("실행할 Prods (IProdStep) 순서대로 할당")]
    public List<GameObject> steps = new();

    [Header("입력")]
    public bool clickAnywhereToSkip = true;

    [Header("타이밍")]
    [Tooltip("각 prod가 끝난 뒤 다음 prod로 넘어가기 전에 대기할 시간")]
    public float nextStepDelay = 0.5f;
    [Tooltip("마지막 prod가 끝난 뒤 초기화/비활성화 전에 대기할 시간 (0이면 즉시 종료)")]
    public float endDelay = 0f;

    [Header("특정 인덱스에서 스킵 시 즉시 종료(-1이면 비활성)")]
    public int endOnSkipIndex = -1; // 예: Prod3이 네 번째면 3

    public bool isTen;
    public bool isGood;

    private int _index = -1;
    private bool _running = false;
    private IProdStep _current;
    private Tween _delayTween;
    private bool _skipNoDelayNext = false;

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var go in steps)
            if (go) go.SetActive(false);
    }

    void Update()
    {
        if (!_running) return;
        if (clickAnywhereToSkip && Input.GetMouseButtonDown(0))
            SkipCurrent();
    }

    public void PlayFromStart(bool isten, bool isGoodStuff)
    {
        isTen = isten;
        isGood = isGoodStuff;
        CancelDelay();
        ResetAll();
        _running = true;
        NextStep();
    }

    public void SkipCurrent()
    {
        if (!_running) return;

        if (_delayTween != null && _delayTween.IsActive())
        {
            CancelDelay();
            if (_index >= steps.Count - 1)
                EndAndReset();
            else
                NextStep();
            return;
        }

        if (_current == null) return;

        if (endOnSkipIndex >= 0 && _index == endOnSkipIndex)
        {
            _current.Skip();
            EndAndReset();
            return;
        }

        _skipNoDelayNext = true;
        _current.Skip();
    }

    private void NextStep()
    {
        if (_index >= 0 && _index < steps.Count && steps[_index])
            steps[_index].SetActive(false);

        _index++;

        if (_index >= steps.Count)
        {
            if (endDelay > 0f)
                _delayTween = DOVirtual.DelayedCall(endDelay, EndAndReset);
            else
                EndAndReset();
            return;
        }

        var go = steps[_index];
        if (!go) { NextStep(); return; }

        var step = go.GetComponent<IProdStep>();
        if (step == null)
        {
            Debug.LogWarning($"[{name}] {go.name} 에 IProdStep이 없습니다. 스킵합니다.");
            NextStep();
            return;
        }

        _current = step;
        go.SetActive(true);
        step.Prepare();
        step.Play(OnStepComplete);
    }

    private void OnStepComplete()
    {
        if (_index >= steps.Count - 1)
        {
            if (_skipNoDelayNext)
            {
                _skipNoDelayNext = false;
                EndAndReset();
            }
            else
            {
                if (endDelay > 0f)
                    _delayTween = DOVirtual.DelayedCall(endDelay, EndAndReset);
                else
                    EndAndReset();
            }
        }
        else
        {
            CancelDelay();
            if (_skipNoDelayNext)
            {
                _skipNoDelayNext = false;
                NextStep();
            }
            else
            {
                _delayTween = DOVirtual.DelayedCall(Mathf.Max(0f, nextStepDelay), NextStep);
            }
        }
    }

    private void EndAndReset()
    {
        _running = false;
        _current = null;
        _skipNoDelayNext = false;
        CancelDelay();
        ResetAll();
    }

    public void ResetAll()
    {
        CancelDelay();
        foreach (var go in steps)
        {
            if (!go) continue;
            var step = go.GetComponent<IProdStep>();
            step?.ResetState();
            go.SetActive(false);
        }
        _index = -1;
    }

    private void CancelDelay()
    {
        if (_delayTween != null && _delayTween.IsActive())
        {
            _delayTween.Kill();
            _delayTween = null;
        }
    }
}
