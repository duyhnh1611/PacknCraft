using System;
using System.Collections.Generic;

[Serializable]
public class RecipeEntry
{
    public string result;
    public List<string> ingredients;
}

[Serializable]
public class RecipeList
{
    public List<RecipeEntry> recipes;
}