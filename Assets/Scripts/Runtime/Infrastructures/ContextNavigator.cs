using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Infrastructures
{
    public sealed class ContextNavigator
    {
        private readonly ContextTable table;
        private readonly Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();

        public ContextNavigator(ContextTable table)
        {
            this.table = table;
        }

        public void Load(string contextName)
        {
            var prefab = table.GetPrefab(contextName);

            if (!map.ContainsKey(contextName))
            {
                map[contextName] = UnityEngine.Object.Instantiate(prefab);
            }
        }

        public void Unload(string contextName)
        {
            if (map.ContainsKey(contextName))
            {
                UnityEngine.Object.Destroy(map[contextName]);
                map.Remove(contextName);
            }
        }
    }
}
