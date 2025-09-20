using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MGSP.PhotoPuzzle.Presentation.Views
{
    public sealed class PuzzleBoardView : MonoBehaviour
    {
        [SerializeField] private PuzzlePieceView prefab;
        [SerializeField] private Transform container;
        [SerializeField] private FlexibleGridLayoutGroup layoutGroup;

        private readonly List<PuzzlePieceView> pieceViews = new();
        private readonly Subject<int> pieceSelected = new();
        private readonly CompositeDisposable disposables = new();
        private ObjectPool<PuzzlePieceView> piecePool;

        public Observable<int> PieceSelected => pieceSelected;

        void Awake()
        {
            piecePool = new ObjectPool<PuzzlePieceView>(
                createFunc: CreatePuzzlePiece,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        void OnDestroy()
        {
            disposables.Dispose();
            piecePool?.Dispose();
        }

        public async UniTask PreparePieces(int width, int height, Texture2D tex, IReadOnlyList<int> initialIndices)
        {
            disposables.Clear();

            ReturnAllPiecesToPool();

            await UniTask.Yield();


            pieceViews.Clear();


            if (layoutGroup != null)
            {
                layoutGroup.enabled = true;
                layoutGroup.columns = width;
                layoutGroup.rows = height;
            }

            float pieceWidth = 1f / width;
            float pieceHeight = 1f / height;

            for (int p = 0; p < initialIndices.Count; p++)
            {
                var index = initialIndices[p];
                int i = index % width;
                int j = index / width;

                // Get piece from pool instead of instantiating
                var pieceView = piecePool.Get();
                pieceView.transform.SetParent(container, false);

                float x = i * pieceWidth;
                float y = j * pieceHeight;

                var pId = p;
                pieceView.SetImage(tex, new Rect(x, 1f - y - pieceHeight, pieceWidth, pieceHeight));
                pieceView.Clicked.Subscribe(_ => pieceSelected.OnNext(pId)).AddTo(disposables);
                pieceViews.Add(pieceView);
            }

            await UniTask.Yield();
        }

        public void SwapPieces(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= pieceViews.Count || indexB < 0 || indexB >= pieceViews.Count)
            {
                Debug.LogWarning("SwapPieces: Invalid indices");
                return;
            }

            var uvRectA = pieceViews[indexA].GetUVRect();
            var uvRectB = pieceViews[indexB].GetUVRect();
            pieceViews[indexA].SetUVRect(uvRectB);
            pieceViews[indexB].SetUVRect(uvRectA);
        }

        private PuzzlePieceView CreatePuzzlePiece()
        {
            var piece = Instantiate(prefab);
            piece.gameObject.SetActive(false); 
            return piece;
        }

        private void OnGetFromPool(PuzzlePieceView piece)
        {
            piece.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(PuzzlePieceView piece)
        {
            piece.ResetPiece(); // Reset piece state
            piece.gameObject.SetActive(false);
            piece.transform.SetParent(null); // Remove from container
        }

        private void OnDestroyPoolObject(PuzzlePieceView piece)
        {
            if (piece != null)
            {
                Destroy(piece.gameObject);
            }
        }

        private void ReturnAllPiecesToPool()
        {
            for (int i = pieceViews.Count - 1; i >= 0; i--)
            {
                if (pieceViews[i] != null)
                {
                    piecePool.Release(pieceViews[i]);
                }
            }
        }

        public void ClearPool()
        {
            ReturnAllPiecesToPool();
            pieceViews.Clear();
            piecePool.Clear();
        }
    }
}
