using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public class BackpackInventoryView : MonoBehaviour
    {
        [SerializeField] private RectTransform container;

        private BackpackHandler handler;
        private float cellSize;

        // Map item -> view
        private readonly Dictionary<PlacedItem, BackpackItemView> itemViews = new();

        public void Bind(BackpackHandler handler, float cellSize)
        {
            this.handler = handler;
            this.cellSize = cellSize;

            handler.OnItemAdded += OnItemAdded;
            handler.OnItemRemoved += OnItemRemoved;
            handler.OnItemChanged += OnItemChanged;
        }

        public void Refresh()
        {
            foreach (Transform c in container)
            {
                Destroy(c.gameObject);
            }

            itemViews.Clear();

            foreach (var item in handler.GetItems())
            {
                _ = CreateItem(item);
            }
        }

        private async Task CreateItem(PlacedItem item)
        {
            var address = $"ItemView/{item.Data.Config.Id}";

            var go = await AssetLoader.InstantiateAsync(
                address,
                Define.GAME_ASSET,
                container
            );

            if (go == null) return;

            var rect = go.GetComponent<RectTransform>();
            var view = go.GetComponent<BackpackItemView>();

            view.Bind(item, cellSize);
            ApplyPosition(rect, item);

            itemViews[item] = view;
        }

        private void ApplyPosition(RectTransform rect, PlacedItem item)
        {
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);

            float x = item.Position.x * cellSize;
            float y = -item.Position.y * cellSize;

            rect.anchoredPosition = new Vector2(x, y);
        }

        private void OnItemAdded(PlacedItem item)
        {
            _ = CreateItem(item);
        }

        private void OnItemChanged(PlacedItem item)
        {
            if (!itemViews.TryGetValue(item, out var view))
                return;

            view.Refresh();
        }

        private void OnItemRemoved(PlacedItem item)
        {
            if (!itemViews.TryGetValue(item, out var view))
                return;

            if (view != null)
            {
                Destroy(view.gameObject);
            }

            itemViews.Remove(item);
        }
    }
}