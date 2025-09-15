using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public float mapLength;
    public GameObject allyBasePrefab;
    public GameObject enemyBasePrefab;
    public Transform mapRoot;
    public static MapManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public IEnumerator MapStart()
    {
        mapLength = WaveManager.Instance.currentStage.BaseDistance;
        InitMap();
        SetupCameraBounds();
        yield break;
    }

    private void InitMap()
    {
        float halfLength = mapLength / 2f;
        GameObject allyBase = Instantiate(allyBasePrefab, new Vector3(-halfLength, -1.0f, 0), Quaternion.identity, mapRoot);
        GameObject enemyBase = Instantiate(enemyBasePrefab, new Vector3(halfLength, -1.0f, 0), Quaternion.identity, mapRoot);
    }

    private void SetupCameraBounds()
    {
        float halfLength = mapLength / 2f;
        float padding = -2f;
        CameraController cam = Camera.main.GetComponent<CameraController>();
    }
}
