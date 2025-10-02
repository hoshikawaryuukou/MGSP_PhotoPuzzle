using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Application.Ports;
using MGSP.PhotoPuzzle.Application.Stores;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Application.UseCases
{
    public sealed class DownloadPhoto
    {
        private readonly IPhotoProvider photoProvider;
        private readonly PhotoStore photoStore;

        [Inject]
        public DownloadPhoto(IPhotoProvider photoProvider, PhotoStore photoStore)
        {
            this.photoProvider = photoProvider;
            this.photoStore = photoStore;
        }

        public async UniTask Execute(int width, int height, CancellationToken cancellationToken = default)
        {
            photoStore.statusRP.Value = PhotoStatus.Downloading;
            var photoTex = await photoProvider.GetRandom(width, height, cancellationToken);

            if (photoTex)
            {
                photoStore.textureRP.Value = photoTex;
                photoStore.statusRP.Value = PhotoStatus.Ready;
            }
            else
            {
                photoStore.statusRP.Value = PhotoStatus.Failed;
            }
        }
    }
}
