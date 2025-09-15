using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    public GameObject skillPanel;
    public GameObject allyActivateSkill;
    public GameObject enemyActivateSkill;
    public GameObject skillPanelBtn;

    [Header("스킬 ID")]
    public int passiveSkillID = 0;
    public int activeSkillID = 0;
    public SkillData passiveSkillData;
    public SkillData activeSkillData;

    [Header("스킬 참조")]
    public GraveSpawnSkill graveSpawnSkill;

    public GameObject unitpool;
    public GameObject heroUnitpool;
    public GameObject spawnBtn;
    public SpawnButton heroUnitBtn;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetSkillID(int a, int b)
    {
        passiveSkillID = a;
        activeSkillID = b;
        passiveSkillData = SkillDataManager.Instance.GetSkillData(a);
        activeSkillData = SkillDataManager.Instance.GetSkillData(b);
        Debug.Log($"{passiveSkillData.Name}!\n{activeSkillData.Name}!");
    }

    public void OnUnitDeath(Unit unit)
    {
        switch (unit.isEnemy ? 0 : passiveSkillID)
        {
                case 1:
                {
                    if(unit.stats.RaceID == passiveSkillData.TargetRaceID) graveSpawnSkill?.TrySpawnGrave(unit, passiveSkillData.EffectValue[0], passiveSkillData.EffectValue[1]);
                    break;
                }
        }
    }

    public void UseSkill(int skillID, bool isEnemy)
    {
        Debug.Log($"스킬 발동 요청: ID={skillID}, isEnemy={isEnemy}");

        switch (skillID)
        {
            case 2:
                if (graveSpawnSkill != null)
                    graveSpawnSkill.ActivateGraves(isEnemy, activeSkillData.EffectValue[0]);
                break;
            case 4:
                BuffSkill(activeSkillData.EffectValue[0],activeSkillData.TargetRaceID, 0);
                break;
            case 6:
                UnitSpawner.Instance.SpawnAlly(5001);
                UnitSpawner.Instance.SpawnAlly(5001);
                UnitSpawner.Instance.SpawnAlly(5002);
                UnitSpawner.Instance.SpawnAlly(5002);
                break;
            case 8:
                BuffSkill(activeSkillData.EffectValue[0], activeSkillData.TargetRaceID, 1);
                break;
            case 10:
                BuffSkill(activeSkillData.EffectValue[0], activeSkillData.TargetRaceID, 2);
                break;
            default:
                Debug.LogWarning($"정의되지 않은 스킬 ID: {skillID}");
                break;
        }
        if (!isEnemy)
        {
            allyActivateSkill.SetActive(true);
        }
        else
        {
            enemyActivateSkill.SetActive(true);
        }
    }
    public UnitStats OnStartBuff(UnitStats stat)
    {
        switch (passiveSkillID)
        {
            case 3:
                if (stat.RaceID == passiveSkillData.TargetRaceID)
                    stat.Cost = Mathf.FloorToInt(stat.Cost * (100 - passiveSkillData.EffectValue[0])/100);
                break;
            case 5:
                if (stat.RaceID == passiveSkillData.TargetRaceID)
                {
                    stat.MaxHP = stat.MaxHP * passiveSkillData.EffectValue[0] / 100;
                    stat.Damage = stat.Damage * passiveSkillData.EffectValue[1] / 100;
                    stat.SpawnInterval = stat.SpawnInterval * (100 + passiveSkillData.EffectValue[2]) / 100;
                }
                break;
            case 7:
                if (stat.RaceID == passiveSkillData.TargetRaceID)
                {
                    stat.MoveSpeed = stat.MoveSpeed * ((passiveSkillData.EffectValue[0]+100) / 100);
                    stat.PostDelay = stat.PostDelay * (100 - passiveSkillData.EffectValue[1]) / 100f;
                    stat.PreDelay = stat.PreDelay * (100 - passiveSkillData.EffectValue[1]) / 100f;
                }
                break;
            case 9:
                if (stat.RaceID == passiveSkillData.TargetRaceID)
                {
                    stat.MaxHP = stat.MaxHP * (100-passiveSkillData.EffectValue[1]) / 100f;
                    stat.Damage = stat.Damage * (100-passiveSkillData.EffectValue[2]) / 100f;
                    stat.SpawnInterval = stat.SpawnInterval * (100 + passiveSkillData.EffectValue[2]) / 100;
                    UnitSpawner.Instance.doubleCrawler = true;
                }
                break;
            default:
                Debug.LogWarning($"정의되지 않은 스킬 ID: {passiveSkillID}");
                break;
        }
        if(passiveSkillID != 0)
            SetSkillPanel();
        return stat;
    }

    // raceId: 적용할 종족 ID (예: 2). 전체 적용하고 싶으면 -1처럼 특별값 사용
    public void BuffSkill(float durationSeconds, int raceId, int num)
    {
        StartCoroutine(ApplyBuffCoroutine(durationSeconds, raceId, num));
    }

    private IEnumerator ApplyBuffCoroutine(float durationSeconds, int raceId, int num)
    {
        Unit[] targets = unitpool.GetComponentsInChildren<Unit>(true);
        SpawnButton[] buttons = spawnBtn.GetComponentsInChildren<SpawnButton>(true);

        float moveBuff = (100f + activeSkillData.EffectValue[1]) / 100f;
        float delayBuff = (100f - activeSkillData.EffectValue[1]) / 100f;
        float InterBuff = (100f - activeSkillData.EffectValue[1]) / 100f;
        float CostBuff = (100f - activeSkillData.EffectValue[1]) / 100f;

        // 백업은 대상 여부와 무관하게 모두 수행
        List<(Unit unit, float moveSpeed, float preDelay, float postDelay, float interver, int cost)> unitBackups = new();
        List<(SpawnButton button, float moveSpeed, float preDelay, float postDelay, float interver, int cost)> buttonBackups = new();

        // 공통 적용 함수
        void ApplyToUnit(Unit t)
        {
            if (raceId >= 0 && t.stats.RaceID != raceId) return; // 해당 종족만 적용
            switch (num)
            {
                case 0:
                    t.stats.MoveSpeed *= moveBuff;
                    t.stats.PreDelay *= delayBuff;
                    t.stats.PostDelay *= delayBuff;
                    break;
                case 1:
                    t.stats.SpawnInterval *= InterBuff;
                    break;
                case 2:
                    t.stats.SpawnInterval *= InterBuff;
                    t.stats.Cost = (int)(t.stats.Cost * CostBuff);
                    break;
                default:
                    break; // 알 수 없는 num이면 아무 것도 안함
            }
        }

        void ApplyToButton(SpawnButton b)
        {
            if (raceId >= 0 && b.stats.RaceID != raceId) return;
            switch (num)
            {
                case 0:
                    b.stats.MoveSpeed *= moveBuff;
                    b.stats.PreDelay *= delayBuff;
                    b.stats.PostDelay *= delayBuff;
                    break;
                case 1:
                    b.stats.MoveSpeed *= moveBuff;
                    break;
                case 2:
                    b.stats.SpawnInterval *= InterBuff;
                    b.stats.Cost = (int)(b.stats.Cost * CostBuff);
                    break;
                default:
                    break;
            }
        }

        // 유닛
        foreach (var t in targets)
        {
            if (t == null) continue;
            unitBackups.Add((t, t.stats.MoveSpeed, t.stats.PreDelay, t.stats.PostDelay, t.stats.SpawnInterval, t.stats.Cost)); // 무조건 백업
            ApplyToUnit(t); // 조건에 맞으면 적용
        }

        // 스폰 버튼
        foreach (var b in buttons)
        {
            if (b == null) continue;
            buttonBackups.Add((b, b.stats.MoveSpeed, b.stats.PreDelay, b.stats.PostDelay, b.stats.SpawnInterval, b.stats.Cost)); // 무조건 백업
            ApplyToButton(b); // 조건에 맞으면 적용
        }

        yield return new WaitForSeconds(durationSeconds);

        // 복원 (사라졌을 수 있으니 null 체크)
        foreach (var (unit, moveSpeed, preDelay, postDelay, spawnInter, cost) in unitBackups)
        {
            if (unit == null) continue;
            unit.stats.MoveSpeed = moveSpeed;
            unit.stats.PreDelay = preDelay;
            unit.stats.PostDelay = postDelay;
            unit.stats.SpawnInterval = spawnInter;
            unit.stats.Cost = cost;
        }
        foreach (var (button, moveSpeed, preDelay, postDelay, spawnInter, cost) in buttonBackups)
        {
            if (button == null) continue;
            button.stats.MoveSpeed = moveSpeed;
            button.stats.PreDelay = preDelay;
            button.stats.PostDelay = postDelay;
            button.stats.SpawnInterval = spawnInter;
            button.stats.Cost = cost;
        }
    }


    private void SetSkillPanel()
    {
        skillPanelBtn.SetActive(passiveSkillData != null);
        var skillcs = skillPanel.GetComponent<SkillPanel>();
        skillcs.skillNameP.text = passiveSkillData.Name;
        skillcs.skillDescriptionP.text = passiveSkillData.Description;
        skillcs.skillNameA.text = activeSkillData.Name;
        skillcs.skillDescriptionA.text = activeSkillData.Description;
    }
}
