using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class BackpackItemView : MonoBehaviour
    {
        // SERIALIZED
        [SerializeField] private RectTransform visual;

        // PRIVATE
        private RectTransform rect;
        private ItemData data;

        // PROPERTIES
        public ItemData Data => data;

        // UNITY
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        // PUBLIC
        public void Bind(ItemData data, float cellSize)
        {
            this.data = data;

            ApplySize(cellSize);
            ApplyRotation();
        }

        public void Refresh()
        {
            ApplyRotation();
        }

        // PRIVATE
        private void ApplySize(float cellSize)
        {
            int size = data.Size;

            float pixelSize = size * cellSize;

            rect.sizeDelta = new Vector2(pixelSize, pixelSize);
        }

        private void ApplyRotation()
        {
            float angle = data.Rotation switch
            {
                ItemRotation.Up => 0,
                ItemRotation.Right => -90,
                ItemRotation.Down => 180,
                ItemRotation.Left => 90,
                _ => 0
            };

            visual.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}