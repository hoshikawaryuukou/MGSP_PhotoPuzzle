using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using Runtime.Infrastructures;
using Runtime.Routes;
using Runtime.Stores;
using System;
using System.Collections.Generic;
using System.Threading;
using VContainer.Unity;

namespace Runtime.Presenters
{
    public sealed class GamePresenter : IStartable, IDisposable
    {
        private readonly GameStore gameStore;
        private readonly List<string> moduleList;
        private readonly ContextNavigator contextNavigator;
        private readonly IPublisher<AppRoute> appRoutePublisher;

        private readonly CancellationTokenSource cts = new();
        private readonly CompositeDisposable disposables = new();

        public GamePresenter(GameStore gameStore, List<string> moduleList, ContextNavigator contextNavigator, IPublisher<AppRoute> appRoutePublisher)
        {
            this.gameStore = gameStore;
            this.moduleList = moduleList;
            this.contextNavigator = contextNavigator;
            this.appRoutePublisher = appRoutePublisher;
        }

        void IStartable.Start()
        {
            disposables.Clear();
            gameStore.ExitSubject.Subscribe(_ => appRoutePublisher.Publish(AppRoute.Gallery)).AddTo(disposables);

            foreach (var module in moduleList)
            {
                contextNavigator.Load(module);
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var module in moduleList)
            {
                contextNavigator.Unload(module);
            }

            cts.Cancel();
            cts.Dispose();
            disposables.Dispose();
        }
    }
}
