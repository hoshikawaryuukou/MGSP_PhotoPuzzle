using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Application.UseCases;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using System.Threading;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presenters
{
    public sealed class GamePlaySheetPresenter : IInitializable, IDisposable
    {
        private readonly CreateGame createGame;
        private readonly ToggleHint toggleHint;
        private readonly ISubscriber<GameRequested> gameRequestedSubscriber;
        private readonly IPublisher<PhotoRequested> photoRequestedPublisher;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlaySheet gamePlaySheet;
        private readonly PuzzleBoardView boardView;
        private readonly PreviewView previewView;
        private readonly GameSetupModalPresenter gameSetupModalPresenter;

        private readonly CompositeDisposable disposables = new();

        [Inject]
        public GamePlaySheetPresenter(CreateGame createGame, ToggleHint toggleHint, ISubscriber<GameRequested> gameRequestedSubscriber, IPublisher<PhotoRequested> photoRequestedPublisher, GamePlayStore gamePlayStore, GamePlaySheet gamePlaySheet, PuzzleBoardView boardView, PreviewView previewView, GameSetupModalPresenter gameSetupModalPresenter)
        {
            this.createGame = createGame;
            this.toggleHint = toggleHint;
            this.gameRequestedSubscriber = gameRequestedSubscriber;
            this.photoRequestedPublisher = photoRequestedPublisher;
            this.gamePlayStore = gamePlayStore;
            this.gamePlaySheet = gamePlaySheet;
            this.boardView = boardView;
            this.previewView = previewView;
            this.gameSetupModalPresenter = gameSetupModalPresenter;
        }

        void IInitializable.Initialize()
        {
            gameRequestedSubscriber
                .Subscribe(_ =>
                {
                    boardView.gameObject.SetActive(true);
                    gamePlaySheet.gameObject.SetActive(true);
                    createGame.Execute(default).Forget();
                })
                .AddTo(disposables);

            gamePlayStore.StatusRP
                .Subscribe(OnStatusChanged)
                .AddTo(disposables);

            gamePlayStore.HintOnRP
                .Subscribe(value => 
                { 
                    gamePlaySheet.SetPreviewOn(value); 
                    previewView.gameObject.SetActive(value); 
                })
                .AddTo(disposables);

            gamePlaySheet.ReplayRequested
                .Subscribe(_ => Replay(default).Forget())
                .AddTo(disposables);

            gamePlaySheet.NewGameRequested
                .Subscribe(_ =>
                {
                    boardView.gameObject.SetActive(false);
                    gamePlaySheet.gameObject.SetActive(false);
                    photoRequestedPublisher.Publish(default);
                })
                .AddTo(disposables);

            gamePlaySheet.PreviewRequested
                .Subscribe(_ => toggleHint.Execute())
                .AddTo(disposables);

            gamePlaySheet.ClearRequested
                .Subscribe(_ => { gamePlaySheet.SetCompletedConfirmed(); })
                .AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private void OnStatusChanged(GamePlayStatus status)
        {
            switch (status)
            {
                case GamePlayStatus.Preparing: gamePlaySheet.SetPreparing(); break;
                case GamePlayStatus.Playing: gamePlaySheet.SetPlaying(); break;
                case GamePlayStatus.Completed: gamePlaySheet.SetCompleted(); break;
                default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private async UniTask Replay( CancellationToken cancellationToken)
        {
            var result = await gameSetupModalPresenter.Show(cancellationToken);
            if (result == GameSetupModalResult.Confirmed)
            {
                createGame.Execute(default).Forget();
            }
        }
    }
}
