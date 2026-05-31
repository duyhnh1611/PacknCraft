using System.Collections.Generic;

namespace PacknCraft.Inventory
{
    public class LootHandler
    {
        private List<ItemData> items;

        public IEnumerable<ItemData> Items => items;

        public LootHandler(List<ItemData> items)
        {
            this.items = new List<ItemData>(items);
        }

        public void Remove(ItemData item)
        {
            items.Remove(item);
        }

        public void Add(ItemData item)
        {
            items.Add(item);
        }
    }
}