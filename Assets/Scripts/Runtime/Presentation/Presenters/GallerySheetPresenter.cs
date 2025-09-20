using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using System.Threading;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GallerySheetPresenter : IInitializable, IDisposable
    {
        private readonly ISubscriber<PhotoRequested> galleryRequestedSubscriber;
        private readonly IPublisher<GameRequested> gameRequestedPublisher;
        private readonly PhotoStore photoStore;
        private readonly GalleryMenuBar galleryMenuView;
        private readonly PreviewView previewView;
        private readonly GameSetupModalPresenter gameSetupModalPresenter;

        private readonly CompositeDisposable disposables = new();

        public GallerySheetPresenter(ISubscriber<PhotoRequested> galleryRequestedSubscriber, IPublisher<GameRequested> gameRequestedPublisher, PhotoStore photoStore, GalleryMenuBar galleryMenuView, PreviewView previewView, GameSetupModalPresenter gameSetupModalPresenter)
        {
            this.galleryRequestedSubscriber = galleryRequestedSubscriber;
            this.gameRequestedPublisher = gameRequestedPublisher;
            this.photoStore = photoStore;
            this.galleryMenuView = galleryMenuView;
            this.previewView = previewView;
            this.gameSetupModalPresenter = gameSetupModalPresenter;
        }

        void IInitializable.Initialize()
        {
            galleryRequestedSubscriber
                .Subscribe(_ =>
                {
                    galleryMenuView.gameObject.SetActive(true);
                    previewView.gameObject.SetActive(true);
                    RandomInage();
                })
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
                    galleryMenuView.SetRnadomButtonLabel("Loading...");
                    galleryMenuView.SetRandomButtonInteractable(false);
                    galleryMenuView.SetPlayButtonInteractable(false);
                    break;
                case PhotoStatus.Ready:
                    galleryMenuView.SetRnadomButtonLabel("Random");
                    galleryMenuView.SetRandomButtonInteractable(true);
                    galleryMenuView.SetPlayButtonInteractable(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private async UniTask OnPlayRequested(CancellationToken cancellationToken)
        {
            var result = await gameSetupModalPresenter.Show(cancellationToken);
            if (result == GameSetupModalResult.Confirmed)
            {
                galleryMenuView.gameObject.SetActive(false);
                previewView.gameObject.SetActive(false);
                gameRequestedPublisher.Publish(default);
            }
        }
    }
}
