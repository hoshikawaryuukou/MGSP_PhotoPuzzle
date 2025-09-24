using MessagePipe;
using MGSP.PhotoPuzzle.Presentation.Routes;
using R3;
using System;
using VContainer;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Presenters
{
    public sealed class AppPresenter : IStartable, IDisposable
    {
        private readonly IPublisher<PhotoRequested> photoRequestedPublisher;

        [Inject]
        public AppPresenter(IPublisher<PhotoRequested> photoRequestedPublisher)
        {
            this.photoRequestedPublisher = photoRequestedPublisher;
        }

        private readonly CompositeDisposable disposables = new();

        void IStartable.Start()
        {
            photoRequestedPublisher.Publish(default);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
