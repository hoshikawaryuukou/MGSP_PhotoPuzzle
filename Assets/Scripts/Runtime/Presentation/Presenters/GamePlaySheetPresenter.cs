using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using System.Threading;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GamePlaySheetPresenter : IInitializable, IDisposable
    {
        private readonly ISubscriber<GameRequested> gameRequestedSubscriber;
        private readonly IPublisher<PhotoRequested> photoRequestedPublisher;
        private readonly OptionStore optionStore;
        private readonly PhotoStore photoStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlaySheet gamePlayMenuView;
        private readonly PuzzleBoardView boardView;
        private readonly PreviewView previewView;
        private readonly GameSetupModalPresenter gameSetupModalPresenter;

        private readonly CompositeDisposable disposables = new();

        [Inject]
        public GamePlaySheetPresenter(ISubscriber<GameRequested> gameRequestedSubscriber, IPublisher<PhotoRequested> photoRequestedPublisher, OptionStore optionStore, PhotoStore photoStore, GamePlayStore gamePlayStore, GamePlaySheet gamePlayMenuView, PuzzleBoardView boardView, PreviewView previewView, GameSetupModalPresenter gameSetupModalPresenter)
        {
            this.gameRequestedSubscriber = gameRequestedSubscriber;
            this.photoRequestedPublisher = photoRequestedPublisher;
            this.optionStore = optionStore;
            this.photoStore = photoStore;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayMenuView = gamePlayMenuView;
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
                    gamePlayMenuView.gameObject.SetActive(true);
                    StartNewGame();
                })
                .AddTo(disposables);

            gamePlayStore.StatusRP
                .Subscribe(OnStatusChanged)
                .AddTo(disposables);

            gamePlayStore.PreviewOnRP
                .Subscribe(value =>
                {
                    gamePlayMenuView.SetPreviewOn(value);
                    previewView.gameObject.SetActive(value);
                })
                .AddTo(disposables);

            gamePlayMenuView.ReplayRequested
                .Subscribe(_ => Replay(default).Forget())
                .AddTo(disposables);

            gamePlayMenuView.NewGameRequested
                .Subscribe(_ => 
                {
                    boardView.gameObject.SetActive(false);
                    gamePlayMenuView.gameObject.SetActive(false);
                    photoRequestedPublisher.Publish(default); 
                })
                .AddTo(disposables);

            gamePlayMenuView.PreviewRequested
                .Subscribe(_ => gamePlayStore.TogglePreview())
                .AddTo(disposables);

            gamePlayMenuView.ClearRequested
                .Subscribe(_ => { gamePlayMenuView.SetCompletedConfirmed(); })
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
                case GamePlayStatus.Preparing:
                    gamePlayMenuView.SetPlaying();
                    break;
                case GamePlayStatus.Playing:
                    gamePlayMenuView.SetPlaying();
                    break;
                case GamePlayStatus.Completed:
                    gamePlayMenuView.SetCompleted();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void StartNewGame()
        {
            var width = optionStore.WidthRP.CurrentValue;
            var height = optionStore.HeightRP.CurrentValue;
            var photoTex = photoStore.PhotoTexRP.CurrentValue;
            if (photoTex != null)
            {
                gamePlayStore.StartNewGame(width, height, photoTex, default).Forget();
            }
        }

        private async UniTask Replay( CancellationToken cancellationToken)
        {
            var result = await gameSetupModalPresenter.Show(cancellationToken);
            if (result == GameSetupModalResult.Confirmed)
            {
                StartNewGame();
            }
        }
    }
}
