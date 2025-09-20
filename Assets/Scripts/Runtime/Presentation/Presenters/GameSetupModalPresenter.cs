using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using R3;
using System.Linq;
using System.Threading;

namespace MGSP.PhotoPuzzle.Presentation.Presenters

{ public enum GameSetupModalResult { Confirmed, Close }

    public sealed class GameSetupModalPresenter
    {
        private readonly OptionStore optionStore;
        private readonly OptionView optionView;

        public GameSetupModalPresenter(OptionStore optionStore, OptionView optionView)
        {
            this.optionStore = optionStore;
            this.optionView = optionView;
        }

        public async UniTask<GameSetupModalResult> Show(CancellationToken cancellationToken)
        {
            var completionSource = new UniTaskCompletionSource<GameSetupModalResult>();
            
            // Set up the view before showing
            SetupView();
            
            // Show the modal
            optionView.gameObject.SetActive(true);
            
            // Subscribe to user interactions
            var closeSubscription = optionView.CloseRequested
                .Subscribe(_ =>
                {
                    optionView.gameObject.SetActive(false);
                    completionSource.TrySetResult(GameSetupModalResult.Close);
                });
                
            var playSubscription = optionView.PlayRequested
                .Subscribe(_ =>
                {
                    UpdateStoreFromView();
                    optionView.gameObject.SetActive(false);
                    completionSource.TrySetResult(GameSetupModalResult.Confirmed);
                });
            
            // Handle cancellation
            var cancellationRegistration = cancellationToken.Register(() =>
            {
                optionView.gameObject.SetActive(false);
                completionSource.TrySetCanceled();
            });
            
            try
            {
                var result = await completionSource.Task;
                return result;
            }
            finally
            {
                // Clean up subscriptions
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

            optionView.SetWidthInfo(widthLabels, currentWidthIndex);
            optionView.SetHeightInfo(heightLabels, currentHeightIndex);
        }

        private void UpdateStoreFromView()
        {
            var selectedWidth = optionStore.WidthOptions[optionView.SelectedWidthIndex];
            var selectedHeight = optionStore.HeightOptions[optionView.SelectedHeightIndex];

            optionStore.SetWidth(selectedWidth);
            optionStore.SetHeight(selectedHeight);
        }
    }
}
