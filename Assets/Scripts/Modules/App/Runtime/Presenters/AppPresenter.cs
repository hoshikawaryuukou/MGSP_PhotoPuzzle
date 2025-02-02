using Cysharp.Threading.Tasks;
using MessagePipe;
using Module.Game.Events;
using Modules.Gallery.Events;
using Modules.Shared.Infrastructures;
using R3;
using System;
using System.Threading;
using UnityEngine;
using VContainer.Unity;

namespace Modules.App.Presenters
{
    public sealed class AppPresenter : IAsyncStartable, IDisposable
    {
        private readonly AssetLoader assetLoader;
        private readonly ContextNavigator contextNavigator;
        private readonly ISubscriber<GalleryPhotoSelected> galleryPhotoSelectedSubscriber;
        private readonly ISubscriber<GameExited> gameExitedSubscriber;

        private readonly CompositeDisposable disposables = new();

        public AppPresenter(AssetLoader assetLoader, ContextNavigator contextNavigator, ISubscriber<GalleryPhotoSelected> galleryPhotoSelectedSubscriber, ISubscriber<GameExited> gameExitedSubscriber)
        {
            this.assetLoader = assetLoader;
            this.contextNavigator = contextNavigator;
            this.galleryPhotoSelectedSubscriber = galleryPhotoSelectedSubscriber;
            this.gameExitedSubscriber = gameExitedSubscriber;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            await assetLoader.Init(cancellation);
            await assetLoader.RequestPackageVersion(cancellation);
            await assetLoader.UpdatePackageManifest(cancellation);

            galleryPhotoSelectedSubscriber.Subscribe(_ => Navigate("GalleryContext", "GameContext", cancellation).Forget()).AddTo(disposables);
            gameExitedSubscriber.Subscribe(_ => Navigate("GameContext", "GalleryContext", cancellation).Forget()).AddTo(disposables);
            await Navigate(null, "GalleryContext", cancellation);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private async UniTask Navigate(string current, string target, CancellationToken cancellationToken)
        {
            await contextNavigator.LoadAsync(target, cancellationToken);
            contextNavigator.Unload(current);
        }
    }
}
