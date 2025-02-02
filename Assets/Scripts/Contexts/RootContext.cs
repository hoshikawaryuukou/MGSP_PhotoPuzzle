using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Contexts
{
    public sealed class RootContext : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
        }
    }
}
