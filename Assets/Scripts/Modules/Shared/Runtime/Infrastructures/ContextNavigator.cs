using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Modules.Shared.Infrastructures
{
    public sealed class ContextNavigator
    {
        private readonly AssetLoader assetLoader;
        private readonly Dictionary<string, GameObject> map = new();

        public ContextNavigator(AssetLoader assetLoader)
        {
            this.assetLoader = assetLoader;
        }

        public async UniTask LoadAsync(string contextName, CancellationToken cancellationToken)
        {
            var prefab = await assetLoader.LoadAsync(contextName, cancellationToken);
            if (!map.ContainsKey(contextName))
            {
                map[contextName] = Object.Instantiate(prefab);
            }
        }

        public void Unload(string contextName)
        {
            if (contextName == null)
                return;

            if (map.TryGetValue(contextName, out var gObj))
            {
                Object.Destroy(gObj);
                map.Remove(contextName);
            }
        }
    }
}
