using TurtlePath.Core;
using UnityEngine;

namespace TurtlePath.Collectibles
{
    public class Collectible
    {
        public CollectibleType Type { get; private set; }
        public Vector2Int Position { get; private set; }
        public bool Collected { get; set; }

        public Collectible(CollectibleType type, Vector2Int position)
        {
            Type = type;
            Position = position;
            Collected = false;
        }
    }
}
