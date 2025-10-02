using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class GallerySheet : MonoBehaviour
    {
        [SerializeField] private Button randomButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_Text randomLabel;

        public Observable<Unit> RandomRequested => randomButton.OnClickAsObservable();
        public Observable<Unit> PlayRequested => playButton.OnClickAsObservable();

        public void SetNone()
        {
            randomLabel.text = "Random";
            randomButton.interactable = true;
            playButton.interactable = false;
        }

        public void SetDownloading()
        {
            randomLabel.text = "Downloading...";
            randomButton.interactable = false;
            playButton.interactable = false;
        }

        public void SetReady()
        {
            randomLabel.text = "Random";
            randomButton.interactable = true;
            playButton.interactable = true;
        }

        public void SetFailed()
        {
            randomLabel.text = "<color=red>Retry</color>";
            randomButton.interactable = true;
            playButton.interactable = false;
        }
    }
}
