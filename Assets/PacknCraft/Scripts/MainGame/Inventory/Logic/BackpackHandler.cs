using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    public class BackpackHandler : MonoBehaviour
    {
        // SERIALIZED FIELDS
        [SerializeField] private Vector2Int gridSize = new(6, 8);

        // PRIVATE FIELDS
        private readonly List<PlacedItem> items = new();

        // PUBLIC METHODS
        public bool IsInsideGrid(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 &&
                   pos.x < gridSize.x && pos.y < gridSize.y;
        }

        public bool CanPlace(ItemData item, Vector2Int origin)
        {
            foreach (var cell in item.GetCells())
            {
                var pos = origin + cell;
                if (!IsInsideGrid(pos))
                    return false;
            }

            return true;
        }

        public PlacedItem PlaceItem(ItemData item, Vector2Int origin)
        {
            var placed = new PlacedItem(item, origin);
            items.Add(placed);
            return placed;
        }

        public List<PlacedItem> GetOverlaps(PlacedItem target)
        {
            var result = new List<PlacedItem>();

            foreach (var other in items)
            {
                if (other == target) continue;
                if (IsOverlap(target, other))
                    result.Add(other);
            }

            return result;
        }

        // PRIVATE METHODS
        private bool IsOverlap(PlacedItem a, PlacedItem b)
        {
            var cellsA = a.GetWorldCells();
            var cellsB = b.GetWorldCells();

            foreach (var ca in cellsA)
                foreach (var cb in cellsB)
                    if (ca == cb)
                        return true;

            return false;
        }
    }
}