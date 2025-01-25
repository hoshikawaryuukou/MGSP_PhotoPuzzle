using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Views
{
    public sealed class GamePlayPuzzlePieceView : MonoBehaviour
    {
        [SerializeField] private RawImage image;
        [SerializeField] private Button button;

        public Observable<Unit> Clicked => button.OnClickAsObservable();

        public void SetPiece(Texture2D tex, Rect uvRect)
        {
            image.texture = tex;
            image.uvRect = uvRect;
        }
    }
}