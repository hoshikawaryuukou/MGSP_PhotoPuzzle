using R3;
using UnityEngine;

namespace Runtime.Stores
{
    public sealed class GameStore
    {
        private readonly Subject<Unit> askedSubject = new();
        private readonly Subject<Unit> playSubject = new();
        private readonly Subject<Unit> exitSubject = new();

        public Texture2D Photo { get; private set; }
        public Observable<Unit> AskedSubject => askedSubject;
        public Observable<Unit> PlaySubject => playSubject;
        public Observable<Unit> ExitSubject => exitSubject;

        public void SetPhoto(Texture2D photo)
        {
            Photo = photo;
        }

        public void Ask() => askedSubject.OnNext(Unit.Default);

        public void Play() => playSubject.OnNext(Unit.Default);

        public void Exit() => exitSubject.OnNext(Unit.Default);
    }
}
