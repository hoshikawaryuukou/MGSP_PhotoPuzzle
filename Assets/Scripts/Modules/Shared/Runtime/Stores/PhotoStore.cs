using UnityEngine;

namespace Modules.Shared.Stores
{
    public class PhotoStore
    {
        public Texture2D PhotoTex { get; private set; }

        public void SetPhoto(Texture2D photo)
        {
            PhotoTex = photo;
        }
    }
}
