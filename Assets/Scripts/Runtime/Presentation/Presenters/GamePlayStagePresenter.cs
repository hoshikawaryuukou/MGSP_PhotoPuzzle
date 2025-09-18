using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using MGSP.PhotoPuzzle.Services.Events;
using R3;
using System;
using System.Threading;
using UnityEngine;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GamePlayStagePresenter : IInitializable, IDisposable
    {
        private readonly PhotoStore photoStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly IAsyncSubscriber<GameStartedEvent> gameStartSubscriber;
        private readonly IAsyncSubscriber<CellSwappedEvent> cellSwappedSubscriber;
        private readonly IAsyncSubscriber<GameEndedEvent> gameEndedSubscriber;
        private readonly PuzzleBoardView boardView;

        private readonly CompositeDisposable disposables = new();

        public GamePlayStagePresenter(PhotoStore photoStore, GamePlayStore gamePlayStore, IAsyncSubscriber<GameStartedEvent> gameStartSubscriber, IAsyncSubscriber<CellSwappedEvent> cellSwappedSubscriber, IAsyncSubscriber<GameEndedEvent> gameEndedSubscriber, PuzzleBoardView boardView)
        {
            this.photoStore = photoStore;
            this.gamePlayStore = gamePlayStore;
            this.gameStartSubscriber = gameStartSubscriber;
            this.cellSwappedSubscriber = cellSwappedSubscriber;
            this.gameEndedSubscriber = gameEndedSubscriber;
            this.boardView = boardView;
        }

        void IInitializable.Initialize()
        {
            gameStartSubscriber.Subscribe(OnGameStarted).AddTo(disposables);
            cellSwappedSubscriber.Subscribe(OnCellSwapped).AddTo(disposables);
            gameEndedSubscriber.Subscribe(OnGameEnded).AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private async UniTask OnGameStarted(GameStartedEvent evt, CancellationToken cancellationToken)
        {
            Debug.Log($"Game Started: {evt.Width}x{evt.Height}");

            var photoTex = photoStore.PhotoTexRP.CurrentValue;
            if (photoTex != null && boardView != null)
            {
                await boardView.PreparePieces(evt.Width, evt.Height);
                for (int i = 0; i < evt.InitialIndices.Count; i++)
                {
                    boardView.SetPieceImage(i, gamePlayStore.PieceTextures[evt.InitialIndices[i]]);
                }
            }
            else
            {
                Debug.LogWarning("Photo texture or BoardView is null");
            }
        }

        private async UniTask OnCellSwapped(CellSwappedEvent evt, CancellationToken cancellationToken)
        {
            Debug.Log($"Cell Swapped: {evt.Index1} <-> {evt.Index2}");

            boardView.SwapPieces(evt.Index1, evt.Index2);

            await UniTask.Yield(cancellationToken);
        }

        private async UniTask OnGameEnded(GameEndedEvent evt, CancellationToken cancellationToken)
        {
            Debug.Log("Game Ended");
            await UniTask.Yield(cancellationToken);
        }
    }
}
