using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using YooAsset.Editor;

namespace UniGears.YooAssetEx.Editor
{
    public class PackageImporterWindow : EditorWindow
    {
        private static PackageImporterWindow window;

        [MenuItem("UniGears/YooAssetEx/PackageImporter", false, 101)]
        static void ShowWindow()
        {
            if (window == null)
            {
                window = GetWindow(typeof(PackageImporterWindow), false, "PackageImporter", true) as PackageImporterWindow;
                window.minSize = new Vector2(800, 300);
            }
            window.Show();
        }

        private string _manifestPath = string.Empty;

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Package", GUILayout.MaxWidth(150)))
            {
                string resultPath = EditorUtility.OpenFilePanel("Find", "Bundles", "bytes");
                if (!string.IsNullOrEmpty(resultPath))
                {
                    _manifestPath = resultPath;
                }
            }
            EditorGUILayout.LabelField(_manifestPath);
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(_manifestPath) == false)
            {
                if (GUILayout.Button("Import", GUILayout.MaxWidth(150)))
                {
                    string streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();
                    EditorTools.ClearFolder(streamingAssetsRoot);
                    CopyPackageFiles(_manifestPath);
                }
            }
        }

        private void CopyPackageFiles(string manifestFilePath)
        {
            string sourcePath = Path.GetDirectoryName(manifestFilePath);
            string destPath = Path.Combine(AssetBundleBuilderHelper.GetStreamingAssetsRoot(), "DefaultPackage");

            Debug.Log($"Copy from {sourcePath} to {destPath}");

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            string[] extensions = new[] { ".bundle", ".hash", ".bytes", ".version" };
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string extension = Path.GetExtension(file);
                if (Array.Exists(extensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    string destFile = Path.Combine(destPath, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                    Debug.Log($"Copied {file} to {destFile}");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
