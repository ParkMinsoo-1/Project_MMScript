using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{
    public UnitPool allyPool;
    public UnitPool enemyPool;

    public UnitPool allyHeroPool;  // 추가: 아군 영웅 풀 (단일 유닛)
    public UnitPool enemyHeroPool; // 추가: 적군 영웅 풀 (단일 유닛)

    public Vector2 allySpawnPosition;
    public Vector2 enemySpawnPosition;

    public static UnitSpawner Instance;
    public Image allyLeaderImg;
    public Image enemyLeaderImg;

    public bool doubleCrawler = false;


    [System.Serializable]
    public class ButtonSetting
    {
        public SpawnButton spawnButton;
        public int unitID;
        public bool isEnemy;
        public bool isHero;
    }

    public List<ButtonSetting> buttonSettings = new();
    public GameObject maxSpawn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public IEnumerator SetSpawnPosition()
    {
        allySpawnPosition = GetSpawnPosition(false);
        enemySpawnPosition = GetSpawnPosition(true);
        yield break;
    }
    public void Init(List<UnitStats> normalDeck, UnitStats leaderDeck)
    {
        for (int i = 0; i < normalDeck.Count; i++)
        {
            buttonSettings[i].unitID = normalDeck[i].ID;
        }
        if (buttonSettings != null && buttonSettings.Count > 6 && buttonSettings[6] != null && leaderDeck != null)
        {
            buttonSettings[6].unitID = leaderDeck.ID;
            SkillManager.Instance.SetSkillID(UnitDataManager.Instance.GetStats(buttonSettings[6].unitID).SkillID[0], UnitDataManager.Instance.GetStats(buttonSettings[6].unitID).SkillID[1]);
        }
        if(leaderDeck!=null)
            allyLeaderImg.sprite = Resources.Load<Sprite>($"SPUMImg/{leaderDeck.ModelName}")??null;
        Debug.Log("덱 세팅!");
    }
    private void TrySpawn(SpawnButton data)
    {
        var stats = data.stats;
        var spawnPos = data.isEnemy ? enemySpawnPosition : allySpawnPosition;

        if (BattleResourceManager.Instance.currentResource<stats.Cost)
        {
            Debug.Log($"자원이 부족합니다.");
            return;
        }

        if (!CoolTimeManager.Instance.CanSpawn(data.unitID))
        {
            Debug.Log($"유닛 쿨타임 중입니다.");
            return;
        }

        if (data.isHero)
        {
            var heroPool = data.isEnemy ? enemyHeroPool : allyHeroPool;
            var hero = heroPool.GetUnit(stats, spawnPos);
            if (hero == null)
            {
                maxSpawn.SetActive(true);
                Debug.LogWarning($"{(data.isEnemy ? "적" : "아군")} 영웅 유닛 풀 부족!");
                return;
            }
            SFXManager.Instance.PlaySFX(9);
        }
        else
        {
            var pool = data.isEnemy ? enemyPool : allyPool;
            var unit = pool.GetUnit(stats, spawnPos);
            if (unit == null)
            {
                maxSpawn.SetActive(true);
                Debug.LogWarning($"{(data.isEnemy ? "적군" : "아군")} 유닛 풀 부족!");
                return;
            }
            if(doubleCrawler&&unit.stats.RaceID == 1) 
            {
                unit = pool.GetUnit(stats, spawnPos);
                if (unit == null)
                {
                    maxSpawn.SetActive(true);
                    Debug.LogWarning($"{(data.isEnemy ? "적군" : "아군")} 유닛 풀 부족!");
                    return;
                }
            }
            SFXManager.Instance.PlaySFX(8);
        }
        BattleResourceManager.Instance.Spend(stats.Cost);
        CoolTimeManager.Instance.SetCooldown(data.unitID, stats.SpawnInterval);
    }

    public Vector3 GetSpawnPosition(bool isEnemy)
    {
        float offset = 1f;
        return isEnemy
            ? new Vector3(WaveManager.Instance.currentStage.BaseDistance / 2f - offset, -2.5f, 0)
            : new Vector3(-WaveManager.Instance.currentStage.BaseDistance / 2f + offset, -2.5f, 0);
    }
    public bool SpawnEnemy(int unitID)
    {
        var stats = UnitDataManager.Instance.GetStats(unitID);
        if (stats == null)
        {
            Debug.LogWarning($"[UnitSpawner] 알 수 없는 유닛 ID: {unitID}");
            return false;
        }

        var unit = enemyPool.GetUnit(stats, enemySpawnPosition);
        if (unit == null)
        {
            Debug.LogWarning("[UnitSpawner] 적군 유닛 풀 부족!");
            return false;
        }
        return true;
    }
    public bool SpawnAlly(int unitID)
    {
        var stats = UnitDataManager.Instance.GetStats(unitID);
        if (stats == null)
        {
            Debug.LogWarning($"[UnitSpawner] 알 수 없는 유닛 ID: {unitID}");
            return false;
        }

        var unit = allyPool.GetUnit(stats, allySpawnPosition);
        if (unit == null)
        {
            Debug.LogWarning("[UnitSpawner] 아군 유닛 풀 부족!");
            return false;
        }
        return true;
    }
    public void SpawnEnemyHero(int unitID)
    {
        var stats = UnitDataManager.Instance.GetStats(unitID);
        if (stats == null)
        {
            Debug.LogWarning($"[UnitSpawner] 알 수 없는 유닛 ID: {unitID}");
            return;
        }

        var hero = enemyHeroPool.GetUnit(stats, enemySpawnPosition);
        if (hero == null)
        {
            Debug.LogWarning("[UnitSpawner] 적군 영웅 유닛 풀 부족!");
        }
        enemyLeaderImg.sprite = Resources.Load<Sprite>($"SPUMImg/{UnitDataManager.Instance.GetStats(WaveManager.Instance.currentStage.EnemyHeroID).ModelName}") ?? null;
    }
    public IEnumerator SetButton()
    {
        foreach (var setting in buttonSettings)
        {
            var button = setting.spawnButton;
            button.unitID = setting.unitID;
            button.isEnemy = setting.isEnemy;
            button.isHero = setting.isHero;

            button.button.onClick.RemoveAllListeners();
            button.GetComponent<SpawnButton>().InitializeUI();
            button.button.onClick.AddListener(() => TrySpawn(button));
        }
        yield break;
    }
    public UnitStats SetGimmick(UnitStats stat)
    {
        UnitStats newStat = stat;
        newStat.SpawnInterval += WaveManager.Instance.Gimmickstats.SpawnInterval;
        newStat.Cost += WaveManager.Instance.Gimmickstats.Cost;
        return newStat;
    }
}
