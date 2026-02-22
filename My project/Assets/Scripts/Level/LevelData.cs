using System;
using UnityEngine;
using TurtlePath.Core;

namespace TurtlePath.Level
{
    [Serializable]
    public class LevelData
    {
        public int id;
        public int width;
        public int height;
        public Vector2Int nestPos;
        public Vector2Int seaPos;
        public TileEntry[] tiles;
        public CollectibleEntry[] collectibles;
    }

    [Serializable]
    public class CollectibleEntry
    {
        public CollectibleType type;
        public Vector2Int position;

        public CollectibleEntry(CollectibleType type, Vector2Int position)
        {
            this.type = type;
            this.position = position;
        }
    }

    [Serializable]
    public class TileEntry
    {
        public TileType type;
        public Vector2Int position;
        public int rotation;
        public bool isFixed;

        public TileEntry(TileType type, Vector2Int position, int rotation = 0, bool isFixed = false)
        {
            this.type = type;
            this.position = position;
            this.rotation = rotation;
            this.isFixed = isFixed;
        }
    }
}
