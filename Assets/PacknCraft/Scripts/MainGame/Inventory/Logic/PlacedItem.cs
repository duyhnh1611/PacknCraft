using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory
{
    public class PlacedItem
    {
        // PRIVATE FIELDS
        private readonly ItemData item;
        private Vector2Int pivotPosition;

        // PUBLIC PROPERTIES
        public ItemData Item => item;
        public Vector2Int PivotPosition => pivotPosition;

        // CONSTRUCTOR
        public PlacedItem(ItemData item, Vector2Int pivotPosition)
        {
            this.item = item;
            this.pivotPosition = pivotPosition;
        }

        // PUBLIC METHODS
        public void SetPivot(Vector2Int value)
        {
            pivotPosition = value;
        }

        public List<Vector2Int> GetWorldCells()
        {
            var result = new List<Vector2Int>();

            foreach (var cell in item.GetCells())
                result.Add(pivotPosition + cell);

            return result;
        }
    }
}