using MessagePipe;
using MGSP.PhotoPuzzle.Infrastructures;
using MGSP.PhotoPuzzle.Presentation.Presenters;
using MGSP.PhotoPuzzle.Presentation.Stores;
using MGSP.PhotoPuzzle.Presentation.Views;
using MGSP.PhotoPuzzle.Services.Events;
using MGSP.PhotoPuzzle.Services.Repositories;
using MGSP.PhotoPuzzle.Services.UseCases;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Contexts
{
    public sealed class AppContext : LifetimeScope
    {
        override protected void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();

            builder.RegisterInstance(new GameRepository());

            builder.Register<CreateGame>(Lifetime.Singleton);
            builder.Register<SwapPieces>(Lifetime.Singleton);

            builder.Register<GameEventEmitter>(Lifetime.Singleton);

            builder.RegisterInstance(new AppStore());
            builder.Register<PhotoStore>(Lifetime.Singleton);
            builder.RegisterInstance(new OptionStore());
            builder.Register<GamePlayStore>(Lifetime.Singleton);

            builder.RegisterInstance(new LoremPicsumImageProvider());

            builder.RegisterComponentInHierarchy<GalleryMenuBar>();
            builder.RegisterComponentInHierarchy<GamePlayMenuView>();
            builder.RegisterComponentInHierarchy<PreviewView>();
            builder.RegisterComponentInHierarchy<PuzzleBoardView>();
            builder.RegisterComponentInHierarchy<OptionView>();

            builder.RegisterEntryPoint<AppPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GalleryPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayStagePresenter>(Lifetime.Singleton);
        }
    }
}
