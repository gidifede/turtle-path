using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TurtlePath.Core;
using TurtlePath.Grid;
using TurtlePath.Level;
using TurtlePath.Path;
using TurtlePath.Tiles;

namespace TurtlePath.Tests
{
    public class LevelValidationTests
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
        public IEnumerator AllLevels_HaveSolvablePath_WithSolutionRotations()
        {
            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                // Level data already has solution rotations (no randomization)
                gridManager.BuildGrid(level);

                var path = PathValidator.FindPath(gridManager, level.nestPos, level.seaPos);
                Assert.IsNotNull(path, $"Level {i} should have a valid path with solution rotations");
                Assert.Greater(path.Count, 1, $"Level {i} path should have at least 2 cells");

                gridManager.ClearGrid();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator AllLevels_Collectibles_AreOnTileCells()
        {
            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                gridManager.BuildGrid(level);

                if (level.collectibles != null)
                {
                    foreach (var collectible in level.collectibles)
                    {
                        var cell = gridManager.GetCell(collectible.position.x, collectible.position.y);
                        Assert.IsNotNull(cell, $"Level {i}: collectible at {collectible.position} should be on a valid cell");
                        Assert.IsNotNull(cell.Tile, $"Level {i}: collectible at {collectible.position} should be on a cell with a tile");
                    }
                }

                gridManager.ClearGrid();
                yield return null;
            }
        }
    }
}
