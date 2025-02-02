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
    public sealed class GameOptionPresenter : IStartable, IDisposable
    {
        private readonly IScopedSubscriber<GameAsked> gameAskedSubscriber;
        private readonly IScopedPublisher<GameConfirmed> gameConfirmedPublisher;
        private readonly GameOptionStore optionStore;
        private readonly GameOptionModal gameOptionScreen;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GameOptionPresenter(IScopedSubscriber<GameAsked> gameAskedSubscriber, IScopedPublisher<GameConfirmed> gameConfirmedPublisher, GameOptionStore optionStore, GameOptionModal gameOptionScreen)
        {
            this.gameAskedSubscriber = gameAskedSubscriber;
            this.gameConfirmedPublisher = gameConfirmedPublisher;
            this.optionStore = optionStore;
            this.gameOptionScreen = gameOptionScreen;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            gameAskedSubscriber.Subscribe(_ => OnAsked()).AddTo(disposables);

            optionStore.ColumnIndexRP.Subscribe(gameOptionScreen.SetColumnIndex).AddTo(disposables);
            optionStore.RowIndexRP.Subscribe(gameOptionScreen.SetRowIndex).AddTo(disposables);

            gameOptionScreen.ColumnIndexClicked.Subscribe(optionStore.SetColumnIndex).AddTo(disposables);
            gameOptionScreen.RowIndexClicked.Subscribe(optionStore.SetRowIndex).AddTo(disposables);
            gameOptionScreen.PlayRequested.Subscribe(_ => OnPlayRequested()).AddTo(disposables);
            gameOptionScreen.CloseRequested.Subscribe(_ => gameOptionScreen.Hide()).AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        private void OnAsked()
        {
            var columnIndex = optionStore.ColumnIndexRP.CurrentValue;
            var rowIndex = optionStore.RowIndexRP.CurrentValue;
            gameOptionScreen.Show(columnIndex, rowIndex);
        }

        private void OnPlayRequested()
        {
            optionStore.SetColumnIndex(gameOptionScreen.GetColIndex());
            optionStore.SetRowIndex(gameOptionScreen.GetRowIndex());
            gameOptionScreen.Hide();
            gameConfirmedPublisher.Publish(new GameConfirmed());
        }
    }
}
