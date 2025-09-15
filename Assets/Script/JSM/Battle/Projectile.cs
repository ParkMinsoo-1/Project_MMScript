using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [Header("정규화 타이밍(0~1)")]
    [Range(0f, 1f)] public float attackFxSpawnPoint = 0.5f;    // 공격 애니 내 소환 시점
    [Range(0f, 1f)] public float projectileDamagePoint = 0.5f; // 투사체 애니 내 데미지 시점

    [Header("길이/수명")]
    [Tooltip("0이면 자동 추정(Animator/ParticleSystem). Animator 없을 때는 기본 이동 시간으로 사용")]
    public float overrideFxLength = 0f;
    public bool destroyOnEnd = true;

    [Header("Animator 없을 때 아크 이동")]
    public bool kinematicWhenNoAnimator = true;
    [Tooltip("Animator 없을 때 기본 이동 시간(초)")]
    public float defaultTravelTime = 0.6f;
    public AnimationCurve motionCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool rotateToTangent = true; // 진행 방향으로 회전

    private Animator _anim;
    private ParticleSystem _ps;

    // 아크 파라미터(Quadratic Bezier: P0->P1->P2)
    private Vector3 _p0, _p1, _p2;
    private bool _hasArc = false;
    public bool isEnemy;

    private float _fxLength; // 실사용 길이(초)
    private Coroutine _arcCo;
    // Projectile 클래스 내부
    private Unit _owner;

    public void SetOwner(Unit owner)
    {
        _owner = owner;
    }

    private void OnDestroy()
    {
        // 소유 유닛 리스트에서 정리
        if (_owner != null)
            _owner.UnregisterProjectile(this);
    }

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        // 애니메이션/이펙트 길이 확정
        _fxLength = CalcFxLength();

        // Animator가 없고, 아크가 세팅되어 있고, kinematic 모드 허용이면 직접 이동
        if (_anim == null && kinematicWhenNoAnimator && _hasArc)
        {
            if (_arcCo != null) StopCoroutine(_arcCo);
            _arcCo = StartCoroutine(ArcMoveRoutine(_fxLength > 0f ? _fxLength : defaultTravelTime));
        }
        else
        {
            // Animator 있는 경우: 항상 처음 프레임부터 재생 보장
            if (_anim)
            {
                _anim.Rebind();
                _anim.Update(0f);
            }
        }

        if (destroyOnEnd)
        {
            float life = _fxLength > 0f ? _fxLength : defaultTravelTime;
            Destroy(gameObject, life + 0.03f); // 약간 여유
        }
    }

    // Unit에서 호출: 시작/도착/정상(apexY)로 아크 설정
    // 요구사항: x는 '적의 x'로 이동, y는 start.y에서 apexY까지 올라갔다가 다시 start.y로 복귀
    public void InitArc(Vector3 startWorld, float targetX, float apexY)
    {
        if (isEnemy && _anim)
        {
            var s = transform.localScale;
            s.x = -s.x;
            transform.localScale = s;
        }
        _p0 = startWorld;
        _p2 = new Vector3(targetX, startWorld.y, startWorld.z);
        float midX = (_p0.x + _p2.x) * 0.5f;
        _p1 = new Vector3(midX, apexY, startWorld.z);

        _hasArc = true;

        if (_anim == null)
        {
            transform.position = _p0;
            if (kinematicWhenNoAnimator)
            {
                if (_arcCo != null) StopCoroutine(_arcCo);
                _arcCo = StartCoroutine(ArcMoveRoutine(_fxLength > 0f ? _fxLength : defaultTravelTime));
            }
        }
    }


    // 외부(공격자 로직)가 참조하는 길이/타이밍 API
    public float FxLength => (_fxLength > 0f) ? _fxLength : CalcFxLength();
    public float SpawnTimeInAttack(float attackAnimLength)
        => Mathf.Clamp01(attackFxSpawnPoint) * Mathf.Max(attackAnimLength, 0f);
    public float DamageTimeInFx()
        => Mathf.Clamp01(projectileDamagePoint) * Mathf.Max(FxLength, 0f);

    private System.Collections.IEnumerator ArcMoveRoutine(float travelTime)
    {
        if (travelTime <= 0f) travelTime = 0.01f;
        float t = 0f;
        while (t < travelTime)
        {
            float u = Mathf.Clamp01(t / travelTime);
            float s = motionCurve.Evaluate(u);

            // 2차 베지어 포인트
            Vector3 pos = Bezier(_p0, _p1, _p2, s);
            transform.position = pos;

            if (rotateToTangent)
            {
                // 접선(미분)으로 회전
                Vector3 vel = BezierTangent(_p0, _p1, _p2, s);
                if (vel.sqrMagnitude > 1e-6f)
                {
                    float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }

            t += Time.deltaTime;
            yield return null;
        }

        // 종점 보정
        transform.position = _p2;

        if (rotateToTangent)
        {
            Vector3 endVel = _p2 - _p1; // 마지막 접선 근사
            if (endVel.sqrMagnitude > 1e-6f)
            {
                float angle = Mathf.Atan2(endVel.y, endVel.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    private static Vector3 Bezier(in Vector3 p0, in Vector3 p1, in Vector3 p2, float t)
    {
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0
             + 2f * oneMinusT * t * p1
             + t * t * p2;
    }

    private static Vector3 BezierTangent(in Vector3 p0, in Vector3 p1, in Vector3 p2, float t)
    {
        // B'(t) = 2(1-t)(p1 - p0) + 2t(p2 - p1)
        float oneMinusT = 1f - t;
        return 2f * oneMinusT * (p1 - p0) + 2f * t * (p2 - p1);
    }

    private float CalcFxLength()
    {
        if (overrideFxLength > 0f) return overrideFxLength;

        // Animator 기반 길이
        if (_anim != null && _anim.runtimeAnimatorController != null)
        {
            var infos = _anim.GetCurrentAnimatorClipInfo(0);
            if (infos != null && infos.Length > 0 && infos[0].clip != null)
                return infos[0].clip.length;

            var clips = _anim.runtimeAnimatorController.animationClips;
            if (clips != null && clips.Length > 0 && clips[0] != null)
                return clips[0].length;
        }

        // 파티클 기반 길이(루프 아님 가정)
        if (_ps != null)
        {
            var m = _ps.main;
            float life;
            switch (m.startLifetime.mode)
            {
                case ParticleSystemCurveMode.TwoConstants: life = Mathf.Max(m.startLifetime.constantMin, m.startLifetime.constantMax); break;
                case ParticleSystemCurveMode.Constant: life = m.startLifetime.constant; break;
                default: life = m.startLifetime.constantMax; break;
            }
            return Mathf.Max(m.duration, m.duration + life);
        }

        // 둘 다 없으면 기본 이동 시간
        return defaultTravelTime;
    }
}
