using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitState
{
    Idle,
    Moving,
    Fighting,
    Hitback,
    Dead
}

public class Unit : MonoBehaviour
{
    public UnitStats stats;
    public bool isEnemy;

    [SerializeField] private Transform spriteRoot;

    private Transform target;
    private float currentHP;
    private bool isAttacking = false;

    private UnitState state;
    private PlayerState currentAnimState = PlayerState.OTHER;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private SPUM_Prefabs spumController;
    private readonly HashSet<int> triggeredHitbackZones = new();
    private Coroutine attackCoroutine;

    public GameObject effectParticle;

    [SerializeField] private Transform projectileSpawnPoint; // 없으면 transform.position 사용
    private GameObject projectilePrefab; // Resources/Projectiles/{stats.projectile} 캐시
    [SerializeField] private float projectileApexY = 2.0f; // "특정 y값" (월드 Y)
    private float attackDamagePoint; // 공격 애니 안에서 데미지 들어갈 지점(0=시작,1=끝)

    // Unit 클래스 내부
    private readonly List<Projectile> _ownedProjectiles = new();

    public void RegisterProjectile(Projectile p)
    {
        if (p == null) return;
        if (!_ownedProjectiles.Contains(p))
            _ownedProjectiles.Add(p);
    }

    public void UnregisterProjectile(Projectile p)
    {
        if (p == null) return;
        _ownedProjectiles.Remove(p);
    }

    private void ClearProjectilesImmediate()
    {
        for (int i = _ownedProjectiles.Count - 1; i >= 0; i--)
        {
            var p = _ownedProjectiles[i];
            if (p) Destroy(p.gameObject);
        }
        _ownedProjectiles.Clear();
    }

    private void OnDisable() => ClearProjectilesImmediate();
    private void OnDestroy() => ClearProjectilesImmediate();

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (stats == null) return;

