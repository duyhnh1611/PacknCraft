using Cysharp.Threading.Tasks;
using PacknCraft.Helpers;
using PacknCraft.Inventory;
using PacknCraft.Inventory.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BackpackUI backpackUI;
    [SerializeField] private LootUI lootUI;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (GameManager.Instance == null)
            return;

        InitLevel();
        backpackUI.OnBackpackUpdated.AddListener(OnBackpackUpdated);
    }

    private void OnDestroy()
    {
        backpackUI.OnBackpackUpdated.RemoveListener(OnBackpackUpdated);
    }

    private void OnBackpackUpdated()
    {
        GameManager.Instance.MissionManager.CheckFinish(backpackUI.Handler, lootUI.Handler);
    }

    private void InitLevel()
    {
        var levelInfo = GameManager.Instance.LevelManager.GetCurrentLevelInfo();
        if(levelInfo == null)
            return;

        titleText.text = $"LEVEL {levelInfo.level} - {levelInfo.title}";
        descriptionText.text = levelInfo.description;

        InitLoot(levelInfo);
    }

    private async void InitLoot(LevelInfo levelInfo)
    {
        if (levelInfo == null || levelInfo.loot == null || levelInfo.loot.Count == 0)
            return;

        List<string> configKeys = levelInfo.loot;

        // Load all required ItemConfig
        var loadTasks = configKeys
            .Select(id => ItemHelper.LoadConfig(id))
            .ToList();

        var configs = await UniTask.WhenAll(loadTasks);

        // Create ItemData
        List<ItemData> items = new();
        foreach (var config in configs)
        {
            if (config == null) continue;
            items.Add(new ItemData(config));
        }

        lootUI.Init(items);
    }
}
