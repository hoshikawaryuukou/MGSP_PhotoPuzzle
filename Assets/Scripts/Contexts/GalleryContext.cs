using Modules.Gallery.Presenters;
using Modules.Gallery.Views;
using Modules.Shared.Infrastructures;
using UniGears.LoremPicsum;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Contexts
{
    public sealed class GalleryContext : LifetimeScope
    {
        [SerializeField] private NetworkHandler networkHandler;
        [SerializeField] private GallerySheet gallerySheet;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(networkHandler);
            builder.Register<LoremPicsumImageProvider>(Lifetime.Singleton);

            builder.RegisterComponent(gallerySheet);

            builder.RegisterEntryPoint<GalleryPresenter>(Lifetime.Singleton);
        }
    }
}
