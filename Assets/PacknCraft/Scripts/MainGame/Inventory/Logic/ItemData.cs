using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    public enum ItemRotation
    {
        Up,
        Right,
        Down,
        Left
    }

    public class ItemData
    {
        // PRIVATE FIELDS
        private readonly ItemConfig config;
        private ItemRotation rotation;

        // PUBLIC PROPERTIES
        public ItemConfig Config => config;
        public ItemRotation Rotation => rotation;

        // CONSTRUCTOR
        public ItemData(ItemConfig config)
        {
            this.config = config;
            rotation = ItemRotation.Up;
        }

        // PUBLIC METHODS
        public void RotateCW()
        {
            rotation = (ItemRotation)(((int)rotation + 1) % 4);
        }

        public List<Vector2Int> GetCells()
        {
            var result = new List<Vector2Int>(config.Cells.Count);

            foreach (var cell in config.Cells)
                result.Add(Rotate(cell));

            return result;
        }

        // PRIVATE METHODS
        private Vector2Int Rotate(Vector2Int cell)
        {
            var pivot = config.Pivot;

            Vector2 p = (Vector2)cell - pivot;

            p = rotation switch
            {
                ItemRotation.Up => p,
                ItemRotation.Right => new(p.y, -p.x),
                ItemRotation.Down => new(-p.x, -p.y),
                ItemRotation.Left => new(-p.y, p.x),
                _ => p
            };

            p += pivot;

            return Vector2Int.RoundToInt(p);
        }
    }
}