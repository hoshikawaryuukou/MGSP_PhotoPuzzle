using Runtime.Presenters;
using Runtime.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class GamePlayContext : LifetimeScope
    {
        [SerializeField] private GamePlayPreviewView gamePlayPreviewView;
        [SerializeField] private GamePlayPuzzleBoardView gamePlayPuzzleBoardView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gamePlayPreviewView);
            builder.RegisterComponent(gamePlayPuzzleBoardView);

            builder.RegisterEntryPoint<GamePlayPresenter>(Lifetime.Singleton);
        }
    }
}
