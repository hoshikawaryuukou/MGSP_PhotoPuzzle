using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class GamePlaySheet : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button previewButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button clearButton;

        public Observable<Unit> NewGameRequested => newGameButton.OnClickAsObservable();
        public Observable<Unit> PreviewRequested => previewButton.OnClickAsObservable();
        public Observable<Unit> ReplayRequested => replayButton.OnClickAsObservable();
        public Observable<Unit> ClearRequested => clearButton.OnClickAsObservable();

        public void SetPreparing()
        {
            newGameButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
            previewButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }

        public void SetPlaying()
        {
            newGameButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(true);
            previewButton.gameObject.SetActive(true);
            clearButton.gameObject.SetActive(false);
        }

        public void SetCompleted()
        {
            newGameButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
            previewButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(true);
        }

        public void SetCompletedConfirmed()
        {
            newGameButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(true);
            previewButton.gameObject.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }

        public void SetPreviewOn(bool value)
        {
            previewButton.image.color = value ? Color.green : Color.white;
        }
    }   
}
