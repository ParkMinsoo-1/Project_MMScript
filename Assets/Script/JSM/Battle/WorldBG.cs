using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldParallaxLayer
{
    public string spritePath;
    public float parallaxFactor = 1f; // 정렬 순서용
    public float yPosition = 0f;
}

public class WorldBG : MonoBehaviour
{
    public Transform parentTransform;
    public int stageID = 0;
    private TextAsset stageDataCSV;

    public List<WorldParallaxLayer> layers = new();
    private readonly List<Transform> generatedTiles = new();

    private void Awake()
    {
        stageDataCSV = Resources.Load<TextAsset>("Data/StageData");
    }
    private void Start()
    {
        StartCoroutine(InitializeStage());
    }

    private IEnumerator InitializeStage()
    {
        // stageID가 0이면 대기
        if (stageID == 0)
        {
            yield return new WaitUntil(() =>
                WaveManager.Instance != null && WaveManager.Instance.stageID != 0);

            stageID = WaveManager.Instance.stageID;
        }

        // WaitUntil 이후에 실행됨
        LoadBGListFromStage();
    }
    private Sprite FindSpriteInSubfolders(string spriteName, out int sortingOrder)
    {
        if (spriteName == null)
        {
            sortingOrder = 0;
            return null;
        }
        var folderOrder = new Dictionary<string, int>
        {
            { "Sky", -100 },
            { "Mountain", -99 },
            { "Middle-Far", -98 },
            { "Middle-Near", -97 },
            { "Ground", -96 },
            { "Foreground", -95 }
        };

        foreach (var kvp in folderOrder)
        {
            string fullPath = $"Backgrounds/{kvp.Key}/{spriteName}";
            Sprite found = Resources.Load<Sprite>(fullPath);
            if (found != null)
            {
                sortingOrder = kvp.Value;
                return found;
            }
        }

        Debug.LogWarning($"스프라이트를 찾을 수 없습니다: {spriteName}");
        sortingOrder = 0;
        return null;
    }



    private void LoadBGListFromStage()
    {
        if (stageDataCSV == null)
        {
            Debug.LogError("Stage CSV가 비어있습니다.");
            return;
        }

        var stageData = StageDataLoader.LoadByID(stageDataCSV, stageID);
        if (stageData == null || stageData.BGList == null)
        {
            Debug.LogError("StageData 또는 BGList를 불러올 수 없습니다.");
            return;
        }

        foreach (string path in stageData.BGList)
        {
            if (path == "") continue;
            layers.Add(new WorldParallaxLayer
            {
                spritePath = path,
                parallaxFactor = 1f,
                yPosition = 0f
            });
        }

        foreach (var layer in layers)
        {
            Sprite sprite = FindSpriteInSubfolders(layer.spritePath, out int sortingOrder);
            if (sprite == null) continue;

            GameObject go = new GameObject($"BG_{layer.spritePath}", typeof(SpriteRenderer));
            go.transform.SetParent(parentTransform);
            go.transform.localPosition = new Vector3(0f, layer.yPosition, 0f);

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            generatedTiles.Add(go.transform);
        }
    }
}
