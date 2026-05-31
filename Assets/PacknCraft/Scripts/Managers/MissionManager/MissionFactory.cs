using System.Collections.Generic;

public static class MissionFactory
{
    public static IMission Create(LevelTargetInfo info)
    {
        switch (info.type)
        {
            case "craft":
                return new MissionCraft(info.detail);

            default:
                UnityEngine.Debug.LogWarning($"Unknown mission type: {info.type}");
                return null;
        }
    }
}