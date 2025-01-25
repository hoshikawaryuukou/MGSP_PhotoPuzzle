using R3;
using UnityEngine;

namespace Runtime.Views
{
    public sealed class GameOptionSelector : MonoBehaviour
    {
        [SerializeField] private GameOptionSelectorItem[] items;

        private readonly CompositeDisposable disposables = new();
        private readonly ReactiveProperty<int> selectedIndexRP = new(-1);

        public ReadOnlyReactiveProperty<int> SelectedIndexRP => selectedIndexRP;

        void Awake()
        {
            for (int i = 0; i < items.Length; i++)
            {
                var index = i;
                items[i].Clicked.Subscribe(_ => selectedIndexRP.Value = index).AddTo(disposables);
            }
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndexRP.Value = index;
            for (int i = 0; i < items.Length; i++)
            {
                items[i].SetSelected(i == index);
            }
        }
    }
}
