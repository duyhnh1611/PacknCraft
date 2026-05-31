using UnityEngine;

namespace PacknCraft.Inventory.UI
{
    public class LootItemView : MonoBehaviour
    {
        public ItemData Data { get; private set; }

        public void Bind(ItemData data)
        {
            Data = data;
        }
    }
}