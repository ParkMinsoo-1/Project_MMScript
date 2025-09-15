using UnityEngine;
using System.Collections;

public class UnitMover : MonoBehaviour
{
    private Vector2 min;
    private Vector2 max;
    private float speed;
    private Transform modelTransform;

    private Vector3 direction;
    private bool isMoving = false;

    private SPUM_Prefabs spum;
    private float maxIdle;
    private float minIdle;
    private float maxMove;
    private float minMove;
    private float maxSpeed;
    private float minSpeed;
    public void Init(Vector2 minBound, Vector2 maxBound, float maxIdle, float minIdle, float maxMove, float minMove, float maxSpeed, float minSpeed)
    {
        min = minBound;
        max = maxBound;
        this.maxSpeed = maxSpeed;
        this.minSpeed = minSpeed;
        modelTransform = transform;
        this.maxIdle = maxIdle;
        this.minIdle = minIdle;
        this.maxMove = maxMove;
        this.minMove = minMove;
        spum = GetComponent<SPUM_Prefabs>();
        spum.OverrideControllerInit();
        StartCoroutine(MovementLoop());
    }

    private IEnumerator MovementLoop()
    {
        while (true)
        {
            isMoving = true;
            direction = GetRandomHorizontalDirection();
            float moveDuration = Random.Range(minMove, maxMove);
            spum.PlayAnimation(PlayerState.MOVE, 0);
            float timer = 0f;
            while (timer < moveDuration)
            {
                Move();
                timer += Time.deltaTime;
                yield return null;
            }

            spum.PlayAnimation(PlayerState.IDLE, 0);

            isMoving = false;
            float waitDuration = Random.Range(minIdle, maxIdle);
            yield return new WaitForSeconds(waitDuration);
        }
    }

    private Vector3 GetRandomHorizontalDirection()
    {
        return Random.value < 0.5f ? Vector3.left : Vector3.right;
    }

    private void Move()
    {
        if (!isMoving || modelTransform == null) return;

        Vector3 pos = modelTransform.position;
        pos += direction * Random.Range(minSpeed, maxSpeed) * Time.deltaTime;

        // 벽에 부딪히면 반사
        if (pos.x < min.x)
        {
            pos.x = min.x;
            direction.x *= -1;
        }
        else if (pos.x > max.x)
        {
            pos.x = max.x;
            direction.x *= -1;
        }

        modelTransform.position = pos;

        // 좌우 반전 (올바른 방향)
        if (direction.x != 0)
        {
            Vector3 scale = modelTransform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction.x > 0 ? -1 : 1);
            modelTransform.localScale = scale;
        }
    }
}
