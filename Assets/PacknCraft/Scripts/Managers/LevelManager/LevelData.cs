using System;
using System.Collections.Generic;

[System.Serializable]
public class LevelInfo
{
    public int level;
    public string title;
    public string description;
    public List<string> loot;
    public List<LevelTargetInfo> targets;
}

[System.Serializable]
public class LevelTargetInfo
{
    public string type;
    public string detail;
}

[Serializable]
public class LevelCatalog
{
    public List<LevelCatalogEntry> entries;

    private Dictionary<int, string> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<int, string>();
        foreach (var e in entries)
        {
            lookup[e.level] = e.key;
        }
    }

    public string GetLevelKey(int level)
    {
        if (lookup == null)
            BuildLookup();

        return lookup.TryGetValue(level, out var key) ? key : null;
    }
}

[Serializable]
public class LevelCatalogEntry
{
    public int level;
    public string key;
}