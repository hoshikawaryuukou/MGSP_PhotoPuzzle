using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Application.UseCases;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presenters
{
    public sealed class GamePlayPlayerPresenter : IInitializable, IDisposable
    {
        private readonly GamePlayStore gamePlayStore;
        private readonly PickPiece pickPiece;
        private readonly PuzzleBoardView boardView;

        private readonly CompositeDisposable disposables = new();

        [Inject]
        public GamePlayPlayerPresenter(GamePlayStore gamePlayStore, PickPiece pickPiece, PuzzleBoardView boardView)
        {
            this.gamePlayStore = gamePlayStore;
            this.pickPiece = pickPiece;
            this.boardView = boardView;
        }

        void IInitializable.Initialize()
        {
            boardView.PieceSelected
                .Where(_ => gamePlayStore.StatusRP.CurrentValue == GamePlayStatus.Playing)
                .Subscribe(index => pickPiece.Execute(index).Forget())
                .AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
