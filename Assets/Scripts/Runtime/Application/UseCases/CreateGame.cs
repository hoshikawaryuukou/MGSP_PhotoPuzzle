using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Events;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Domain;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Application.UseCases
{
    public sealed class CreateGame
    {
        private readonly PhotoStore photoStore;
        private readonly OptionStore optionStore;
        private readonly GamePlayStore gamePlayStore;
        private readonly IAsyncPublisher<GameStartedEvent> gameStartedPub;

        [Inject]
        public CreateGame(PhotoStore photoStore, OptionStore optionStore, GamePlayStore gamePlayStore, IAsyncPublisher<GameStartedEvent> gameStartedPub)
        {
            this.photoStore = photoStore;
            this.optionStore = optionStore;
            this.gamePlayStore = gamePlayStore;
            this.gameStartedPub = gameStartedPub;
        }

        public async UniTask Execute(CancellationToken cancellationToken = default)
        {
            if (photoStore.TextureRP.CurrentValue == null)
            {
                // No photo selected, cannot create game
                return;
            }

            gamePlayStore.hintOnRP.Value = false;
            gamePlayStore.statusRP.Value = GamePlayStatus.Preparing;

            await UniTask.Yield(cancellationToken);

            var width = optionStore.WidthRP.CurrentValue;
            var height = optionStore.HeightRP.CurrentValue;
            var game = new Game(width, height);

            gamePlayStore.game = game;
            await gameStartedPub.PublishAsync(new GameStartedEvent(width, height, game.GetGrid()), cancellationToken);

            gamePlayStore.statusRP.Value = GamePlayStatus.Playing;
        }
    }
}
