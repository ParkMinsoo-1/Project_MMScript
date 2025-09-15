using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UnitCardSlot : MonoBehaviour
{
    [SerializeField] private GameObject frontSide;
    [SerializeField] private GameObject backSide;
    [SerializeField] private TMP_Text UnitICardNametext;        // 유닛 이름

    [SerializeField] private Image UnitIcon;                     // 유닛 아이콘
    [SerializeField] private GameObject goodStuffBG;                     // 유닛 아이콘
    public int shopPrice;
    public GameObject duplicate;
    public TextMeshProUGUI dupTxt;
    private Coroutine blinkCoroutine;
    private bool isDuplicate;
    public void init(PickInfo Alliance)
    {
        UnitICardNametext.text = Alliance.Name;

        var stats = UnitDataManager.Instance.GetStats(Alliance.ID);
        shopPrice = Alliance.duplication;
        Debug.Log(stats.ModelName);

        UnitIcon.sprite = Resources.Load<Sprite>($"SPUMImg/{stats.ModelName}");

        // 유닛 보유 여부만 저장
        isDuplicate = !PlayerDataManager.Instance.AddUnit(Alliance.ID);
        dupTxt.text = Alliance.duplication.ToString();

        if (Alliance.IsHero)
        {
            backSide.GetComponent<Image>().color = Color.yellow;
            goodStuffBG.SetActive(true);
        }
    }

    private IEnumerator BlinkObject(GameObject obj)
    {
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        if (group == null) group = obj.AddComponent<CanvasGroup>();

        CanvasGroup iconGroup = UnitIcon.GetComponent<CanvasGroup>();
        if (iconGroup == null) iconGroup = UnitIcon.gameObject.AddComponent<CanvasGroup>();

        float duration = 1f;

        group.alpha = 0f;
        iconGroup.alpha = 1f;
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // Fade In (obj), Fade Out (icon)
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float lerpT = t / duration;
                group.alpha = Mathf.Lerp(0f, 1f, lerpT);
                iconGroup.alpha = Mathf.Lerp(1f, 0f, lerpT);
                yield return null;
            }
            group.alpha = 1f;
            iconGroup.alpha = 0f;

            yield return new WaitForSeconds(2f);

            // Fade Out (obj), Fade In (icon)
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                float lerpT = t / duration;
                group.alpha = Mathf.Lerp(1f, 0f, lerpT);
                iconGroup.alpha = Mathf.Lerp(0f, 1f, lerpT);
                yield return null;
            }
            group.alpha = 0f;
            iconGroup.alpha = 1f;

            yield return new WaitForSeconds(2f);
        }
    }
    private PickInfo pickInfo;

    public void SetPickInfo(PickInfo info)
    {
        pickInfo = info;
    }

    public void Reveal()
    {
        init(pickInfo);
        ShowBack(); // 처음엔 뒷면
    }

    public void ShowBack()
    {
        frontSide.SetActive(false);
        backSide.SetActive(true);
    }

    public void ShowFront()
    {
        frontSide.SetActive(true);
        backSide.SetActive(false);
    }

    public void Flip()
    {
        Sequence flip = DOTween.Sequence();
        SFXManager.Instance.PlaySFX(12);
        flip.Append(transform.DORotate(new Vector3(0, 90, 0), 0.2f).SetEase(Ease.InQuad))
            .AppendCallback(() =>
            {
                ShowFront();
            })
            .Append(transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.OutQuad))
            .OnComplete(() =>
            {
                duplicate.SetActive(isDuplicate ? true : false);
                // Flip이 끝난 후 깜빡이기 시작
                if (duplicate.activeSelf)
                {
                    if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
                    blinkCoroutine = StartCoroutine(BlinkObject(duplicate));
                }
            });
    }
}