using Cysharp.Threading.Tasks;
using MessagePipe;
using Modules.Gallery.Events;
using Modules.Gallery.Views;
using Modules.Shared.Infrastructures;
using Modules.Shared.Stores;
using R3;
using System;
using System.Threading;
using UniGears.LoremPicsum;
using UnityEngine;
using VContainer.Unity;

namespace Modules.Gallery.Presenters
{
    public sealed class GalleryPresenter : IStartable, IDisposable
    {
        private readonly NetworkHandler networkHandler;
        private readonly LoremPicsumImageProvider loremPicsumImageProvider;
        private readonly IPublisher<GalleryPhotoSelected> publisher;
        private readonly PhotoStore photoStore;
        private readonly GallerySheet gallerySheet;

        private readonly CompositeDisposable disposables = new();
        private readonly CancellationTokenSource cts = new();
        private readonly string error_NoNetwork = "No Network";
        private readonly string error_Retry = "Retry";

        public GalleryPresenter(NetworkHandler networkHandler, LoremPicsumImageProvider loremPicsumImageProvider, IPublisher<GalleryPhotoSelected> publisher, PhotoStore photoStore, GallerySheet gallerySheet)
        {
            this.networkHandler = networkHandler;
            this.loremPicsumImageProvider = loremPicsumImageProvider;
            this.publisher = publisher;
            this.photoStore = photoStore;
            this.gallerySheet = gallerySheet;
        }

        void IStartable.Start()
        {
            gallerySheet.PlayRequested.Subscribe(_ => publisher.Publish(new GalleryPhotoSelected())).AddTo(disposables);
            gallerySheet.RandomRequested.Subscribe(_ => DownloadPhoto(cts.Token).Forget()).AddTo(disposables);
            gallerySheet.RetryRequested.Subscribe(_ => DownloadPhoto(cts.Token).Forget()).AddTo(disposables);

            DownloadPhoto(cts.Token).Forget();
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
            cts.Cancel();
            cts.Dispose();
        }

        private async UniTask DownloadPhoto(CancellationToken cancellationToken)
        {
            if (!networkHandler.IsConnected())
            {
                gallerySheet.SetStatusError(error_NoNetwork);
                return;
            }

            gallerySheet.SetStatusDownloading();
            var photoSize = gallerySheet.GetTargetPhotoSize();
            var photo = await loremPicsumImageProvider.GetRandomImage(CreateRequest(photoSize), cancellationToken);
            if (photo == null)
            {
                gallerySheet.SetStatusError(error_Retry);
                return;
            }

            gallerySheet.SetStatusReady(photo);
            photoStore.SetPhoto(photo);
        }

        private LoremPicsumImageRequest CreateRequest(Vector2Int photoRectSize)
        {
            var photoSize = RemapMax1024(photoRectSize);
            return new LoremPicsumImageRequest(photoSize.x, photoSize.y, TimeSpan.FromSeconds(5));
        }

        private Vector2Int RemapMax1024(Vector2Int size)
        {
            if (size.x <= 1024 && size.y <= 1024)
            {
                return size;
            }

            int max = Mathf.Max(size.x, size.y);
            float ratio = 1024f / max;
            int x = Mathf.RoundToInt(size.x * ratio);
            int y = Mathf.RoundToInt(size.y * ratio);
            return new Vector2Int(x, y);
        }
    }
}
