using UnityEngine;

namespace Runtime.Infrastructures
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

            return !(UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable);
        }
    }
}
