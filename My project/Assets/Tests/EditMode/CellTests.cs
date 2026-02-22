using NUnit.Framework;
using UnityEngine;
using TurtlePath.Core;
using TurtlePath.Grid;
using TurtlePath.Tiles;

namespace TurtlePath.Tests
{
    public class CellTests
    {
        [Test]
        public void Nest_HasOnly_SouthPort()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Nest);
            var ports = cell.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.South }, ports);
        }

        [Test]
        public void Sea_HasOnly_NorthPort()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Sea);
            var ports = cell.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.North }, ports);
        }

        [Test]
        public void Normal_WithTile_DelegatesToTile()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Normal);
            cell.Tile = new Tile(TileType.Straight, 0);
            var ports = cell.GetPorts();

            CollectionAssert.AreEquivalent(
                new[] { Direction.North, Direction.South }, ports);
        }

        [Test]
        public void Normal_WithoutTile_HasNoPorts()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Normal);
            var ports = cell.GetPorts();

            Assert.AreEqual(0, ports.Length);
        }

        [Test]
        public void IsRotatable_NestAndSea_ReturnFalse()
        {
            var nest = new Cell(Vector2Int.zero, CellType.Nest);
            var sea = new Cell(Vector2Int.zero, CellType.Sea);

            Assert.IsFalse(nest.IsRotatable());
            Assert.IsFalse(sea.IsRotatable());
        }

        [Test]
        public void IsRotatable_NormalWithNonFixedTile_ReturnsTrue()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Normal);
            cell.Tile = new Tile(TileType.Curve, 0, isFixed: false);

            Assert.IsTrue(cell.IsRotatable());
        }

        [Test]
        public void IsRotatable_NormalWithFixedTile_ReturnsFalse()
        {
            var cell = new Cell(Vector2Int.zero, CellType.Normal);
            cell.Tile = new Tile(TileType.Curve, 0, isFixed: true);

            Assert.IsFalse(cell.IsRotatable());
        }
    }
}
