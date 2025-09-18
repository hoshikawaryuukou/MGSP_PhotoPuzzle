using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;

namespace MGSP.PhotoPuzzle.Services.Events
{
    public sealed class GameEventEmitter
    {
        private readonly IAsyncPublisher<GameStartedEvent> gameStartedPub;
        private readonly IAsyncPublisher<CellSwappedEvent> cellSwappedPub;
        private readonly IAsyncPublisher<GameEndedEvent> gameEndedPub;

        public GameEventEmitter(IAsyncPublisher<GameStartedEvent> gameStartedPub, IAsyncPublisher<CellSwappedEvent> cellSwappedPub, IAsyncPublisher<GameEndedEvent> gameEndedPub)
        {
            this.gameStartedPub = gameStartedPub;
            this.cellSwappedPub = cellSwappedPub;
            this.gameEndedPub = gameEndedPub;
        }

        public async UniTask Process(IReadOnlyList<IGameEvent> evts, CancellationToken cancellationToken)
        {
            for (int i = 0; i < evts.Count; i++)
            {
                var evt = evts[i];
                switch (evt)
                {
                    case GameStartedEvent e: await gameStartedPub.PublishAsync(e, cancellationToken); break;
                    case CellSwappedEvent e: await cellSwappedPub.PublishAsync(e, cancellationToken); break;
                    case GameEndedEvent e: await gameEndedPub.PublishAsync(e, cancellationToken); break;
                }

                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
