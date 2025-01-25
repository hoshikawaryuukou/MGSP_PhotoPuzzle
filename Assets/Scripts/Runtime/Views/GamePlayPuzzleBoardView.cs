using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Views
{
    public sealed class GamePlayPuzzleBoardRequest
    {
        public readonly int rows;
        public readonly int cols;
        public readonly Texture2D tex;
        public readonly IList<int> indices;

        public GamePlayPuzzleBoardRequest(int rows, int cols, Texture2D tex, IList<int> indices)
        {
            this.rows = rows;
            this.cols = cols;
            this.tex = tex;
            this.indices = indices;
        }
    }

    public readonly struct GamePlayPuzzleBoardSelectedPair
    {
        public readonly int index0;
        public readonly int index1;
        public GamePlayPuzzleBoardSelectedPair(int index0, int index1)
        {
            this.index0 = index0;
            this.index1 = index1;
        }
    }

    public sealed class GamePlayPuzzleBoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform pieceRoot;
        [SerializeField] private GamePlayPuzzlePieceView pieceViewPrefab;
        [SerializeField] private FlexibleGridLayoutGroup gridLayoutGroup;
        [SerializeField] private float spacing = 2f;

        private readonly List<GamePlayPuzzlePieceView> pieceViews = new();
        private readonly List<Vector3> pieceGridPos = new();
        private readonly List<int> selectedPieces = new();
        private readonly Subject<GamePlayPuzzleBoardSelectedPair> piecePairedSubject = new();
        private readonly CompositeDisposable disposables = new();

        public Observable<GamePlayPuzzleBoardSelectedPair> PiecePaired => piecePairedSubject;

        public async UniTask Create(GamePlayPuzzleBoardRequest request, CancellationToken token)
        {
            selectedPieces.Clear();
            disposables.Clear();
            var pieceViews = await Build(request.rows, request.cols, request.tex, token);
            foreach (var pieceView in pieceViews)
            {
                pieceView.Clicked.Subscribe(_ => OnPieceViewSelected(pieceView)).AddTo(disposables);
            }

            await UniTask.Yield(token);
            await Arrange(request.indices, token);
        }

        public async UniTask<IList<GamePlayPuzzlePieceView>> Build(int rows, int cols, Texture2D tex, CancellationToken token)
        {
            foreach (var pieceView in pieceViews)
            {
                Destroy(pieceView.gameObject);
            }

            await UniTask.Yield(token);
            pieceViews.Clear();
            pieceGridPos.Clear();

            gridLayoutGroup.enabled = false;
            gridLayoutGroup.columns = cols;
            gridLayoutGroup.rows = rows;
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);

            float width = 1f / cols;
            float height = 1f / rows;

            for (int j = rows - 1; j > -1; j--)
            {
                for (int i = 0; i < cols; i++)
                {
                    var pieceView = Instantiate(pieceViewPrefab, pieceRoot);
                    pieceView.SetPiece(tex, new Rect(i * width, j * height, width, height));
                    pieceViews.Add(pieceView);
                }
            }

            gridLayoutGroup.enabled = true;
            gridLayoutGroup.CalculateLayoutInputVertical();

            await UniTask.Yield(token);
            await UniTask.Yield(token);
            gridLayoutGroup.enabled = false;

            for (int i = 0; i < pieceViews.Count; i++)
            {
                pieceGridPos.Add(pieceViews[i].transform.localPosition);
            }

            return new List<GamePlayPuzzlePieceView>(pieceViews);
        }

        public UniTask Arrange(IList<int> indices, CancellationToken token)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                pieceViews[i].transform.localPosition = pieceGridPos[indices[i]];
            }

            return UniTask.CompletedTask;
        }

        public UniTask Swap(int index0, int index1, CancellationToken token)
        {
            var pieceView0Transform = pieceViews[index0].transform;
            var pieceView1Transform = pieceViews[index1].transform;

            (pieceView0Transform.localPosition, pieceView1Transform.localPosition)
                = (pieceView1Transform.localPosition, pieceView0Transform.localPosition);

            return UniTask.CompletedTask;
        }

        private void OnPieceViewSelected(GamePlayPuzzlePieceView pieceView)
        {
            selectedPieces.Add(pieceViews.IndexOf(pieceView));
            if (selectedPieces.Count == 2)
            {
                piecePairedSubject.OnNext(new GamePlayPuzzleBoardSelectedPair(selectedPieces[0], selectedPieces[1]));
                selectedPieces.Clear();
            }
        }
    }
}