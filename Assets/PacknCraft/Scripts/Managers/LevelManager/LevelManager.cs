using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private const string LEVEL_GROUP = "LEVEL_DATA";
    private const string CATALOG_GROUP = "LEVEL_CATALOG";

    private Dictionary<int, LevelInfo> loadedLevels = new();
    private Dictionary<int, string> levelAddressMap = new();

    private Dictionary<int, LevelCatalog> loadedCatalogs = new();

    private int currentLevel = 1;

    public int CurrentLevel => currentLevel;

    public LevelInfo GetLevelInfo(int level)
    {
        return loadedLevels.TryGetValue(level, out var info) ? info : null;
    }

    public LevelInfo GetCurrentLevelInfo()
    {
        return GetLevelInfo(currentLevel);
    }

    public async UniTask<LevelInfo> LoadCurrentLevelAsync()
    {
        return await LoadLevelAsync(currentLevel);
    }

    public async UniTask<LevelInfo> LoadNextLevelAsync()
    {
        return await LoadLevelAsync(currentLevel + 1);
    }

    public async UniTask<LevelInfo> LoadLevelAsync(int level)
    {
        currentLevel = level;

        await PreloadLevelAsync(currentLevel);

        CleanupPreviousLevels();

        return GetCurrentLevelInfo();
    }

    public async UniTask PreloadLevelAsync(int level)
    {
        if (loadedLevels.ContainsKey(level))
            return;

        int catalogIndex = GetCatalogIndex(level);

        if (!loadedCatalogs.TryGetValue(catalogIndex, out var catalog))
        {
            string catalogAddress = $"Level_Catalog_{catalogIndex}";

            var catalogText = await AssetLoader.LoadAsync<TextAsset>(catalogAddress, CATALOG_GROUP);

            catalog = JsonUtility.FromJson<LevelCatalog>(catalogText.text);
            catalog.BuildLookup();

            loadedCatalogs[catalogIndex] = catalog;
        }

        string levelKey = catalog.GetLevelKey(level);

        if (string.IsNullOrEmpty(levelKey))
            return;

        var levelText = await AssetLoader.LoadAsync<TextAsset>(levelKey, LEVEL_GROUP);

        var levelInfo = JsonUtility.FromJson<LevelInfo>(levelText.text);

        loadedLevels[level] = levelInfo;
        levelAddressMap[level] = levelKey;
    }

    private void CleanupPreviousLevels()
    {
        var keysToRemove = new List<int>();

        foreach (var kvp in loadedLevels)
        {
            if (kvp.Key < currentLevel)
                keysToRemove.Add(kvp.Key);
        }

        foreach (var level in keysToRemove)
        {
            if (levelAddressMap.TryGetValue(level, out var address))
            {
                AssetLoader.ReleaseAsset(address, LEVEL_GROUP);
            }

            loadedLevels.Remove(level);
            levelAddressMap.Remove(level);
        }
    }

    private int GetCatalogIndex(int level)
    {
        return ((level - 1) / 100) + 1;
    }
}