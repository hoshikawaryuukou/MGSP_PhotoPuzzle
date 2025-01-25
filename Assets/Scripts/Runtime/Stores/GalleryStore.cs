using Cysharp.Threading.Tasks;
using R3;
using Runtime.Infrastructures;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Stores
{
    public enum GalleryStatus
    {
        None,
        Downloading,
        Ready,
        Error_NoNetwork,
        Error_Timeout,
    }

    public sealed class GalleryStore
    {
        private readonly NetworkHandler networkHandler;
        private readonly LoremPicsumImageProvider loremPicsumImageProvider;

        private readonly ReactiveProperty<GalleryStatus> statusRP = new(GalleryStatus.None);
        private Texture2D photoTex = null;

        public GalleryStore(NetworkHandler networkHandler, LoremPicsumImageProvider loremPicsumImageProvider)
        {
            this.networkHandler = networkHandler;
            this.loremPicsumImageProvider = loremPicsumImageProvider;
        }

        public ReadOnlyReactiveProperty<GalleryStatus> StatusRP => statusRP;
        public Texture2D PhotoTex => photoTex;

        public async UniTask DownloadPhoto(Vector2Int photoSize, CancellationToken cancellationToken)
        {
            if (statusRP.Value == GalleryStatus.Downloading)
            {
                Debug.LogWarning("Already downloading.");
                return;
            }

            if (!networkHandler.IsConnected())
            {
                statusRP.Value = GalleryStatus.Error_NoNetwork;
                return;
            }

            statusRP.Value = GalleryStatus.Downloading;

            photoTex = await loremPicsumImageProvider.GetRandomImage(CreateRequest(photoSize), cancellationToken);
            if (photoTex == null)
            {
                statusRP.Value = GalleryStatus.Error_Timeout;
                return;
            }

            statusRP.Value = GalleryStatus.Ready;
        }

        public void SetPhoto(Texture2D photo)
        {
            photoTex = photo;
            statusRP.Value = GalleryStatus.Ready;
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
