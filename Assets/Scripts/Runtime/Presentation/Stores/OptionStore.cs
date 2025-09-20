using R3;
using System.Collections.Generic;

namespace MGSP.PhotoPuzzle.Presentation.Stores
{
    public sealed class OptionStore
    {
        private readonly ReactiveProperty<int> widthRP = new(2);
        private readonly ReactiveProperty<int> heightRP = new(2);

        public ReadOnlyReactiveProperty<int> WidthRP => widthRP;
        public ReadOnlyReactiveProperty<int> HeightRP => heightRP;

        public void SetWidth(int width) => widthRP.Value = width;
        public void SetHeight(int height) => heightRP.Value = height;

        public IReadOnlyList<int> WidthOptions { get; } = new List<int> { 3, 4, 5, 6, 7 };
        public IReadOnlyList<int> HeightOptions { get; } = new List<int> { 3, 4, 5, 6, 7 };
    }
}
