using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickSlotSpawner : MonoBehaviour
{
    [SerializeField] private GameObject UnitICard;

    [SerializeField] private Transform Grid1; //1
    [SerializeField] private Transform Grid2; //10
    [SerializeField] private int heroPie = 2;
    [SerializeField] private int normPie = 99;

    List<PickInfo> heroes;
    List<PickInfo> nonHeroes;
    List<PickInfo> Alliance = new List<PickInfo>();

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject textPrefab;
    public GotchaInit gotchaInit;

    private readonly List<GameObject> spawnedCards = new();

    [SerializeField] private Transform centerPoint;

    [SerializeField] private Button revealButton;     // ✅ 씬에 미리 배치한 버튼 참조
    [SerializeField] private ProdManager prodManager; // 또는 ProdDirector

    public GameObject BtnList;

    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private void Start()
    {
        foreach (PickInfo pickInfo in PickUpListLoader.Instance.GetAllPickList().Values.ToList())
        {
            if (!pickInfo.IsEnemy)
            {
                Alliance.Add(pickInfo);
            }
        }
        heroes = Alliance.Where(p => p.IsHero).ToList();
        nonHeroes = Alliance.Where(p => !p.IsHero).ToList();

        // 버튼 초기화
        revealButton.gameObject.SetActive(false);
        revealButton.onClick.RemoveAllListeners();
        revealButton.onClick.AddListener(RevealCards);
    }

    public void SpawnCard(int num)
    {
        foreach (Transform child in Grid2)
            Destroy(child.gameObject);
        spawnedCards.Clear();

        gridLayoutGroup.enabled = false;

        bool isGoodStuff = false;
        for (int i = 0; i < num; i++)
        {
            PickInfo pick = CreateCard(gotchaInit.state);
            if (pick != null && pick.IsHero) isGoodStuff = true; // ✅ null 가드

            GameObject card = Instantiate(UnitICard, centerPoint.position, Quaternion.identity, Grid2);
            card.GetComponent<UnitCardSlot>().SetPickInfo(pick);
            card.GetComponent<UnitCardSlot>().ShowBack();
            card.SetActive(false);
            spawnedCards.Add(card);
        }

        // prod 연출 시작
        prodManager.PlayFromStart(num==10,isGoodStuff);

        // 버튼 켜고 색상만 갱신
        revealButton.gameObject.SetActive(true);
        var img = revealButton.GetComponent<Image>();
        if (img) img.color = isGoodStuff ? Color.yellow : Color.white;
        // ✅ 더 이상 리스너 재등록/인스턴스 생성 없음
    }

    private PickInfo CreateCard(int state)
    {
        var heroCandidates = heroes;
        var nonHeroCandidates = nonHeroes;

        if (state != -1)
        {
            heroCandidates = heroes.Where(h => h.raceId == state).ToList();
            nonHeroCandidates = nonHeroes.Where(n => n.raceId == state).ToList();
        }

        int heroCount = heroCandidates.Count;
        int normCount = nonHeroCandidates.Count;

        int totalPie = (heroCount > 0 ? heroPie : 0) + (normCount > 0 ? normPie : 0);
        if (totalPie == 0)
        {
            Debug.LogWarning($"state {state}에 해당하는 카드가 없습니다.");
            return null;
        }

        int groupRand = Random.Range(0, totalPie);
        bool isHeroGroup = (groupRand < (heroCount > 0 ? heroPie : 0));

        PickInfo selected = null;
        if (isHeroGroup && heroCount > 0)
            selected = heroCandidates[Random.Range(0, heroCount)];
        else if (normCount > 0)
            selected = nonHeroCandidates[Random.Range(0, normCount)];

        if (selected != null)
            QuestEvent.OnRecruit?.Invoke(1);

        return selected;
    }

    public void ShowProbabilityTable()
    {
        int state = gotchaInit.state;

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        var heroCandidates = heroes;
        var nonHeroCandidates = nonHeroes;
        if (state != -1)
        {
            heroCandidates = heroes.Where(h => h.raceId == state).ToList();
            nonHeroCandidates = nonHeroes.Where(n => n.raceId == state).ToList();
        }

        int totalPie = (heroCandidates.Count > 0 ? heroPie : 0) + (nonHeroCandidates.Count > 0 ? normPie : 0);

        if (heroCandidates.Count > 0 && heroPie > 0)
        {
            float groupRate = heroPie / (float)totalPie;
            float unitRate = groupRate / heroCandidates.Count * 100f;
            foreach (var hero in heroCandidates)
                CreateText($"[영웅] {hero.Name} → {unitRate:F2}%");
        }

        if (nonHeroCandidates.Count > 0 && normPie > 0)
        {
            float groupRate = normPie / (float)totalPie;
            float unitRate = groupRate / nonHeroCandidates.Count * 100f;
            foreach (var norm in nonHeroCandidates)
                CreateText($"[일반] {norm.Name} → {unitRate:F2}%");
        }
    }

    private void RevealCards()
    {
        revealButton.gameObject.SetActive(false);
        Vector3 btnWorldPos = revealButton.transform.position;

        foreach (var card in spawnedCards)
            card.SetActive(true);

        // 1) 최종 위치 계산
        gridLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Grid2);

        List<Vector3> finalPositions = new();
        foreach (var card in spawnedCards)
            finalPositions.Add(card.transform.position);

        gridLayoutGroup.enabled = false;

        // 2) 초기 위치: 버튼 위치
        foreach (var card in spawnedCards)
        {
            card.transform.position = btnWorldPos;
            card.transform.localScale = Vector3.zero;
        }

        // 3) 리빌 연출
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            GameObject card = spawnedCards[i];
            card.GetComponent<UnitCardSlot>().Reveal();

            Sequence seq = DOTween.Sequence();
            seq.Append(card.transform.DOMove(centerPoint.position, 0.3f).SetEase(Ease.InOutSine));
            seq.Append(card.transform.DOMove(finalPositions[i], 0.5f).SetEase(Ease.OutBack).SetDelay(0.03f * i));
            card.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }

        // 4) 뒤집기 & 마무리
        DOVirtual.DelayedCall(1.2f, () =>
        {
            for (int i = 0; i < spawnedCards.Count; i++)
            {
                int index = i;
                DOVirtual.DelayedCall(0.1f * i, () =>
                {
                    spawnedCards[index].GetComponent<UnitCardSlot>().Flip();
                });
            }
            float lastFlipDelay = 0.1f * spawnedCards.Count;
            DOVirtual.DelayedCall(lastFlipDelay + 1f, () =>
            {
                BtnList.SetActive(true);
                TutorialManager.Instance.OnEventTriggered("GotchaGotcha");
                BackHandlerManager.Instance.SetBackEnabled(true);
            });
        });
    }

    private void CreateText(string content)
    {
        GameObject textObj = Instantiate(textPrefab, contentParent);
        var text = textObj.GetComponent<TMP_Text>();
        if (text != null) text.text = content;
    }
}
