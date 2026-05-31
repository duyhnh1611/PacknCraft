namespace PacknCraft
{
    public enum SceneId
    {
        Game
    }

    public enum ItemRotation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    public enum CellState
    {
        Empty,
        Filled,
        PreviewValid,
        PreviewInvalid
    }

    public static class Define
    {
        // ASSET GROUP
        public const string GAME_ASSET = "GameAsset";
    }
}