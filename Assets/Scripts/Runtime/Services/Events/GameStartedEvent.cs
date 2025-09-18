using System.Collections.Generic;

namespace MGSP.PhotoPuzzle.Services.Events
{
    public sealed class GameStartedEvent : IGameEvent
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
