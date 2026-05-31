using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public class BackpackUI : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private BackpackGridView gridView;
        [SerializeField] private BackpackInventoryView inventoryView;

        [Header("Config")]
        [SerializeField] private int width = 6;
        [SerializeField] private int height = 8;
        [SerializeField] private float cellSize = 150f;

        private BackpackHandler handler;
        private Crafting.RecipeManager recipeManager;

        private void Awake()
        {
            handler = new BackpackHandler(width, height);
            recipeManager = new Crafting.RecipeManager();
        }

        private void Start()
        {
            inventoryView.Bind(handler, cellSize);
            gridView.Bind(handler, cellSize);

            gridView.Refresh();
            inventoryView.Refresh();

            recipeManager.InitAsync().Forget();
        }

        public bool TryAddItem(ItemData data, Vector2Int pos)
        {
            if (!handler.CanPlace(data, pos))
                return false;

            handler.Place(data, pos);
            return true;
        }

        public void RemoveItem(PlacedItem item)
        {
            handler.Remove(item);
        }

        public void ShowPreview(ItemData data, Vector2Int pos)
        {
            gridView.ShowPreview(data, pos);
        }

        public void ClearPreview()
        {
            gridView.Refresh();
        }

        public bool TryRotateItem(PlacedItem item)
        {
            return handler.TryRotateItem(item);
        }

        public PlacedItem GetItemAt(Vector2Int pos)
        {
            return handler.GetItemAt(pos);
        }

        public ItemData TryCraftItem(PlacedItem a, PlacedItem b)
        {
            if (recipeManager == null)
                return null;

            if (a == null || b == null || a == b)
                return null;

            if (!ShapeAdjacency.AreAdjacent(a, b))
                return null;

            var recipe = recipeManager.Find(a.Data.Config, b.Data.Config);
            if (recipe == null)
                return null;

            handler.Remove(a);
            handler.Remove(b);

            var newItem = new ItemData(recipe.Result);
            return newItem;
        }
    }
}