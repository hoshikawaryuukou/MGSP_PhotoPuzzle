using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class PuzzlePieceView : MonoBehaviour
    {
        [SerializeField] private RawImage rawimage;
        [SerializeField] private Button button;

        public Observable<Unit> Clicked => button.OnClickAsObservable();

        public void SetImage(Texture2D tex, Rect uvRect)
        {
            rawimage.texture = tex;
            rawimage.uvRect = uvRect;
        }

        public void SetUVRect(Rect uvRect) => rawimage.uvRect = uvRect;

        public Rect GetUVRect() => rawimage.uvRect;

        /// <summary>
        /// Reset the piece to default state when returning to pool
        /// </summary>
        public void ResetPiece()
        {
            rawimage.texture = null;
            rawimage.uvRect = new Rect(0, 0, 1, 1);
        }
    }
}
