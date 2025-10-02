using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MGSP.PhotoPuzzle.Application.Ports
{
    public interface IPhotoProvider 
    {   
        UniTask<Texture2D> GetRandom(int width, int height, CancellationToken cancellationToken);
    }
}
