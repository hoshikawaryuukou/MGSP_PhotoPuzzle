using Runtime.Infrastructures;
using Runtime.Presenters;
using Runtime.Stores;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class AppContext : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoremPicsumImageProvider>(Lifetime.Singleton);

            builder.Register<GalleryStore>(Lifetime.Singleton);
            builder.Register<GameOptionStore>(Lifetime.Singleton);
            builder.Register<GameStore>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>(Lifetime.Singleton);
        }
    }
}
