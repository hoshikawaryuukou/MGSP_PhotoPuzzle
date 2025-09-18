using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class PreviewView : MonoBehaviour
    {
        [SerializeField] private RawImage rawImage;

        public void SetImage(Texture2D texture)
        {
            rawImage.texture = texture;
        }

        public Rect GetRect()
        {
            return rawImage.rectTransform.rect;
        }
    }
}
