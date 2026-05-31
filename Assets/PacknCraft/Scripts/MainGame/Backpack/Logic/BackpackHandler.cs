using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    public class BackpackHandler
    {
        // PUBLIC FIELDS
        public System.Action<Vector2Int, CellState> OnCellStateChanged;
        public System.Action<PlacedItem> OnItemAdded;
        public System.Action<PlacedItem> OnItemRemoved;
        public System.Action<PlacedItem> OnItemChanged;

        // PRIVATE FIELDS
        private readonly CellState[,] grid;
        private readonly PlacedItem[,] itemGrid;
        private readonly List<PlacedItem> items = new();

        // PROPERTIES
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<PlacedItem> Items => items.AsReadOnly();

        // PUBLIC METHODS
        public BackpackHandler(int width, int height)
        {
            Width = width;
            Height = height;

            grid = new CellState[width, height];
            itemGrid = new PlacedItem[width, height];
            Clear();
        }

        public IEnumerable<PlacedItem> GetItems() => items;

        public CellState GetCellState(int x, int y) => grid[x, y];

        public PlacedItem GetItemAt(Vector2Int pos)
        {
            if (!Inside(pos.x, pos.y))
                return null;

            return itemGrid[pos.x, pos.y];
        }

        public bool CanPlace(ItemData data, Vector2Int pos)
        {
            Vector2Int topLeft = CenterToTopLeft(data, pos);

            var shape = data.GetShape();
            int size = data.Size;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y]) continue;

                    int gx = topLeft.x + x;
                    int gy = topLeft.y + y;

                    if (!Inside(gx, gy)) return false;
                    if (grid[gx, gy] != CellState.Empty) return false;
                }

            return true;
        }

        public void Place(ItemData data, Vector2Int pos)
        {
            if (!CanPlace(data, pos))
                return;

            Vector2Int topLeft = CenterToTopLeft(data, pos);

            var item = new PlacedItem(data, topLeft);
            items.Add(item);

            ApplyToGrid(item, CellState.Filled);

            OnItemAdded?.Invoke(item);
        }

        public void Remove(PlacedItem item)
        {
            if (!items.Contains(item))
                return;

            ApplyToGrid(item, CellState.Empty);
            items.Remove(item);

            OnItemRemoved?.Invoke(item);
        }

        public List<(Vector2Int pos, bool valid)> CheckItem(ItemData data, Vector2Int pos)
        {
            Vector2Int topLeft = CenterToTopLeft(data, pos);

            var result = new List<(Vector2Int, bool)>();

            var shape = data.GetShape();
            int size = data.Size;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y])
                        continue;

                    int gx = topLeft.x + x;
                    int gy = topLeft.y + y;

                    bool valid = Inside(gx, gy) && grid[gx, gy] == CellState.Empty;

                    result.Add((new Vector2Int(gx, gy), valid));
                }
            }

            return result;
        }

        public bool TryRotateItem(PlacedItem item)
        {
            ApplyToGrid(item, CellState.Empty);

            var data = item.Data;
            var oldRotation = data.Rotation;

            Vector2Int center = GetItemCenter(item);

            data.Rotation = (ItemRotation)(((int)data.Rotation + 1) % 4);

            if (CanPlace(data, center))
            {
                item.Position = CenterToTopLeft(data, center);
                ApplyToGrid(item, CellState.Filled);

                OnItemChanged?.Invoke(item);
                return true;
            }

            data.Rotation = oldRotation;
            ApplyToGrid(item, CellState.Filled);

            return false;
        }

        // PRIVATE METHODS
        private void ApplyToGrid(PlacedItem item, CellState state)
        {
            var shape = item.Data.GetShape();
            int size = item.Data.Size;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y])
                        continue;

                    int gx = item.Position.x + x;
                    int gy = item.Position.y + y;

                    grid[gx, gy] = state;
                    itemGrid[gx, gy] = state == CellState.Empty ? null : item;

                    OnCellStateChanged?.Invoke(new Vector2Int(gx, gy), state);
                }
            }
        }

        private bool Inside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        private void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    grid[x, y] = CellState.Empty;
                    itemGrid[x, y] = null;
                }
            }
        }

        private Vector2Int CenterToTopLeft(ItemData data, Vector2Int centerPos)
        {
            int size = data.Size;

            if (size % 2 == 0)
            {
                int offset = size / 2 - 1;
                return new Vector2Int(centerPos.x - offset, centerPos.y - offset);
            }
            else
            {
                int offset = size / 2;
                return new Vector2Int(centerPos.x - offset, centerPos.y - offset);
            }
        }

        private Vector2Int GetItemCenter(PlacedItem item)
        {
            int size = item.Data.Size;

            if (size % 2 == 0)
            {
                return new Vector2Int(
                    item.Position.x + size / 2 - 1,
                    item.Position.y + size / 2 - 1
                );
            }
            else
            {
                return new Vector2Int(
                    item.Position.x + size / 2,
                    item.Position.y + size / 2
                );
            }
        }
    }
}