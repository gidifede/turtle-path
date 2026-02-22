using System.Collections.Generic;
using TurtlePath.Core;

namespace TurtlePath.Tiles
{
    public class Tile
    {
        public TileType Type { get; private set; }
        public int Rotation { get; private set; } // 0, 90, 180, 270
        public bool IsFixed { get; private set; }

        // Base ports at rotation 0 for each tile type
        private static readonly Dictionary<TileType, Direction[]> BasePorts = new Dictionary<TileType, Direction[]>
        {
            { TileType.Straight, new[] { Direction.North, Direction.South } },
            { TileType.Curve,    new[] { Direction.North, Direction.East } },
            { TileType.T,        new[] { Direction.North, Direction.East, Direction.South } }
        };

        public Tile(TileType type, int rotation = 0, bool isFixed = false)
        {
            Type = type;
            Rotation = rotation % 360;
            IsFixed = isFixed;
        }

        public Direction[] GetPorts()
        {
            Direction[] basePorts = BasePorts[Type];
            int steps = Rotation / 90;
            Direction[] rotatedPorts = new Direction[basePorts.Length];

            for (int i = 0; i < basePorts.Length; i++)
            {
                Direction d = basePorts[i];
                for (int s = 0; s < steps; s++)
                    d = d.RotateCW();
                rotatedPorts[i] = d;
            }

            return rotatedPorts;
        }

        public bool HasPort(Direction dir)
        {
            Direction[] ports = GetPorts();
            for (int i = 0; i < ports.Length; i++)
            {
                if (ports[i] == dir)
                    return true;
            }
            return false;
        }

        public void RotateCW()
        {
            if (!IsFixed)
                Rotation = (Rotation + 90) % 360;
        }
    }
}
