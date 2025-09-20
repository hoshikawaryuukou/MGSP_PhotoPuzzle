using MessagePipe;
using R3;
using System;
using UnityEngine;
using VContainer.Unity;

namespace MGSP.PhotoPuzzle.Presentation.Flows
{
    public sealed class AppFlowController : IStartable, IDisposable
    {
        private readonly IPublisher<PhotoRequested> photoRequestedPublisher;

        public AppFlowController(IPublisher<PhotoRequested> photoRequestedPublisher)
        {
            this.photoRequestedPublisher = photoRequestedPublisher;
        }

        private readonly CompositeDisposable disposables = new();

        void IStartable.Start()
        {
            Debug.Log("AppFlowController started");

            photoRequestedPublisher.Publish(default);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }
    }
}
