using NUnit.Framework;
using TurtlePath.Core;
using TurtlePath.Tiles;

namespace TurtlePath.Tests
{
    public class TileTests
    {
        [Test]
        public void Straight_Rot0_HasPorts_NorthSouth()
        {
            var tile = new Tile(TileType.Straight, 0);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.North, Direction.South }, ports);
        }

        [Test]
        public void Straight_Rot90_HasPorts_EastWest()
        {
            var tile = new Tile(TileType.Straight, 90);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.East, Direction.West }, ports);
        }

        [Test]
        public void Curve_Rot0_HasPorts_NorthEast()
        {
            var tile = new Tile(TileType.Curve, 0);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.North, Direction.East }, ports);
        }

        [Test]
        public void Curve_Rot90_HasPorts_EastSouth()
        {
            var tile = new Tile(TileType.Curve, 90);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.East, Direction.South }, ports);
        }

        [Test]
        public void Curve_Rot180_HasPorts_SouthWest()
        {
            var tile = new Tile(TileType.Curve, 180);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.South, Direction.West }, ports);
        }

        [Test]
        public void Curve_Rot270_HasPorts_WestNorth()
        {
            var tile = new Tile(TileType.Curve, 270);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.West, Direction.North }, ports);
        }

        [Test]
        public void RotateCW_CyclesThrough_0_90_180_270_0()
        {
            var tile = new Tile(TileType.Straight, 0);

            Assert.AreEqual(0, tile.Rotation);

            tile.RotateCW();
            Assert.AreEqual(90, tile.Rotation);

            tile.RotateCW();
            Assert.AreEqual(180, tile.Rotation);

            tile.RotateCW();
            Assert.AreEqual(270, tile.Rotation);

            tile.RotateCW();
            Assert.AreEqual(0, tile.Rotation);
        }

        [Test]
        public void Fixed_Tile_DoesNotRotate()
        {
            var tile = new Tile(TileType.Curve, 90, isFixed: true);

            tile.RotateCW();

            Assert.AreEqual(90, tile.Rotation);
        }

        [Test]
        public void HasPort_ReturnsTrue_ForValidPort()
        {
            var tile = new Tile(TileType.Straight, 0);

            Assert.IsTrue(tile.HasPort(Direction.North));
            Assert.IsTrue(tile.HasPort(Direction.South));
        }

        [Test]
        public void HasPort_ReturnsFalse_ForInvalidPort()
        {
            var tile = new Tile(TileType.Straight, 0);

            Assert.IsFalse(tile.HasPort(Direction.East));
            Assert.IsFalse(tile.HasPort(Direction.West));
        }

        [Test]
        public void T_Rot0_HasPorts_NorthEastSouth()
        {
            var tile = new Tile(TileType.T, 0);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.North, Direction.East, Direction.South }, ports);
        }

        [Test]
        public void T_Rot90_HasPorts_EastSouthWest()
        {
            var tile = new Tile(TileType.T, 90);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.East, Direction.South, Direction.West }, ports);
        }

        [Test]
        public void T_Rot180_HasPorts_SouthWestNorth()
        {
            var tile = new Tile(TileType.T, 180);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.South, Direction.West, Direction.North }, ports);
        }

        [Test]
        public void T_Rot270_HasPorts_WestNorthEast()
        {
            var tile = new Tile(TileType.T, 270);
            var ports = tile.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.West, Direction.North, Direction.East }, ports);
        }
    }
}
