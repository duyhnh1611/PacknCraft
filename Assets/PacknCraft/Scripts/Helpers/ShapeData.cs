using System;

namespace PacknCraft.Inventory
{
    [Serializable]
    public class ShapeData
    {
        public int size;
        public bool[] data;

        // RUNTIME CACHE
        private bool[][,] rotatedCache;

        public void Resize(int newSize)
        {
            size = newSize;
            data = new bool[size * size];
            rotatedCache = null;
        }

        public bool Get(int x, int y)
        {
            return data[y * size + x];
        }

        public void Set(int x, int y, bool value)
        {
            data[y * size + x] = value;
            rotatedCache = null;
        }

        public bool[,] GetShape(ItemRotation rot)
        {
            if (rotatedCache == null)
                BuildCache();

            return rotatedCache[(int)rot];
        }

        private void BuildCache()
        {
            rotatedCache = new bool[4][,];

            var baseShape = To2D();

            rotatedCache[0] = baseShape;
            rotatedCache[1] = RotateRight(baseShape);
            rotatedCache[2] = RotateRight(rotatedCache[1]);
            rotatedCache[3] = RotateRight(rotatedCache[2]);
        }

        private bool[,] To2D()
        {
            var result = new bool[size, size];

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    result[x, y] = Get(x, y);

            return result;
        }

        private bool[,] RotateRight(bool[,] src)
        {
            int n = size;
            var dst = new bool[n, n];

            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                    dst[x, y] = src[y, n - 1 - x];

            return dst;
        }
    }
}