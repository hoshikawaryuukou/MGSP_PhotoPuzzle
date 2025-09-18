namespace MGSP.PhotoPuzzle.Services.Events
{
    public sealed class CellSwappedEvent : IGameEvent
    {
        public int Index1 { get; }
        public int Index2 { get; }

        public CellSwappedEvent(int index1, int index2)
        {
            Index1 = index1;
            Index2 = index2;
        }
    }
}
