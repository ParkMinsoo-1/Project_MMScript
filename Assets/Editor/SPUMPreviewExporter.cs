using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SPUMPreviewExporter
{
    private const int Width = 400;
    private const int Height = 400;
    private const float OrthoSize = 0.6f;
    private const float CameraOffsetY = 0.4f;

    [MenuItem("Tools/Export SPUM Preview")]
    public static void Export()
    {
        string prefabPath = "Units";
        string savePath = "Assets/Resources/SPUMImg";
        Directory.CreateDirectory(savePath);

        Object[] prefabs = Resources.LoadAll(prefabPath, typeof(GameObject));

        foreach (var prefab in prefabs)
        {
            GameObject inst = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            inst.transform.position = Vector3.zero;

            // ✅ shadowRenderer 유연하게 찾기
            var shadowRenderer = inst.GetComponentsInChildren<Renderer>()
                .FirstOrDefault(r => r.name == "Shadow");
            if (shadowRenderer == null)
            {
                Debug.LogWarning($"❌ 그림자를 찾을 수 없음: {prefab.name}");
                Object.DestroyImmediate(inst);
                continue;
            }

            // ✅ 모든 렌더러의 z값 정리
            foreach (var r in inst.GetComponentsInChildren<Renderer>())
            {
                Vector3 pos = r.transform.position;
                r.transform.position = new Vector3(pos.x, pos.y, 0);
            }

            // ✅ 카메라 설정
            var camGO = new GameObject("PreviewCamera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = OrthoSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            cam.cullingMask = ~0;
            cam.allowHDR = false;
            cam.enabled = false;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 20f;

            // ✅ 카메라 위치 조정
            Vector3 center = shadowRenderer.bounds.center + new Vector3(0f, CameraOffsetY, 0f);
            cam.transform.position = new Vector3(center.x, center.y, -10f);

            // ✅ 렌더링
            var rt = new RenderTexture(Width, Height, 24);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            var tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            tex.Apply();

            // ✅ 저장
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes($"{savePath}/{prefab.name}.png", bytes);

            // ✅ 정리
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
            Object.DestroyImmediate(camGO);
            Object.DestroyImmediate(inst);
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ SPUM 이미지 추출 완료");
    }
}
