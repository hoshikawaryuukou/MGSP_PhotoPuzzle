using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GalleryPresenter : IInitializable, IPostStartable, IDisposable
    {
        private readonly AppStore appStore;
        private readonly PhotoStore photoStore;
        private readonly GalleryMenuBar galleryMenuView;
        private readonly PreviewView previewView;

        private readonly CompositeDisposable disposables = new();

        public GalleryPresenter(AppStore appStore, PhotoStore photoStore, GalleryMenuBar galleryMenuView, PreviewView previewView)
        {
            this.appStore = appStore;
            this.photoStore = photoStore;
            this.galleryMenuView = galleryMenuView;
            this.previewView = previewView;
        }

        void IInitializable.Initialize()
        {
            appStore.StatusRP
                .Select(status => status == AppStatus.Gallery)
                .DistinctUntilChanged()
                .Subscribe(isActive => SetActive(isActive))
                .AddTo(disposables);

            photoStore.PhotoTexRP
                .Subscribe(tex => previewView.SetImage(tex))
                .AddTo(disposables);

            photoStore.StatusRP
                .Subscribe(OnStatusChanged)
                .AddTo(disposables);

            galleryMenuView.RandomRequested
                .Subscribe(_ => RandomInage())
                .AddTo(disposables);

            galleryMenuView.PlayRequested
                .Subscribe(_ => appStore.SetStatus(AppStatus.GamePlay))
                .AddTo(disposables);
        }

        void IPostStartable.PostStart()
        {
            RandomInage();
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private void SetActive(bool value)
        {
            galleryMenuView.gameObject.SetActive(value);
            previewView.gameObject.SetActive(value);
        }

        private void RandomInage()
        {
            var rect = previewView.GetRect();
            var widthInt = (int)rect.width;
            var heightInt = (int)rect.height;

            photoStore.DownloadInage(widthInt, heightInt, default).Forget();
        }

        private void OnStatusChanged(PhotoStatus status)
        {
            switch (status)
            {
                case PhotoStatus.None:
                    galleryMenuView.SetRnadomButtonLabel("Random");
                    galleryMenuView.SetRandomButtonInteractable(true);
                    galleryMenuView.SetPlayButtonInteractable(false);
                    break;
                case PhotoStatus.Downloading:
                    galleryMenuView.SetRnadomButtonLabel("Downloading...");
                    galleryMenuView.SetRandomButtonInteractable(false);
                    galleryMenuView.SetPlayButtonInteractable(false);
                    break;
                case PhotoStatus.Ready:
                    galleryMenuView.SetRnadomButtonLabel("Random");
                    galleryMenuView.SetRandomButtonInteractable(true);
                    galleryMenuView.SetPlayButtonInteractable(true);
                    break;
            }
        }
    }
}
