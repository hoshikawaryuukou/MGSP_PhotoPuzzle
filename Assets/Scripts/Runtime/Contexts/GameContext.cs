using Alchemy.Inspector;
using MessagePipe;
using R3;
using Runtime.Presenters;
using Runtime.Stores;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class GameContext : LifetimeScope
    {
        [SerializeField] private List<string> subModulelist;

        [Title("Debug")]
        [SerializeField] private bool _isDebug;
        [SerializeField] private Texture2D photo;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_isDebug)
            {
                Debug.LogWarning("GameContext is in debug mode.");
                builder.Register<GameOptionStore>(Lifetime.Singleton);
                builder.Register<GameStore>(Lifetime.Singleton);
                builder.RegisterBuildCallback(container =>
                {
                    container.Resolve<GameStore>().SetPhoto(photo);
                });
            }

            builder.RegisterInstance(subModulelist);
            builder.Register<GamePlayStore>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton);
        }
    }
}
