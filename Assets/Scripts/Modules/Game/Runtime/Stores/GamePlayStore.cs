using Modules.Game.Domain;
using R3;
using System.Collections.Generic;

namespace Modules.Game.Stores
{
    public enum GamePlayStatus
    {
        None,
        Playing,
        Completed
    }

    public sealed class GamePlayStore
    {
        private readonly ReactiveProperty<GamePlayStatus> playStatusRP = new(GamePlayStatus.None);
        private PuzzleGame game = null;

        public ReadOnlyReactiveProperty<GamePlayStatus> PlayStatusRP => playStatusRP;

        public void CreateGame(int size)
        {
            game = PuzzleGame.CreateFromSize(size);
            playStatusRP.Value = GamePlayStatus.Playing;
        }

        public List<int> GetPieces()
        {
            if (playStatusRP.CurrentValue != GamePlayStatus.Playing)
            {
                throw new System.InvalidOperationException("Game is not playing.");
            }

            return new List<int>(game.GetPieces());
        }

        public void SwapPieces(int index0, int index1)
        {
            if (playStatusRP.CurrentValue != GamePlayStatus.Playing)
            {
                return;
            }

            game.Swap(index0, index1);
            if (game.IsCompleted())
            {
                playStatusRP.Value = GamePlayStatus.Completed;
                game = null;
            }
        }
    }
}
