using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniGears.GithubPagesEx.Editor
{
    public sealed class GithubPagesHelper
    {
        [MenuItem("UniGears/GithubPagesEx/GithubPagesHelper/Clean", priority = 0)]
        public static void Clean()
        {
            RunPowerShellCommand("clean.ps1");
        }

        [MenuItem("UniGears/GithubPagesEx/GithubPagesHelper/Build", priority = 1)]
        public static void Build()
        {
            BuildForGithubPages();
        }

        [MenuItem("UniGears/GithubPagesEx/GithubPagesHelper/Deploy", priority = 2)]
        public static void Deploy()
        {
            RunPowerShellCommand("gh-pages.ps1");
        }

        private static void BuildForGithubPages()
        {
            bool originalDecompressionFallback = PlayerSettings.WebGL.decompressionFallback;

            PlayerSettings.WebGL.decompressionFallback = true;

            string projectRoot = Directory.GetParent(Application.dataPath).ToString();
            string outputPath = Path.Combine(projectRoot, "GithubPagesHelper", "GithubPagesBuild");

            BuildOptions buildOptions = BuildOptions.None;
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.WebGL, buildOptions);

            PlayerSettings.WebGL.decompressionFallback = originalDecompressionFallback;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("WebGL Build has been completed and exported to " + outputPath);
        }

        private static bool RunPowerShellCommand(string scriptName)
        {
            string projectRoot = Directory.GetParent(Application.dataPath).ToString();
            string workingDirectory = Path.Combine(projectRoot, "PowerShellCommands");
            string scriptPath = Path.Combine(workingDirectory, scriptName);

            if (File.Exists(scriptPath))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    UseShellExecute = true,
                    WorkingDirectory = workingDirectory
                };

                using Process process = new()
                { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();

                return true;
            }

            return false;
        }
    }
}
