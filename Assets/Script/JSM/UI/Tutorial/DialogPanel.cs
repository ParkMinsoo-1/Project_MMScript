using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogPanel : MonoBehaviour
{
    public GameObject dialog;
    public GameObject npc;
    public Button nextBtn;
    public Button skipBtn;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogText;

    private Vector3 originalNpcPos;
    private Tween npcTween;

    private void Awake()
    {
        originalNpcPos = npc.transform.localPosition;
        skipBtn.onClick.AddListener(TutorialManager.Instance.EndTutorial);
        //dialog.SetActive(false); // 초기에는 숨김
    }

    public IEnumerator PlayNpcIntroAnimationWithYield()
    {
        npc.transform.localPosition = originalNpcPos + Vector3.down * 300f;

        Tween t = npc.transform.DOLocalMove(originalNpcPos, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        yield return t.WaitForCompletion(); // DOTween 확장기능 사용
        //dialog.SetActive(true);
    }


    public void ShowDialogInstant()
    {
        npc.transform.localPosition = originalNpcPos;
        dialog.SetActive(true);
    }
    public bool IsNpcAnimating()
    {
        return npcTween != null && npcTween.IsActive() && npcTween.IsPlaying();
    }

    public void ForceFinishAnimation()
    {
        if (npcTween != null && npcTween.IsActive() && npcTween.IsPlaying())
            npcTween.Complete();
    }

}
