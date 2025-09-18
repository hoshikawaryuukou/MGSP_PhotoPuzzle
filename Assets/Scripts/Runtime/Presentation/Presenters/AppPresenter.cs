using MGSP.PhotoPuzzle.Presentation.Stores;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class AppPresenter : IPostStartable
    {
        private readonly AppStore appStore;

        public AppPresenter(AppStore appStore)
        {
            this.appStore = appStore;
        }

        void IPostStartable.PostStart()
        {
            appStore.SetStatus(AppStatus.Gallery);
        }
    }
}
