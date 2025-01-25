using Runtime.Presenters;
using Runtime.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class GameMenuContext : LifetimeScope
    {
        [SerializeField] private GamePlayMenuSheet gamePlayMenuBarView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gamePlayMenuBarView);
            builder.RegisterEntryPoint<GameMenuPresenter>(Lifetime.Singleton);
        }
    }
}
