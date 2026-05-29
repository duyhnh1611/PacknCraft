using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public abstract class BackpackCellViewBase : MonoBehaviour
    {
        // PRIVATE FIELDS
        private Vector2Int position;

        // PUBLIC METHODS
        public void SetPosition(Vector2Int pos)
        {
            position = pos;
        }

        public Vector2Int GetPosition()
        {
            return position;
        }

        public abstract void SetState(CellState state);
    }
}