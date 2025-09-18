using Cysharp.Threading.Tasks;
using MGSP.PhotoPuzzle.Services.Events;
using MGSP.PhotoPuzzle.Services.UseCases;
using R3;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MGSP.PhotoPuzzle.Presentation.Stores
{
    public enum GamePlayStatus { Preparing, Playing, Completed }

    public sealed class GamePlayStore
    {
        private readonly CreateGame createGame;
        private readonly SwapPieces swapPieces;
        private readonly GameEventEmitter gameEventEmitter;

        private readonly ReactiveProperty<GamePlayStatus> statusRP = new(GamePlayStatus.Preparing);
        private readonly ReactiveProperty<bool> previewOnRP = new(false);
        private readonly List<Sprite> pieceTexs = new();
        private int firstPickedIndex = -1;

        public ReadOnlyReactiveProperty<GamePlayStatus> StatusRP => statusRP;
        public ReadOnlyReactiveProperty<bool> PreviewOnRP => previewOnRP;
        public IReadOnlyList<Sprite> PieceTextures => pieceTexs;

        public GamePlayStore(CreateGame createGame, SwapPieces swapPieces, GameEventEmitter gameEventEmitter)
        {
            this.createGame = createGame;
            this.swapPieces = swapPieces;
            this.gameEventEmitter = gameEventEmitter;
        }

        public async UniTask StartNewGame(int width, int height, Texture2D tex, CancellationToken cancellationToken)
        {
            previewOnRP.Value = false;
            statusRP.Value = GamePlayStatus.Preparing;

            await UniTask.Yield(cancellationToken);

            var events = await createGame.Execute(width, height, cancellationToken);

            CreatePieceTextures(tex, width, height);
            firstPickedIndex = -1;
            await gameEventEmitter.Process(events, cancellationToken);

            statusRP.Value = GamePlayStatus.Playing;
        }

        public async UniTask Pick(int value)
        {
            if (firstPickedIndex == -1)
            {
                firstPickedIndex = value;
            }
            else
            {
                var events = await swapPieces.Execute(firstPickedIndex, value, default);
                await gameEventEmitter.Process(events, default);
                firstPickedIndex = -1;

                if (events.Count > 0 && events[events.Count - 1] is GameEndedEvent)
                {
                    statusRP.Value = GamePlayStatus.Completed;
                }
            }
        }

        public void TogglePreview()
        {
            previewOnRP.Value = !previewOnRP.Value;
        }

        private void CreatePieceTextures(Texture2D tex, int width, int height)
        {
            pieceTexs.Clear();
            int cellWidth = tex.width / width;
            int cellHeight = tex.height / height;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var x = col * cellWidth;
                    var y = (height - 1 - row) * cellHeight;
                    var rect = new Rect(x, y, cellWidth, cellHeight);
                    var pivot = new Vector2(0.5f, 0.5f);
                    var cellSprite = Sprite.Create(tex, rect, pivot);
                    pieceTexs.Add(cellSprite);
                }
            }
        }
    }
}
