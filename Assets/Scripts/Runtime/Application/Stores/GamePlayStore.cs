using MGSP.PhotoPuzzle.Domain;
using R3;

namespace MGSP.PhotoPuzzle.Application.Stores
{
    public enum GamePlayStatus { Preparing, Playing, Completed }

    public sealed class GamePlayStore
    {
        internal readonly ReactiveProperty<GamePlayStatus> statusRP = new(GamePlayStatus.Preparing);
        internal readonly ReactiveProperty<bool> hintOnRP = new(false);
        internal int firstPickedIndex = -1;
        internal Game game = null;

        public ReadOnlyReactiveProperty<GamePlayStatus> StatusRP => statusRP;
        public ReadOnlyReactiveProperty<bool> HintOnRP => hintOnRP;
    }
}
