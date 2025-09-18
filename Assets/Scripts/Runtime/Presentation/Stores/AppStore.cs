using R3;

namespace MGSP.PhotoPuzzle.Presentation.Stores
{
    public enum AppStatus { None, Gallery, GamePlay }

    public sealed class AppStore
    {
        private readonly ReactiveProperty<AppStatus> statusRP = new(AppStatus.None);

        public ReadOnlyReactiveProperty<AppStatus> StatusRP => statusRP;

        public void SetStatus(AppStatus status) => statusRP.Value = status;
    }
}
