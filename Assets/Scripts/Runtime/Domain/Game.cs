using System.Collections.Generic;

namespace MGSP.PhotoPuzzle.Domain
{
    public sealed class Game
    {
        private readonly int[] grid;
        private readonly System.Random _random = new();

        public Game(int width, int height)
        {
            if (width <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(width), width, "width must be greater than 0.");
            
            if (height <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(height), height, "height must be greater than 0.");

            if (width + height < 3)
                throw new System.ArgumentException("The sum of width and height must be at least 3 to create a valid puzzle.");

            int size = width * height;
            grid = new int[size];
            for (int i = 0; i < size; i++)
            {
                grid[i] = i;
            }

            // Only shuffle if the puzzle has more than one tile
            if (size > 1)
            {
                // Fisher-Yates Shuffle - repeat until the grid is not in winning state
                do
                {
                    // Reset to ordered state before each shuffle attempt
                    for (int i = 0; i < size; i++)
                    {
                        grid[i] = i;
                    }
                    
                    // Perform Fisher-Yates shuffle
                    for (int i = size - 1; i > 0; i--)
                    {
                        int j = _random.Next(i + 1);
                        (grid[i], grid[j]) = (grid[j], grid[i]);
                    }
                } while (CheckWin());
            }
        }

        public void Swap(int index1, int index2)
        {
            if (index1 < 0 || index1 >= grid.Length)
                throw new System.ArgumentOutOfRangeException(nameof(index1), index1, $"index1 must be in range [0, {grid.Length - 1}].");
            
            if (index2 < 0 || index2 >= grid.Length)
                throw new System.ArgumentOutOfRangeException(nameof(index2), index2, $"index2 must be in range [0, {grid.Length - 1}].");

            if (index1 == index2) 
                return;

            (grid[index1], grid[index2]) = (grid[index2], grid[index1]);
        }

        public bool CheckWin()
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (grid[i] != i)
                {
                    return false;
                }
            }
            return true;
        }
    
        public IReadOnlyList<int> GetGrid() => grid;
    }
}
