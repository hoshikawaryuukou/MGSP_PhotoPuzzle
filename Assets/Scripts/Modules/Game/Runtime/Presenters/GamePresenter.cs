using Cysharp.Threading.Tasks;
using MessagePipe;
using Module.Game.Events;
using R3;
using System;
using System.Threading;
using VContainer.Unity;

namespace Modules.Game.Presenters
{
    public sealed class GamePresenter : IStartable, IPostStartable, IDisposable
    {
        private readonly IPublisher<GameExited> publisher;
        private readonly IScopedSubscriber<GameFinished> gameFinishedSubscriber;
        private readonly IScopedPublisher<GameAsked> gameAskedPublisher;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GamePresenter(IPublisher<GameExited> publisher, IScopedSubscriber<GameFinished> gameFinishedSubscriber, IScopedPublisher<GameAsked> gameAskedPublisher)
        {
            this.publisher = publisher;
            this.gameFinishedSubscriber = gameFinishedSubscriber;
            this.gameAskedPublisher = gameAskedPublisher;
        }

        void IStartable.Start()
        {
            disposables.Clear();
            gameFinishedSubscriber.Subscribe(_ => publisher.Publish(new GameExited())).AddTo(disposables);
        }

        void IPostStartable.PostStart()
        {
            gameAskedPublisher.Publish(new GameAsked());
        }

        void IDisposable.Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }
    }
}
