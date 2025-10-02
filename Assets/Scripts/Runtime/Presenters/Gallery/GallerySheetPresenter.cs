using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Application.UseCases;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using System.Threading;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presenters
{
    public sealed class GallerySheetPresenter : IInitializable, IDisposable
    {
        private readonly ISubscriber<PhotoRequested> photoRequestedSubscriber;
        private readonly IPublisher<GameRequested> gameRequestedPublisher;
        private readonly DownloadPhoto downloadPhoto;
        private readonly PhotoStore photoStore;
        private readonly GallerySheet gallerySheet;
        private readonly PreviewView previewView;
        private readonly GameSetupModalPresenter gameSetupModalPresenter;

        private readonly CompositeDisposable disposables = new();

        [Inject]
        public GallerySheetPresenter(ISubscriber<PhotoRequested> photoRequestedSubscriber, IPublisher<GameRequested> gameRequestedPublisher, DownloadPhoto downloadPhoto, PhotoStore photoStore, GallerySheet gallerySheet, PreviewView previewView, GameSetupModalPresenter gameSetupModalPresenter)
        {
            this.photoRequestedSubscriber = photoRequestedSubscriber;
            this.gameRequestedPublisher = gameRequestedPublisher;
            this.downloadPhoto = downloadPhoto;
            this.photoStore = photoStore;
            this.gallerySheet = gallerySheet;
            this.previewView = previewView;
            this.gameSetupModalPresenter = gameSetupModalPresenter;
        }

        void IInitializable.Initialize()
        {
            photoRequestedSubscriber
                .Subscribe(_ =>
                {
                    gallerySheet.gameObject.SetActive(true);
                    previewView.gameObject.SetActive(true);
                    RandomInage();
                })
                .AddTo(disposables);

            photoStore.TextureRP
                .Subscribe(tex => previewView.SetImage(tex))
                .AddTo(disposables);

            photoStore.StatusRP
                .Subscribe(OnStatusChanged)
                .AddTo(disposables);

            gallerySheet.RandomRequested
                .Subscribe(_ => RandomInage())
                .AddTo(disposables);

            gallerySheet.PlayRequested
                .Subscribe(_ => OnPlayRequested(default).Forget())
                .AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private void RandomInage()
        {
            var rect = previewView.GetRect();
            var widthInt = (int)rect.width;
            var heightInt = (int)rect.height;

            downloadPhoto.Execute(widthInt, heightInt, default).Forget();
        }

        private void OnStatusChanged(PhotoStatus status)
        {
            switch (status)
            {
                case PhotoStatus.None: gallerySheet.SetNone(); break;
                case PhotoStatus.Downloading: gallerySheet.SetDownloading(); break;
                case PhotoStatus.Ready: gallerySheet.SetReady(); break;
                case PhotoStatus.Failed: gallerySheet.SetFailed(); break;
                default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private async UniTask OnPlayRequested(CancellationToken cancellationToken)
        {
            var result = await gameSetupModalPresenter.Show(cancellationToken);
            if (result == GameSetupModalResult.Confirmed)
            {
                gallerySheet.gameObject.SetActive(false);
                previewView.gameObject.SetActive(false);
                gameRequestedPublisher.Publish(default);
            }
        }
    }
}
