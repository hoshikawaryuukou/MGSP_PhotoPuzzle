using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Infrastructures;
using R3;
using System;
using System.Threading;
using UnityEngine;

namespace MGSP.PhotoPuzzle.Presentation.Stores
{
    public enum PhotoStatus { None, Downloading, Ready }

    public sealed class PhotoStore
    {
        private readonly LoremPicsumImageProvider imageProvider;

        private readonly ReactiveProperty<PhotoStatus> statusRP = new(PhotoStatus.None);
        private readonly ReactiveProperty<Texture2D> photoTexRP = new(null);

        public PhotoStore(LoremPicsumImageProvider imageProvider)
        {
            this.imageProvider = imageProvider;
        }

        public ReadOnlyReactiveProperty<Texture2D> PhotoTexRP => photoTexRP;
        public ReadOnlyReactiveProperty<PhotoStatus> StatusRP => statusRP;

        public async UniTask DownloadInage(int width, int height, CancellationToken cancellationToken)
        {
            statusRP.Value = PhotoStatus.Downloading;

            var request = new LoremPicsumImageRequest(width, height, TimeSpan.FromSeconds(10));
            photoTexRP.Value = await imageProvider.GetRandomImage(request, cancellationToken);

            statusRP.Value = PhotoStatus.Ready;
        }
    }
}
