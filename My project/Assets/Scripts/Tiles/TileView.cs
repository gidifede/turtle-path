using System;
using UnityEngine;
using DG.Tweening;
using TurtlePath.Core;
using TurtlePath.Grid;

namespace TurtlePath.Tiles
{
    public class TileView : MonoBehaviour
    {
        public event Action<TileView> OnClicked;

        /// <summary>Set to true to show red port indicators (debug). Default: hidden for M2-Bis.</summary>
        public static bool ShowPortIndicators = false;

        private Cell cell;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer connectionOverlay;
        private bool isAnimating;
        private float cellSize;
        private float cumulativeRotationZ;

        private static readonly Color ConnectedOverlayColor = new Color(0.59f, 0.81f, 0.71f, 0.5f); // Ocean Teal #96CEB4 alpha 50%
        private static readonly Color PortColor = new Color(0.8f, 0.2f, 0.2f);         // Red for ports

        public Cell Cell => cell;
        public bool FromInventory { get; set; }

        public void Initialize(Cell cell, float cellSize, Sprite sprite)
        {
            this.cell = cell;
            this.cellSize = cellSize;

            // Main tile sprite
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = 1;
            transform.localScale = Vector3.one * cellSize * 0.9f;

            // Collider for input
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            // Connection overlay (child SpriteRenderer, toggled on/off)
            GameObject overlayGO = new GameObject("ConnectionOverlay");
            overlayGO.transform.SetParent(transform, false);
            SpriteRenderer overlaySR = overlayGO.AddComponent<SpriteRenderer>();
            overlaySR.sprite = sprite;
            overlaySR.color = ConnectedOverlayColor;
            overlaySR.sortingOrder = 2;
            connectionOverlay = overlaySR;
            overlayGO.SetActive(false);

            // Create port indicators at base positions (children rotate with parent)
            if (ShowPortIndicators)
                CreatePortIndicators(sprite);

            // Apply initial rotation
            cumulativeRotationZ = -cell.Tile.Rotation;
            transform.rotation = Quaternion.Euler(0, 0, cumulativeRotationZ);
        }

        private void CreatePortIndicators(Sprite sprite)
        {
            // Get base ports (rotation 0) — indicators are placed at base positions
            // and rotate visually with the parent transform
            Direction[] basePorts = GetBasePorts();

            foreach (Direction dir in basePorts)
            {
                GameObject portGO = new GameObject($"Port_{dir}");
                portGO.transform.SetParent(transform, false);

                SpriteRenderer portSR = portGO.AddComponent<SpriteRenderer>();
                portSR.sprite = sprite;
                portSR.color = PortColor;
                portSR.sortingOrder = 2;

                // Position and scale port indicator on the edge
                switch (dir)
                {
                    case Direction.North:
                        portGO.transform.localPosition = new Vector3(0, 0.4f, 0);
                        portGO.transform.localScale = new Vector3(0.4f, 0.1f, 1f);
                        break;
                    case Direction.South:
                        portGO.transform.localPosition = new Vector3(0, -0.4f, 0);
                        portGO.transform.localScale = new Vector3(0.4f, 0.1f, 1f);
                        break;
                    case Direction.East:
                        portGO.transform.localPosition = new Vector3(0.4f, 0, 0);
                        portGO.transform.localScale = new Vector3(0.1f, 0.4f, 1f);
                        break;
                    case Direction.West:
                        portGO.transform.localPosition = new Vector3(-0.4f, 0, 0);
                        portGO.transform.localScale = new Vector3(0.1f, 0.4f, 1f);
                        break;
                }
            }
        }

        private Direction[] GetBasePorts()
        {
            switch (cell.Tile.Type)
            {
                case TileType.Straight: return new[] { Direction.North, Direction.South };
                case TileType.Curve:    return new[] { Direction.North, Direction.East };
                case TileType.T:        return new[] { Direction.North, Direction.East, Direction.South };
                default:                return new Direction[0];
            }
        }

        private void OnMouseDown()
        {
            if (isAnimating) return;
            if (cell == null) return;
            if (!cell.IsRotatable()) return;

            OnClicked?.Invoke(this);
        }

        public void AnimateRotation()
        {
            if (isAnimating) return;

            // Update data
            cell.Tile.RotateCW();

            // Animate visual using cumulative rotation to avoid wrap-around issues
            isAnimating = true;
            cumulativeRotationZ -= 90f;

            transform.DORotate(new Vector3(0, 0, cumulativeRotationZ), 0.15f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => isAnimating = false);
        }

        public void SetConnected(bool connected)
        {
            if (connectionOverlay != null)
                connectionOverlay.gameObject.SetActive(connected);
        }

        public bool IsAnimating => isAnimating;
    }
}
