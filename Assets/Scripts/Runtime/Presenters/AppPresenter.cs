using MessagePipe;
using R3;
using Runtime.Infrastructures;
using Runtime.Routes;
using Runtime.Stores;
using System;
using UnityEngine;
using VContainer.Unity;

namespace Runtime.Presenters
{
    public sealed class AppPresenter : IStartable, IDisposable
    {
        private readonly GalleryStore photoStore;
        private readonly GameStore gameStore;
        private readonly ContextNavigator contextNavigator;
        private readonly ISubscriber<AppRoute> appRouteSubscriber;

        private readonly CompositeDisposable disposables = new();

        public AppPresenter(GalleryStore photoStore, GameStore gameStore, ContextNavigator contextNavigator, ISubscriber<AppRoute> appRouteSubscriber)
        {
            this.photoStore = photoStore;
            this.gameStore = gameStore;
            this.contextNavigator = contextNavigator;
            this.appRouteSubscriber = appRouteSubscriber;
        }

        void IStartable.Start()
        {
            appRouteSubscriber.Subscribe(Navigate).AddTo(disposables);
            Navigate(AppRoute.Gallery);
        }

        void IDisposable.Dispose()
        {
            disposables.Dispose();
        }

        private void Navigate(AppRoute appRoute)
        {
            Debug.Log($"Navigate to {appRoute}");
            switch (appRoute)
            {
                case AppRoute.Gallery:
                    contextNavigator.Load("Gallery");
                    contextNavigator.Unload("Game");
                    break;
                case AppRoute.Game:
                    gameStore.SetPhoto(photoStore.PhotoTex);
                    contextNavigator.Load("Game");
                    contextNavigator.Unload("Gallery");
                    break;

            }
        }
    }
}
