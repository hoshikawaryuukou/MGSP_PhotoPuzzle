using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class GalleryMenuBar : MonoBehaviour
    {
        [SerializeField] private Button randomButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_Text randomLabel;

        public Observable<Unit> RandomRequested => randomButton.OnClickAsObservable();
        public Observable<Unit> PlayRequested => playButton.OnClickAsObservable();

        public void SetRnadomButtonLabel(string value)
        {
            randomLabel.text = value;
        }

        public void SetRandomButtonInteractable(bool value)
        {
            randomButton.interactable = value;
        }

        public void SetPlayButtonInteractable(bool value)
        {
            playButton.interactable = value;
        }
    }
}
