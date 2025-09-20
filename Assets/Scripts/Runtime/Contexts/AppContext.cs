using MessagePipe;
using MGSP.PhotoPuzzle.Infrastructures;
using MGSP.PhotoPuzzle.Presentation.Flows;
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

            builder.Register<PhotoStore>(Lifetime.Singleton);
            builder.RegisterInstance(new OptionStore());
            builder.Register<GamePlayStore>(Lifetime.Singleton);

            builder.RegisterInstance(new LoremPicsumImageProvider());

            builder.RegisterComponentInHierarchy<GalleryMenuBar>();
            builder.RegisterComponentInHierarchy<GamePlayMenuView>();
            builder.RegisterComponentInHierarchy<PreviewView>();
            builder.RegisterComponentInHierarchy<PuzzleBoardView>();
            builder.RegisterComponentInHierarchy<OptionView>();

            builder.RegisterEntryPoint<GallerySheetPresenter>(Lifetime.Singleton);
            builder.Register<GameSetupModalPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlaySheetPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayPlayerPresenter>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GamePlayDisplayPresenter>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppFlowController>(Lifetime.Singleton);
        }
    }
}
