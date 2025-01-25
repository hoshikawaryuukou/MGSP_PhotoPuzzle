using Alchemy.Serialization;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Runtime.Infrastructures
{
    [AlchemySerialize, CreateAssetMenu(fileName = "ContextTable", menuName = "App/ContextTable")]
    public partial class ContextTable : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, Tooltip("Folder to drag and drop prefabs")]
        private DefaultAsset source;
#endif

        [AlchemySerializeField, NonSerialized]
        private Dictionary<string, GameObject> contextPrefabTable = new();

        public GameObject GetPrefab(string path)
        {
            var prefab = contextPrefabTable.GetValueOrDefault(path);
            if (prefab == null)
            {
                throw new KeyNotFoundException($"Prefab not found: {path}");
            }

            return prefab;
        }

#if UNITY_EDITOR
        [ContextMenu("Reload Prefabs")]
        private void ReloadPrefabs()
        {
            LoadPrefabs();
        }

        private void OnValidate()
        {
            LoadPrefabs();
        }

        private void LoadPrefabs()
        {
            if (source != null)
            {
                string folderPath = AssetDatabase.GetAssetPath(source);
                string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });

                contextPrefabTable.Clear();
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

                    if (fileName.EndsWith("Context"))
                    {
                        string key = fileName.Substring(0, fileName.Length - "Context".Length);
                        contextPrefabTable[key] = prefab;
                    }
                }
            }
        }
#endif
    }
}
