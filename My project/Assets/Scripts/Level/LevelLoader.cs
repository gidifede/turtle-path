using System;
using UnityEngine;
using TurtlePath.Core;

namespace TurtlePath.Level
{
    public static class LevelLoader
    {
        private const int TotalLevels = 4;

        public static int GetTotalLevels() => TotalLevels;

        public static LevelData LoadLevel(int levelId)
        {
            string path = $"Levels/level_{levelId:D2}";
            TextAsset textAsset = Resources.Load<TextAsset>(path);

            if (textAsset == null)
            {
                Debug.LogError($"Level file not found: {path}");
                return null;
            }

            JsonLevelData json = JsonUtility.FromJson<JsonLevelData>(textAsset.text);
            return ConvertToLevelData(json);
        }

        private static LevelData ConvertToLevelData(JsonLevelData json)
        {
            LevelData level = new LevelData
            {
                id = json.id,
                width = json.gridSize.width,
                height = json.gridSize.height,
                nestPos = new Vector2Int(json.nest.x, json.nest.y),
                seaPos = new Vector2Int(json.sea.x, json.sea.y)
            };

            // Convert tiles
            level.tiles = new TileEntry[json.tiles.Length];
            for (int i = 0; i < json.tiles.Length; i++)
            {
                JsonTileEntry jt = json.tiles[i];
                TileType tileType = ParseTileType(jt.type);
                level.tiles[i] = new TileEntry(
                    tileType,
                    new Vector2Int(jt.position.x, jt.position.y),
                    jt.rotation,
                    jt.isFixed
                );
            }

            // Convert collectibles
            if (json.collectibles != null && json.collectibles.Length > 0)
            {
                level.collectibles = new CollectibleEntry[json.collectibles.Length];
                for (int i = 0; i < json.collectibles.Length; i++)
                {
                    JsonCollectibleEntry jc = json.collectibles[i];
                    CollectibleType collectibleType = ParseCollectibleType(jc.type);
                    level.collectibles[i] = new CollectibleEntry(
                        collectibleType,
                        new Vector2Int(jc.position.x, jc.position.y)
                    );
                }
            }
            else
            {
                level.collectibles = new CollectibleEntry[0];
            }

            return level;
        }

        private static TileType ParseTileType(string type)
        {
            switch (type.ToLower())
            {
                case "straight": return TileType.Straight;
                case "curve": return TileType.Curve;
                case "t": return TileType.T;
                default:
                    Debug.LogWarning($"Unknown tile type: {type}");
                    return TileType.Straight;
            }
        }

        private static CollectibleType ParseCollectibleType(string type)
        {
            switch (type.ToLower())
            {
                case "shell": return CollectibleType.Shell;
                case "baby_turtle": return CollectibleType.BabyTurtle;
                default:
                    Debug.LogWarning($"Unknown collectible type: {type}");
                    return CollectibleType.Shell;
            }
        }

        public static void RandomizeTileRotations(LevelData level)
        {
            for (int i = 0; i < level.tiles.Length; i++)
            {
                if (!level.tiles[i].isFixed)
                {
                    level.tiles[i].rotation = UnityEngine.Random.Range(0, 4) * 90;
                }
            }
        }

        // JSON intermediate classes
        [Serializable]
        private class JsonLevelData
        {
            public int id;
            public JsonGridSize gridSize;
            public JsonVec2 nest;
            public JsonVec2 sea;
            public JsonTileEntry[] tiles;
            public JsonCollectibleEntry[] collectibles;
        }

        [Serializable]
        private class JsonGridSize
        {
            public int width;
            public int height;
        }

        [Serializable]
        private class JsonVec2
        {
            public int x;
            public int y;
        }

        [Serializable]
        private class JsonTileEntry
        {
            public string type;
            public JsonVec2 position;
            public int rotation;
            public bool isFixed;
        }

        [Serializable]
        private class JsonCollectibleEntry
        {
            public string type;
            public JsonVec2 position;
        }
    }
}
