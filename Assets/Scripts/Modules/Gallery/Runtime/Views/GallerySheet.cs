using Alchemy.Inspector;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Gallery.Views
{
    public sealed class GallerySheet : MonoBehaviour
    {
        [Title("Photo Pannel")]
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private RawImage photoRawImage;

        [Title("Menu Bar")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button randomButton;
        [SerializeField] private Button retryButton;

        public Observable<Unit> PlayRequested => playButton.OnClickAsObservable();
        public Observable<Unit> RandomRequested => randomButton.OnClickAsObservable();
        public Observable<Unit> RetryRequested => retryButton.OnClickAsObservable();

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public Vector2Int GetTargetPhotoSize()
        {
            return Vector2Int.FloorToInt(photoRawImage.rectTransform.rect.size);
        }

        public void SetStatusDownloading()
        {
            infoText.text = "Downloading...";
            infoText.gameObject.SetActive(true);
            photoRawImage.texture = null;
            photoRawImage.enabled = false;
            playButton.gameObject.SetActive(true);
            playButton.interactable = false;
            randomButton.gameObject.SetActive(true);
            randomButton.interactable = false;
            retryButton.gameObject.SetActive(false);
        }

        public void SetStatusReady(Texture2D tex)
        {
            infoText.gameObject.SetActive(false);
            photoRawImage.texture = tex;
            photoRawImage.enabled = true;
            playButton.gameObject.SetActive(true);
            playButton.interactable = true;
            randomButton.gameObject.SetActive(true);
            randomButton.interactable = true;
            retryButton.gameObject.SetActive(false);
        }

        public void SetStatusError(string errorMessage)
        {
            infoText.text = errorMessage;
            infoText.gameObject.SetActive(true);
            photoRawImage.texture = null;
            photoRawImage.enabled = false;
            playButton.gameObject.SetActive(false);
            randomButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(true);
        }
    }
}
