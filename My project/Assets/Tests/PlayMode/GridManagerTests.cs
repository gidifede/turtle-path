using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TurtlePath.Grid;
using TurtlePath.Level;

namespace TurtlePath.Tests
{
    public class GridManagerTests
    {
        private GridManager gridManager;
        private GameObject gridManagerObj;

        [SetUp]
        public void SetUp()
        {
            gridManagerObj = new GameObject("TestGridManager");
            gridManager = gridManagerObj.AddComponent<GridManager>();
        }

        [TearDown]
        public void TearDown()
        {
            gridManager.ClearGrid();
            Object.DestroyImmediate(gridManagerObj);
        }

        [UnityTest]
        public IEnumerator BuildGrid_SetsCorrectDimensions()
        {
            var level = LevelLoader.LoadLevel(1);
            Assert.IsNotNull(level);

            gridManager.BuildGrid(level);
            yield return null;

            Assert.AreEqual(level.width, gridManager.Width);
            Assert.AreEqual(level.height, gridManager.Height);
        }

        [UnityTest]
        public IEnumerator GetCell_ValidInBounds_NullOutOfBounds()
        {
            var level = LevelLoader.LoadLevel(1);
            Assert.IsNotNull(level);

            gridManager.BuildGrid(level);
            yield return null;

            // Valid cell inside grid
            var cell = gridManager.GetCell(0, 0);
            Assert.IsNotNull(cell, "Cell at (0,0) should exist");

            // Out of bounds
            Assert.IsNull(gridManager.GetCell(-1, 0), "Cell at (-1,0) should be null");
            Assert.IsNull(gridManager.GetCell(0, -1), "Cell at (0,-1) should be null");
            Assert.IsNull(gridManager.GetCell(level.width, 0), "Cell beyond width should be null");
            Assert.IsNull(gridManager.GetCell(0, level.height), "Cell beyond height should be null");
        }

        [UnityTest]
        public IEnumerator GetCollectible_ReturnsView_ForCollectiblePositions()
        {
            var level = LevelLoader.LoadLevel(1);
            Assert.IsNotNull(level);

            gridManager.BuildGrid(level);
            yield return null;

            if (level.collectibles != null && level.collectibles.Length > 0)
            {
                var pos = level.collectibles[0].position;
                var view = gridManager.GetCollectible(pos);
                Assert.IsNotNull(view, $"Should have collectible view at {pos}");
            }
        }
    }
}
