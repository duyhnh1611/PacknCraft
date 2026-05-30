using System.Collections.Generic;
using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public class BackpackDragView : MonoBehaviour
    {
        // SERIALIZED FIELDS
        [SerializeField] private RectTransform rootCanvas;
        [SerializeField] private float cellSize = 150f;

        // PRIVATE FIELDS
        private Camera uiCamera;

        private BackpackItemView currentView;
        private ItemData currentData;

        private readonly Dictionary<string, Stack<BackpackItemView>> pool = new();

        // UNITY METHODS
        private void Awake()
        {
            if (rootCanvas == null)
                rootCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            var canvas = rootCanvas.GetComponentInParent<Canvas>();

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                uiCamera = null;
            else
                uiCamera = canvas.worldCamera;
        }

        // PUBLIC METHODS
        public async void Show(ItemData data)
        {
            currentData = data;

            string id = data.Config.Id;

            BackpackItemView view = GetFromPool(id);

            if (view == null)
            {
                var go = await AssetLoader.InstantiateAsync(
                    $"ItemView/{id}",
                    Define.GAME_ASSET,
                    transform
                );

                if (go == null) return;

                view = go.GetComponent<BackpackItemView>();
            }

            view.gameObject.SetActive(true);
            view.Bind(data, cellSize);

            var rect = view.GetComponent<RectTransform>();
            rect.pivot = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.localScale = Vector3.one;

            currentView = view;
        }

        public void Hide()
        {
            if (currentView == null) return;

            string id = currentData.Config.Id;

            if (!pool.ContainsKey(id))
                pool[id] = new Stack<BackpackItemView>();

            currentView.gameObject.SetActive(false);
            pool[id].Push(currentView);

            currentView = null;
            currentData = null;
        }

        public void UpdatePosition(Vector2 screenPos)
        {
            if (currentView == null || currentData == null)
                return;

            RectTransform rect = currentView.GetComponent<RectTransform>();

            Vector3 worldPos;

            if (uiCamera == null)
            {
                worldPos = screenPos;
            }
            else
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    rootCanvas,
                    screenPos,
                    uiCamera,
                    out worldPos
                );
            }

            rect.position = worldPos + (Vector3)CalculateOffset(currentData);
        }

        // PRIVATE METHODS
        private BackpackItemView GetFromPool(string id)
        {
            if (pool.TryGetValue(id, out var stack) && stack.Count > 0)
                return stack.Pop();

            return null;
        }

        private Vector2 CalculateOffset(ItemData data)
        {
            int size = data.Size;

            float width = size * cellSize;
            float height = size * cellSize;

            if (size % 2 == 1)
            {
                return new Vector2(
                    -width * 0.5f,
                    height * 0.5f
                );
            }
            else
            {
                return new Vector2(
                    -(width * 0.5f - cellSize * 0.5f),
                    (height * 0.5f - cellSize * 0.5f)
                );
            }
        }
    }
}