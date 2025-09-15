using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

public class ReferenceFinder
{
    [MenuItem("Tools/Find Prefabs Referencing UnitDataManager")]
    public static void FindReferences()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 이 경로가 GameObject인지 미리 확인
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
            if (mainAsset is not GameObject) continue;

            GameObject prefab = mainAsset as GameObject;
            if (prefab == null) continue;

            Component[] components = prefab.GetComponentsInChildren<Component>(true);
            foreach (var comp in components)
            {
                if (comp == null) continue;

                var so = new SerializedObject(comp);
                var prop = so.GetIterator();

                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ManagedReference ||
                        prop.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (prop.objectReferenceValue != null &&
                            prop.objectReferenceValue.GetType().Name == "UnitDataManager")
                        {
                            Debug.Log($"Prefab {path} references UnitDataManager via {comp.GetType().Name}", prefab);
                        }
                    }
                }
            }
        }

        Debug.Log("검색 완료");
    }

}
