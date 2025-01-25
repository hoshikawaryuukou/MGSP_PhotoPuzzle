using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Views
{
    public sealed class GameOptionModal : MonoBehaviour
    {
        [SerializeField] private GameOptionSelector colSelector;
        [SerializeField] private GameOptionSelector rowSelector;
        [SerializeField] private Button playButton;
        [SerializeField] private Button closeButton;

        public Observable<int> ColumnIndexClicked => colSelector.SelectedIndexRP;
        public Observable<int> RowIndexClicked => rowSelector.SelectedIndexRP;
        public Observable<Unit> PlayRequested => playButton.OnClickAsObservable();
        public Observable<Unit> CloseRequested => closeButton.OnClickAsObservable();

        public void Show(int colToggleIndex, int rowToggleIndex)
        {
            colSelector.SetSelectedIndex(colToggleIndex);
            rowSelector.SetSelectedIndex(rowToggleIndex);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetColumnIndex(int index)
        {
            colSelector.SetSelectedIndex(index);
        }

        public void SetRowIndex(int index)
        {
            rowSelector.SetSelectedIndex(index);
        }

        public int GetColIndex()
        {
            return colSelector.SelectedIndexRP.CurrentValue;
        }

        public int GetRowIndex()
        {
            return rowSelector.SelectedIndexRP.CurrentValue;
        }
    }
}
