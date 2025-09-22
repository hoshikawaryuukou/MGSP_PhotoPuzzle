using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using MGSP.PhotoPuzzle.Services.Events;
using R3;
using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GamePlayDisplayPresenter : IInitializable, IDisposable
    {
        private readonly IAsyncSubscriber<GameStartedEvent> gameStartSubscriber;
        private readonly IAsyncSubscriber<CellSwappedEvent> cellSwappedSubscriber;
        private readonly IAsyncSubscriber<GameEndedEvent> gameEndedSubscriber;
        private readonly PhotoStore photoStore;
        private readonly PuzzleBoardView boardView;

        private readonly CompositeDisposable disposables = new();

        [Inject]
        public GamePlayDisplayPresenter(IAsyncSubscriber<GameStartedEvent> gameStartSubscriber, IAsyncSubscriber<CellSwappedEvent> cellSwappedSubscriber, IAsyncSubscriber<GameEndedEvent> gameEndedSubscriber, PhotoStore photoStore, PuzzleBoardView boardView)
        {
            this.gameStartSubscriber = gameStartSubscriber;
            this.cellSwappedSubscriber = cellSwappedSubscriber;
            this.gameEndedSubscriber = gameEndedSubscriber;
            this.photoStore = photoStore;
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
                await boardView.PreparePieces(evt.Width, evt.Height, photoTex, evt.InitialIndices);
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
