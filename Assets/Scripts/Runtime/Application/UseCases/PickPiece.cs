using Cysharp.Threading.Tasks;
using MessagePipe;
using MGSP.PhotoPuzzle.Application.Events;
using MGSP.PhotoPuzzle.Application.Stores;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Application.UseCases
{
    public sealed class PickPiece
    {
        private readonly GamePlayStore gamePlayStore;
        private readonly IAsyncPublisher<CellSwappedEvent> cellSwappedPub;
        private readonly IAsyncPublisher<GameEndedEvent> gameEndedPub;

        [Inject]
        public PickPiece(GamePlayStore gamePlayStore, IAsyncPublisher<CellSwappedEvent> cellSwappedPub, IAsyncPublisher<GameEndedEvent> gameEndedPub)
        {
            this.gamePlayStore = gamePlayStore;
            this.cellSwappedPub = cellSwappedPub;
            this.gameEndedPub = gameEndedPub;
        }

        public async UniTask Execute(int index, CancellationToken cancellationToken = default)
        {
            if (gamePlayStore.firstPickedIndex == -1)
            {
                gamePlayStore.firstPickedIndex = index;
                return;
            }

            var index1 = gamePlayStore.firstPickedIndex;
            var index2 = index;
            gamePlayStore.firstPickedIndex = -1;

            var game = gamePlayStore.game;

            game.Swap(index1, index2);
            await cellSwappedPub.PublishAsync(new CellSwappedEvent(index1, index2), cancellationToken);

            if (game.CheckWin())
            {
                await gameEndedPub.PublishAsync(new GameEndedEvent(), cancellationToken);
                gamePlayStore.statusRP.Value = GamePlayStatus.Completed;
            }
        }
    }
}
