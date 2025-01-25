using Cysharp.Threading.Tasks;
using R3;
using Runtime.Stores;
using Runtime.Views;
using System;
using System.Threading;
using VContainer.Unity;

namespace Runtime.Presenters
{
    public sealed class GameMenuPresenter : IStartable, IDisposable
    {
        private readonly GameStore gameStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlayMenuSheet gamePlayMenuBarView;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GameMenuPresenter(GameStore gameStore, GamePlayStore gamePlayStore, GamePlayMenuSheet gamePlayMenuBarView)
        {
            this.gameStore = gameStore;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayMenuBarView = gamePlayMenuBarView;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            gameStore.ExitSubject.Subscribe(_ => gamePlayMenuBarView.SetIdle()).AddTo(disposables);
            gamePlayStore.PlayStatusRP.Subscribe(OnGamePlayStatusChanged).AddTo(disposables);

            gamePlayMenuBarView.ReplayRequested.Subscribe(_ => gameStore.Ask()).AddTo(disposables);
            gamePlayMenuBarView.NewGameRequested.Subscribe(_ => gameStore.Exit()).AddTo(disposables);
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
