using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
    public bool isEnemy;
    public GameObject unitPrefab;
    public int poolSize = 10;

    private Queue<Unit> pool = new();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(unitPrefab, transform);
            obj.SetActive(false);

            Unit unit = obj.GetComponent<Unit>();
            unit.isEnemy = isEnemy;
            pool.Enqueue(unit);
        }
    }

    public Unit GetUnit(UnitStats stats, Vector2 spawnPos)
    {
        foreach (var unit in pool)
        {
            if (!unit.gameObject.activeInHierarchy)
            {
                unit.stats = stats;
                unit.transform.position = spawnPos + new Vector2(0, -Random.value*0.2f);
                unit.Initialize();
                return unit;
            }
        }

        Debug.LogWarning($"[풀링] {name}에 유닛이 부족합니다!");
        return null;
    }

    public void ReturnUnit(Unit unit)
    {
        unit.stats = null;
        unit.transform.SetParent(transform);
        unit.gameObject.SetActive(false);
    }
    public bool HasAvailable()
    {
        foreach (var unit in pool)
        {
            if (!unit.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }

}
