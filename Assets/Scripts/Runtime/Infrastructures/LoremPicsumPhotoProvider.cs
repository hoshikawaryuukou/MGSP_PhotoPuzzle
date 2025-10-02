using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Application.Ports;
using System.Threading;
using UnityEngine;

namespace MGSP.PhotoPuzzle.Infrastructures
{
    public sealed class LoremPicsumPhotoProvider : IPhotoProvider
    {
        private readonly LoremPicsumImageProvider imageProvider;

        public LoremPicsumPhotoProvider()
        {
            imageProvider = new LoremPicsumImageProvider();
        }

        UniTask<Texture2D> IPhotoProvider.GetRandom(int width, int height, CancellationToken cancellationToken)
        {
            return imageProvider.GetRandomImage(new LoremPicsumImageRequest(width, height, System.TimeSpan.FromSeconds(10)), cancellationToken);
        }
    }
}
