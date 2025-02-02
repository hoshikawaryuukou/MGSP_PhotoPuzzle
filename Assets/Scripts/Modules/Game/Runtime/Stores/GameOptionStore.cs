using R3;

namespace Modules.Game.Stores
{
    public sealed class GameOptionStore
    {
        private readonly int[] columnOptions = { 3, 4, 5, 6, 7 };
        private readonly int[] rowOptions = { 3, 4, 5, 6, 7 };
        private readonly ReactiveProperty<int> columnIndexRP = new(0);
        private readonly ReactiveProperty<int> rowIndexRP = new(0);

        public ReadOnlyReactiveProperty<int> ColumnIndexRP => columnIndexRP;
        public ReadOnlyReactiveProperty<int> RowIndexRP => rowIndexRP;

        public void SetColumnIndex(int index)
        {
            columnIndexRP.Value = index;
        }

        public void SetRowIndex(int index)
        {
            rowIndexRP.Value = index;
        }

        public int GetColumn()
        {
            return columnOptions[columnIndexRP.Value];
        }

        public int GetRow()
        {
            return rowOptions[rowIndexRP.Value];
        }
    }
}
