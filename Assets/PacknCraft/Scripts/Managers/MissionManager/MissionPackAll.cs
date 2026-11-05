using PacknCraft.Inventory;
using System.Linq;

/// <summary>
/// Mission: every item must be packed into the backpack (no item left in loot).
/// </summary>
public class MissionPackAll : IMission
{
    public bool CheckFinish(BackpackHandler backpack, LootHandler loot)
    {
        return !loot.Items.Any() && backpack.Items.Any();
    }
}
