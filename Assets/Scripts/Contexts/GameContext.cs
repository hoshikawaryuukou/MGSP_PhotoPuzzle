using Modules.Game.Presenters;
using Modules.Game.Stores;
using Modules.Game.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Contexts
{
    public sealed class GameContext : LifetimeScope
    {
        [SerializeField] private GamePlayPreviewView gamePlayPreviewView;
        [SerializeField] private GamePlayMenuSheet gamePlayMenuBarView;
        [SerializeField] private GameOptionModal gameOptionScreen;
        [SerializeField] private GamePlayPuzzleBoardView gamePlayPuzzleBoardView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameOptionStore>(Lifetime.Singleton);
            builder.Register<GamePlayStore>(Lifetime.Singleton);

            builder.RegisterComponent(gamePlayPreviewView);
            builder.RegisterComponent(gameOptionScreen);
            builder.RegisterComponent(gamePlayMenuBarView);
            builder.RegisterComponent(gamePlayPuzzleBoardView);

            builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePreviewPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameMenuPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameOptionPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayPresenter>(Lifetime.Singleton);
        }
    }
}
