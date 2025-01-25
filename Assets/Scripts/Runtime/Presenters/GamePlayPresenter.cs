using Cysharp.Threading.Tasks;
using R3;
using Runtime.Stores;
using Runtime.Views;
using System;
using System.Threading;
using VContainer.Unity;

namespace Runtime.Presenters
{
    public sealed class GamePlayPresenter : IStartable, IDisposable
    {
        private readonly GameStore gameStore;
        private readonly GameOptionStore gameOptionStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlayPreviewView gamePlayPreviewView;
        private readonly GamePlayPuzzleBoardView gamePlayPuzzleBoardView;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GamePlayPresenter(GameStore gameStore, GameOptionStore gameOptionStore, GamePlayStore gamePlayStore, GamePlayPreviewView gamePlayPreviewView, GamePlayPuzzleBoardView gamePlayPuzzleBoardView)
        {
            this.gameStore = gameStore;
            this.gameOptionStore = gameOptionStore;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayPreviewView = gamePlayPreviewView;
            this.gamePlayPuzzleBoardView = gamePlayPuzzleBoardView;
        }

        void IStartable.Start()
        {
            disposables.Clear();

            gameStore.PlaySubject.Subscribe(_ => Play(cts.Token).Forget()).AddTo(disposables);

            gamePlayPreviewView.SetImage(gameStore.Photo);
            gamePlayPreviewView.Show();
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
            gamePlayPreviewView.Hide();

            var photo = gameStore.Photo;
            var rows = gameOptionStore.GetRow();
            var cols = gameOptionStore.GetColumn();

            gamePlayStore.CreateNewGame(rows * cols);
            var indices = gamePlayStore.GetPieces();

            var request = new GamePlayPuzzleBoardRequest(rows, cols, photo, indices);
            await gamePlayPuzzleBoardView.Create(request, token);
        }

        private async UniTask OnPiecePaired(GamePlayPuzzleBoardSelectedPair pair, CancellationToken token)
        {
            gamePlayStore.SwapPieces(pair.index0, pair.index1);
            await gamePlayPuzzleBoardView.Swap(pair.index0, pair.index1, token);
        }
    }
}
