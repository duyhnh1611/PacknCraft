using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    public class BackpackHandler
    {
        // PUBLIC FIELDS
        public System.Action<Vector2Int, CellState> OnCellStateChanged;
        public System.Action<PlacedItem> OnItemAdded;

        // PRIVATE FIELDS
        private readonly CellState[,] grid;
        private readonly List<PlacedItem> items = new();

        // PROPERTIES
        public int Width { get; }
        public int Height { get; }

        // PUBLIC METHODS
        public BackpackHandler(int width, int height)
        {
            Width = width;
            Height = height;

            grid = new CellState[width, height];
            Clear();
        }

        public IEnumerable<PlacedItem> GetItems() => items;

        public CellState GetCellState(int x, int y) => grid[x, y];

        public bool CanPlace(ItemData data, Vector2Int pos)
        {
            var shape = data.GetShape();
            int size = data.Size;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y]) continue;

                    int gx = pos.x + x;
                    int gy = pos.y + y;

                    if (!Inside(gx, gy)) return false;
                    if (grid[gx, gy] != CellState.Empty) return false;
                }

            return true;
        }

        public void Place(ItemData data, Vector2Int pos)
        {
            if (!CanPlace(data, pos))
                return;

            var item = new PlacedItem(data, pos);
            items.Add(item);

            ApplyToGrid(item, CellState.Filled);

            OnItemAdded?.Invoke(item);
        }

        public List<(Vector2Int pos, bool valid)> CheckItem(ItemData data, Vector2Int pos)
        {
            var result = new List<(Vector2Int, bool)>();

            var shape = data.GetShape();
            int size = data.Size;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y]) continue;

                    int gx = pos.x + x;
                    int gy = pos.y + y;

                    bool valid = Inside(gx, gy) && grid[gx, gy] == CellState.Empty;

                    result.Add((new Vector2Int(gx, gy), valid));
                }

            return result;
        }

        public bool TryRotateItem(PlacedItem item)
        {
            // Remove from grid
            ApplyToGrid(item, CellState.Empty);

            var data = item.Data;
            var oldRotation = data.Rotation;

            // Rotate
            data.Rotation = (ItemRotation)(((int)data.Rotation + 1) % 4);

            // Validate
            if (CanPlace(data, item.Position))
            {
                ApplyToGrid(item, CellState.Filled);
                return true;
            }

            // Rollback if failed
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
                for (int y = 0; y < size; y++)
                {
                    if (!shape[x, y]) continue;

                    int gx = item.Position.x + x;
                    int gy = item.Position.y + y;

                    grid[gx, gy] = state;
                    OnCellStateChanged?.Invoke(new Vector2Int(gx, gy), state);
                }
        }

        private bool Inside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        private void Clear()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    grid[x, y] = CellState.Empty;
        }
    }
}