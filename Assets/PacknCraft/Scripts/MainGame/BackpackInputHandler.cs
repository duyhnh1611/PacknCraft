using Cysharp.Threading.Tasks;
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
        [SerializeField] private LootUI lootUI;

        [Header("Items")]
        [SerializeField] private BackpackDragView dragView;

        [Header("Drag Settings")]
        [SerializeField] private float holdThreshold = 0.15f;
        [SerializeField] private float dragThreshold = 10f;

        // PRIVATE FIELDS
        private PlacedItem draggingItem;
        private ItemData draggingLootItem;
        private bool isDraggingFromLoot;

        private Vector2Int originalPos;
        private ItemRotation originalRotation;

        private float pressTime;
        private bool isHolding;

        private Vector2 pressStartPos;
        private bool isPointerOverLoot;
        private bool blockLootDrag;

        private int originalLootIndex;

        private void Update()
        {
            Vector2 pointerPos = GetPointerPosition();

            HandlePress(pointerPos);
            HandleHold(pointerPos);
            HandleHover(pointerPos);
            HandleRelease(pointerPos);

            dragView.UpdatePosition(pointerPos);
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

            pressStartPos = screenPos;
            blockLootDrag = false;
            isPointerOverLoot = false;

            var results = Raycast(screenPos);

            draggingItem = null;
            draggingLootItem = null;
            isDraggingFromLoot = false;

            foreach (var hit in results)
            {
                var loot = hit.gameObject.GetComponentInParent<LootItemView>();
                if (loot != null)
                {
                    draggingLootItem = loot.Data;
                    isDraggingFromLoot = true;
                    isPointerOverLoot = true;

                    originalLootIndex = loot.transform.GetSiblingIndex();

                    pressTime = Time.time;
                    isHolding = false;
                    return;
                }
            }

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
            if (isDraggingFromLoot && draggingLootItem != null)
            {
                if (!IsPressed()) return;
                if (isHolding) return;

                float moveDist = Vector2.Distance(screenPos, pressStartPos);

                if (isPointerOverLoot)
                {
                    Vector2 dir = screenPos - pressStartPos;
                    bool isDraggingHoriz = moveDist > dragThreshold && Mathf.Abs(dir.x) > Mathf.Abs(dir.y);
                    bool isDraggingVert = moveDist > dragThreshold && Mathf.Abs(dir.y) > Mathf.Abs(dir.x);

                    // Drag vertical or hold long enough - drag item out of loot
                    if ((isDraggingVert || Time.time - pressTime >= holdThreshold) && !blockLootDrag)
                    {
                        isHolding = true;

                        lootUI.Remove(draggingLootItem);
                        lootUI.SetScrollEnabled(false);

                        dragView.Show(draggingLootItem);
                    }
                    // Drag horizontal - scroll loot view
                    else if (isDraggingHoriz)
                    {
                        blockLootDrag = true;
                        return;
                    }
                }
                return;
            }

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

        private async void HandleRelease(Vector2 screenPos)
        {
            if (!IsReleased()) return;

            dragView.Hide();
            backpackUI.ClearPreview();

            lootUI.SetScrollEnabled(true);

            var cell = RaycastCell(screenPos);

            if (isDraggingFromLoot && draggingLootItem != null)
            {
                if (isHolding && cell != null)
                {
                    Vector2Int pos = cell.GetPosition();

                    bool success = backpackUI.TryAddItem(draggingLootItem, pos);

                    if (!success)
                    {
                        var view = await lootUI.Add(draggingLootItem);
                        view.transform.SetSiblingIndex(originalLootIndex);
                    }
                }
                else if (isHolding)
                {
                    var view = await lootUI.Add(draggingLootItem);
                    view.transform.SetSiblingIndex(originalLootIndex);
                }

                draggingLootItem = null;
                isDraggingFromLoot = false;
                return;
            }

            if (draggingItem != null && isHolding)
            {
                bool moved = false;
                if (cell != null)
                {
                    Vector2Int pos = cell.GetPosition();

                    var targetItem = backpackUI.GetItemAt(pos);

                    if (targetItem != null && targetItem != draggingItem)
                    {
                        var crafted = backpackUI.TryCraftItem(draggingItem, targetItem);

                        if (crafted != null)
                        {
                            Vector2Int preferredPos = targetItem.Position;

                            if (!backpackUI.TryAddItem(crafted, preferredPos))
                            {
                                if (!backpackUI.TryAddItem(crafted, originalPos))
                                {
                                    await lootUI.Add(crafted);
                                }
                            }

                            draggingItem = null;
                            return;
                        }
                    }

                    moved = backpackUI.TryAddItem(draggingItem.Data, pos);
                }

                if (!moved)
                {
                    draggingItem.Data.Rotation = originalRotation;
                    backpackUI.TryAddItem(draggingItem.Data, originalPos);
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
            }

            draggingItem = null;
        }

        private void HandleHover(Vector2 screenPos)
        {
            if (!isHolding)
                return;

            var cell = RaycastCell(screenPos);

            if (cell == null)
            {
                backpackUI.ClearPreview();
                return;
            }

            Vector2Int pos = cell.GetPosition();

            if (isDraggingFromLoot && draggingLootItem != null)
            {
                backpackUI.ShowPreview(draggingLootItem, pos);
            }
            else if (draggingItem != null)
            {
                backpackUI.ShowPreview(draggingItem.Data, pos);
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