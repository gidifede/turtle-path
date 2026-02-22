using UnityEngine;

namespace TurtlePath.Core
{
    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public static class DirectionExtensions
    {
        public static Direction RotateCW(this Direction dir)
        {
            return (Direction)(((int)dir + 1) % 4);
        }

        public static Direction Opposite(this Direction dir)
        {
            return (Direction)(((int)dir + 2) % 4);
        }

        public static Vector2Int ToOffset(this Direction dir)
        {
            switch (dir)
            {
                case Direction.North: return new Vector2Int(0, -1);
                case Direction.East:  return new Vector2Int(1, 0);
                case Direction.South: return new Vector2Int(0, 1);
                case Direction.West:  return new Vector2Int(-1, 0);
                default:              return Vector2Int.zero;
            }
        }
    }
}
