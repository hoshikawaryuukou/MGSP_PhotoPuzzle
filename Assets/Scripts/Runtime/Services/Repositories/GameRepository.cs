using MGSP.PhotoPuzzle.Domain;

namespace MGSP.PhotoPuzzle.Services.Repositories
{
    public sealed class GameRepository
    {
        private Game game;

        public void SetInstance(Game value)
        {
            game = value;
        }

        public Game GetInstance()
        {
            return game;
        }
    }
}
