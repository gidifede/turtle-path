using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using TurtlePath.Level;
using TurtlePath.Core;

namespace TurtlePath.Tests
{
    public class LevelLoaderTests
    {
        [Test]
        public void LoadLevel_1To15_NotNull()
        {
            for (int i = 1; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load successfully");
            }
        }

        [Test]
        public void Levels1To6_Are4x4()
        {
            for (int i = 1; i <= 6; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.AreEqual(4, level.width, $"Level {i} width");
                Assert.AreEqual(4, level.height, $"Level {i} height");
            }
        }

        [Test]
        public void Levels7To10_Are5x5()
        {
            for (int i = 7; i <= 10; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.AreEqual(5, level.width, $"Level {i} width");
                Assert.AreEqual(5, level.height, $"Level {i} height");
            }
        }

        [Test]
        public void Levels11To15_Are6x6()
        {
            for (int i = 11; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.AreEqual(6, level.width, $"Level {i} width");
                Assert.AreEqual(6, level.height, $"Level {i} height");
            }
        }

        [Test]
        public void AllLevels_NestAndSea_InBounds()
        {
            for (int i = 1; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);

                Assert.IsTrue(level.nestPos.x >= 0 && level.nestPos.x < level.width,
                    $"Level {i}: nest X in bounds");
                Assert.IsTrue(level.nestPos.y >= 0 && level.nestPos.y < level.height,
                    $"Level {i}: nest Y in bounds");
                Assert.IsTrue(level.seaPos.x >= 0 && level.seaPos.x < level.width,
                    $"Level {i}: sea X in bounds");
                Assert.IsTrue(level.seaPos.y >= 0 && level.seaPos.y < level.height,
                    $"Level {i}: sea Y in bounds");
            }
        }

        [Test]
        public void AllLevels_HaveTiles()
        {
            for (int i = 1; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level.tiles, $"Level {i} tiles not null");
                Assert.Greater(level.tiles.Length, 0, $"Level {i} should have tiles");
            }
        }

        [Test]
        public void Levels7To15_HaveObstacles()
        {
            for (int i = 7; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level.obstacles, $"Level {i} obstacles not null");
                Assert.Greater(level.obstacles.Length, 0, $"Level {i} should have obstacles");
            }
        }

        [Test]
        public void Levels1To6_HaveNoObstacles()
        {
            for (int i = 1; i <= 6; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                int count = level.obstacles != null ? level.obstacles.Length : 0;
                Assert.AreEqual(0, count, $"Level {i} should have no obstacles");
            }
        }

        [Test]
        public void Levels5And6_HaveTpieces()
        {
            for (int i = 5; i <= 6; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                bool hasT = false;
                foreach (var tile in level.tiles)
                {
                    if (tile.type == TileType.T) hasT = true;
                }
                Assert.IsTrue(hasT, $"Level {i} should have at least one T-piece");
            }
        }

        [Test]
        public void Levels11To15_HaveInventory()
        {
            for (int i = 11; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level.inventory, $"Level {i} inventory not null");
                Assert.Greater(level.inventory.Length, 0, $"Level {i} should have inventory tiles");
            }
        }

        [Test]
        public void Levels1To10_HaveNoInventory()
        {
            for (int i = 1; i <= 10; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                int count = level.inventory != null ? level.inventory.Length : 0;
                Assert.AreEqual(0, count, $"Level {i} should have no inventory");
            }
        }

        [Test]
        public void Obstacles_InBounds()
        {
            for (int i = 7; i <= 15; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                if (level.obstacles == null) continue;

                foreach (var obs in level.obstacles)
                {
                    Assert.IsTrue(obs.position.x >= 0 && obs.position.x < level.width,
                        $"Level {i}: obstacle X in bounds");
                    Assert.IsTrue(obs.position.y >= 0 && obs.position.y < level.height,
                        $"Level {i}: obstacle Y in bounds");
                }
            }
        }
    }
}
