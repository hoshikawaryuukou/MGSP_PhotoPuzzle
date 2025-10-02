using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace MGSP.PhotoPuzzle.Infrastructures
{
    public sealed class LoremPicsumImageRequest
    {
        public readonly int width;
        public readonly int height;
        public readonly TimeSpan timeout;

        public LoremPicsumImageRequest(int width, int height, TimeSpan timeout)
        {
            this.width = width;
            this.height = height;
            this.timeout = timeout;
        }
    }

    public sealed class LoremPicsumImageProvider : IDisposable
    {
        private readonly TimeoutController timeoutController = new();

        public async UniTask<Texture2D> GetRandomImage(LoremPicsumImageRequest request, CancellationToken cancellationToken)
        {
            var timeoutToken = timeoutController.Timeout(request.timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutToken, cancellationToken);
            using var req = UnityWebRequestTexture.GetTexture(GetUrl(request.width, request.height));
            try
            {
                await req.SendWebRequest().WithCancellation(linkedCts.Token);
                timeoutController.Reset();

                return DownloadHandlerTexture.GetContent(req);
            }
            catch (OperationCanceledException)
            {
                if (timeoutController.IsTimeout())
                {
                    Debug.Log("Timeout");
                }
                else if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Cancel");
                }

                return null;
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError($"Request error: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            timeoutController.Dispose();
        }

        private string GetUrl(int width, int height)
        {
            width = Mathf.Clamp(width, 1, 2048);
            height = Mathf.Clamp(height, 1, 2048);

            return $"https://picsum.photos/{width}/{height}";
        }
    }
}
