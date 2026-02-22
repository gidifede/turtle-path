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
        public void LoadLevel_1To4_NotNull()
        {
            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level, $"Level {i} should load successfully");
            }
        }

        [Test]
        public void AllLevels_Are4x4()
        {
            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.AreEqual(4, level.width, $"Level {i} width");
                Assert.AreEqual(4, level.height, $"Level {i} height");
            }
        }

        [Test]
        public void AllLevels_NestAndSea_InBounds()
        {
            for (int i = 1; i <= 4; i++)
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
            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                Assert.IsNotNull(level.tiles, $"Level {i} tiles not null");
                Assert.Greater(level.tiles.Length, 0, $"Level {i} should have tiles");
            }
        }

        [Test]
        public void Levels_HaveExpectedCollectibleCounts()
        {
            int[] expected = { 1, 2, 2, 3 };

            for (int i = 1; i <= 4; i++)
            {
                var level = LevelLoader.LoadLevel(i);
                int count = level.collectibles != null ? level.collectibles.Length : 0;
                Assert.AreEqual(expected[i - 1], count,
                    $"Level {i} collectible count");
            }
        }
    }
}
