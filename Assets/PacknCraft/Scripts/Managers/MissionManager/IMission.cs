using PacknCraft.Inventory;

public interface IMission
{
    bool CheckFinish(BackpackHandler backpack, LootHandler loot);
}