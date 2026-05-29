using System.Threading.Tasks;
using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class BackpackGridView : MonoBehaviour
    {
        // SERIALIZED FIELDS
        [SerializeField] private RectTransform container;
        [SerializeField] private string cellAddress;

        // PRIVATE FIELDS
        private BackpackHandler handler;
        private BackpackCellViewBase[,] cells;
        private RectTransform rect;
        private float cellSize;
        private bool initialized;

        // PUBLIC METHODS
        public async void Bind(BackpackHandler handler, float cellSize)
        {
            initialized = false;

            this.handler = handler;
            this.cellSize = cellSize;

            rect = GetComponent<RectTransform>();
            ResizeSelf();

            cells = new BackpackCellView[handler.Width, handler.Height];
            await CreateCells();

            handler.OnCellStateChanged += OnCellChanged;

            initialized = true;
        }

        public void Refresh()
        {
            if (!initialized)
                return;

            for (int x = 0; x < handler.Width; x++)
            {
                for (int y = 0; y < handler.Height; y++)
                {
                    cells[x, y].SetState(handler.GetCellState(x, y));
                }
            }
        }

        public void ShowPreview(ItemData data, Vector2Int pivot)
        {
            Refresh();

            var result = handler.CheckItem(data, pivot);

            foreach (var r in result)
            {
                if (!Inside(r.pos)) continue;

                cells[r.pos.x, r.pos.y].SetState(r.valid ? CellState.PreviewValid : CellState.PreviewInvalid);
            }
        }

        // PRIVATE METHODS
        private async Task CreateCells()
        {
            var prefab = await AssetLoader.LoadAsync<GameObject>(
                cellAddress,
                Define.GAME_ASSET
            );

            for (int x = 0; x < handler.Width; x++)
            {
                for (int y = 0; y < handler.Height; y++)
                {
                    var go = Instantiate(prefab, container);

                    var rect = go.GetComponent<RectTransform>();
                    var cell = go.GetComponent<BackpackCellViewBase>();

                    rect.anchoredPosition = GetCellPosition(x, y);
                    rect.sizeDelta = Vector2.one * cellSize;

                    cell.SetPosition(new Vector2Int(x, y));
                    cell.SetState(handler.GetCellState(x, y));

                    cells[x, y] = cell;
                }
            }
        }

        private void ResizeSelf()
        {
            float width = handler.Width * cellSize;
            float height = handler.Height * cellSize;
            rect.sizeDelta = new Vector2(width, height);
        }

        private Vector2 GetCellPosition(int x, int y)
        {
            float px = x * cellSize;
            float py = -y * cellSize;

            return new Vector2(px, py);
        }

        private void OnCellChanged(Vector2Int pos, CellState state)
        {
            if (!Inside(pos)) return;

            cells[pos.x, pos.y].SetState(state);
        }

        private bool Inside(Vector2Int p)
        {
            return p.x >= 0 && p.y >= 0 &&
                   p.x < handler.Width && p.y < handler.Height;
        }
    }
}