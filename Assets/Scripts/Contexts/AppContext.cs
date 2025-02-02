using Modules.App.Presenters;
using Modules.Shared.Infrastructures;
using Modules.Shared.Stores;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Contexts
{
    public sealed class AppContext : LifetimeScope
    {
        [SerializeField] private AssetMode assetMode;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(new AssetLoader(assetMode));
            builder.Register<ContextNavigator>(Lifetime.Singleton);

            builder.Register<PhotoStore>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppPresenter>(Lifetime.Singleton);
        }
    }
}
