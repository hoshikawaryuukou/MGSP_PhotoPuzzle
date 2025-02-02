using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Game.Domain
{
    public sealed class PuzzleGame
    {
        private readonly List<int> pieces;

        public static PuzzleGame CreateFromSize(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Size must be greater than zero.");
            }

            var random = new Random();
            var sequence = Enumerable.Range(0, size).OrderBy(_ => random.Next()).ToArray();
            return new PuzzleGame(sequence);
        }

        private PuzzleGame(int[] source)
        {
            pieces = new List<int>(source);
        }

        public void Swap(int index0, int index1)
        {
            ValidateIndex(index0);
            ValidateIndex(index1);
            (pieces[index0], pieces[index1]) = (pieces[index1], pieces[index0]);
        }

        public bool IsCompleted()
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i] != i)
                    return false;
            }

            return true;
        }

        public int GetPiece(int index)
        {
            ValidateIndex(index);
            return pieces[index];
        }

        public IReadOnlyList<int> GetPieces()
        {
            return pieces;
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= pieces.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within bounds.");
            }
        }
    }
}
