using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class PuzzlePieceView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        public Observable<Unit> Clicked => button.OnClickAsObservable();

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public Sprite GetImage()
        {
            return image.sprite;
        }
    }
}
