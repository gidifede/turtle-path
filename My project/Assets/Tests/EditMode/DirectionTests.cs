using NUnit.Framework;
using UnityEngine;
using TurtlePath.Core;

namespace TurtlePath.Tests
{
    public class DirectionTests
    {
        [Test]
        public void RotateCW_FullCycle_N_E_S_W_N()
        {
            var dir = Direction.North;

            dir = dir.RotateCW();
            Assert.AreEqual(Direction.East, dir);

            dir = dir.RotateCW();
            Assert.AreEqual(Direction.South, dir);

            dir = dir.RotateCW();
            Assert.AreEqual(Direction.West, dir);

            dir = dir.RotateCW();
            Assert.AreEqual(Direction.North, dir);
        }

        [Test]
        public void Opposite_NorthSouth_EastWest()
        {
            Assert.AreEqual(Direction.South, Direction.North.Opposite());
            Assert.AreEqual(Direction.North, Direction.South.Opposite());
            Assert.AreEqual(Direction.West, Direction.East.Opposite());
            Assert.AreEqual(Direction.East, Direction.West.Opposite());
        }

        [Test]
        public void ToOffset_North_IsUp()
        {
            Assert.AreEqual(new Vector2Int(0, -1), Direction.North.ToOffset());
        }

        [Test]
        public void ToOffset_AllDirections_Correct()
        {
            Assert.AreEqual(new Vector2Int(0, -1), Direction.North.ToOffset());
            Assert.AreEqual(new Vector2Int(1, 0), Direction.East.ToOffset());
            Assert.AreEqual(new Vector2Int(0, 1), Direction.South.ToOffset());
            Assert.AreEqual(new Vector2Int(-1, 0), Direction.West.ToOffset());
        }
    }
}
