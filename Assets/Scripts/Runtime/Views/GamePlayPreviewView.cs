using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Views
{
    public sealed class GamePlayPreviewView : MonoBehaviour
    {
        [SerializeField] private RawImage image;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetImage(Texture2D texture)
        {
            image.texture = texture;
        }
    }
}