        switch (state)
        {
            case UnitState.Moving:
                TryDetectEnemy();
                break;
            case UnitState.Idle:
                TryAttack();
                break;
        }
    }

    public virtual void Initialize()
    {
        currentHP = stats.MaxHP;
        triggeredHitbackZones.Clear();
        SetState(UnitState.Moving);
        gameObject.SetActive(true);
        spriteRoot.localScale = new Vector3(stats.Size * 1.5f, stats.Size * 1.5f, 0);

        SetLayerRecursively(gameObject, LayerMask.NameToLayer(isEnemy ? "Enemy" : "Ally"));
        LoadModel();

        LoadProjectilePrefab(); // << 추가

        switch (stats.AttackType)
        {
            case 0:
                attackDamagePoint = 0.5f;
                break;
            case 1:
                attackDamagePoint = 0.5f;
                break;
            case 2:
                attackDamagePoint = 0.8f;
                break;
            case 4:
                attackDamagePoint = 0.5f;
                break;
        }
    }
    private void LoadProjectilePrefab()
    {
        projectilePrefab = null;
        if (stats == null) return;

        // 이름 정리
        string projName = (stats.projectile ?? string.Empty).Trim();
        Debug.Log(stats.Name+" : "+ stats.projectile+"+");
        // 비었거나 "-"면 투사체 없음
        if (string.IsNullOrEmpty(projName) || projName == "-")
            return;
        
        string path = $"Projectiles/{projName}";
        projectilePrefab = Resources.Load<GameObject>(path);
        if (projectilePrefab == null)
            Debug.LogWarning($"Projectile prefab not found at Resources/{path}");
        else
            Debug.Log("프리팹 찾음");

        // 스폰 포인트 없으면 모델 하위에서 찾아보기(선택)
        if (projectileSpawnPoint == null && spriteRoot != null)
        {
            var t = spriteRoot.Find("ProjectileSocket");
            if (t != null) projectileSpawnPoint = t;
        }
    }


    public virtual void OnSpawned()
    {
    }
    private void LoadModel()
    {
        if (spriteRoot == null)
        {
            Debug.LogWarning("SpriteRoot가 설정되지 않았습니다.");
            return;
        }

        string path = $"Units/{stats.ModelName}";
        var modelPrefab = Resources.Load<GameObject>(path);

        if (modelPrefab == null)
        {
            Debug.LogWarning($"모델 프리팹을 찾을 수 없습니다: {path}");
            return;
        }

        foreach (Transform child in spriteRoot)
            Destroy(child.gameObject);

        GameObject instance = Instantiate(modelPrefab, spriteRoot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        if (!isEnemy)
        {
            var scale = instance.transform.localScale;
            scale.x *= -1f;
            instance.transform.localScale = scale;
        }

        spumController = instance.GetComponent<SPUM_Prefabs>();
        spumController.OverrideControllerInit();
        spumController.PopulateAnimationLists();

        animator = spumController.GetComponentInChildren<Animator>();
    }

    private void MoveForward()
    {
        float dir = isEnemy ? -1f : 1f;
        transform.position += new Vector3(dir * stats.MoveSpeed * Time.deltaTime, 0, 0);

        if (spriteRenderer != null)
            spriteRenderer.flipX = isEnemy;
    }

    private void TryDetectEnemy()
    {
        Vector2 origin = transform.position;
        Vector2 direction = isEnemy ? Vector2.left : Vector2.right;
        var layerMask = LayerMask.GetMask(isEnemy ? "Ally" : "Enemy");

        var hit = Physics2D.Raycast(origin, direction, stats.AttackRange, layerMask);
        Debug.DrawRay(origin, direction * stats.AttackRange, Color.red);

        if (hit.collider == null)
            MoveForward();
        else if (hit.collider.TryGetComponent<BaseCastle>(out var castle))
        {
            if (castle.isEnemy == this.isEnemy) MoveForward();
            else
            {
                target = castle.transform;
                SetState(UnitState.Idle);
            }
        }
        else if (hit.collider?.GetComponent<Unit>() is Unit enemy)
        {
            if (enemy.state == UnitState.Hitback || enemy.state == UnitState.Dead)
                return;
            target = enemy.transform;
            SetState(UnitState.Idle);
        }
        else
            target = null;
    }

    private void TryAttack()
    {
        if (isAttacking) return;
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = null;
            SetState(UnitState.Moving);
            return;
        }
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float attackAnimLength = spumController.GetAnimationLength(PlayerState.ATTACK);

        // 프리팹에서 투사체 정보 읽기
        Projectile projCfgOnPrefab = projectilePrefab ? projectilePrefab.GetComponent<Projectile>() : null;

        // 소환 시점: 공격 애니 기준
        float spawnWait = projCfgOnPrefab
            ? projCfgOnPrefab.SpawnTimeInAttack(attackAnimLength)
            : 0.5f * attackAnimLength; // 없으면 절반 지점

        // 공격 모션 대기시간: PreDelay 안에 포함
        float attackMotionLeadTime = spawnWait;
        Debug.Log(stats.PreDelay);
        // (PreDelay - 공격모션 대기) 대기
        float firstWait = Mathf.Max(0.05f, stats.PreDelay - attackMotionLeadTime);
        yield return new WaitForSeconds(firstWait);

        // 공격 모션 시작
        SetState(UnitState.Fighting);

        // (공격모션 대기 - 투사체 소환) 대기
        if (attackMotionLeadTime > 0f) yield return new WaitForSeconds(attackMotionLeadTime);

        // 투사체 소환
        GameObject fx = null;
        Projectile projCfgOnInstance = null;
        float fxLen = 0f;
        if (projectilePrefab)
        {
            Vector3 spawnPos = projectileSpawnPoint ? projectileSpawnPoint.position : transform.position;
            fx = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            projCfgOnInstance = fx.GetComponent<Projectile>();
            projCfgOnInstance.isEnemy = isEnemy ? true : false;

            float targetX = (target != null) ? target.position.x : spawnPos.x + (isEnemy ? -stats.AttackRange : stats.AttackRange);
            float apexY = projectileApexY;
            projCfgOnInstance?.InitArc(spawnPos, targetX, apexY);

            fxLen = projCfgOnInstance?.FxLength ?? projCfgOnPrefab?.FxLength ?? 0f;
            fx.transform.SetParent(transform, true);

            projCfgOnInstance = fx.GetComponent<Projectile>();
            projCfgOnInstance?.SetOwner(this);
            RegisterProjectile(projCfgOnInstance);
        }

        // 데미지 시점: 투사체 애니 길이에 비례
        if (projCfgOnInstance != null)
        {
            float dmgWait = projCfgOnInstance.DamageTimeInFx();
            if (dmgWait > 0f) yield return new WaitForSeconds(dmgWait);
        }

        // 데미지 처리
        DoDamage();

        // FX 나머지 시간 대기
        if (fxLen > 0f)
        {
            float remain = fxLen - projCfgOnInstance.DamageTimeInFx();
            if (remain > 0f) yield return new WaitForSeconds(remain);
        }

        // PostDelay 대기
        float postWait = Mathf.Max(0.05f, stats.PostDelay);
        if (postWait > 0f) yield return new WaitForSeconds(postWait);

        SetState(UnitState.Moving);
        isAttacking = false;
    }




    private void DoDamage()
    {
        Vector2 center = transform.position;
        float radius = stats.AttackRange;
        int enemyLayer = LayerMask.NameToLayer(isEnemy ? "Ally" : "Enemy");
        int mask = 1 << enemyLayer;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, mask);

        if (stats.IsAOE)
        {
            foreach (var h in hits) ApplyDamage(h);
        }
        else
        {
            Collider2D closest = null;
            float minDist = float.MaxValue;
            foreach (var h in hits)
            {
                float d = Vector2.Distance(center, h.transform.position);
                if (d < minDist) { minDist = d; closest = h; }
            }
            if (closest != null) ApplyDamage(closest);
        }
    }

    private void ApplyDamage(Collider2D hit)
    {
        var unit = hit.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(stats.Damage);
        }
        else
        {
            var castle = hit.GetComponent<BaseCastle>();
            if (castle != null)
            {
                castle.TakeDamage((int)stats.Damage);
            }
        }
    }

    private void HitEffect()
    {
        if(effectParticle == null)
        {
            return;
        }
        SFXManager.Instance.PlaySFX(1);
        Vector3 pos = transform.position + new Vector3(0, 1f, 0);
        GameObject obj = Instantiate(effectParticle, pos, Quaternion.Euler(90,0,0));
        Destroy(obj, 1f);
    }

    public void TakeDamage(float amount)
    {
        if (stats == null || state is UnitState.Hitback or UnitState.Dead) return;

        float oldHP = currentHP;
        currentHP -= amount;

        HitEffect();

        if (currentHP <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            float slice = stats.MaxHP / Mathf.Max(stats.Hitback, 1);
            for (int i = stats.Hitback; i >= 1; i--)
            {
                float threshold = slice * (i - 1);
                if (oldHP > threshold && currentHP <= threshold && !triggeredHitbackZones.Contains(i))
                {
                    triggeredHitbackZones.Add(i);
                    StartCoroutine(DoHitback());
                    break;
                }
            }
        }
    }


    private IEnumerator Die()
    {
        SkillManager.Instance?.OnUnitDeath(this);
        if (isAttacking && attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            isAttacking = false;
            attackCoroutine = null;
        }
        SetState(UnitState.Dead);

        if (isEnemy) BattleResourceManager.Instance.Add(stats.Cost);
        stats = null;
        target = null;

        float deathAnimTime = 0f;
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            deathAnimTime = stateInfo.length;
        }
        yield return new WaitForSeconds(deathAnimTime > 0 ? deathAnimTime : 1f);
        spriteRoot.localScale = Vector3.one;


        var pool = GetComponentInParent<UnitPool>();
        if (pool != null) pool.ReturnUnit(this);
        else gameObject.SetActive(false);
    }

    private IEnumerator DoHitback()
    {
        if (isAttacking && attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            isAttacking = false;
            attackCoroutine = null;
        }

        SetState(UnitState.Hitback);

        spumController.PlayAnimation(PlayerState.DAMAGED, 0);

        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        float duration = 0.5f;
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(isEnemy ? 1f : -1f, 0f, 0f);

        float t = 0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        gameObject.layer = originalLayer;

        SetState(UnitState.Moving);
    }


    private void SetState(UnitState newState)
    {
        if (state == newState && currentAnimState != PlayerState.OTHER) return;
        state = newState;
        UpdateAnimation();

        if (newState == UnitState.Hitback || newState == UnitState.Dead)
            ClearProjectilesImmediate();
    }

    private void UpdateAnimation()
    {
        if (spumController == null || !spumController.allListsHaveItemsExist()) return;

        PlayerState newAnim = ConvertToPlayerState(state);
        if (newAnim != currentAnimState)
        {
            currentAnimState = newAnim;
            spumController.PlayAnimation(newAnim, state == UnitState.Fighting ? stats.AttackType : 0);
        }
    }

    private PlayerState ConvertToPlayerState(UnitState unitState) => unitState switch
    {
        UnitState.Idle => PlayerState.IDLE,
        UnitState.Moving => PlayerState.MOVE,
        UnitState.Fighting => PlayerState.ATTACK,
        UnitState.Hitback => PlayerState.DAMAGED,
        UnitState.Dead => PlayerState.DEATH,
        _ => PlayerState.OTHER
    };

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
