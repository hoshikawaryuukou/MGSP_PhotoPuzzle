using Cysharp.Threading.Tasks;
using MessagePipe;
using Module.Game.Events;
using Modules.Game.Stores;
using Modules.Game.Views;
using R3;
using System;
using System.Threading;
using VContainer.Unity;

namespace Modules.Game.Presenters
{
    public sealed class GameMenuPresenter : IStartable, IDisposable
    {
        private readonly IScopedPublisher<GameAsked> gameAskedPublisher;
        private readonly IScopedPublisher<GameFinished> gameFinishedPublisher;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlayMenuSheet gamePlayMenuBarView;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GameMenuPresenter(IScopedPublisher<GameAsked> gameAskedPublisher, IScopedPublisher<GameFinished> gameFinishedPublisher, GamePlayStore gamePlayStore, GamePlayMenuSheet gamePlayMenuBarView)
        {
            this.gameAskedPublisher = gameAskedPublisher;
            this.gameFinishedPublisher = gameFinishedPublisher;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayMenuBarView = gamePlayMenuBarView;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            gamePlayStore.PlayStatusRP.Subscribe(OnGamePlayStatusChanged).AddTo(disposables);

            gamePlayMenuBarView.ReplayRequested.Subscribe(_ => gameAskedPublisher.Publish(new GameAsked())).AddTo(disposables);
            gamePlayMenuBarView.NewGameRequested.Subscribe(_ => gameFinishedPublisher.Publish(new GameFinished())).AddTo(disposables);
            gamePlayMenuBarView.ClearConfirmed.Subscribe(_ => gamePlayMenuBarView.SetIdle()).AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        private void OnGamePlayStatusChanged(GamePlayStatus status)
        {
            switch (status)
            {
                case GamePlayStatus.Playing:
                    gamePlayMenuBarView.SetPlaying();
                    break;
                case GamePlayStatus.Completed:
                    gamePlayMenuBarView.SetCompleted();
                    break;
            }
        }
    }
}
