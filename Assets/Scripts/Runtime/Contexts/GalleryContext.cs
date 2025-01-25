using Runtime.Presenters;
using Runtime.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Runtime.Contexts
{
    public sealed class GalleryContext : LifetimeScope
    {
        [SerializeField] private GallerySheet gallerySheet;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(gallerySheet);
            builder.RegisterEntryPoint<GalleryPresenter>(Lifetime.Singleton);
        }
    }
}
