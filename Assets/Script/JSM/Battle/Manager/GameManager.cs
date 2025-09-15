using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadThenStartWave());
    }
    public IEnumerator LoadThenStartWave()
    {
        yield return StartCoroutine(WaveManager.Instance.WaitForStageIDThenLoad());
        yield return StartCoroutine(UnitSpawner.Instance.SetSpawnPosition());
        yield return StartCoroutine(MapManager.Instance.MapStart());
        yield return StartCoroutine(UnitSpawner.Instance.SetButton());
        yield return StartCoroutine(WaveManager.Instance.StartWaveAfterTeaTime());
    }
}