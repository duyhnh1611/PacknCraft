using PacknCraft.Inventory;
using System.Linq;
using UnityEngine;

public class MissionCraft : IMission
{
    private string rawDetail;
    private CraftDetail detail;

    public MissionCraft(string detailRaw)
    {
        rawDetail = detailRaw;

        try
        {
            detail = JsonUtility.FromJson<CraftDetail>(rawDetail);
        }
        catch
        {
            Debug.LogError($"Failed to parse craft detail: {rawDetail}");
        }
    }

    public bool CheckFinish(BackpackHandler backpack, LootHandler loot)
    {
        if (detail == null || detail.itemsToCraft == null)
            return false;

        var backpackItems = backpack.Items;
        foreach (var item in detail.itemsToCraft)
        {
            if(!backpackItems.Any(i => i.Data.Config.Id == item))
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class CraftDetail
{
    public string[] itemsToCraft;
}