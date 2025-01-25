using Runtime.Presenters;
using Runtime.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class GameOptionContext : LifetimeScope
    {
        [SerializeField] private GameOptionModal gameOptionScreen;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gameOptionScreen);
            builder.RegisterEntryPoint<GameOptionPresenter>(Lifetime.Singleton);
        }
    }
}
