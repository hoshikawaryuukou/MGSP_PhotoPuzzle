using MGSP.PhotoPuzzle.Application.Stores;
using VContainer;

namespace MGSP.PhotoPuzzle.Application.UseCases
{
    public sealed class ToggleHint
    {
        private readonly GamePlayStore gamePlayStore;

        [Inject]
        public ToggleHint(GamePlayStore gamePlayStore)
        {
            this.gamePlayStore = gamePlayStore;
        }

        public void Execute()
        {
            gamePlayStore.hintOnRP.Value = !gamePlayStore.hintOnRP.Value;
        }
    }
}
