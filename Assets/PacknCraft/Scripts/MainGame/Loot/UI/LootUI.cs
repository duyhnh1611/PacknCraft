using Cysharp.Threading.Tasks;
using PacknCraft.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public class LootUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.ScrollRect scrollRect;
        [SerializeField] private Transform container;

        private LootHandler handler;
        private Dictionary<ItemData, LootItemView> itemViewDict = new();

        public LootHandler Handler => handler;

        public void Init(List<ItemData> items)
        {
            handler = new LootHandler(items);
            Refresh();
        }

        private async void Refresh()
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);

            itemViewDict.Clear();

            List<UniTask> tasks = new();

            foreach (var item in handler.Items)
                tasks.Add(CreateItemView(item));

            await UniTask.WhenAll(tasks);
        }

        private async UniTask<LootItemView> CreateItemView(ItemData data)
        {
            var go = await AssetLoader.InstantiateAsync(
                $"LootItem/{data.Config.Id}",
                Define.GAME_ASSET,
                container
            );

            if (go == null) return null;

            if (!handler.Items.Contains(data))
            {
                Destroy(go);
                return null;
            }

            var view = go.GetComponent<LootItemView>();
            view.Bind(data);

            itemViewDict[data] = view;
            return view;
        }

        public async UniTask<LootItemView> Add(ItemData data)
        {
            handler.Add(data);
            return await CreateItemView(data);
        }

        public void Remove(ItemData data)
        {
            handler.Remove(data);

            if (itemViewDict.TryGetValue(data, out var view))
            {
                if (view != null)
                    Destroy(view.gameObject);

                itemViewDict.Remove(data);
            }
        }

        public void SetScrollEnabled(bool value)
        {
            if (scrollRect != null)
                scrollRect.enabled = value;
        }
    }
}