using MessagePipe;
using Runtime.Infrastructures;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class RootContext : LifetimeScope
    {
        [SerializeField] private NetworkHandler networkHandler;
        [SerializeField] private ContextTable contextTable;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();

            builder.RegisterInstance(networkHandler);
            builder.RegisterInstance(new ContextNavigator(contextTable));
        }
    }
}
