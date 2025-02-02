using Cysharp.Threading.Tasks;
using MessagePipe;
using Module.Game.Events;
using Modules.Game.Views;
using Modules.Shared.Stores;
using R3;
using System;
using VContainer.Unity;

namespace Modules.Game.Presenters
{
    public sealed class GamePreviewPresenter : IStartable, IDisposable
    {
        private readonly IScopedSubscriber<GameConfirmed> gameConfirmedSubscriber;
        private readonly PhotoStore photoStore;
        private readonly GamePlayPreviewView gamePlayPreviewView;

        private readonly CompositeDisposable disposables = new();

        public GamePreviewPresenter(IScopedSubscriber<GameConfirmed> gameConfirmedSubscriber, PhotoStore photoStore, GamePlayPreviewView gamePlayPreviewView)
        {
            this.gameConfirmedSubscriber = gameConfirmedSubscriber;
            this.photoStore = photoStore;
            this.gamePlayPreviewView = gamePlayPreviewView;
        }

        void IStartable.Start()
        {
            gameConfirmedSubscriber.Subscribe(_ => gamePlayPreviewView.Hide()).AddTo(disposables);

            gamePlayPreviewView.SetImage(photoStore.PhotoTex);
            gamePlayPreviewView.Show();
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
