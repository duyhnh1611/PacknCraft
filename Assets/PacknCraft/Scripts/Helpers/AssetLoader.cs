using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetLoader
{
    // CONSTS
    private static readonly Dictionary<string, Dictionary<string, Object>> cache = new();
    private static readonly Dictionary<string, List<AsyncOperationHandle>> handles = new();

    // PUBLIC METHODS
    public static async UniTask<T> LoadAsync<T>(string address, string group) where T : Object
    {
        if (!cache.TryGetValue(group, out var groupCache))
        {
            groupCache = new();
            cache[group] = groupCache;
        }

        if (!handles.TryGetValue(group, out var groupHandles))
        {
            groupHandles = new();
            handles[group] = groupHandles;
        }

        if (groupCache.TryGetValue(address, out var cached))
            return cached as T;

        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        var result = handle.Result;

        groupCache[address] = result;
        groupHandles.Add(handle);

        return result;
    }

    public static async UniTask<GameObject> InstantiateAsync(string address, string group, Transform parent = null)
    {
        var prefab = await LoadAsync<GameObject>(address, group);
        return Object.Instantiate(prefab, parent);
    }

    public static void Release(string group)
    {
        if (!handles.TryGetValue(group, out var groupHandles))
            return;

        foreach (var handle in groupHandles)
            Addressables.Release(handle);

        handles.Remove(group);
        cache.Remove(group);
    }

    public static void ReleaseAsset(string address, string group)
    {
        if (!cache.TryGetValue(group, out var groupCache))
            return;

        if (!handles.TryGetValue(group, out var groupHandles))
            return;

        if (!groupCache.TryGetValue(address, out var obj))
            return;

        for (int i = groupHandles.Count - 1; i >= 0; i--)
        {
            var handle = groupHandles[i];

            if ((Object)handle.Result == obj)
            {
                Addressables.Release(handle);
                groupHandles.RemoveAt(i);
                break;
            }
        }

        groupCache.Remove(address);
    }
}