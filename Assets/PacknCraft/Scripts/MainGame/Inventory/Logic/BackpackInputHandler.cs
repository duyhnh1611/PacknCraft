using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace PacknCraft.Inventory.UI
{
    public class BackpackInputHandler : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private BackpackUI backpackUI;

        [Header("Items")]
        [SerializeField] private BackpackDragView dragView;
        [SerializeField] private List<ItemConfig> itemConfigs;

        [Header("Drag Settings")]
        [SerializeField] private float holdThreshold = 0.15f;

        // PRIVATE FIELDS
        private List<ItemData> items;
        private ItemData currentItem;

        private PlacedItem draggingItem;
        private Vector2Int originalPos;
        private ItemRotation originalRotation;

        private float pressTime;
        private bool isHolding;

        // UNITY METHODS
        private void Awake()
        {
            items = new List<ItemData>();

            foreach (var config in itemConfigs)
                items.Add(new ItemData(config));

            PickRandomItem();
        }

        private void Update()
        {
            Vector2 pointerPos = GetPointerPosition();

            HandlePress(pointerPos);
            HandleHold(pointerPos);
            HandleRelease(pointerPos);
            HandleHover(pointerPos);

            dragView.UpdatePosition(pointerPos);
        }

        // PRIVATE METHODS
        private void PickRandomItem()
        {
            if (items.Count == 0)
            {
                currentItem = null;
                return;
            }

            currentItem = items[Random.Range(0, items.Count)];
        }

        private Vector2 GetPointerPosition()
        {
            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();

            if (Touchscreen.current != null)
                return Touchscreen.current.primaryTouch.position.ReadValue();

            return Vector2.zero;
        }

        private bool IsPressedDown()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                return true;

            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                return true;

            return false;
        }

        private bool IsPressed()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                return true;

            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.isPressed)
                return true;

            return false;
        }

        private bool IsReleased()
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
                return true;

            if (Touchscreen.current != null &&
                Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
                return true;

            return false;
        }

        private void HandlePress(Vector2 screenPos)
        {
            if (!IsPressedDown()) return;

            var cell = RaycastCell(screenPos);

            if (cell != null)
            {
                Vector2Int pos = cell.GetPosition();
                draggingItem = backpackUI.GetItemAt(pos);
            }
            else
            {
                draggingItem = null;
            }

            pressTime = Time.time;
            isHolding = false;
        }

        private void HandleHold(Vector2 screenPos)
        {
            if (draggingItem == null) return;
            if (!IsPressed()) return;
            if (isHolding) return;

            if (Time.time - pressTime >= holdThreshold)
            {
                isHolding = true;

                originalPos = draggingItem.Position;
                originalRotation = draggingItem.Data.Rotation;

                backpackUI.RemoveItem(draggingItem);

                dragView.Show(draggingItem.Data);
            }
        }

        private void HandleRelease(Vector2 screenPos)
        {
            if (!IsReleased()) return;

            dragView.Hide();

            var cell = RaycastCell(screenPos);

            if (draggingItem != null && isHolding)
            {
                if (cell != null)
                {
                    Vector2Int pos = cell.GetPosition();

                    bool success = backpackUI.TryAddItem(draggingItem.Data, pos);

                    if (!success)
                    {
                        draggingItem.Data.Rotation = originalRotation;

                        Vector2Int originalCenter = new Vector2Int(
                            originalPos.x + draggingItem.Data.Size / 2,
                            originalPos.y + draggingItem.Data.Size / 2
                        );

                        backpackUI.TryAddItem(draggingItem.Data, originalCenter);
                    }
                }
                else
                {
                    draggingItem.Data.Rotation = originalRotation;

                    Vector2Int originalCenter = new Vector2Int(
                        originalPos.x + draggingItem.Data.Size / 2,
                        originalPos.y + draggingItem.Data.Size / 2
                    );

                    backpackUI.TryAddItem(draggingItem.Data, originalCenter);
                }

                draggingItem = null;
                return;
            }

            if (cell != null)
            {
                Vector2Int pos = cell.GetPosition();
                var item = backpackUI.GetItemAt(pos);

                if (item != null && !isHolding)
                {
                    backpackUI.TryRotateItem(item);
                }
                else if (item == null)
                {
                    if (currentItem != null)
                    {
                        bool success = backpackUI.TryAddItem(currentItem, pos);

                        if (success)
                            PickRandomItem();
                    }
                }
            }

            draggingItem = null;
        }

        private void HandleHover(Vector2 screenPos)
        {
            var cell = RaycastCell(screenPos);

            if (cell == null)
            {
                backpackUI.ClearPreview();
                return;
            }

            Vector2Int pos = cell.GetPosition();

            if (draggingItem != null)
            {
                backpackUI.ShowPreview(draggingItem.Data, pos);
            }
            else if (currentItem != null)
            {
                backpackUI.ShowPreview(currentItem, pos);
            }
        }

        private BackpackCellViewBase RaycastCell(Vector2 screenPos)
        {
            var results = Raycast(screenPos);

            foreach (var hit in results)
            {
                var cell = hit.gameObject.GetComponentInParent<BackpackCellViewBase>();
                if (cell != null)
                    return cell;
            }

            return null;
        }

        private List<RaycastResult> Raycast(Vector2 screenPos)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = screenPos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results;
        }
    }
}