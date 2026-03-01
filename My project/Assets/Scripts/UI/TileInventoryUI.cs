using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TurtlePath.Core;
using TurtlePath.Grid;
using TurtlePath.Level;

namespace TurtlePath.UI
{
    public class TileInventoryUI : MonoBehaviour
    {
        public GridManager gridManager;
        public Camera mainCamera;
        public RectTransform slotContainer;

        public event Action OnInventoryChanged;

        private List<InventorySlot> slots = new List<InventorySlot>();
        private InventorySlot dragSlot;
        private GameObject dragVisual;
        private bool isDragging;
        private Canvas parentCanvas;

        private static readonly Color SlotBGColor = new Color(0.922f, 0.898f, 0.882f); // BG Off-White
        private static readonly Color DragColor = new Color(1f, 1f, 1f, 0.8f);

        public void Setup(InventoryEntry[] entries, GridManager gm, Camera cam)
        {
            gridManager = gm;
            mainCamera = cam;
            Clear();

            // Cache parent canvas for drag visual
            parentCanvas = GetComponentInParent<Canvas>();

            if (entries == null || entries.Length == 0) return;

            for (int i = 0; i < entries.Length; i++)
            {
                AddSlot(entries[i].type);
            }
        }

        private void AddSlot(TileType type)
        {
            GameObject slotGO = new GameObject($"Slot_{slots.Count}");
            slotGO.transform.SetParent(slotContainer, false);

            RectTransform rect = slotGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(64, 64);

            Image bg = slotGO.AddComponent<Image>();
            bg.color = SlotBGColor;

            // Tile icon child
            GameObject iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(slotGO.transform, false);
            RectTransform iconRect = iconGO.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(4, 4);
            iconRect.offsetMax = new Vector2(-4, -4);

            Image icon = iconGO.AddComponent<Image>();
            Sprite sprite = GetSpriteForType(type);
            if (sprite != null)
                icon.sprite = sprite;
            icon.preserveAspect = true;

            InventorySlot slot = new InventorySlot
            {
                type = type,
                slotGO = slotGO,
                icon = icon
            };
            slots.Add(slot);

            // Add drag trigger — capture slot reference directly, not index
            EventTrigger trigger = slotGO.AddComponent<EventTrigger>();
            InventorySlot capturedSlot = slot;

            EventTrigger.Entry beginDrag = new EventTrigger.Entry();
            beginDrag.eventID = EventTriggerType.BeginDrag;
            beginDrag.callback.AddListener((data) => OnBeginDragSlot(capturedSlot, (PointerEventData)data));
            trigger.triggers.Add(beginDrag);

            EventTrigger.Entry drag = new EventTrigger.Entry();
            drag.eventID = EventTriggerType.Drag;
            drag.callback.AddListener((data) => OnDragSlot((PointerEventData)data));
            trigger.triggers.Add(drag);

            EventTrigger.Entry endDrag = new EventTrigger.Entry();
            endDrag.eventID = EventTriggerType.EndDrag;
            endDrag.callback.AddListener((data) => OnEndDragSlot((PointerEventData)data));
            trigger.triggers.Add(endDrag);
        }

        public void ReturnTile(TileType type)
        {
            AddSlot(type);
            OnInventoryChanged?.Invoke();
        }

        public int RemainingCount()
        {
            return slots.Count;
        }

        private void OnBeginDragSlot(InventorySlot slot, PointerEventData eventData)
        {
            if (slot == null || slot.slotGO == null) return;
            if (!slots.Contains(slot)) return;

            dragSlot = slot;
            isDragging = true;

            // Create UI-based drag visual (same Canvas layer, always visible)
            CreateDragVisual(slot.type);
            UpdateDragPosition(eventData);

            // Hide the slot
            dragSlot.slotGO.SetActive(false);
        }

        private void CreateDragVisual(TileType type)
        {
            dragVisual = new GameObject("DragVisual");

            // Parent to Canvas root so it renders on top of everything
            if (parentCanvas != null)
            {
                dragVisual.transform.SetParent(parentCanvas.transform, false);
            }

            RectTransform rect = dragVisual.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(64, 64);

            Image img = dragVisual.AddComponent<Image>();
            Sprite sprite = GetSpriteForType(type);
            if (sprite != null)
                img.sprite = sprite;
            img.color = DragColor;
            img.preserveAspect = true;
            img.raycastTarget = false; // Don't block raycasts

            // Render on top of other UI
            Canvas dragCanvas = dragVisual.AddComponent<Canvas>();
            dragCanvas.overrideSorting = true;
            dragCanvas.sortingOrder = 200;
        }

        private void OnDragSlot(PointerEventData eventData)
        {
            if (!isDragging) return;
            UpdateDragPosition(eventData);
        }

        private void OnEndDragSlot(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;

            // Convert screen position to world position for grid check
            Vector3 screenPos = new Vector3(eventData.position.x, eventData.position.y, 0f);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;
            Vector2Int gridPos = gridManager.WorldToGrid(worldPos);

            bool placed = false;
            if (gridManager.CanPlaceTile(gridPos))
            {
                var view = gridManager.PlaceInventoryTile(gridPos, dragSlot.type);
                if (view != null)
                {
                    placed = true;

                    // Remove slot from list
                    slots.Remove(dragSlot);
                    Destroy(dragSlot.slotGO);
                    OnInventoryChanged?.Invoke();
                }
            }

            if (!placed && dragSlot != null && dragSlot.slotGO != null)
            {
                // Snap back — show slot again
                dragSlot.slotGO.SetActive(true);
            }

            if (dragVisual != null)
                Destroy(dragVisual);
            dragVisual = null;
            dragSlot = null;
        }

        private void UpdateDragPosition(PointerEventData eventData)
        {
            if (dragVisual == null) return;

            // Position the UI drag visual at the pointer location
            RectTransform dragRect = dragVisual.GetComponent<RectTransform>();
            if (parentCanvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    eventData.position,
                    parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
                    out Vector2 localPoint);
                dragRect.localPosition = localPoint;
            }
        }

        private Sprite GetSpriteForType(TileType type)
        {
            if (gridManager == null) return null;
            switch (type)
            {
                case TileType.Straight: return gridManager.tileSprite_straight;
                case TileType.Curve:    return gridManager.tileSprite_curve;
                case TileType.T:        return gridManager.tileSprite_t;
                default: return null;
            }
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                if (slot.slotGO != null)
                    Destroy(slot.slotGO);
            }
            slots.Clear();

            if (dragVisual != null)
                Destroy(dragVisual);
            dragVisual = null;
            isDragging = false;
            dragSlot = null;
        }

        private class InventorySlot
        {
            public TileType type;
            public GameObject slotGO;
            public Image icon;
        }
    }
}
