using Cysharp.Threading.Tasks;
using MessagePipe;
using Module.Game.Events;
using Modules.Game.Stores;
using Modules.Game.Views;
using Modules.Shared.Stores;
using R3;
using System;
using System.Threading;
using VContainer.Unity;

namespace Modules.Game.Presenters
{
    public sealed class GamePlayPresenter : IStartable, IDisposable
    {
        private readonly IScopedSubscriber<GameConfirmed> gameConfirmedSubscriber;
        private readonly PhotoStore photoStore;
        private readonly GameOptionStore gameOptionStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlayPuzzleBoardView gamePlayPuzzleBoardView;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GamePlayPresenter(IScopedSubscriber<GameConfirmed> gameConfirmedSubscriber, PhotoStore photoStore, GameOptionStore gameOptionStore, GamePlayStore gamePlayStore, GamePlayPuzzleBoardView gamePlayPuzzleBoardView)
        {
            this.gameConfirmedSubscriber = gameConfirmedSubscriber;
            this.photoStore = photoStore;
            this.gameOptionStore = gameOptionStore;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayPuzzleBoardView = gamePlayPuzzleBoardView;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            gameConfirmedSubscriber.Subscribe(_ => Play(cts.Token).Forget()).AddTo(disposables);

            gamePlayPuzzleBoardView.PiecePaired.Subscribe(pair => OnPiecePaired(pair, cts.Token).Forget()).AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }

        private async UniTask Play(CancellationToken token)
        {
            var photo = photoStore.PhotoTex;
            var rows = gameOptionStore.GetRow();
            var cols = gameOptionStore.GetColumn();

            gamePlayStore.CreateGame(rows * cols);
            var indices = gamePlayStore.GetPieces();

            var request = new GamePlayPuzzleBoardRequest(rows, cols, photo, indices);
            await gamePlayPuzzleBoardView.Create(request, token);
        }

        private async UniTask OnPiecePaired(GamePlayPuzzleBoardSelectedPair pair, CancellationToken token)
        {
            if (gamePlayStore.PlayStatusRP.CurrentValue != GamePlayStatus.Playing)
                return;

            gamePlayStore.SwapPieces(pair.index0, pair.index1);
            await gamePlayPuzzleBoardView.Swap(pair.index0, pair.index1, token);
        }
    }
}
