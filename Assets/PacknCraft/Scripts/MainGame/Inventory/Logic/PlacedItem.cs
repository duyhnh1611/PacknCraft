using UnityEngine;

namespace PacknCraft.Inventory
{
    public class PlacedItem
    {
        // PUBLIC FIELDS
        public ItemData Data;
        public Vector2Int Position;

        // CONSTRUCTOR
        public PlacedItem(ItemData data, Vector2Int pos)
        {
            Data = data;
            Position = pos;
        }
    }
}