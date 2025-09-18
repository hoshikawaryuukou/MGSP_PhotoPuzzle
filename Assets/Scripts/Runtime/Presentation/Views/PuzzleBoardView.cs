using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using UnityEngine;

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

        public Observable<int> PieceSelected => pieceSelected;

        public async UniTask PreparePieces(int width, int height)
        {
            // Clear subscriptions
            disposables.Clear();

            // Clear any existing cells
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            await UniTask.Yield();

            // Clear the list
            pieceViews.Clear();

            // Configure grid layout
            if (layoutGroup != null)
            {
                layoutGroup.enabled = true;
                layoutGroup.columns = width;
                layoutGroup.rows = height;
                layoutGroup.fitType = FlexibleGridLayoutGroup.FitType.Uniform;
                layoutGroup.fitX = true;
                layoutGroup.fitY = true;
            }

            var count = width * height;
            for (int i = 0; i < count; i++)
            {
                var index = i;
                var pieceView = Instantiate(prefab, container);
                pieceView.name = $"P_{index}";
                pieceView.Clicked.Subscribe(_ => pieceSelected.OnNext(index)).AddTo(disposables);
                pieceViews.Add(pieceView);
            }

            await UniTask.Yield();
        }

        public void SetPieceImage(int index, Sprite sprite)
        {
            if (index < 0 || index >= pieceViews.Count)
            {
                Debug.LogWarning("SetPieceImage: Invalid index");
                return;
            }

            pieceViews[index].SetImage(sprite);
        }

        public void SwapPieces(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= pieceViews.Count || indexB < 0 || indexB >= pieceViews.Count)
            {
                Debug.LogWarning("SwapPieces: Invalid indices");
                return;
            }

            var spriteA = pieceViews[indexA].GetImage();
            var spriteB = pieceViews[indexB].GetImage();
            pieceViews[indexA].SetImage(spriteB);
            pieceViews[indexB].SetImage(spriteA);
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}
