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
        public IEnumerator Levels1To10_HaveSolvablePath_WithSolutionRotations()
        {
            for (int i = 1; i <= 10; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                gridManager.BuildGrid(level);

                var path = PathValidator.FindPath(gridManager, level.nestPos, level.seaPos);
                Assert.IsNotNull(path, $"Level {i} should have a valid path with solution rotations");
                Assert.Greater(path.Count, 1, $"Level {i} path should have at least 2 cells");

                gridManager.ClearGrid();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Levels11To15_NeedInventory_NoPathWithoutIt()
        {
            for (int i = 11; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");
                Assert.IsNotNull(level.inventory, $"Level {i} should have inventory");
                Assert.Greater(level.inventory.Length, 0, $"Level {i} should have inventory tiles");

                // Build grid with only pre-placed tiles (no inventory placed)
                gridManager.BuildGrid(level);

                var path = PathValidator.FindPath(gridManager, level.nestPos, level.seaPos);
                Assert.IsNull(path, $"Level {i} should NOT have a path without inventory tiles placed");

                gridManager.ClearGrid();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator AllLevels_Collectibles_AreOnTileCells()
        {
            for (int i = 1; i <= 15; i++)
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

        [UnityTest]
        public IEnumerator Levels1To10_Collectibles_AreOnSolutionPath()
        {
            for (int i = 1; i <= 10; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                gridManager.BuildGrid(level);

                var path = PathValidator.FindPath(gridManager, level.nestPos, level.seaPos);
                Assert.IsNotNull(path, $"Level {i} must have path");

                if (level.collectibles != null)
                {
                    foreach (var collectible in level.collectibles)
                    {
                        Assert.IsTrue(path.Contains(collectible.position),
                            $"Level {i}: collectible at {collectible.position} should be on the solution path");
                    }
                }

                gridManager.ClearGrid();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Levels7To15_Obstacles_CellTypeIsCorrect()
        {
            for (int i = 7; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                gridManager.BuildGrid(level);

                if (level.obstacles != null)
                {
                    foreach (var obs in level.obstacles)
                    {
                        var cell = gridManager.GetCell(obs.position.x, obs.position.y);
                        Assert.IsNotNull(cell, $"Level {i}: obstacle cell at {obs.position} should exist");
                        Assert.AreEqual(obs.type, cell.CellType,
                            $"Level {i}: cell at {obs.position} should be {obs.type}");
                        Assert.IsFalse(cell.IsRotatable(),
                            $"Level {i}: obstacle at {obs.position} should not be rotatable");
                    }
                }

                gridManager.ClearGrid();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Levels7To10_Obstacles_NotOnPath()
        {
            for (int i = 7; i <= 10; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load");

                gridManager.BuildGrid(level);

                var path = PathValidator.FindPath(gridManager, level.nestPos, level.seaPos);
                Assert.IsNotNull(path, $"Level {i} must have path");

                if (level.obstacles != null)
                {
                    foreach (var obs in level.obstacles)
                    {
                        Assert.IsFalse(path.Contains(obs.position),
                            $"Level {i}: obstacle at {obs.position} should NOT be on the solution path");
                    }
                }

                gridManager.ClearGrid();
                yield return null;
            }
        }
    }
}
