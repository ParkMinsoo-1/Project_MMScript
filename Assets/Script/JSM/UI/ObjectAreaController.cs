using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ObjectAreaController : MonoBehaviour
{
    public Camera objectCamera;

    [Header("여백 설정")]
    public float marginLeft = 0.2f;
    public float marginRight = 0.2f;
    public float marginTop = 0.2f;
    public float marginBottom = 0.2f;

    [Header("움직임 설정")]
    public float maxIdle = 35f;
    public float minIdle = 20f;
    public float maxMove = 10f;
    public float minMove = 5f;
    public float maxSpeed = 1.5f;
    public float minSpeed = 1.5f;
    public float scale = 1f;
    private Vector2 worldMin;
    private Vector2 worldMax;
    private bool initialized = false;

    private readonly List<GameObject> spawnedUnits = new();

    public void OnSpawn()
    {
        SetupMovementAreaFromCamera();
        SpawnRandomUnits();
    }

    private void OnDisable()
    {
        foreach (var go in spawnedUnits)
        {
            if (go != null)
                Destroy(go);
        }
        spawnedUnits.Clear();
    }

    private void SetupMovementAreaFromCamera()
    {
        if (objectCamera == null) return;

        float orthoSize = objectCamera.orthographicSize;
        float aspect = objectCamera.aspect;

        float height = orthoSize * 2f;
        float width = height * aspect;

        Vector3 camCenter = objectCamera.transform.position;

        float left = camCenter.x - (width / 2f) + marginLeft;
        float right = camCenter.x + (width / 2f) - marginRight;
        float bottom = camCenter.y - (height / 2f) + marginBottom;
        float top = camCenter.y + (height / 2f) - marginTop;

        worldMin = new Vector2(left, bottom);
        worldMax = new Vector2(right, top);

        initialized = true;
    }

    private void SpawnRandomUnits()
    {
        if (!initialized) return;

        var unitIDs = PlayerDataManager.Instance.player.myUnitIDs;
        if (unitIDs == null || unitIDs.Count == 0) return;

        // 랜덤으로 최대 5개 뽑기
        var shuffled = unitIDs.OrderBy(_ => Random.value).Take(5).ToList();

        foreach (int unitID in shuffled)
        {
            var stats = UnitDataManager.Instance.GetStats(unitID);
            if (stats == null) continue;

            string modelName = stats.ModelName;
            GameObject prefab = Resources.Load<GameObject>($"Units/{modelName}");
            if (prefab == null)
            {
                Debug.LogWarning($"[스폰 실패] 프리팹 없음: {modelName}");
                continue;
            }

            // 랜덤 위치 계산
            Vector3 spawnPos = GetRandomPositionWithinBounds();
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            go.transform.localScale *= scale;

            spawnedUnits.Add(go);

            var mover = go.GetComponent<UnitMover>();
            if (mover == null)
                mover = go.AddComponent<UnitMover>();

            mover.Init(worldMin, worldMax, maxIdle, minIdle, maxMove, minMove, maxSpeed, minSpeed);
        }
    }

    private Vector3 GetRandomPositionWithinBounds()
    {
        float x = Random.Range(worldMin.x, worldMax.x);
        float y = Random.Range(worldMin.y, worldMax.y);
        float z = 0f; // Z는 평면 기준
        return new Vector3(x, y, z);
    }
}
