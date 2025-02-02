using UnityEngine;

namespace UniGears.Common
{
    public sealed class FrameRateControl : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 30;

        private void Awake()
        {
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
