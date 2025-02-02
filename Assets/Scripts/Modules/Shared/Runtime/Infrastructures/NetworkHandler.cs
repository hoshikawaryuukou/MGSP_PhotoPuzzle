using UnityEngine;

namespace Modules.Shared.Infrastructures
{
    [System.Serializable]
    public sealed class NetworkHandler
    {
        [SerializeField] private bool isNoInternetSimulation;

        public bool IsConnected()
        {
            if (isNoInternetSimulation)
            {
                return false;
            }

            return !(Application.internetReachability == NetworkReachability.NotReachable);
        }
    }
}
