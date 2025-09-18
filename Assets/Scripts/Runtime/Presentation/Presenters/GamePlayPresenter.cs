using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class GamePlayPresenter : IInitializable, IDisposable
    {
        private readonly AppStore appStore;
        private readonly OptionStore optionStore;
        private readonly PhotoStore photoStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly GamePlayMenuView gamePlayMenuView;
        private readonly PuzzleBoardView boardView;
        private readonly PreviewView previewView;

        private readonly CompositeDisposable disposables = new();

        public GamePlayPresenter(AppStore appStore, OptionStore optionStore, PhotoStore photoStore, GamePlayStore gamePlayStore, GamePlayMenuView gamePlayMenuView, PuzzleBoardView boardView, PreviewView previewView)
        {
            this.appStore = appStore;
            this.optionStore = optionStore;
            this.photoStore = photoStore;
            this.gamePlayStore = gamePlayStore;
            this.gamePlayMenuView = gamePlayMenuView;
            this.boardView = boardView;
            this.previewView = previewView;
        }

        void IInitializable.Initialize()
        {
            appStore.StatusRP
                .Select(status => status == AppStatus.GamePlay)
                .DistinctUntilChanged()
                .Subscribe(isActive =>
                {
                    SetActive(isActive);

                    var width = optionStore.WidthRP.CurrentValue;
                    var height = optionStore.HeightRP.CurrentValue;
                    var photoTex = photoStore.PhotoTexRP.CurrentValue;
                    if (isActive && photoTex != null)
                    {
                        gamePlayStore.StartNewGame(width, height, photoTex, default).Forget();
                    }
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
                .Subscribe(_ =>
                {
                    var width = optionStore.WidthRP.CurrentValue;
                    var height = optionStore.HeightRP.CurrentValue;
                    var photoTex = photoStore.PhotoTexRP.CurrentValue;
                    if (photoTex != null)
                    {
                        gamePlayStore.StartNewGame(width, height, photoTex, default).Forget();
                    }
                })
                .AddTo(disposables);

            gamePlayMenuView.NewGameRequested
                .Subscribe(_ => appStore.SetStatus(AppStatus.Gallery))
                .AddTo(disposables);

            gamePlayMenuView.PreviewRequested
                .Subscribe(_ => gamePlayStore.TogglePreview())
                .AddTo(disposables);

            gamePlayMenuView.ClearRequested
                .Subscribe(_ => { gamePlayMenuView.SetCompletedConfirmed(); })
                .AddTo(disposables);

            boardView.PieceSelected
                .Subscribe(index => gamePlayStore.Pick(index).Forget())
                .AddTo(disposables);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private void SetActive(bool isActive)
        {
            boardView.gameObject.SetActive(isActive);
            gamePlayMenuView.gameObject.SetActive(isActive);
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
    }
}
