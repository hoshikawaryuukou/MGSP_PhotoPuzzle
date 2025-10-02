using MessagePipe;
using MGSP.PhotoPuzzle.Application.Ports;
using MGSP.PhotoPuzzle.Application.Stores;
using MGSP.PhotoPuzzle.Application.UseCases;
using MGSP.PhotoPuzzle.Infrastructures;
using MGSP.PhotoPuzzle.Presentation.Views;
using MGSP.PhotoPuzzle.Presenters;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Contexts
{
    public sealed class AppContext : LifetimeScope
    {
        override protected void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();

            builder.Register<DownloadPhoto>(Lifetime.Singleton);
            builder.Register<CreateGame>(Lifetime.Singleton);
            builder.Register<PickPiece>(Lifetime.Singleton);
            builder.Register<ToggleHint>(Lifetime.Singleton);

            builder.RegisterInstance(new PhotoStore());
            builder.RegisterInstance(new OptionStore());
            builder.RegisterInstance(new GamePlayStore());

            builder.RegisterInstance<IPhotoProvider>(new LoremPicsumPhotoProvider());

            builder.RegisterComponentInHierarchy<GallerySheet>();
            builder.RegisterComponentInHierarchy<GamePlaySheet>();
            builder.RegisterComponentInHierarchy<PreviewView>();
            builder.RegisterComponentInHierarchy<PuzzleBoardView>();
            builder.RegisterComponentInHierarchy<OptionModal>();

            builder.RegisterEntryPoint<AppPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GallerySheetPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlaySheetPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayPlayerPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayDisplayPresenter>(Lifetime.Singleton);
            builder.Register<GameSetupModalPresenter>(Lifetime.Singleton);
        }
    }
}
