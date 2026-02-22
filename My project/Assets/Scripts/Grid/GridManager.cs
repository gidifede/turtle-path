using System.Collections.Generic;
using UnityEngine;
using TurtlePath.Core;
using TurtlePath.Level;
using TurtlePath.Tiles;
using TurtlePath.Collectibles;

namespace TurtlePath.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Settings")]
        public float cellSize = 1.0f;

        [Header("Cell Sprites")]
        public Sprite cellSprite;              // White square for normal cells (tinted Sand Light)
        public Sprite cellSprite_nest;         // Custom sprite for nest cell (colored in PNG)
        public Sprite cellSprite_sea;          // Custom sprite for sea cell (colored in PNG)

        [Header("Tile Sprites")]
        public Sprite tileSprite_straight;     // 64x64 sand path straight (from Pipes col 1)
        public Sprite tileSprite_curve;        // 64x64 sand path curve (from Pipes col 2)

        [Header("Collectible Sprites")]
        public Sprite collectibleSprite_shell; // 32x32 Coral Pink shell (custom pixel art)
        public Sprite collectibleSprite_baby;  // 32x32 Baby Pink turtle (custom pixel art)

        private Cell[,] cells;
        private int width, height;
        private float gridOriginX, gridOriginY;
        private List<GameObject> spawnedObjects = new List<GameObject>();
        private Dictionary<Vector2Int, TileView> tileViews = new Dictionary<Vector2Int, TileView>();
        private Dictionary<Vector2Int, CollectibleView> collectibleViews = new Dictionary<Vector2Int, CollectibleView>();

        // Colors
        private static readonly Color NestColor = new Color(0.4f, 0.8f, 0.4f);
        private static readonly Color SeaColor = new Color(0.3f, 0.5f, 0.9f);
        private static readonly Color EmptyColor = new Color(1f, 0.933f, 0.678f); // Sand Light #FFEEAD

        public System.Action<TileView> OnTileClicked;

        public void BuildGrid(LevelData levelData)
        {
            width = levelData.width;
            height = levelData.height;

            gridOriginX = -(width * cellSize) / 2f + cellSize / 2f;
            gridOriginY = (height * cellSize) / 2f - cellSize / 2f;

            cells = new Cell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(new Vector2Int(x, y), CellType.Normal);
                }
            }

            cells[levelData.nestPos.x, levelData.nestPos.y] = new Cell(levelData.nestPos, CellType.Nest);
            cells[levelData.seaPos.x, levelData.seaPos.y] = new Cell(levelData.seaPos, CellType.Sea);

            for (int i = 0; i < levelData.tiles.Length; i++)
            {
                TileEntry entry = levelData.tiles[i];
                Vector2Int pos = entry.position;
                Tile tile = new Tile(entry.type, entry.rotation, entry.isFixed);
                cells[pos.x, pos.y].Tile = tile;
            }

            SpawnCellVisuals();
            SpawnCollectibles(levelData);
        }

        private void SpawnCellVisuals()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = cells[x, y];
                    Vector3 worldPos = GridToWorld(x, y);

                    GameObject cellGO = new GameObject($"Cell_{x}_{y}");
                    cellGO.transform.position = worldPos;
                    cellGO.transform.localScale = Vector3.one * cellSize * 0.95f;

                    SpriteRenderer sr = cellGO.AddComponent<SpriteRenderer>();
                    sr.sprite = cellSprite;
                    sr.sortingOrder = 0;

                    switch (cell.CellType)
                    {
                        case CellType.Nest:
                            sr.sprite = cellSprite_nest ?? cellSprite;
                            sr.color = cellSprite_nest != null ? Color.white : NestColor;
                            break;
                        case CellType.Sea:
                            sr.sprite = cellSprite_sea ?? cellSprite;
                            sr.color = cellSprite_sea != null ? Color.white : SeaColor;
                            break;
                        default:
                            sr.color = EmptyColor;
                            break;
                    }

                    spawnedObjects.Add(cellGO);

                    if (cell.Tile != null)
                    {
                        SpawnTileView(cell, worldPos);
                    }
                }
            }
        }

        private void SpawnTileView(Cell cell, Vector3 worldPos)
        {
            GameObject tileGO = new GameObject($"Tile_{cell.GridPosition.x}_{cell.GridPosition.y}");
            tileGO.transform.position = worldPos;

            TileView view = tileGO.AddComponent<TileView>();
            Sprite tileSprite = GetTileSpriteForType(cell.Tile.Type);
            view.Initialize(cell, cellSize, tileSprite);
            view.OnClicked += HandleTileClicked;

            tileViews[cell.GridPosition] = view;
            spawnedObjects.Add(tileGO);
        }

        private Sprite GetTileSpriteForType(TileType type)
        {
            switch (type)
            {
                case TileType.Straight: return tileSprite_straight ?? cellSprite;
                case TileType.Curve:    return tileSprite_curve ?? cellSprite;
                default:                return cellSprite;
            }
        }

        private void SpawnCollectibles(LevelData levelData)
        {
            if (levelData.collectibles == null) return;

            for (int i = 0; i < levelData.collectibles.Length; i++)
            {
                CollectibleEntry entry = levelData.collectibles[i];
                Vector3 worldPos = GridToWorld(entry.position.x, entry.position.y);

                GameObject collectGO = new GameObject($"Collectible_{entry.type}_{entry.position.x}_{entry.position.y}");
                collectGO.transform.position = worldPos;

                CollectibleView view = collectGO.AddComponent<CollectibleView>();
                Sprite sprite = entry.type == CollectibleType.Shell ? collectibleSprite_shell : collectibleSprite_baby;
                bool useColorTint = sprite == null;
                if (sprite == null) sprite = cellSprite;
                view.Initialize(entry.type, entry.position, sprite, useColorTint);

                collectibleViews[entry.position] = view;
                spawnedObjects.Add(collectGO);
            }
        }

        private void HandleTileClicked(TileView view)
        {
            OnTileClicked?.Invoke(view);
        }

        public void UpdateCellVisuals(HashSet<Vector2Int> connected)
        {
            foreach (var kvp in tileViews)
            {
                bool isConnected = connected.Contains(kvp.Key);
                kvp.Value.SetConnected(isConnected);
            }
        }

        public CollectibleView GetCollectible(Vector2Int pos)
        {
            if (collectibleViews.TryGetValue(pos, out CollectibleView view))
            {
                if (view != null && !view.IsCollected)
                    return view;
            }
            return null;
        }

        public Vector3 GridToWorld(int x, int y)
        {
            float worldX = gridOriginX + x * cellSize;
            float worldY = gridOriginY - y * cellSize;
            return new Vector3(worldX, worldY, 0f);
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - gridOriginX) / cellSize);
            int y = Mathf.RoundToInt((gridOriginY - worldPos.y) / cellSize);
            return new Vector2Int(x, y);
        }

        public Cell GetCell(int x, int y)
        {
            if (!IsInBounds(x, y)) return null;
            return cells[x, y];
        }

        public Cell GetNeighbor(Cell cell, Direction dir)
        {
            Vector2Int offset = dir.ToOffset();
            int nx = cell.GridPosition.x + offset.x;
            int ny = cell.GridPosition.y + offset.y;
            return GetCell(nx, ny);
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public int Width => width;
        public int Height => height;

        public void ClearGrid()
        {
            foreach (var kvp in tileViews)
            {
                if (kvp.Value != null)
                    kvp.Value.OnClicked -= HandleTileClicked;
            }
            tileViews.Clear();
            collectibleViews.Clear();

            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (spawnedObjects[i] != null)
                    Destroy(spawnedObjects[i]);
            }
            spawnedObjects.Clear();
            cells = null;
        }
    }
}
