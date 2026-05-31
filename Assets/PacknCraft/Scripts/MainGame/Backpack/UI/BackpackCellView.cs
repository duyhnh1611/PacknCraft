using UnityEngine;
using UnityEngine.UI;

namespace PacknCraft.Inventory.UI
{
    public class BackpackCellView : BackpackCellViewBase
    {
        // SERIALIZED FIELDS
        [SerializeField] private Image image;

        [SerializeField] private Color empty;
        [SerializeField] private Color filled;
        [SerializeField] private Color previewValid;
        [SerializeField] private Color previewInvalid;

        // PUBLIC METHODS
        public override void SetState(CellState state)
        {
            image.color = state switch
            {
                CellState.Empty => empty,
                CellState.Filled => filled,
                CellState.PreviewValid => previewValid,
                CellState.PreviewInvalid => previewInvalid,
                _ => empty
            };
        }
    }
}