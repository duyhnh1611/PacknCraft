using PacknCraft.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    private List<IMission> missions = new();

    public event Action OnAllMissionsCompleted;

    private bool isCompleted = false;

    public void Init(List<LevelTargetInfo> targets)
    {
        missions.Clear();
        isCompleted = false;

        foreach (var t in targets)
        {
            var mission = MissionFactory.Create(t);
            if (mission != null)
                missions.Add(mission);
        }
    }

    public bool CheckFinish(BackpackHandler backpack, LootHandler loot)
    {
        if (isCompleted)
            return true;

        foreach (var mission in missions)
        {
            if (!mission.CheckFinish(backpack, loot))
                return false;
        }

        isCompleted = true;

        OnAllMissionsCompleted?.Invoke();
        return true;
    }
}