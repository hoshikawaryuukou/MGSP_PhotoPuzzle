using System;
using System.Linq;
using NUnit.Framework;

namespace MGSP.PhotoPuzzle.Domain.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void Constructor_ShufflesGrid_ContainsAllExpectedValues()
        {
            // Arrange & Act
            var game = new Game(3, 3);
            var grid = game.GetGrid();

            // Assert
            Assert.AreEqual(9, grid.Count);
            // Verify all numbers 0-8 are present exactly once
            for (int i = 0; i < 9; i++)
            {
                Assert.AreEqual(1, grid.Count(x => x == i), $"Number {i} should appear exactly once");
            }
        }

        [Test]
        public void Constructor_WithSingleTile_CreatesValidGame()
        {
            // Arrange & Act - Single tile should now throw exception due to width + height < 2 rule
            var ex = Assert.Throws<ArgumentException>(() => new Game(1, 1));
            Assert.That(ex.Message, Contains.Substring("The sum of width and height must be at least 3 to create a valid puzzle"));
        }

        [Test]
        public void Constructor_WithInvalidDimensions_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert - Test zero and negative dimensions
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(0, 5));
            Assert.AreEqual("width", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("width must be greater than 0"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(5, 0));
            Assert.AreEqual("height", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("height must be greater than 0"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(0, 0));
            Assert.AreEqual("width", ex.ParamName); // width is checked first
            Assert.That(ex.Message, Contains.Substring("width must be greater than 0"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(-1, 5));
            Assert.AreEqual("width", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("width must be greater than 0"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(5, -1));
            Assert.AreEqual("height", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("height must be greater than 0"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Game(-1, -1));
            Assert.AreEqual("width", ex.ParamName); // width is checked first
            Assert.That(ex.Message, Contains.Substring("width must be greater than 0"));
        }

        [Test]
        public void Swap_WithValidIndices_SwapsCorrectly()
        {
            // Arrange
            var game = new Game(3, 3);
            var gridBefore = game.GetGrid().ToArray();
            int index1 = 0;
            int index2 = 8;
            int expectedAtIndex1 = gridBefore[index2];
            int expectedAtIndex2 = gridBefore[index1];

            // Act
            game.Swap(index1, index2);
            var gridAfter = game.GetGrid();

            // Assert
            Assert.AreEqual(expectedAtIndex1, gridAfter[index1]);
            Assert.AreEqual(expectedAtIndex2, gridAfter[index2]);

            // Verify other positions remain unchanged
            for (int i = 0; i < gridAfter.Count; i++)
            {
                if (i != index1 && i != index2)
                {
                    Assert.AreEqual(gridBefore[i], gridAfter[i]);
                }
            }
        }

        [Test]
        public void Swap_WithInvalidIndices_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var game = new Game(3, 3); // Grid size is 9 (indices 0-8)

            // Act & Assert - Test index1 out of range
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => game.Swap(-1, 0));
            Assert.AreEqual("index1", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("index1 must be in range [0, 8]"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => game.Swap(9, 0));
            Assert.AreEqual("index1", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("index1 must be in range [0, 8]"));

            // Test index2 out of range
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => game.Swap(0, -1));
            Assert.AreEqual("index2", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("index2 must be in range [0, 8]"));

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => game.Swap(0, 9));
            Assert.AreEqual("index2", ex.ParamName);
            Assert.That(ex.Message, Contains.Substring("index2 must be in range [0, 8]"));

            // Test both indices out of range - should throw for index1 first
            ex = Assert.Throws<ArgumentOutOfRangeException>(() => game.Swap(-1, -1));
            Assert.AreEqual("index1", ex.ParamName);

            // Test same index swap does nothing
            var gridBefore = game.GetGrid().ToArray();
            bool initialWinState = game.CheckWin();
            game.Swap(4, 4);
            var gridAfter = game.GetGrid();
            Assert.AreEqual(initialWinState, game.CheckWin());
            CollectionAssert.AreEqual(gridBefore, gridAfter);
        }

        [Test]
        public void CheckWin_WithShuffledGame_ReturnsFalse()
        {
            // Arrange
            var game = new Game(3, 3);

            // Act - Perform some swaps to ensure the game is not in winning state
            game.Swap(0, 1);
            bool isWin = game.CheckWin();
            var grid = game.GetGrid();

            // Assert
            Assert.IsFalse(isWin);
            // Verify grid is not in winning order
            bool isInOrder = true;
            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i] != i)
                {
                    isInOrder = false;
                    break;
                }
            }
            Assert.IsFalse(isInOrder);
        }

        [Test]
        public void CheckWin_WithOrderedGrid_ReturnsTrue()
        {
            // Arrange
            var game = new Game(2, 2); // 2x2 = 4 tiles

            // Act - Manually restore order by finding where each number should go
            var grid = game.GetGrid();

            // Find current positions and restore order
            for (int targetValue = 0; targetValue < grid.Count; targetValue++)
            {
                int currentPosition = grid.ToList().IndexOf(targetValue);
                if (currentPosition != targetValue)
                {
                    game.Swap(targetValue, currentPosition);
                }
            }

            bool isWin = game.CheckWin();
            var finalGrid = game.GetGrid();

            // Assert
            Assert.IsTrue(isWin);
            for (int i = 0; i < finalGrid.Count; i++)
            {
                Assert.AreEqual(i, finalGrid[i]);
            }
        }

        [Test]
        public void CheckWin_WithMinimumValidGame_WorksCorrectly()
        {
            // Test smallest valid game (1x2 or 2x1) - should be able to win
            var game = new Game(1, 2);

            // The game starts shuffled, so manually restore order
            var grid = game.GetGrid();

            // Find current positions and restore order
            for (int targetValue = 0; targetValue < grid.Count; targetValue++)
            {
                int currentPosition = grid.ToList().IndexOf(targetValue);
                if (currentPosition != targetValue)
                {
                    game.Swap(targetValue, currentPosition);
                }
            }

            bool isWin = game.CheckWin();
            var finalGrid = game.GetGrid();

            // Assert
            Assert.IsTrue(isWin);
            for (int i = 0; i < finalGrid.Count; i++)
            {
                Assert.AreEqual(i, finalGrid[i]);
            }
        }
    }
}
