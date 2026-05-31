namespace PacknCraft.Inventory
{
    public class ItemData
    {
        public ItemConfig Config;
        public ItemRotation Rotation;

        public ItemData(ItemConfig config)
        {
            Config = config;
            Rotation = ItemRotation.Up;
        }

        public bool[,] GetShape()
        {
            return Config.Shape.GetShape(Rotation);
        }

        public int Size => Config.Shape.size;
    }
}