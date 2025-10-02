using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System.Linq;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Presenters
{
    public enum GameSetupModalResult { Confirmed, Closed }

    public sealed class GameSetupModalPresenter
    {
        private readonly OptionStore optionStore;
        private readonly OptionModal optionModal;

        [Inject]
        public GameSetupModalPresenter(OptionStore optionStore, OptionModal optionModal)
        {
            this.optionStore = optionStore;
            this.optionModal = optionModal;
        }

        public async UniTask<GameSetupModalResult> Show(CancellationToken cancellationToken)
        {
            var completionSource = new UniTaskCompletionSource<GameSetupModalResult>();

            SetupView();

            optionModal.gameObject.SetActive(true);

            var closeSubscription = optionModal.CloseRequested
                .Subscribe(_ =>
                {
                    optionModal.gameObject.SetActive(false);
                    completionSource.TrySetResult(GameSetupModalResult.Closed);
                });

            var playSubscription = optionModal.PlayRequested
                .Subscribe(_ =>
                {
                    ConfirmValues();
                    optionModal.gameObject.SetActive(false);
                    completionSource.TrySetResult(GameSetupModalResult.Confirmed);
                });

            var cancellationRegistration = cancellationToken.Register(() =>
            {
                optionModal.gameObject.SetActive(false);
                completionSource.TrySetCanceled();
            });

            try
            {
                var result = await completionSource.Task;
                return result;
            }
            finally
            {
                closeSubscription?.Dispose();
                playSubscription?.Dispose();
                cancellationRegistration.Dispose();
            }
        }

        private void SetupView()
        {
            var widthLabels = optionStore.WidthOptions.Select(x => x.ToString()).ToArray();
            var heightLabels = optionStore.HeightOptions.Select(x => x.ToString()).ToArray();

            var currentWidthIndex = optionStore.WidthOptions.ToList().IndexOf(optionStore.WidthRP.CurrentValue);
            var currentHeightIndex = optionStore.HeightOptions.ToList().IndexOf(optionStore.HeightRP.CurrentValue);

            if (currentWidthIndex == -1) currentWidthIndex = 0;
            if (currentHeightIndex == -1) currentHeightIndex = 0;

            optionModal.SetWidthInfo(widthLabels, currentWidthIndex);
            optionModal.SetHeightInfo(heightLabels, currentHeightIndex);
        }

        private void ConfirmValues()
        {
            var selectedWidth = optionStore.WidthOptions[optionModal.SelectedWidthIndex];
            var selectedHeight = optionStore.HeightOptions[optionModal.SelectedHeightIndex];

            optionStore.SetWidth(selectedWidth);
            optionStore.SetHeight(selectedHeight);
        }
    }
}
