using UnityEngine;
using TurtlePath.Core;
using TurtlePath.Tiles;

namespace TurtlePath.Grid
{
    public class Cell
    {
        public Vector2Int GridPosition { get; private set; }
        public CellType CellType { get; private set; }
        public Tile Tile { get; set; }

        public Cell(Vector2Int gridPosition, CellType cellType)
        {
            GridPosition = gridPosition;
            CellType = cellType;
        }

        public Direction[] GetPorts()
        {
            if (CellType == CellType.Nest)
                return new[] { Direction.South };
            if (CellType == CellType.Sea)
                return new[] { Direction.North };
            if (Tile != null)
                return Tile.GetPorts();
            return new Direction[0];
        }

        public bool HasPort(Direction dir)
        {
            if (CellType == CellType.Nest)
                return dir == Direction.South;
            if (CellType == CellType.Sea)
                return dir == Direction.North;
            if (Tile != null)
                return Tile.HasPort(dir);
            return false;
        }

        public bool IsRotatable()
        {
            if (CellType == CellType.Nest || CellType == CellType.Sea)
                return false;
            if (CellType == CellType.Rock || CellType == CellType.Hole)
                return false;
            if (Tile == null)
                return false;
            return !Tile.IsFixed;
        }
    }
}
