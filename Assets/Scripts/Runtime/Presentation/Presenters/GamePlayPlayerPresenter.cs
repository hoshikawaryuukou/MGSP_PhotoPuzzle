using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GamePlayPlayerPresenter : IInitializable, IDisposable
    {
        private readonly GamePlayStore gamePlayStore;
        private readonly PuzzleBoardView boardView;

        private readonly CompositeDisposable disposables = new();

        public GamePlayPlayerPresenter(GamePlayStore gamePlayStore, PuzzleBoardView boardView)
        {
            this.gamePlayStore = gamePlayStore;
            this.boardView = boardView;
        }

        void IInitializable.Initialize()
        {
            boardView.PieceSelected
                .Subscribe(index => gamePlayStore.Pick(index).Forget())
                .AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
