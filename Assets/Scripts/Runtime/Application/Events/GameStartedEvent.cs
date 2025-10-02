using System.Collections.Generic;

namespace MGSP.PhotoPuzzle.Application.Events
{
    public sealed class GameStartedEvent
    {
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<int> InitialIndices { get; }

        public GameStartedEvent(int width, int height, IReadOnlyList<int> initialIndices)
        {
            Width = width;
            Height = height;
            InitialIndices = initialIndices;
        }
    }
}
