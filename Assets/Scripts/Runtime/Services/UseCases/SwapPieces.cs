using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Services.Events;
using MGSP.PhotoPuzzle.Services.Repositories;
using System.Collections.Generic;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Services.UseCases
{
    public sealed class SwapPieces
    {
        private readonly GameRepository gameRepository;

        [Inject]
        public SwapPieces(GameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public UniTask<IReadOnlyList<IGameEvent>> Execute(int index1, int index2, CancellationToken cancellationToken)
        {
            var game = gameRepository.GetInstance(); 

            var events = new List<IGameEvent>();

            game.Swap(index1, index2);
            events.Add(new CellSwappedEvent(index1, index2));

            if (game.CheckWin())
            {
                events.Add(new GameEndedEvent());
            }

            return UniTask.FromResult<IReadOnlyList<IGameEvent>>(events);
        }
    }
}
