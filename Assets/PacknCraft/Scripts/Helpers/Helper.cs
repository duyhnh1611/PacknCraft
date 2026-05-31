using PacknCraft.Inventory;
using UnityEngine;

public static class ShapeAdjacency
{
    public static bool AreAdjacent(PlacedItem a, PlacedItem b)
    {
        var shapeA = a.Data.Config.Shape.GetShape(a.Data.Rotation);
        var shapeB = b.Data.Config.Shape.GetShape(b.Data.Rotation);

        for (int ax = 0; ax < shapeA.GetLength(0); ax++)
        {
            for (int ay = 0; ay < shapeA.GetLength(1); ay++)
            {
                if (!shapeA[ax, ay]) continue;

                var worldA = a.Position + new Vector2Int(ax, ay);

                for (int bx = 0; bx < shapeB.GetLength(0); bx++)
                {
                    for (int by = 0; by < shapeB.GetLength(1); by++)
                    {
                        if (!shapeB[bx, by]) continue;

                        var worldB = b.Position + new Vector2Int(bx, by);

                        int dx = Mathf.Abs(worldA.x - worldB.x);
                        int dy = Mathf.Abs(worldA.y - worldB.y);

                        // chỉ cho phép cạnh (4-direction)
                        if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
                            return true;
                    }
                }
            }
        }

        return false;
    }
}