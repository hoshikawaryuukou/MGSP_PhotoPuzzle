using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using YooAsset;

namespace Modules.Shared.Infrastructures
{
    public enum AssetMode
    {
        EditorSimulate,
        WebPlayMode,
    }

    public sealed class AssetLoader
    {
        private readonly AssetMode assetMode;
        private ResourcePackage package;
        private string packageVersion;

        public AssetLoader(AssetMode assetMode)
        {
            YooAssets.Initialize();

            package = YooAssets.CreatePackage("DefaultPackage");
            YooAssets.SetDefaultPackage(package);
            this.assetMode = assetMode;
        }

        public async UniTask Init(CancellationToken cancellationToken = default)
        {
            switch (assetMode)
            {
                case AssetMode.EditorSimulate:
                    await InitPackageSimulate(cancellationToken);
                    break;
                case AssetMode.WebPlayMode:
                    await InitPackageWeb(cancellationToken);
                    break;
            }
        }

        public async UniTask RequestPackageVersion(CancellationToken cancellationToken = default)
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.RequestPackageVersionAsync();
            await operation.ToUniTask(cancellationToken: cancellationToken);

            if (operation.Status == EOperationStatus.Succeed)
            {
                packageVersion = operation.PackageVersion;
                Debug.Log($"Request package Version : {packageVersion}");
            }
            else
            {
                Debug.LogError(operation.Error);
            }
        }

        public async UniTask UpdatePackageManifest(CancellationToken cancellationToken = default)
        {
            var package = YooAssets.GetPackage("DefaultPackage");
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask(cancellationToken: cancellationToken);

            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("Update Package Manifest Succeed");
            }
            else
            {
                Debug.LogError(operation.Error);
            }
        }

        public async UniTask<GameObject> LoadAsync(string assetPath, CancellationToken cancellationToken = default)
        {
            var loadOperation = package.LoadAssetAsync<GameObject>(assetPath);
            await loadOperation.ToUniTask(cancellationToken: cancellationToken);
            if (loadOperation.Status == EOperationStatus.Succeed)
                return loadOperation.AssetObject as GameObject;
            else
            {
                Debug.LogError($"Asset loading failed: {loadOperation.LastError}");
                return null;
            }
        }

        private async UniTask InitPackageSimulate(CancellationToken cancellationToken = default)
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters
            {
                EditorFileSystemParameters = editorFileSystemParams
            };
            var initOperation = package.InitializeAsync(initParameters);
            await initOperation.ToUniTask(cancellationToken: cancellationToken);

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("Package initialization succeeded!");
            else
                Debug.LogError($"Package initialization failed: {initOperation.Error}");
        }

        private async UniTask InitPackageWeb(CancellationToken cancellationToken = default)
        {
            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var initParameters = new WebPlayModeParameters
            {
                WebServerFileSystemParameters = webServerFileSystemParams
            };

            var initOperation = package.InitializeAsync(initParameters);
            await initOperation.ToUniTask(cancellationToken: cancellationToken);

            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("Package initialization succeeded!");
            else
                Debug.LogError($"Package initialization failed: {initOperation.Error}");
        }
    }
}
