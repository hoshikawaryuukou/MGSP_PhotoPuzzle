using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using Runtime.Routes;
using Runtime.Stores;
using Runtime.Views;
using System;
using System.Threading;
using VContainer.Unity;

namespace Runtime.Presenters
{
    public sealed class GalleryPresenter : IStartable, IDisposable
    {
        private readonly GalleryStore photoStore;
        private readonly IPublisher<AppRoute> appRoutePublisher;
        private readonly GallerySheet gallerySheet;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();
        private readonly string error_NoNetwork = "No Network";
        private readonly string error_Timeout = "Timeout";

        public GalleryPresenter(GalleryStore photoStore, IPublisher<AppRoute> appRoutePublisher, GallerySheet gallerySheet)
        {
            this.photoStore = photoStore;
            this.appRoutePublisher = appRoutePublisher;
            this.gallerySheet = gallerySheet;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            photoStore.StatusRP.Subscribe(OnPhotoStatusChanged).AddTo(disposables);
            gallerySheet.PlayRequested.Subscribe(_ => PlayGame()).AddTo(disposables);
            gallerySheet.RandomRequested.Subscribe(_ => DownloadPhoto()).AddTo(disposables);
            gallerySheet.RetryRequested.Subscribe(_ => DownloadPhoto()).AddTo(disposables);
            
            gallerySheet.Show();
            DownloadPhoto();
        }

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        private void DownloadPhoto()
        {
            photoStore.DownloadPhoto(gallerySheet.GetTargetPhotoSize(), cts.Token).Forget();
        }

        private void PlayGame()
        {
            appRoutePublisher.Publish(AppRoute.Game);
        }

        private void OnPhotoStatusChanged(GalleryStatus photoStatus)
        {
            switch (photoStatus)
            {
                case GalleryStatus.Downloading:
                    gallerySheet.SetStatusDownloading();
                    break;
                case GalleryStatus.Ready:
                    gallerySheet.SetStatusReady(photoStore.PhotoTex);
                    break;
                case GalleryStatus.Error_NoNetwork:
                    gallerySheet.SetStatusError(error_NoNetwork);
                    break;
                case GalleryStatus.Error_Timeout:
                    gallerySheet.SetStatusError(error_Timeout);
                    break;
            }
        }
    }
}
