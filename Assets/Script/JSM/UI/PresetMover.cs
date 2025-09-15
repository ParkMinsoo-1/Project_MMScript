using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PresetMover : MonoBehaviour
{
    private Vector3 originalPos;
    private Tween moveTween;

    public float moveDuration = 0.5f;

    private Coroutine coroutine;

    private void Awake()
    {
        originalPos = transform.localPosition;
        
    }

    public void MoveRight()//오른쪽으로 움직이는 함수(좌우반전 고려)
    {
        moveTween?.Kill();

        Vector3 currentPos = transform.localPosition;
        Vector3 targetPos = currentPos + -Vector3.right * 150f;
        moveTween = transform.DOLocalMove(targetPos, moveDuration).SetEase(Ease.OutQuad);
        MoveBackDelay();

    }

    public IEnumerator MoveBack()//원래 좌표로 돌아가는 함수
    {
        yield return new WaitForSeconds(0.5f);
        moveTween?.Kill();
        moveTween = transform.DOLocalMove(originalPos, moveDuration).SetEase(Ease.InQuad);
    }

    public void MoveBackDelay()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(MoveBack());
    }
}
