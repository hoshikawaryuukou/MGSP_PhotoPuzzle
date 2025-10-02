using R3;
using UnityEngine;

namespace MGSP.PhotoPuzzle.Application.Stores
{
    public enum PhotoStatus { None, Downloading, Ready, Failed }

    public sealed class PhotoStore
    {
        internal readonly ReactiveProperty<PhotoStatus> statusRP = new(PhotoStatus.None);
        internal readonly ReactiveProperty<Texture2D> textureRP = new(null);

        public ReadOnlyReactiveProperty<PhotoStatus> StatusRP => statusRP;
        public ReadOnlyReactiveProperty<Texture2D> TextureRP => textureRP;
    }
}
