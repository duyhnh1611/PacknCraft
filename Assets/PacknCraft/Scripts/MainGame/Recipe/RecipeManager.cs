using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using PacknCraft.Helpers;

namespace PacknCraft.Inventory.Crafting
{
    public class RecipeManager
    {
        private Dictionary<(string, string), CraftRecipe> recipeDict = new();

        private (string, string) MakeKey(string a, string b)
        {
            return string.Compare(a, b) < 0 ? (a, b) : (b, a);
        }

        public async UniTask InitAsync()
        {
            var textAsset = await AssetLoader.LoadAsync<TextAsset>("RecipeConfig", "config");
            var data = JsonUtility.FromJson<RecipeList>(textAsset.text);

            foreach (var entry in data.recipes)
            {
                if (entry.ingredients == null || entry.ingredients.Count != 2)
                    continue;

                var result = await ItemHelper.LoadConfig(entry.result);
                var a = await ItemHelper.LoadConfig(entry.ingredients[0]);
                var b = await ItemHelper.LoadConfig(entry.ingredients[1]);

                var recipe = new CraftRecipe
                {
                    Result = result,
                    Ingredients = new List<ItemConfig> { a, b }
                };

                var key = MakeKey(a.Id, b.Id);
                recipeDict[key] = recipe;
            }
        }

        public CraftRecipe Find(ItemConfig a, ItemConfig b)
        {
            var key = MakeKey(a.Id, b.Id);
            recipeDict.TryGetValue(key, out var recipe);
            return recipe;
        }
    }
}