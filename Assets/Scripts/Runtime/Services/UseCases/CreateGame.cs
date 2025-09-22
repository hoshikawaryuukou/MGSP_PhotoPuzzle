using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Domain;
using MGSP.PhotoPuzzle.Services.Events;
using MGSP.PhotoPuzzle.Services.Repositories;
using System.Collections.Generic;
using System.Threading;
using VContainer;

namespace MGSP.PhotoPuzzle.Services.UseCases
{
    public sealed class CreateGame
    {
        private readonly GameRepository gameRepository;

        [Inject]
        public CreateGame(GameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public UniTask<IReadOnlyList<IGameEvent>> Execute(int width, int height, CancellationToken cancellation = default)
        {
            var game = new Game(width, height);

            gameRepository.SetInstance(game);

            return UniTask.FromResult<IReadOnlyList<IGameEvent>>(new IGameEvent[]
            {
                new GameStartedEvent(width, height, game.GetGrid())
            });
        }
    }
}
